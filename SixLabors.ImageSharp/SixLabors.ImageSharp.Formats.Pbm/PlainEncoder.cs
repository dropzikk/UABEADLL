using System;
using System.Buffers;
using System.Buffers.Text;
using System.IO;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Pbm;

internal class PlainEncoder
{
	private const byte NewLine = 10;

	private const byte Space = 32;

	private const byte Zero = 48;

	private const byte One = 49;

	private const int MaxCharsPerPixelBlackAndWhite = 2;

	private const int MaxCharsPerPixelGrayscale = 4;

	private const int MaxCharsPerPixelGrayscaleWide = 6;

	private const int MaxCharsPerPixelRgb = 12;

	private const int MaxCharsPerPixelRgbWide = 18;

	private static readonly StandardFormat DecimalFormat = StandardFormat.Parse("D");

	public static void WritePixels<TPixel>(Configuration configuration, Stream stream, ImageFrame<TPixel> image, PbmColorType colorType, PbmComponentType componentType) where TPixel : unmanaged, IPixel<TPixel>
	{
		switch (colorType)
		{
		case PbmColorType.Grayscale:
			if (componentType == PbmComponentType.Byte)
			{
				WriteGrayscale(configuration, stream, image);
			}
			else
			{
				WriteWideGrayscale(configuration, stream, image);
			}
			break;
		case PbmColorType.Rgb:
			if (componentType == PbmComponentType.Byte)
			{
				WriteRgb(configuration, stream, image);
			}
			else
			{
				WriteWideRgb(configuration, stream, image);
			}
			break;
		default:
			WriteBlackAndWhite(configuration, stream, image);
			break;
		}
		stream.WriteByte(32);
	}

	private static void WriteGrayscale<TPixel>(Configuration configuration, Stream stream, ImageFrame<TPixel> image) where TPixel : unmanaged, IPixel<TPixel>
	{
		int width = image.Width;
		int height = image.Height;
		Buffer2D<TPixel> pixelBuffer = image.PixelBuffer;
		MemoryAllocator memoryAllocator = configuration.MemoryAllocator;
		using IMemoryOwner<L8> buffer = memoryAllocator.Allocate<L8>(width);
		Span<L8> span = buffer.GetSpan();
		using IMemoryOwner<byte> buffer2 = memoryAllocator.Allocate<byte>(width * 4);
		Span<byte> span2 = buffer2.GetSpan();
		for (int i = 0; i < height; i++)
		{
			Span<TPixel> span3 = pixelBuffer.DangerousGetRowSpan(i);
			PixelOperations<TPixel>.Instance.ToL8(configuration, span3, span);
			int num = 0;
			for (int j = 0; j < width; j++)
			{
				byte packedValue = span[j].PackedValue;
				int num2 = num;
				Utf8Formatter.TryFormat(packedValue, span2.Slice(num2, span2.Length - num2), out var bytesWritten, DecimalFormat);
				num += bytesWritten;
				span2[num++] = 32;
			}
			span2[num - 1] = 10;
			stream.Write(span2, 0, num);
		}
	}

	private static void WriteWideGrayscale<TPixel>(Configuration configuration, Stream stream, ImageFrame<TPixel> image) where TPixel : unmanaged, IPixel<TPixel>
	{
		int width = image.Width;
		int height = image.Height;
		Buffer2D<TPixel> pixelBuffer = image.PixelBuffer;
		MemoryAllocator memoryAllocator = configuration.MemoryAllocator;
		using IMemoryOwner<L16> buffer = memoryAllocator.Allocate<L16>(width);
		Span<L16> span = buffer.GetSpan();
		using IMemoryOwner<byte> buffer2 = memoryAllocator.Allocate<byte>(width * 6);
		Span<byte> span2 = buffer2.GetSpan();
		for (int i = 0; i < height; i++)
		{
			Span<TPixel> span3 = pixelBuffer.DangerousGetRowSpan(i);
			PixelOperations<TPixel>.Instance.ToL16(configuration, span3, span);
			int num = 0;
			for (int j = 0; j < width; j++)
			{
				ushort packedValue = span[j].PackedValue;
				int num2 = num;
				Utf8Formatter.TryFormat(packedValue, span2.Slice(num2, span2.Length - num2), out var bytesWritten, DecimalFormat);
				num += bytesWritten;
				span2[num++] = 32;
			}
			span2[num - 1] = 10;
			stream.Write(span2, 0, num);
		}
	}

	private static void WriteRgb<TPixel>(Configuration configuration, Stream stream, ImageFrame<TPixel> image) where TPixel : unmanaged, IPixel<TPixel>
	{
		int width = image.Width;
		int height = image.Height;
		Buffer2D<TPixel> pixelBuffer = image.PixelBuffer;
		MemoryAllocator memoryAllocator = configuration.MemoryAllocator;
		using IMemoryOwner<Rgb24> buffer = memoryAllocator.Allocate<Rgb24>(width);
		Span<Rgb24> span = buffer.GetSpan();
		using IMemoryOwner<byte> buffer2 = memoryAllocator.Allocate<byte>(width * 12);
		Span<byte> span2 = buffer2.GetSpan();
		for (int i = 0; i < height; i++)
		{
			Span<TPixel> span3 = pixelBuffer.DangerousGetRowSpan(i);
			PixelOperations<TPixel>.Instance.ToRgb24(configuration, span3, span);
			int num = 0;
			for (int j = 0; j < width; j++)
			{
				byte r = span[j].R;
				ref Span<byte> reference = ref span2;
				int num2 = num;
				Utf8Formatter.TryFormat(r, reference.Slice(num2, reference.Length - num2), out var bytesWritten, DecimalFormat);
				num += bytesWritten;
				span2[num++] = 32;
				byte g = span[j].G;
				reference = ref span2;
				num2 = num;
				Utf8Formatter.TryFormat(g, reference.Slice(num2, reference.Length - num2), out bytesWritten, DecimalFormat);
				num += bytesWritten;
				span2[num++] = 32;
				byte b = span[j].B;
				reference = ref span2;
				num2 = num;
				Utf8Formatter.TryFormat(b, reference.Slice(num2, reference.Length - num2), out bytesWritten, DecimalFormat);
				num += bytesWritten;
				span2[num++] = 32;
			}
			span2[num - 1] = 10;
			stream.Write(span2, 0, num);
		}
	}

	private static void WriteWideRgb<TPixel>(Configuration configuration, Stream stream, ImageFrame<TPixel> image) where TPixel : unmanaged, IPixel<TPixel>
	{
		int width = image.Width;
		int height = image.Height;
		Buffer2D<TPixel> pixelBuffer = image.PixelBuffer;
		MemoryAllocator memoryAllocator = configuration.MemoryAllocator;
		using IMemoryOwner<Rgb48> buffer = memoryAllocator.Allocate<Rgb48>(width);
		Span<Rgb48> span = buffer.GetSpan();
		using IMemoryOwner<byte> buffer2 = memoryAllocator.Allocate<byte>(width * 18);
		Span<byte> span2 = buffer2.GetSpan();
		for (int i = 0; i < height; i++)
		{
			Span<TPixel> span3 = pixelBuffer.DangerousGetRowSpan(i);
			PixelOperations<TPixel>.Instance.ToRgb48(configuration, span3, span);
			int num = 0;
			for (int j = 0; j < width; j++)
			{
				ushort r = span[j].R;
				ref Span<byte> reference = ref span2;
				int num2 = num;
				Utf8Formatter.TryFormat(r, reference.Slice(num2, reference.Length - num2), out var bytesWritten, DecimalFormat);
				num += bytesWritten;
				span2[num++] = 32;
				ushort g = span[j].G;
				reference = ref span2;
				num2 = num;
				Utf8Formatter.TryFormat(g, reference.Slice(num2, reference.Length - num2), out bytesWritten, DecimalFormat);
				num += bytesWritten;
				span2[num++] = 32;
				ushort b = span[j].B;
				reference = ref span2;
				num2 = num;
				Utf8Formatter.TryFormat(b, reference.Slice(num2, reference.Length - num2), out bytesWritten, DecimalFormat);
				num += bytesWritten;
				span2[num++] = 32;
			}
			span2[num - 1] = 10;
			stream.Write(span2, 0, num);
		}
	}

	private static void WriteBlackAndWhite<TPixel>(Configuration configuration, Stream stream, ImageFrame<TPixel> image) where TPixel : unmanaged, IPixel<TPixel>
	{
		int width = image.Width;
		int height = image.Height;
		Buffer2D<TPixel> pixelBuffer = image.PixelBuffer;
		MemoryAllocator memoryAllocator = configuration.MemoryAllocator;
		using IMemoryOwner<L8> buffer = memoryAllocator.Allocate<L8>(width);
		Span<L8> span = buffer.GetSpan();
		using IMemoryOwner<byte> buffer2 = memoryAllocator.Allocate<byte>(width * 2);
		Span<byte> span2 = buffer2.GetSpan();
		for (int i = 0; i < height; i++)
		{
			Span<TPixel> span3 = pixelBuffer.DangerousGetRowSpan(i);
			PixelOperations<TPixel>.Instance.ToL8(configuration, span3, span);
			int num = 0;
			for (int j = 0; j < width; j++)
			{
				byte b = (byte)((span[j].PackedValue < 128) ? 49 : 48);
				span2[num++] = b;
				span2[num++] = 32;
			}
			span2[num - 1] = 10;
			stream.Write(span2, 0, num);
		}
	}
}
