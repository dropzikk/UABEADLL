using System;
using System.Buffers;
using System.Buffers.Binary;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Tga;

internal sealed class TgaEncoderCore : IImageEncoderInternals
{
	private readonly MemoryAllocator memoryAllocator;

	private TgaBitsPerPixel? bitsPerPixel;

	private readonly TgaCompression compression;

	public TgaEncoderCore(TgaEncoder encoder, MemoryAllocator memoryAllocator)
	{
		this.memoryAllocator = memoryAllocator;
		bitsPerPixel = encoder.BitsPerPixel;
		compression = encoder.Compression;
	}

	public void Encode<TPixel>(Image<TPixel> image, Stream stream, CancellationToken cancellationToken) where TPixel : unmanaged, IPixel<TPixel>
	{
		Guard.NotNull(image, "image");
		Guard.NotNull(stream, "stream");
		TgaMetadata tgaMetadata = image.Metadata.GetTgaMetadata();
		TgaBitsPerPixel valueOrDefault = bitsPerPixel.GetValueOrDefault();
		if (!bitsPerPixel.HasValue)
		{
			valueOrDefault = tgaMetadata.BitsPerPixel;
			bitsPerPixel = valueOrDefault;
		}
		TgaImageType imageType = ((compression == TgaCompression.RunLength) ? TgaImageType.RleTrueColor : TgaImageType.TrueColor);
		if (bitsPerPixel == TgaBitsPerPixel.Pixel8)
		{
			imageType = ((compression == TgaCompression.RunLength) ? TgaImageType.RleBlackAndWhite : TgaImageType.BlackAndWhite);
		}
		byte b = 0;
		if (compression == TgaCompression.RunLength)
		{
			b |= 0x20;
		}
		TgaBitsPerPixel? tgaBitsPerPixel = bitsPerPixel;
		if (tgaBitsPerPixel.HasValue && tgaBitsPerPixel == TgaBitsPerPixel.Pixel32)
		{
			b |= 8;
		}
		tgaBitsPerPixel = bitsPerPixel;
		if (tgaBitsPerPixel.HasValue && tgaBitsPerPixel == TgaBitsPerPixel.Pixel16)
		{
			b |= 1;
		}
		TgaFileHeader tgaFileHeader = new TgaFileHeader(0, 0, imageType, 0, 0, 0, 0, (short)((compression == TgaCompression.RunLength) ? ((short)image.Height) : 0), (short)image.Width, (short)image.Height, (byte)bitsPerPixel.Value, b);
		Span<byte> buffer = stackalloc byte[18];
		tgaFileHeader.WriteTo(buffer);
		stream.Write(buffer, 0, 18);
		if (compression == TgaCompression.RunLength)
		{
			WriteRunLengthEncodedImage(stream, image.Frames.RootFrame);
		}
		else
		{
			WriteImage(image.Configuration, stream, image.Frames.RootFrame);
		}
		stream.Flush();
	}

	private void WriteImage<TPixel>(Configuration configuration, Stream stream, ImageFrame<TPixel> image) where TPixel : unmanaged, IPixel<TPixel>
	{
		Buffer2D<TPixel> pixelBuffer = image.PixelBuffer;
		TgaBitsPerPixel? tgaBitsPerPixel = bitsPerPixel;
		if (tgaBitsPerPixel.HasValue)
		{
			switch (tgaBitsPerPixel.GetValueOrDefault())
			{
			case TgaBitsPerPixel.Pixel8:
				Write8Bit(configuration, stream, pixelBuffer);
				break;
			case TgaBitsPerPixel.Pixel16:
				Write16Bit(configuration, stream, pixelBuffer);
				break;
			case TgaBitsPerPixel.Pixel24:
				Write24Bit(configuration, stream, pixelBuffer);
				break;
			case TgaBitsPerPixel.Pixel32:
				Write32Bit(configuration, stream, pixelBuffer);
				break;
			}
		}
	}

	private void WriteRunLengthEncodedImage<TPixel>(Stream stream, ImageFrame<TPixel> image) where TPixel : unmanaged, IPixel<TPixel>
	{
		Rgba32 dest = default(Rgba32);
		Buffer2D<TPixel> pixelBuffer = image.PixelBuffer;
		for (int i = 0; i < image.Height; i++)
		{
			Span<TPixel> pixelRow = pixelBuffer.DangerousGetRowSpan(i);
			int num = 0;
			while (num < image.Width)
			{
				TPixel currentPixel = pixelRow[num];
				currentPixel.ToRgba32(ref dest);
				byte b = FindEqualPixels(pixelRow, num);
				if (b > 0)
				{
					stream.WriteByte((byte)(b | 0x80));
					WritePixel(stream, currentPixel, dest);
					num += b + 1;
					continue;
				}
				byte b2 = FindUnEqualPixels(pixelRow, num);
				stream.WriteByte(b2);
				WritePixel(stream, currentPixel, dest);
				num++;
				for (int j = 0; j < b2; j++)
				{
					currentPixel = pixelRow[num];
					currentPixel.ToRgba32(ref dest);
					WritePixel(stream, currentPixel, dest);
					num++;
				}
			}
		}
	}

	private void WritePixel<TPixel>(Stream stream, TPixel currentPixel, Rgba32 color) where TPixel : unmanaged, IPixel<TPixel>
	{
		TgaBitsPerPixel? tgaBitsPerPixel = bitsPerPixel;
		if (tgaBitsPerPixel.HasValue)
		{
			switch (tgaBitsPerPixel.GetValueOrDefault())
			{
			case TgaBitsPerPixel.Pixel8:
			{
				int luminance = GetLuminance(currentPixel);
				stream.WriteByte((byte)luminance);
				break;
			}
			case TgaBitsPerPixel.Pixel16:
			{
				Bgra5551 bgra = new Bgra5551(color.ToVector4());
				Span<byte> destination = stackalloc byte[2];
				BinaryPrimitives.WriteInt16LittleEndian(destination, (short)bgra.PackedValue);
				stream.WriteByte(destination[0]);
				stream.WriteByte(destination[1]);
				break;
			}
			case TgaBitsPerPixel.Pixel24:
				stream.WriteByte(color.B);
				stream.WriteByte(color.G);
				stream.WriteByte(color.R);
				break;
			case TgaBitsPerPixel.Pixel32:
				stream.WriteByte(color.B);
				stream.WriteByte(color.G);
				stream.WriteByte(color.R);
				stream.WriteByte(color.A);
				break;
			}
		}
	}

	private static byte FindEqualPixels<TPixel>(Span<TPixel> pixelRow, int xStart) where TPixel : unmanaged, IPixel<TPixel>
	{
		byte b = 0;
		TPixel val = pixelRow[xStart];
		for (int i = xStart + 1; i < pixelRow.Length; i++)
		{
			TPixel other = pixelRow[i];
			if (val.Equals(other))
			{
				b++;
				if (b >= 127)
				{
					return b;
				}
				continue;
			}
			return b;
		}
		return b;
	}

	private static byte FindUnEqualPixels<TPixel>(Span<TPixel> pixelRow, int xStart) where TPixel : unmanaged, IPixel<TPixel>
	{
		byte b = 0;
		TPixel val = pixelRow[xStart];
		for (int i = xStart + 1; i < pixelRow.Length; i++)
		{
			TPixel val2 = pixelRow[i];
			if (val.Equals(val2))
			{
				return b;
			}
			b++;
			if (b >= 127)
			{
				return b;
			}
			val = val2;
		}
		return b;
	}

	private IMemoryOwner<byte> AllocateRow(int width, int bytesPerPixel)
	{
		return memoryAllocator.AllocatePaddedPixelRowBuffer(width, bytesPerPixel, 0);
	}

	private void Write8Bit<TPixel>(Configuration configuration, Stream stream, Buffer2D<TPixel> pixels) where TPixel : unmanaged, IPixel<TPixel>
	{
		using IMemoryOwner<byte> buffer = AllocateRow(pixels.Width, 1);
		Span<byte> span = buffer.GetSpan();
		for (int num = pixels.Height - 1; num >= 0; num--)
		{
			Span<TPixel> span2 = pixels.DangerousGetRowSpan(num);
			PixelOperations<TPixel>.Instance.ToL8Bytes(configuration, span2, span, span2.Length);
			stream.Write(span);
		}
	}

	private void Write16Bit<TPixel>(Configuration configuration, Stream stream, Buffer2D<TPixel> pixels) where TPixel : unmanaged, IPixel<TPixel>
	{
		using IMemoryOwner<byte> buffer = AllocateRow(pixels.Width, 2);
		Span<byte> span = buffer.GetSpan();
		for (int num = pixels.Height - 1; num >= 0; num--)
		{
			Span<TPixel> span2 = pixels.DangerousGetRowSpan(num);
			PixelOperations<TPixel>.Instance.ToBgra5551Bytes(configuration, span2, span, span2.Length);
			stream.Write(span);
		}
	}

	private void Write24Bit<TPixel>(Configuration configuration, Stream stream, Buffer2D<TPixel> pixels) where TPixel : unmanaged, IPixel<TPixel>
	{
		using IMemoryOwner<byte> buffer = AllocateRow(pixels.Width, 3);
		Span<byte> span = buffer.GetSpan();
		for (int num = pixels.Height - 1; num >= 0; num--)
		{
			Span<TPixel> span2 = pixels.DangerousGetRowSpan(num);
			PixelOperations<TPixel>.Instance.ToBgr24Bytes(configuration, span2, span, span2.Length);
			stream.Write(span);
		}
	}

	private void Write32Bit<TPixel>(Configuration configuration, Stream stream, Buffer2D<TPixel> pixels) where TPixel : unmanaged, IPixel<TPixel>
	{
		using IMemoryOwner<byte> buffer = AllocateRow(pixels.Width, 4);
		Span<byte> span = buffer.GetSpan();
		for (int num = pixels.Height - 1; num >= 0; num--)
		{
			Span<TPixel> span2 = pixels.DangerousGetRowSpan(num);
			PixelOperations<TPixel>.Instance.ToBgra32Bytes(configuration, span2, span, span2.Length);
			stream.Write(span);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetLuminance<TPixel>(TPixel sourcePixel) where TPixel : unmanaged, IPixel<TPixel>
	{
		Vector4 vector = sourcePixel.ToVector4();
		return ColorNumerics.GetBT709Luminance(ref vector, 256);
	}
}
