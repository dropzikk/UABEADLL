using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using SixLabors.ImageSharp.Common.Helpers;
using SixLabors.ImageSharp.IO;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.Metadata;
using SixLabors.ImageSharp.Metadata.Profiles.Icc;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Bmp;

internal sealed class BmpDecoderCore : IImageDecoderInternals
{
	private const int DefaultRgb16RMask = 31744;

	private const int DefaultRgb16GMask = 992;

	private const int DefaultRgb16BMask = 31;

	private const int RleCommand = 0;

	private const int RleEndOfLine = 0;

	private const int RleEndOfBitmap = 1;

	private const int RleDelta = 2;

	private ImageMetadata? metadata;

	private BmpMetadata? bmpMetadata;

	private BmpFileHeader fileHeader;

	private BmpFileMarkerType fileMarkerType;

	private BmpInfoHeader infoHeader;

	private readonly Configuration configuration;

	private readonly MemoryAllocator memoryAllocator;

	private readonly RleSkippedPixelHandling rleSkippedPixelHandling;

	public DecoderOptions Options { get; }

	public Size Dimensions => new Size(infoHeader.Width, infoHeader.Height);

	public BmpDecoderCore(BmpDecoderOptions options)
	{
		Options = options.GeneralOptions;
		rleSkippedPixelHandling = options.RleSkippedPixelHandling;
		configuration = options.GeneralOptions.Configuration;
		memoryAllocator = configuration.MemoryAllocator;
	}

	public Image<TPixel> Decode<TPixel>(BufferedReadStream stream, CancellationToken cancellationToken) where TPixel : unmanaged, IPixel<TPixel>
	{
		Image<TPixel> image = null;
		try
		{
			bool inverted;
			byte[] palette;
			int bytesPerColorMapEntry = ReadImageHeaders(stream, out inverted, out palette);
			image = new Image<TPixel>(configuration, infoHeader.Width, infoHeader.Height, metadata);
			Buffer2D<TPixel> rootFramePixelBuffer = image.GetRootFramePixelBuffer();
			switch (infoHeader.Compression)
			{
			case BmpCompression.RGB:
				if (infoHeader.BitsPerPixel == 32)
				{
					if (bmpMetadata.InfoHeaderType == BmpInfoHeaderType.WinVersion3)
					{
						ReadRgb32Slow(stream, rootFramePixelBuffer, infoHeader.Width, infoHeader.Height, inverted);
					}
					else
					{
						ReadRgb32Fast(stream, rootFramePixelBuffer, infoHeader.Width, infoHeader.Height, inverted);
					}
				}
				else if (infoHeader.BitsPerPixel == 24)
				{
					ReadRgb24(stream, rootFramePixelBuffer, infoHeader.Width, infoHeader.Height, inverted);
				}
				else if (infoHeader.BitsPerPixel == 16)
				{
					ReadRgb16(stream, rootFramePixelBuffer, infoHeader.Width, infoHeader.Height, inverted);
				}
				else if (infoHeader.BitsPerPixel <= 8)
				{
					ReadRgbPalette(stream, rootFramePixelBuffer, palette, infoHeader.Width, infoHeader.Height, infoHeader.BitsPerPixel, bytesPerColorMapEntry, inverted);
				}
				break;
			case BmpCompression.RLE24:
				ReadRle24(stream, rootFramePixelBuffer, infoHeader.Width, infoHeader.Height, inverted);
				break;
			case BmpCompression.RLE8:
			case BmpCompression.RLE4:
				ReadRle(stream, infoHeader.Compression, rootFramePixelBuffer, palette, infoHeader.Width, infoHeader.Height, inverted);
				break;
			case BmpCompression.BitFields:
			case BmpCompression.BI_ALPHABITFIELDS:
				ReadBitFields(stream, rootFramePixelBuffer, inverted);
				break;
			default:
				BmpThrowHelper.ThrowNotSupportedException("ImageSharp does not support this kind of bitmap files.");
				break;
			}
			return image;
		}
		catch (IndexOutOfRangeException innerException)
		{
			image?.Dispose();
			throw new ImageFormatException("Bitmap does not have a valid format.", innerException);
		}
		catch
		{
			image?.Dispose();
			throw;
		}
	}

	public ImageInfo Identify(BufferedReadStream stream, CancellationToken cancellationToken)
	{
		ReadImageHeaders(stream, out bool _, out byte[] _);
		return new ImageInfo(new PixelTypeInfo(infoHeader.BitsPerPixel), new Size(infoHeader.Width, infoHeader.Height), metadata);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int Invert(int y, int height, bool inverted)
	{
		if (inverted)
		{
			return y;
		}
		return height - y - 1;
	}

	private static int CalculatePadding(int width, int componentCount)
	{
		int num = width * componentCount % 4;
		if (num != 0)
		{
			num = 4 - num;
		}
		return num;
	}

	private void ReadBitFields<TPixel>(BufferedReadStream stream, Buffer2D<TPixel> pixels, bool inverted) where TPixel : unmanaged, IPixel<TPixel>
	{
		if (infoHeader.BitsPerPixel == 16)
		{
			ReadRgb16(stream, pixels, infoHeader.Width, infoHeader.Height, inverted, infoHeader.RedMask, infoHeader.GreenMask, infoHeader.BlueMask);
		}
		else
		{
			ReadRgb32BitFields(stream, pixels, infoHeader.Width, infoHeader.Height, inverted, infoHeader.RedMask, infoHeader.GreenMask, infoHeader.BlueMask, infoHeader.AlphaMask);
		}
	}

	private void ReadRle<TPixel>(BufferedReadStream stream, BmpCompression compression, Buffer2D<TPixel> pixels, byte[] colors, int width, int height, bool inverted) where TPixel : unmanaged, IPixel<TPixel>
	{
		TPixel val = default(TPixel);
		using IMemoryOwner<byte> memoryOwner3 = memoryAllocator.Allocate<byte>(width * height, AllocationOptions.Clean);
		using IMemoryOwner<bool> memoryOwner2 = memoryAllocator.Allocate<bool>(width * height, AllocationOptions.Clean);
		using IMemoryOwner<bool> memoryOwner = memoryAllocator.Allocate<bool>(height, AllocationOptions.Clean);
		Memory<bool> memory = memoryOwner.Memory;
		Span<bool> span = memory.Span;
		memory = memoryOwner2.Memory;
		Span<bool> span2 = memory.Span;
		Span<byte> span3 = memoryOwner3.Memory.Span;
		if (compression == BmpCompression.RLE8)
		{
			UncompressRle8(stream, width, span3, span2, span);
		}
		else
		{
			UncompressRle4(stream, width, span3, span2, span);
		}
		for (int i = 0; i < height; i++)
		{
			int y = Invert(i, height, inverted);
			int num = i * width;
			Span<byte> span4 = span3.Slice(num, width);
			Span<TPixel> span5 = pixels.DangerousGetRowSpan(y);
			if (span[i])
			{
				for (int j = 0; j < width; j++)
				{
					byte b = span4[j];
					if (span2[num + j])
					{
						switch (rleSkippedPixelHandling)
						{
						case RleSkippedPixelHandling.FirstColorOfPalette:
							val.FromBgr24(Unsafe.As<byte, Bgr24>(ref colors[b * 4]));
							break;
						case RleSkippedPixelHandling.Transparent:
							val.FromScaledVector4(Vector4.Zero);
							break;
						default:
							val.FromScaledVector4(new Vector4(0f, 0f, 0f, 1f));
							break;
						}
					}
					else
					{
						val.FromBgr24(Unsafe.As<byte, Bgr24>(ref colors[b * 4]));
					}
					span5[j] = val;
				}
			}
			else
			{
				for (int k = 0; k < width; k++)
				{
					val.FromBgr24(Unsafe.As<byte, Bgr24>(ref colors[span4[k] * 4]));
					span5[k] = val;
				}
			}
		}
	}

	private void ReadRle24<TPixel>(BufferedReadStream stream, Buffer2D<TPixel> pixels, int width, int height, bool inverted) where TPixel : unmanaged, IPixel<TPixel>
	{
		TPixel val = default(TPixel);
		using IMemoryOwner<byte> buffer = memoryAllocator.Allocate<byte>(width * height * 3, AllocationOptions.Clean);
		using IMemoryOwner<bool> memoryOwner2 = memoryAllocator.Allocate<bool>(width * height, AllocationOptions.Clean);
		using IMemoryOwner<bool> memoryOwner = memoryAllocator.Allocate<bool>(height, AllocationOptions.Clean);
		Memory<bool> memory = memoryOwner.Memory;
		Span<bool> span = memory.Span;
		memory = memoryOwner2.Memory;
		Span<bool> span2 = memory.Span;
		Span<byte> span3 = buffer.GetSpan();
		UncompressRle24(stream, width, span3, span2, span);
		for (int i = 0; i < height; i++)
		{
			int y = Invert(i, height, inverted);
			Span<TPixel> span4 = pixels.DangerousGetRowSpan(y);
			if (span[i])
			{
				int num = i * width;
				int num2 = num * 3;
				for (int j = 0; j < width; j++)
				{
					int index = num2 + j * 3;
					if (span2[num + j])
					{
						switch (rleSkippedPixelHandling)
						{
						case RleSkippedPixelHandling.FirstColorOfPalette:
							val.FromBgr24(Unsafe.As<byte, Bgr24>(ref span3[index]));
							break;
						case RleSkippedPixelHandling.Transparent:
							val.FromScaledVector4(Vector4.Zero);
							break;
						default:
							val.FromScaledVector4(new Vector4(0f, 0f, 0f, 1f));
							break;
						}
					}
					else
					{
						val.FromBgr24(Unsafe.As<byte, Bgr24>(ref span3[index]));
					}
					span4[j] = val;
				}
			}
			else
			{
				int num3 = i * width * 3;
				for (int k = 0; k < width; k++)
				{
					int index2 = num3 + k * 3;
					val.FromBgr24(Unsafe.As<byte, Bgr24>(ref span3[index2]));
					span4[k] = val;
				}
			}
		}
	}

	private void UncompressRle4(BufferedReadStream stream, int w, Span<byte> buffer, Span<bool> undefinedPixels, Span<bool> rowsWithUndefinedPixels)
	{
		Span<byte> span = stackalloc byte[128];
		Span<byte> buffer2 = stackalloc byte[2];
		int num = 0;
		while (num < buffer.Length)
		{
			if (stream.Read(buffer2, 0, buffer2.Length) != 2)
			{
				BmpThrowHelper.ThrowInvalidImageContentException("Failed to read 2 bytes from the stream while uncompressing RLE4 bitmap.");
			}
			if (buffer2[0] == 0)
			{
				switch (buffer2[1])
				{
				case 1:
				{
					int skipPixelCount = buffer.Length - num;
					RleSkipEndOfBitmap(num, w, skipPixelCount, undefinedPixels, rowsWithUndefinedPixels);
					return;
				}
				case 0:
					num += RleSkipEndOfLine(num, w, undefinedPixels, rowsWithUndefinedPixels);
					continue;
				case 2:
				{
					int dx = stream.ReadByte();
					int dy = stream.ReadByte();
					num += RleSkipDelta(num, w, dx, dy, undefinedPixels, rowsWithUndefinedPixels);
					continue;
				}
				}
				int num2 = buffer2[1];
				int num3 = (int)((uint)(num2 + 1) / 2u);
				Span<byte> buffer3 = ((num3 <= 128) ? span.Slice(0, num3) : ((Span<byte>)new byte[num3]));
				stream.Read(buffer3);
				int num4 = 0;
				for (int i = 0; i < num2; i++)
				{
					byte b = buffer3[num4];
					if (i % 2 == 0)
					{
						buffer[num++] = (byte)((b >> 4) & 0xF);
						continue;
					}
					buffer[num++] = (byte)(b & 0xF);
					num4++;
				}
				int count = num3 & 1;
				stream.Skip(count);
				continue;
			}
			int num5 = buffer2[0];
			byte num6 = buffer2[1];
			byte b2 = (byte)(num6 & 0xF);
			byte b3 = (byte)((num6 >> 4) & 0xF);
			for (int j = 0; j < num5; j++)
			{
				if (j % 2 == 0)
				{
					buffer[num] = b3;
				}
				else
				{
					buffer[num] = b2;
				}
				num++;
			}
		}
	}

	private void UncompressRle8(BufferedReadStream stream, int w, Span<byte> buffer, Span<bool> undefinedPixels, Span<bool> rowsWithUndefinedPixels)
	{
		Span<byte> span = stackalloc byte[128];
		Span<byte> buffer2 = stackalloc byte[2];
		int i = 0;
		while (i < buffer.Length)
		{
			if (stream.Read(buffer2, 0, buffer2.Length) != 2)
			{
				BmpThrowHelper.ThrowInvalidImageContentException("Failed to read 2 bytes from stream while uncompressing RLE8 bitmap.");
			}
			if (buffer2[0] == 0)
			{
				switch (buffer2[1])
				{
				case 1:
				{
					int skipPixelCount = buffer.Length - i;
					RleSkipEndOfBitmap(i, w, skipPixelCount, undefinedPixels, rowsWithUndefinedPixels);
					return;
				}
				case 0:
					i += RleSkipEndOfLine(i, w, undefinedPixels, rowsWithUndefinedPixels);
					continue;
				case 2:
				{
					int dx = stream.ReadByte();
					int dy = stream.ReadByte();
					i += RleSkipDelta(i, w, dx, dy, undefinedPixels, rowsWithUndefinedPixels);
					continue;
				}
				}
				int num = buffer2[1];
				Span<byte> buffer3 = ((num <= 128) ? span.Slice(0, num) : ((Span<byte>)new byte[num]));
				stream.Read(buffer3);
				int num2 = i;
				buffer3.CopyTo(buffer.Slice(num2, buffer.Length - num2));
				i += num;
				int count = num & 1;
				stream.Skip(count);
			}
			else
			{
				int num3 = i + buffer2[0];
				byte b = buffer2[1];
				for (; i < num3; i++)
				{
					buffer[i] = b;
				}
			}
		}
	}

	private void UncompressRle24(BufferedReadStream stream, int w, Span<byte> buffer, Span<bool> undefinedPixels, Span<bool> rowsWithUndefinedPixels)
	{
		Span<byte> span = stackalloc byte[128];
		Span<byte> buffer2 = stackalloc byte[2];
		int i = 0;
		while (i < buffer.Length)
		{
			if (stream.Read(buffer2, 0, buffer2.Length) != 2)
			{
				BmpThrowHelper.ThrowInvalidImageContentException("Failed to read 2 bytes from stream while uncompressing RLE24 bitmap.");
			}
			if (buffer2[0] == 0)
			{
				switch (buffer2[1])
				{
				case 1:
				{
					int skipPixelCount = (buffer.Length - i * 3) / 3;
					RleSkipEndOfBitmap(i, w, skipPixelCount, undefinedPixels, rowsWithUndefinedPixels);
					return;
				}
				case 0:
					i += RleSkipEndOfLine(i, w, undefinedPixels, rowsWithUndefinedPixels);
					continue;
				case 2:
				{
					int dx = stream.ReadByte();
					int dy = stream.ReadByte();
					i += RleSkipDelta(i, w, dx, dy, undefinedPixels, rowsWithUndefinedPixels);
					continue;
				}
				}
				int num = buffer2[1];
				int num2 = num * 3;
				Span<byte> buffer3 = ((num2 <= 128) ? span.Slice(0, num2) : ((Span<byte>)new byte[num2]));
				stream.Read(buffer3);
				int num3 = i * 3;
				buffer3.CopyTo(buffer.Slice(num3, buffer.Length - num3));
				i += num;
				int count = num2 & 1;
				stream.Skip(count);
			}
			else
			{
				int num4 = i + buffer2[0];
				byte b = buffer2[1];
				byte b2 = (byte)stream.ReadByte();
				byte b3 = (byte)stream.ReadByte();
				int num5 = i * 3;
				for (; i < num4; i++)
				{
					buffer[num5++] = b;
					buffer[num5++] = b2;
					buffer[num5++] = b3;
				}
			}
		}
	}

	private static void RleSkipEndOfBitmap(int count, int w, int skipPixelCount, Span<bool> undefinedPixels, Span<bool> rowsWithUndefinedPixels)
	{
		for (int i = count; i < count + skipPixelCount; i++)
		{
			undefinedPixels[i] = true;
		}
		int num = count / w;
		int num2 = skipPixelCount / w - 1;
		int num3 = Math.Min(num + num2, rowsWithUndefinedPixels.Length - 1);
		for (int j = num; j <= num3; j++)
		{
			rowsWithUndefinedPixels[j] = true;
		}
	}

	private static int RleSkipEndOfLine(int count, int w, Span<bool> undefinedPixels, Span<bool> rowsWithUndefinedPixels)
	{
		rowsWithUndefinedPixels[count / w] = true;
		int num = count % w;
		if (num > 0)
		{
			int num2 = w - num;
			for (int i = count; i < count + num2; i++)
			{
				undefinedPixels[i] = true;
			}
			return num2;
		}
		return 0;
	}

	private static int RleSkipDelta(int count, int w, int dx, int dy, Span<bool> undefinedPixels, Span<bool> rowsWithUndefinedPixels)
	{
		int num = w * dy + dx;
		for (int i = count; i < count + num; i++)
		{
			undefinedPixels[i] = true;
		}
		int num2 = count / w;
		int num3 = Math.Min(num2 + dy, rowsWithUndefinedPixels.Length - 1);
		for (int j = num2; j <= num3; j++)
		{
			rowsWithUndefinedPixels[j] = true;
		}
		return num;
	}

	private void ReadRgbPalette<TPixel>(BufferedReadStream stream, Buffer2D<TPixel> pixels, byte[] colors, int width, int height, int bitsPerPixel, int bytesPerColorMapEntry, bool inverted) where TPixel : unmanaged, IPixel<TPixel>
	{
		int num = 8 / bitsPerPixel;
		int num2 = (width + num - 1) / num;
		int num3 = 255 >> 8 - bitsPerPixel;
		int num4 = num2 % 4;
		if (num4 != 0)
		{
			num4 = 4 - num4;
		}
		using IMemoryOwner<byte> buffer = memoryAllocator.Allocate<byte>(num2 + num4, AllocationOptions.Clean);
		TPixel val = default(TPixel);
		Span<byte> span = buffer.GetSpan();
		for (int i = 0; i < height; i++)
		{
			int y = Invert(i, height, inverted);
			if (stream.Read(span) == 0)
			{
				BmpThrowHelper.ThrowInvalidImageContentException("Could not read enough data for a pixel row!");
			}
			int num5 = 0;
			Span<TPixel> span2 = pixels.DangerousGetRowSpan(y);
			for (int j = 0; j < num2; j++)
			{
				int num6 = j * num;
				int num7 = 0;
				int num8 = num6;
				while (num7 < num && num8 < width)
				{
					int num9 = ((span[num5] >> 8 - bitsPerPixel - num7 * bitsPerPixel) & num3) * bytesPerColorMapEntry;
					val.FromBgr24(Unsafe.As<byte, Bgr24>(ref colors[num9]));
					span2[num8] = val;
					num7++;
					num8++;
				}
				num5++;
			}
		}
	}

	private void ReadRgb16<TPixel>(BufferedReadStream stream, Buffer2D<TPixel> pixels, int width, int height, bool inverted, int redMask = 31744, int greenMask = 992, int blueMask = 31) where TPixel : unmanaged, IPixel<TPixel>
	{
		int num = CalculatePadding(width, 2);
		int length = width * 2 + num;
		TPixel val = default(TPixel);
		int num2 = CalculateRightShift((uint)redMask);
		int num3 = CalculateRightShift((uint)greenMask);
		int num4 = CalculateRightShift((uint)blueMask);
		int num5 = CountBits((uint)redMask);
		int num6 = CountBits((uint)greenMask);
		int num7 = CountBits((uint)blueMask);
		using IMemoryOwner<byte> buffer = memoryAllocator.Allocate<byte>(length);
		Span<byte> span = buffer.GetSpan();
		for (int i = 0; i < height; i++)
		{
			if (stream.Read(span) == 0)
			{
				BmpThrowHelper.ThrowInvalidImageContentException("Could not read enough data for a pixel row!");
			}
			int y = Invert(i, height, inverted);
			Span<TPixel> span2 = pixels.DangerousGetRowSpan(y);
			int num8 = 0;
			for (int j = 0; j < width; j++)
			{
				int num9 = num8;
				short num10 = BinaryPrimitives.ReadInt16LittleEndian(span.Slice(num9, span.Length - num9));
				int num11 = ((num5 == 5) ? GetBytesFrom5BitValue((num10 & redMask) >> num2) : GetBytesFrom6BitValue((num10 & redMask) >> num2));
				int num12 = ((num6 == 5) ? GetBytesFrom5BitValue((num10 & greenMask) >> num3) : GetBytesFrom6BitValue((num10 & greenMask) >> num3));
				int num13 = ((num7 == 5) ? GetBytesFrom5BitValue((num10 & blueMask) >> num4) : GetBytesFrom6BitValue((num10 & blueMask) >> num4));
				Rgb24 source = new Rgb24((byte)num11, (byte)num12, (byte)num13);
				val.FromRgb24(source);
				span2[j] = val;
				num8 += 2;
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static byte GetBytesFrom5BitValue(int value)
	{
		return (byte)((value << 3) | (value >> 2));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static byte GetBytesFrom6BitValue(int value)
	{
		return (byte)((value << 2) | (value >> 4));
	}

	private void ReadRgb24<TPixel>(BufferedReadStream stream, Buffer2D<TPixel> pixels, int width, int height, bool inverted) where TPixel : unmanaged, IPixel<TPixel>
	{
		int paddingInBytes = CalculatePadding(width, 3);
		using IMemoryOwner<byte> buffer = memoryAllocator.AllocatePaddedPixelRowBuffer(width, 3, paddingInBytes);
		Span<byte> span = buffer.GetSpan();
		for (int i = 0; i < height; i++)
		{
			if (stream.Read(span) == 0)
			{
				BmpThrowHelper.ThrowInvalidImageContentException("Could not read enough data for a pixel row!");
			}
			int y = Invert(i, height, inverted);
			Span<TPixel> destinationPixels = pixels.DangerousGetRowSpan(y);
			PixelOperations<TPixel>.Instance.FromBgr24Bytes(configuration, span, destinationPixels, width);
		}
	}

	private void ReadRgb32Fast<TPixel>(BufferedReadStream stream, Buffer2D<TPixel> pixels, int width, int height, bool inverted) where TPixel : unmanaged, IPixel<TPixel>
	{
		int paddingInBytes = CalculatePadding(width, 4);
		using IMemoryOwner<byte> buffer = memoryAllocator.AllocatePaddedPixelRowBuffer(width, 4, paddingInBytes);
		Span<byte> span = buffer.GetSpan();
		for (int i = 0; i < height; i++)
		{
			if (stream.Read(span) == 0)
			{
				BmpThrowHelper.ThrowInvalidImageContentException("Could not read enough data for a pixel row!");
			}
			int y = Invert(i, height, inverted);
			Span<TPixel> destinationPixels = pixels.DangerousGetRowSpan(y);
			PixelOperations<TPixel>.Instance.FromBgra32Bytes(configuration, span, destinationPixels, width);
		}
	}

	private void ReadRgb32Slow<TPixel>(BufferedReadStream stream, Buffer2D<TPixel> pixels, int width, int height, bool inverted) where TPixel : unmanaged, IPixel<TPixel>
	{
		int paddingInBytes = CalculatePadding(width, 4);
		using IMemoryOwner<byte> buffer = memoryAllocator.AllocatePaddedPixelRowBuffer(width, 4, paddingInBytes);
		using IMemoryOwner<Bgra32> buffer2 = memoryAllocator.Allocate<Bgra32>(width);
		Span<byte> span = buffer.GetSpan();
		Span<Bgra32> span2 = buffer2.GetSpan();
		long position = stream.Position;
		bool flag = false;
		for (int i = 0; i < height; i++)
		{
			if (stream.Read(span) == 0)
			{
				BmpThrowHelper.ThrowInvalidImageContentException("Could not read enough data for a pixel row!");
			}
			PixelOperations<Bgra32>.Instance.FromBgra32Bytes(configuration, span, span2, width);
			for (int j = 0; j < width; j++)
			{
				if (span2[j].A > 0)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				break;
			}
		}
		stream.Position = position;
		if (flag)
		{
			for (int k = 0; k < height; k++)
			{
				if (stream.Read(span) == 0)
				{
					BmpThrowHelper.ThrowInvalidImageContentException("Could not read enough data for a pixel row!");
				}
				int y = Invert(k, height, inverted);
				Span<TPixel> destinationPixels = pixels.DangerousGetRowSpan(y);
				PixelOperations<TPixel>.Instance.FromBgra32Bytes(configuration, span, destinationPixels, width);
			}
			return;
		}
		for (int l = 0; l < height; l++)
		{
			if (stream.Read(span) == 0)
			{
				BmpThrowHelper.ThrowInvalidImageContentException("Could not read enough data for a pixel row!");
			}
			PixelOperations<Bgra32>.Instance.FromBgra32Bytes(configuration, span, span2, width);
			int y2 = Invert(l, height, inverted);
			Span<TPixel> span3 = pixels.DangerousGetRowSpan(y2);
			for (int m = 0; m < width; m++)
			{
				Bgra32 source = span2[m];
				source.A = byte.MaxValue;
				span3[m].FromBgra32(source);
			}
		}
	}

	private void ReadRgb32BitFields<TPixel>(BufferedReadStream stream, Buffer2D<TPixel> pixels, int width, int height, bool inverted, int redMask, int greenMask, int blueMask, int alphaMask) where TPixel : unmanaged, IPixel<TPixel>
	{
		TPixel val = default(TPixel);
		int num = CalculatePadding(width, 4);
		int length = width * 4 + num;
		int num2 = CalculateRightShift((uint)redMask);
		int num3 = CalculateRightShift((uint)greenMask);
		int num4 = CalculateRightShift((uint)blueMask);
		int num5 = CalculateRightShift((uint)alphaMask);
		int num6 = CountBits((uint)redMask);
		int num7 = CountBits((uint)greenMask);
		int num8 = CountBits((uint)blueMask);
		int num9 = CountBits((uint)alphaMask);
		float num10 = 1f / (float)(uint.MaxValue >> 32 - num6);
		float num11 = 1f / (float)(uint.MaxValue >> 32 - num7);
		float num12 = 1f / (float)(uint.MaxValue >> 32 - num8);
		uint num13 = uint.MaxValue >> 32 - num9;
		float num14 = 1f / (float)num13;
		bool flag = num6 > 8 || num7 > 8 || num8 > 8 || num14 > 8f;
		using IMemoryOwner<byte> buffer = memoryAllocator.Allocate<byte>(length);
		Span<byte> span = buffer.GetSpan();
		for (int i = 0; i < height; i++)
		{
			if (stream.Read(span) == 0)
			{
				BmpThrowHelper.ThrowInvalidImageContentException("Could not read enough data for a pixel row!");
			}
			int y = Invert(i, height, inverted);
			Span<TPixel> span2 = pixels.DangerousGetRowSpan(y);
			int num15 = 0;
			for (int j = 0; j < width; j++)
			{
				int num16 = num15;
				uint num17 = BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(num16, span.Length - num16));
				if (flag)
				{
					uint num18 = (uint)(num17 & redMask) >> num2;
					uint num19 = (uint)(num17 & greenMask) >> num3;
					uint num20 = (uint)(num17 & blueMask) >> num4;
					float w = ((alphaMask != 0) ? (num14 * (float)((uint)(num17 & alphaMask) >> num5)) : 1f);
					Vector4 vector = new Vector4((float)num18 * num10, (float)num19 * num11, (float)num20 * num12, w);
					val.FromScaledVector4(vector);
				}
				else
				{
					byte r = (byte)((num17 & redMask) >> num2);
					byte g = (byte)((num17 & greenMask) >> num3);
					byte b = (byte)((num17 & blueMask) >> num4);
					byte a = ((alphaMask != 0) ? ((byte)((num17 & alphaMask) >> num5)) : byte.MaxValue);
					val.FromRgba32(new Rgba32(r, g, b, a));
				}
				span2[j] = val;
				num15 += 4;
			}
		}
	}

	private static int CalculateRightShift(uint n)
	{
		int num = 0;
		while (n != 0 && (1 & n) == 0)
		{
			num++;
			n >>= 1;
		}
		return num;
	}

	private static int CountBits(uint n)
	{
		int num = 0;
		while (n != 0)
		{
			num++;
			n &= n - 1;
		}
		return num;
	}

	[MemberNotNull("metadata")]
	[MemberNotNull("bmpMetadata")]
	private void ReadInfoHeader(BufferedReadStream stream)
	{
		Span<byte> span = stackalloc byte[124];
		long position = stream.Position;
		metadata = new ImageMetadata
		{
			ResolutionUnits = PixelResolutionUnit.PixelsPerMeter
		};
		stream.Read(span, 0, 4);
		int num = BinaryPrimitives.ReadInt32LittleEndian(span);
		if ((num < 12 || num > 124) ? true : false)
		{
			BmpThrowHelper.ThrowNotSupportedException($"ImageSharp does not support this BMP file. HeaderSize is '{num}'.");
		}
		stream.Read(span, 4, num - 4);
		BmpInfoHeaderType infoHeaderType = BmpInfoHeaderType.WinVersion2;
		if (num == 12)
		{
			infoHeaderType = BmpInfoHeaderType.WinVersion2;
			infoHeader = BmpInfoHeader.ParseCore(span);
		}
		else if (num == 16)
		{
			infoHeaderType = BmpInfoHeaderType.Os2Version2Short;
			infoHeader = BmpInfoHeader.ParseOs22Short(span);
		}
		else if (num == 40)
		{
			infoHeaderType = BmpInfoHeaderType.WinVersion3;
			infoHeader = BmpInfoHeader.ParseV3(span);
			if (infoHeader.Compression == BmpCompression.BitFields)
			{
				Span<byte> span2 = stackalloc byte[12];
				stream.Read(span2);
				Span<byte> span3 = span2;
				infoHeader.RedMask = BinaryPrimitives.ReadInt32LittleEndian(span3.Slice(0, 4));
				infoHeader.GreenMask = BinaryPrimitives.ReadInt32LittleEndian(span3.Slice(4, 4));
				infoHeader.BlueMask = BinaryPrimitives.ReadInt32LittleEndian(span3.Slice(8, 4));
			}
			else if (infoHeader.Compression == BmpCompression.BI_ALPHABITFIELDS)
			{
				Span<byte> span4 = stackalloc byte[16];
				stream.Read(span4);
				Span<byte> span5 = span4;
				infoHeader.RedMask = BinaryPrimitives.ReadInt32LittleEndian(span5.Slice(0, 4));
				infoHeader.GreenMask = BinaryPrimitives.ReadInt32LittleEndian(span5.Slice(4, 4));
				infoHeader.BlueMask = BinaryPrimitives.ReadInt32LittleEndian(span5.Slice(8, 4));
				infoHeader.AlphaMask = BinaryPrimitives.ReadInt32LittleEndian(span5.Slice(12, 4));
			}
		}
		else if (num == 52)
		{
			infoHeaderType = BmpInfoHeaderType.AdobeVersion3;
			infoHeader = BmpInfoHeader.ParseAdobeV3(span, withAlpha: false);
		}
		else if (num == 56)
		{
			infoHeaderType = BmpInfoHeaderType.AdobeVersion3WithAlpha;
			infoHeader = BmpInfoHeader.ParseAdobeV3(span);
		}
		else if (num == 64)
		{
			infoHeaderType = BmpInfoHeaderType.Os2Version2;
			infoHeader = BmpInfoHeader.ParseOs2Version2(span);
		}
		else if (num == 108)
		{
			infoHeaderType = BmpInfoHeaderType.WinVersion4;
			infoHeader = BmpInfoHeader.ParseV4(span);
		}
		else if (num > 108)
		{
			infoHeaderType = BmpInfoHeaderType.WinVersion5;
			infoHeader = BmpInfoHeader.ParseV5(span);
			if (infoHeader.ProfileData != 0 && infoHeader.ProfileSize != 0)
			{
				long position2 = stream.Position;
				byte[] array = new byte[infoHeader.ProfileSize];
				stream.Position = position + infoHeader.ProfileData;
				stream.Read(array);
				metadata.IccProfile = new IccProfile(array);
				stream.Position = position2;
			}
		}
		else
		{
			BmpThrowHelper.ThrowNotSupportedException($"ImageSharp does not support this BMP file. HeaderSize '{num}'.");
		}
		if (infoHeader.XPelsPerMeter > 0 && infoHeader.YPelsPerMeter > 0)
		{
			metadata.HorizontalResolution = infoHeader.XPelsPerMeter;
			metadata.VerticalResolution = infoHeader.YPelsPerMeter;
		}
		else
		{
			metadata.HorizontalResolution = Math.Round(UnitConverter.InchToMeter(96.0));
			metadata.VerticalResolution = Math.Round(UnitConverter.InchToMeter(96.0));
		}
		ushort bitsPerPixel = infoHeader.BitsPerPixel;
		bmpMetadata = metadata.GetBmpMetadata();
		bmpMetadata.InfoHeaderType = infoHeaderType;
		bmpMetadata.BitsPerPixel = (BmpBitsPerPixel)bitsPerPixel;
	}

	private void ReadFileHeader(BufferedReadStream stream)
	{
		Span<byte> span = stackalloc byte[14];
		stream.Read(span, 0, 14);
		short num = BinaryPrimitives.ReadInt16LittleEndian(span);
		switch (num)
		{
		case 19778:
			fileMarkerType = BmpFileMarkerType.Bitmap;
			fileHeader = BmpFileHeader.Parse(span);
			break;
		case 16706:
			fileMarkerType = BmpFileMarkerType.BitmapArray;
			stream.Read(span, 0, 14);
			fileHeader = BmpFileHeader.Parse(span);
			if (fileHeader.Type != 19778)
			{
				BmpThrowHelper.ThrowNotSupportedException($"Unsupported bitmap file inside a BitmapArray file. File header bitmap type marker '{fileHeader.Type}'.");
			}
			break;
		default:
			BmpThrowHelper.ThrowNotSupportedException($"ImageSharp does not support this BMP file. File header bitmap type marker '{num}'.");
			break;
		}
	}

	[MemberNotNull("metadata")]
	[MemberNotNull("bmpMetadata")]
	private int ReadImageHeaders(BufferedReadStream stream, out bool inverted, out byte[] palette)
	{
		ReadFileHeader(stream);
		ReadInfoHeader(stream);
		inverted = false;
		if (infoHeader.Height < 0)
		{
			inverted = true;
			infoHeader.Height = -infoHeader.Height;
		}
		int num = 4;
		int num2 = -1;
		if (infoHeader.ClrUsed == 0)
		{
			ushort bitsPerPixel = infoHeader.BitsPerPixel;
			if (((uint)(bitsPerPixel - 1) <= 1u || bitsPerPixel == 4 || bitsPerPixel == 8) ? true : false)
			{
				switch (fileMarkerType)
				{
				case BmpFileMarkerType.Bitmap:
				{
					num2 = fileHeader.Offset - 14 - infoHeader.HeaderSize;
					int colorCountForBitDepth = ColorNumerics.GetColorCountForBitDepth(infoHeader.BitsPerPixel);
					num = num2 / colorCountForBitDepth;
					num = Math.Max(num, 3);
					break;
				}
				case BmpFileMarkerType.BitmapArray:
				case BmpFileMarkerType.ColorIcon:
				case BmpFileMarkerType.ColorPointer:
				case BmpFileMarkerType.Icon:
				case BmpFileMarkerType.Pointer:
					num = 3;
					num2 = ColorNumerics.GetColorCountForBitDepth(infoHeader.BitsPerPixel) * num;
					break;
				}
			}
		}
		else
		{
			num2 = infoHeader.ClrUsed * num;
		}
		palette = Array.Empty<byte>();
		if (num2 > 0)
		{
			if (stream.Position > fileHeader.Offset - num2)
			{
				BmpThrowHelper.ThrowInvalidImageContentException($"Reading the color map would read beyond the bitmap offset. Either the color map size of '{num2}' is invalid or the bitmap offset.");
			}
			palette = new byte[num2];
			if (stream.Read(palette, 0, num2) == 0)
			{
				BmpThrowHelper.ThrowInvalidImageContentException("Could not read enough data for the palette!");
			}
		}
		int num3 = fileHeader.Offset - (int)stream.Position;
		if (num3 + (int)stream.Position > stream.Length)
		{
			BmpThrowHelper.ThrowInvalidImageContentException("Invalid file header offset found. Offset is greater than the stream length.");
		}
		if (num3 > 0)
		{
			stream.Skip(num3);
		}
		return num;
	}
}
