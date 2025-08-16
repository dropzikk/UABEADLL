using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using SixLabors.ImageSharp.IO;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.Metadata;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Tga;

internal sealed class TgaDecoderCore : IImageDecoderInternals
{
	private readonly Configuration configuration;

	private ImageMetadata? metadata;

	private TgaMetadata? tgaMetadata;

	private TgaFileHeader fileHeader;

	private readonly MemoryAllocator memoryAllocator;

	private bool hasAlpha;

	public DecoderOptions Options { get; }

	public Size Dimensions => new Size(fileHeader.Width, fileHeader.Height);

	public TgaDecoderCore(DecoderOptions options)
	{
		Options = options;
		configuration = options.Configuration;
		memoryAllocator = configuration.MemoryAllocator;
	}

	public Image<TPixel> Decode<TPixel>(BufferedReadStream stream, CancellationToken cancellationToken) where TPixel : unmanaged, IPixel<TPixel>
	{
		try
		{
			TgaImageOrigin origin = ReadFileHeader(stream);
			stream.Skip(fileHeader.IdLength);
			byte colorMapType = fileHeader.ColorMapType;
			if (colorMapType != 0 && colorMapType != 1)
			{
				TgaThrowHelper.ThrowNotSupportedException($"Unknown tga colormap type {fileHeader.ColorMapType} found");
			}
			if (fileHeader.Width == 0 || fileHeader.Height == 0)
			{
				throw new UnknownImageFormatException("Width or height cannot be 0");
			}
			Image<TPixel> image = new Image<TPixel>(configuration, fileHeader.Width, fileHeader.Height, metadata);
			Buffer2D<TPixel> rootFramePixelBuffer = image.GetRootFramePixelBuffer();
			if (fileHeader.ColorMapType == 1)
			{
				if (fileHeader.CMapLength <= 0)
				{
					TgaThrowHelper.ThrowInvalidImageContentException("Missing tga color map length");
				}
				if (fileHeader.CMapDepth <= 0)
				{
					TgaThrowHelper.ThrowInvalidImageContentException("Missing tga color map depth");
				}
				int num = fileHeader.CMapDepth / 8;
				int num2 = fileHeader.CMapLength * num;
				using (IMemoryOwner<byte> buffer = memoryAllocator.Allocate<byte>(num2, AllocationOptions.Clean))
				{
					Span<byte> span = buffer.GetSpan();
					if (stream.Read(span, fileHeader.CMapStart, num2) != num2)
					{
						TgaThrowHelper.ThrowInvalidImageContentException("Not enough data to read the color map");
					}
					if (fileHeader.ImageType == TgaImageType.RleColorMapped)
					{
						ReadPalettedRle(stream, fileHeader.Width, fileHeader.Height, rootFramePixelBuffer, span, num, origin);
					}
					else
					{
						ReadPaletted(stream, fileHeader.Width, fileHeader.Height, rootFramePixelBuffer, span, num, origin);
					}
				}
				return image;
			}
			if (fileHeader.CMapLength > 0)
			{
				int num3 = fileHeader.CMapDepth / 8;
				stream.Skip(fileHeader.CMapLength * num3);
			}
			switch (fileHeader.PixelDepth)
			{
			case 8:
				if (fileHeader.ImageType.IsRunLengthEncoded())
				{
					ReadRle(stream, fileHeader.Width, fileHeader.Height, rootFramePixelBuffer, 1, origin);
				}
				else
				{
					ReadMonoChrome(stream, fileHeader.Width, fileHeader.Height, rootFramePixelBuffer, origin);
				}
				break;
			case 15:
			case 16:
				if (fileHeader.ImageType.IsRunLengthEncoded())
				{
					ReadRle(stream, fileHeader.Width, fileHeader.Height, rootFramePixelBuffer, 2, origin);
				}
				else
				{
					ReadBgra16(stream, fileHeader.Width, fileHeader.Height, rootFramePixelBuffer, origin);
				}
				break;
			case 24:
				if (fileHeader.ImageType.IsRunLengthEncoded())
				{
					ReadRle(stream, fileHeader.Width, fileHeader.Height, rootFramePixelBuffer, 3, origin);
				}
				else
				{
					ReadBgr24(stream, fileHeader.Width, fileHeader.Height, rootFramePixelBuffer, origin);
				}
				break;
			case 32:
				if (fileHeader.ImageType.IsRunLengthEncoded())
				{
					ReadRle(stream, fileHeader.Width, fileHeader.Height, rootFramePixelBuffer, 4, origin);
				}
				else
				{
					ReadBgra32(stream, fileHeader.Width, fileHeader.Height, rootFramePixelBuffer, origin);
				}
				break;
			default:
				TgaThrowHelper.ThrowNotSupportedException("ImageSharp does not support this kind of tga files.");
				break;
			}
			return image;
		}
		catch (IndexOutOfRangeException innerException)
		{
			throw new ImageFormatException("TGA image does not have a valid format.", innerException);
		}
	}

	private void ReadPaletted<TPixel>(BufferedReadStream stream, int width, int height, Buffer2D<TPixel> pixels, Span<byte> palette, int colorMapPixelSizeInBytes, TgaImageOrigin origin) where TPixel : unmanaged, IPixel<TPixel>
	{
		TPixel color = default(TPixel);
		bool flag = InvertX(origin);
		for (int i = 0; i < height; i++)
		{
			int y = InvertY(i, height, origin);
			Span<TPixel> pixelRow = pixels.DangerousGetRowSpan(y);
			switch (colorMapPixelSizeInBytes)
			{
			case 2:
				if (flag)
				{
					for (int num2 = width - 1; num2 >= 0; num2--)
					{
						ReadPalettedBgra16Pixel(stream, palette, colorMapPixelSizeInBytes, num2, color, pixelRow);
					}
				}
				else
				{
					for (int k = 0; k < width; k++)
					{
						ReadPalettedBgra16Pixel(stream, palette, colorMapPixelSizeInBytes, k, color, pixelRow);
					}
				}
				break;
			case 3:
				if (flag)
				{
					for (int num3 = width - 1; num3 >= 0; num3--)
					{
						ReadPalettedBgr24Pixel(stream, palette, colorMapPixelSizeInBytes, num3, color, pixelRow);
					}
				}
				else
				{
					for (int l = 0; l < width; l++)
					{
						ReadPalettedBgr24Pixel(stream, palette, colorMapPixelSizeInBytes, l, color, pixelRow);
					}
				}
				break;
			case 4:
				if (flag)
				{
					for (int num = width - 1; num >= 0; num--)
					{
						ReadPalettedBgra32Pixel(stream, palette, colorMapPixelSizeInBytes, num, color, pixelRow);
					}
				}
				else
				{
					for (int j = 0; j < width; j++)
					{
						ReadPalettedBgra32Pixel(stream, palette, colorMapPixelSizeInBytes, j, color, pixelRow);
					}
				}
				break;
			}
		}
	}

	private void ReadPalettedRle<TPixel>(BufferedReadStream stream, int width, int height, Buffer2D<TPixel> pixels, Span<byte> palette, int colorMapPixelSizeInBytes, TgaImageOrigin origin) where TPixel : unmanaged, IPixel<TPixel>
	{
		using IMemoryOwner<byte> buffer = memoryAllocator.Allocate<byte>(width * height, AllocationOptions.Clean);
		TPixel color = default(TPixel);
		Span<byte> span = buffer.GetSpan();
		UncompressRle(stream, width, height, span, 1);
		for (int i = 0; i < height; i++)
		{
			int y = InvertY(i, height, origin);
			Span<TPixel> span2 = pixels.DangerousGetRowSpan(y);
			int num = i * width;
			for (int j = 0; j < width; j++)
			{
				int index = num + j;
				switch (colorMapPixelSizeInBytes)
				{
				case 1:
					color.FromL8(Unsafe.As<byte, L8>(ref palette[span[index] * colorMapPixelSizeInBytes]));
					break;
				case 2:
					ReadPalettedBgra16Pixel(palette, span[index], colorMapPixelSizeInBytes, ref color);
					break;
				case 3:
					color.FromBgr24(Unsafe.As<byte, Bgr24>(ref palette[span[index] * colorMapPixelSizeInBytes]));
					break;
				case 4:
					color.FromBgra32(Unsafe.As<byte, Bgra32>(ref palette[span[index] * colorMapPixelSizeInBytes]));
					break;
				}
				int index2 = InvertX(j, width, origin);
				span2[index2] = color;
			}
		}
	}

	private void ReadMonoChrome<TPixel>(BufferedReadStream stream, int width, int height, Buffer2D<TPixel> pixels, TgaImageOrigin origin) where TPixel : unmanaged, IPixel<TPixel>
	{
		if (InvertX(origin))
		{
			TPixel color = default(TPixel);
			for (int i = 0; i < height; i++)
			{
				int y = InvertY(i, height, origin);
				Span<TPixel> pixelSpan = pixels.DangerousGetRowSpan(y);
				for (int num = width - 1; num >= 0; num--)
				{
					ReadL8Pixel(stream, color, num, pixelSpan);
				}
			}
			return;
		}
		using IMemoryOwner<byte> buffer = memoryAllocator.AllocatePaddedPixelRowBuffer(width, 1, 0);
		Span<byte> span = buffer.GetSpan();
		if (InvertY(origin))
		{
			for (int num2 = height - 1; num2 >= 0; num2--)
			{
				ReadL8Row(stream, width, pixels, span, num2);
			}
		}
		else
		{
			for (int j = 0; j < height; j++)
			{
				ReadL8Row(stream, width, pixels, span, j);
			}
		}
	}

	private void ReadBgra16<TPixel>(BufferedReadStream stream, int width, int height, Buffer2D<TPixel> pixels, TgaImageOrigin origin) where TPixel : unmanaged, IPixel<TPixel>
	{
		TPixel val = default(TPixel);
		bool flag = InvertX(origin);
		using IMemoryOwner<byte> buffer = memoryAllocator.AllocatePaddedPixelRowBuffer(width, 2, 0);
		Span<byte> span = buffer.GetSpan();
		Span<byte> span2 = stackalloc byte[2];
		for (int i = 0; i < height; i++)
		{
			int y = InvertY(i, height, origin);
			Span<TPixel> destinationPixels = pixels.DangerousGetRowSpan(y);
			if (flag)
			{
				for (int num = width - 1; num >= 0; num--)
				{
					if (stream.Read(span2) != 2)
					{
						TgaThrowHelper.ThrowInvalidImageContentException("Not enough data to read a pixel row");
					}
					if (!hasAlpha)
					{
						span2[1] |= 128;
					}
					if (fileHeader.ImageType == TgaImageType.BlackAndWhite)
					{
						val.FromLa16(Unsafe.As<byte, La16>(ref MemoryMarshal.GetReference(span2)));
					}
					else
					{
						val.FromBgra5551(Unsafe.As<byte, Bgra5551>(ref MemoryMarshal.GetReference(span2)));
					}
					destinationPixels[num] = val;
				}
				continue;
			}
			if (stream.Read(span) != span.Length)
			{
				TgaThrowHelper.ThrowInvalidImageContentException("Not enough data to read a pixel row");
			}
			if (!hasAlpha)
			{
				for (int j = 1; j < span.Length; j += 2)
				{
					span[j] |= 128;
				}
			}
			if (fileHeader.ImageType == TgaImageType.BlackAndWhite)
			{
				PixelOperations<TPixel>.Instance.FromLa16Bytes(configuration, span, destinationPixels, width);
			}
			else
			{
				PixelOperations<TPixel>.Instance.FromBgra5551Bytes(configuration, span, destinationPixels, width);
			}
		}
	}

	private void ReadBgr24<TPixel>(BufferedReadStream stream, int width, int height, Buffer2D<TPixel> pixels, TgaImageOrigin origin) where TPixel : unmanaged, IPixel<TPixel>
	{
		if (InvertX(origin))
		{
			Span<byte> scratchBuffer = stackalloc byte[4];
			TPixel color = default(TPixel);
			for (int i = 0; i < height; i++)
			{
				int y = InvertY(i, height, origin);
				Span<TPixel> pixelSpan = pixels.DangerousGetRowSpan(y);
				for (int num = width - 1; num >= 0; num--)
				{
					ReadBgr24Pixel(stream, color, num, pixelSpan, scratchBuffer);
				}
			}
			return;
		}
		using IMemoryOwner<byte> buffer = memoryAllocator.AllocatePaddedPixelRowBuffer(width, 3, 0);
		Span<byte> span = buffer.GetSpan();
		if (InvertY(origin))
		{
			for (int num2 = height - 1; num2 >= 0; num2--)
			{
				ReadBgr24Row(stream, width, pixels, span, num2);
			}
		}
		else
		{
			for (int j = 0; j < height; j++)
			{
				ReadBgr24Row(stream, width, pixels, span, j);
			}
		}
	}

	private void ReadBgra32<TPixel>(BufferedReadStream stream, int width, int height, Buffer2D<TPixel> pixels, TgaImageOrigin origin) where TPixel : unmanaged, IPixel<TPixel>
	{
		TPixel color = default(TPixel);
		bool flag = InvertX(origin);
		Guard.NotNull(tgaMetadata, "this.tgaMetadata");
		if (tgaMetadata.AlphaChannelBits == 8 && !flag)
		{
			using (IMemoryOwner<byte> buffer = memoryAllocator.AllocatePaddedPixelRowBuffer(width, 4, 0))
			{
				Span<byte> span = buffer.GetSpan();
				if (InvertY(origin))
				{
					for (int num = height - 1; num >= 0; num--)
					{
						ReadBgra32Row(stream, width, pixels, span, num);
					}
				}
				else
				{
					for (int i = 0; i < height; i++)
					{
						ReadBgra32Row(stream, width, pixels, span, i);
					}
				}
				return;
			}
		}
		Span<byte> scratchBuffer = stackalloc byte[4];
		for (int j = 0; j < height; j++)
		{
			int y = InvertY(j, height, origin);
			Span<TPixel> pixelRow = pixels.DangerousGetRowSpan(y);
			if (flag)
			{
				for (int num2 = width - 1; num2 >= 0; num2--)
				{
					ReadBgra32Pixel(stream, num2, color, pixelRow, scratchBuffer);
				}
			}
			else
			{
				for (int k = 0; k < width; k++)
				{
					ReadBgra32Pixel(stream, k, color, pixelRow, scratchBuffer);
				}
			}
		}
	}

	private void ReadRle<TPixel>(BufferedReadStream stream, int width, int height, Buffer2D<TPixel> pixels, int bytesPerPixel, TgaImageOrigin origin) where TPixel : unmanaged, IPixel<TPixel>
	{
		TPixel val = default(TPixel);
		Guard.NotNull(tgaMetadata, "this.tgaMetadata");
		byte alphaChannelBits = tgaMetadata.AlphaChannelBits;
		using IMemoryOwner<byte> buffer = memoryAllocator.Allocate<byte>(width * height * bytesPerPixel, AllocationOptions.Clean);
		Span<byte> span = buffer.GetSpan();
		UncompressRle(stream, width, height, span, bytesPerPixel);
		for (int i = 0; i < height; i++)
		{
			int y = InvertY(i, height, origin);
			Span<TPixel> span2 = pixels.DangerousGetRowSpan(y);
			int num = i * width * bytesPerPixel;
			for (int j = 0; j < width; j++)
			{
				int num2 = num + j * bytesPerPixel;
				switch (bytesPerPixel)
				{
				case 1:
					val.FromL8(Unsafe.As<byte, L8>(ref span[num2]));
					break;
				case 2:
					if (!hasAlpha)
					{
						span[num2 + 1] = (byte)(span[num2 + 1] | 0x80);
					}
					if (fileHeader.ImageType == TgaImageType.RleBlackAndWhite)
					{
						val.FromLa16(Unsafe.As<byte, La16>(ref span[num2]));
					}
					else
					{
						val.FromBgra5551(Unsafe.As<byte, Bgra5551>(ref span[num2]));
					}
					break;
				case 3:
					val.FromBgr24(Unsafe.As<byte, Bgr24>(ref span[num2]));
					break;
				case 4:
				{
					if (hasAlpha)
					{
						val.FromBgra32(Unsafe.As<byte, Bgra32>(ref span[num2]));
						break;
					}
					byte a = ((alphaChannelBits == 0) ? byte.MaxValue : span[num2 + 3]);
					val.FromBgra32(new Bgra32(span[num2 + 2], span[num2 + 1], span[num2], a));
					break;
				}
				}
				int index = InvertX(j, width, origin);
				span2[index] = val;
			}
		}
	}

	public ImageInfo Identify(BufferedReadStream stream, CancellationToken cancellationToken)
	{
		ReadFileHeader(stream);
		return new ImageInfo(new PixelTypeInfo(fileHeader.PixelDepth), new Size(fileHeader.Width, fileHeader.Height), metadata);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void ReadL8Row<TPixel>(BufferedReadStream stream, int width, Buffer2D<TPixel> pixels, Span<byte> row, int y) where TPixel : unmanaged, IPixel<TPixel>
	{
		if (stream.Read(row) != row.Length)
		{
			TgaThrowHelper.ThrowInvalidImageContentException("Not enough data to read a pixel row");
		}
		Span<TPixel> destinationPixels = pixels.DangerousGetRowSpan(y);
		PixelOperations<TPixel>.Instance.FromL8Bytes(configuration, row, destinationPixels, width);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void ReadL8Pixel<TPixel>(BufferedReadStream stream, TPixel color, int x, Span<TPixel> pixelSpan) where TPixel : unmanaged, IPixel<TPixel>
	{
		byte source = (byte)stream.ReadByte();
		color.FromL8(Unsafe.As<byte, L8>(ref source));
		pixelSpan[x] = color;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void ReadBgr24Pixel<TPixel>(BufferedReadStream stream, TPixel color, int x, Span<TPixel> pixelSpan, Span<byte> scratchBuffer) where TPixel : unmanaged, IPixel<TPixel>
	{
		if (stream.Read(scratchBuffer, 0, 3) != 3)
		{
			TgaThrowHelper.ThrowInvalidImageContentException("Not enough data to read a bgr pixel");
		}
		color.FromBgr24(Unsafe.As<byte, Bgr24>(ref MemoryMarshal.GetReference(scratchBuffer)));
		pixelSpan[x] = color;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void ReadBgr24Row<TPixel>(BufferedReadStream stream, int width, Buffer2D<TPixel> pixels, Span<byte> row, int y) where TPixel : unmanaged, IPixel<TPixel>
	{
		if (stream.Read(row) != row.Length)
		{
			TgaThrowHelper.ThrowInvalidImageContentException("Not enough data to read a pixel row");
		}
		Span<TPixel> destinationPixels = pixels.DangerousGetRowSpan(y);
		PixelOperations<TPixel>.Instance.FromBgr24Bytes(configuration, row, destinationPixels, width);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void ReadBgra32Pixel<TPixel>(BufferedReadStream stream, int x, TPixel color, Span<TPixel> pixelRow, Span<byte> scratchBuffer) where TPixel : unmanaged, IPixel<TPixel>
	{
		if (stream.Read(scratchBuffer, 0, 4) != 4)
		{
			TgaThrowHelper.ThrowInvalidImageContentException("Not enough data to read a bgra pixel");
		}
		Guard.NotNull(tgaMetadata, "this.tgaMetadata");
		byte a = ((tgaMetadata.AlphaChannelBits == 0) ? byte.MaxValue : scratchBuffer[3]);
		color.FromBgra32(new Bgra32(scratchBuffer[2], scratchBuffer[1], scratchBuffer[0], a));
		pixelRow[x] = color;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void ReadBgra32Row<TPixel>(BufferedReadStream stream, int width, Buffer2D<TPixel> pixels, Span<byte> row, int y) where TPixel : unmanaged, IPixel<TPixel>
	{
		if (stream.Read(row) != row.Length)
		{
			TgaThrowHelper.ThrowInvalidImageContentException("Not enough data to read a pixel row");
		}
		Span<TPixel> destinationPixels = pixels.DangerousGetRowSpan(y);
		PixelOperations<TPixel>.Instance.FromBgra32Bytes(configuration, row, destinationPixels, width);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void ReadPalettedBgra16Pixel<TPixel>(BufferedReadStream stream, Span<byte> palette, int colorMapPixelSizeInBytes, int x, TPixel color, Span<TPixel> pixelRow) where TPixel : unmanaged, IPixel<TPixel>
	{
		int num = stream.ReadByte();
		if (num == -1)
		{
			TgaThrowHelper.ThrowInvalidImageContentException("Not enough data to read color index");
		}
		ReadPalettedBgra16Pixel(palette, num, colorMapPixelSizeInBytes, ref color);
		pixelRow[x] = color;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void ReadPalettedBgra16Pixel<TPixel>(Span<byte> palette, int index, int colorMapPixelSizeInBytes, ref TPixel color) where TPixel : unmanaged, IPixel<TPixel>
	{
		Bgra5551 source = default(Bgra5551);
		source.FromBgra5551(Unsafe.As<byte, Bgra5551>(ref palette[index * colorMapPixelSizeInBytes]));
		if (!hasAlpha)
		{
			source.PackedValue |= 32768;
		}
		color.FromBgra5551(source);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void ReadPalettedBgr24Pixel<TPixel>(BufferedReadStream stream, Span<byte> palette, int colorMapPixelSizeInBytes, int x, TPixel color, Span<TPixel> pixelRow) where TPixel : unmanaged, IPixel<TPixel>
	{
		int num = stream.ReadByte();
		if (num == -1)
		{
			TgaThrowHelper.ThrowInvalidImageContentException("Not enough data to read color index");
		}
		color.FromBgr24(Unsafe.As<byte, Bgr24>(ref palette[num * colorMapPixelSizeInBytes]));
		pixelRow[x] = color;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void ReadPalettedBgra32Pixel<TPixel>(BufferedReadStream stream, Span<byte> palette, int colorMapPixelSizeInBytes, int x, TPixel color, Span<TPixel> pixelRow) where TPixel : unmanaged, IPixel<TPixel>
	{
		int num = stream.ReadByte();
		if (num == -1)
		{
			TgaThrowHelper.ThrowInvalidImageContentException("Not enough data to read color index");
		}
		color.FromBgra32(Unsafe.As<byte, Bgra32>(ref palette[num * colorMapPixelSizeInBytes]));
		pixelRow[x] = color;
	}

	private void UncompressRle(BufferedReadStream stream, int width, int height, Span<byte> buffer, int bytesPerPixel)
	{
		int num = 0;
		Span<byte> buffer2 = stackalloc byte[bytesPerPixel];
		int num2 = width * height;
		while (num < num2)
		{
			byte b = (byte)stream.ReadByte();
			if (b >> 7 == 1)
			{
				int num3 = b & 0x7F;
				if (stream.Read(buffer2) != bytesPerPixel)
				{
					TgaThrowHelper.ThrowInvalidImageContentException("Not enough data to read a pixel from the stream");
				}
				int num4 = num * bytesPerPixel;
				int num5 = 0;
				while (num5 < num3 + 1)
				{
					ref Span<byte> reference = ref buffer;
					int num6 = num4;
					buffer2.CopyTo(reference.Slice(num6, reference.Length - num6));
					num4 += bytesPerPixel;
					num5++;
					num++;
				}
				continue;
			}
			int num7 = b;
			int num8 = num * bytesPerPixel;
			int num9 = 0;
			while (num9 < num7 + 1)
			{
				if (stream.Read(buffer2) != bytesPerPixel)
				{
					TgaThrowHelper.ThrowInvalidImageContentException("Not enough data to read a pixel from the stream");
				}
				ref Span<byte> reference = ref buffer;
				int num6 = num8;
				buffer2.CopyTo(reference.Slice(num6, reference.Length - num6));
				num8 += bytesPerPixel;
				num9++;
				num++;
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int InvertY(int y, int height, TgaImageOrigin origin)
	{
		if (InvertY(origin))
		{
			return height - y - 1;
		}
		return y;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool InvertY(TgaImageOrigin origin)
	{
		return origin switch
		{
			TgaImageOrigin.BottomLeft => true, 
			TgaImageOrigin.BottomRight => true, 
			_ => false, 
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int InvertX(int x, int width, TgaImageOrigin origin)
	{
		if (InvertX(origin))
		{
			return width - x - 1;
		}
		return x;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool InvertX(TgaImageOrigin origin)
	{
		return origin switch
		{
			TgaImageOrigin.TopRight => true, 
			TgaImageOrigin.BottomRight => true, 
			_ => false, 
		};
	}

	[MemberNotNull("metadata")]
	[MemberNotNull("tgaMetadata")]
	private TgaImageOrigin ReadFileHeader(BufferedReadStream stream)
	{
		Span<byte> span = stackalloc byte[18];
		stream.Read(span, 0, 18);
		fileHeader = TgaFileHeader.Parse(span);
		metadata = new ImageMetadata();
		tgaMetadata = metadata.GetTgaMetadata();
		tgaMetadata.BitsPerPixel = (TgaBitsPerPixel)fileHeader.PixelDepth;
		int num = fileHeader.ImageDescriptor & 0xF;
		if (num != 0 && num != 1 && num != 8)
		{
			TgaThrowHelper.ThrowInvalidImageContentException("Invalid alpha channel bits");
		}
		tgaMetadata.AlphaChannelBits = (byte)num;
		hasAlpha = num > 0;
		return (TgaImageOrigin)((fileHeader.ImageDescriptor & 0x30) >> 4);
	}
}
