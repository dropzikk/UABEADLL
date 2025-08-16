using System;
using System.Buffers;
using System.IO;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Pbm;

internal class BinaryEncoder
{
	public static void WritePixels<TPixel>(Configuration configuration, Stream stream, ImageFrame<TPixel> image, PbmColorType colorType, PbmComponentType componentType) where TPixel : unmanaged, IPixel<TPixel>
	{
		switch (colorType)
		{
		case PbmColorType.Grayscale:
			switch (componentType)
			{
			case PbmComponentType.Byte:
				WriteGrayscale(configuration, stream, image);
				break;
			case PbmComponentType.Short:
				WriteWideGrayscale(configuration, stream, image);
				break;
			default:
				throw new ImageFormatException("Component type not supported for Grayscale PBM.");
			}
			break;
		case PbmColorType.Rgb:
			switch (componentType)
			{
			case PbmComponentType.Byte:
				WriteRgb(configuration, stream, image);
				break;
			case PbmComponentType.Short:
				WriteWideRgb(configuration, stream, image);
				break;
			default:
				throw new ImageFormatException("Component type not supported for Color PBM.");
			}
			break;
		default:
			if (componentType == PbmComponentType.Bit)
			{
				WriteBlackAndWhite(configuration, stream, image);
				break;
			}
			throw new ImageFormatException("Component type not supported for Black & White PBM.");
		}
	}

	private static void WriteGrayscale<TPixel>(Configuration configuration, Stream stream, ImageFrame<TPixel> image) where TPixel : unmanaged, IPixel<TPixel>
	{
		int width = image.Width;
		int height = image.Height;
		Buffer2D<TPixel> pixelBuffer = image.PixelBuffer;
		using IMemoryOwner<byte> buffer = configuration.MemoryAllocator.Allocate<byte>(width);
		Span<byte> span = buffer.GetSpan();
		for (int i = 0; i < height; i++)
		{
			Span<TPixel> span2 = pixelBuffer.DangerousGetRowSpan(i);
			PixelOperations<TPixel>.Instance.ToL8Bytes(configuration, span2, span, width);
			stream.Write(span);
		}
	}

	private static void WriteWideGrayscale<TPixel>(Configuration configuration, Stream stream, ImageFrame<TPixel> image) where TPixel : unmanaged, IPixel<TPixel>
	{
		int width = image.Width;
		int height = image.Height;
		Buffer2D<TPixel> pixelBuffer = image.PixelBuffer;
		using IMemoryOwner<byte> buffer = configuration.MemoryAllocator.Allocate<byte>(width * 2);
		Span<byte> span = buffer.GetSpan();
		for (int i = 0; i < height; i++)
		{
			Span<TPixel> span2 = pixelBuffer.DangerousGetRowSpan(i);
			PixelOperations<TPixel>.Instance.ToL16Bytes(configuration, span2, span, width);
			stream.Write(span);
		}
	}

	private static void WriteRgb<TPixel>(Configuration configuration, Stream stream, ImageFrame<TPixel> image) where TPixel : unmanaged, IPixel<TPixel>
	{
		int width = image.Width;
		int height = image.Height;
		Buffer2D<TPixel> pixelBuffer = image.PixelBuffer;
		using IMemoryOwner<byte> buffer = configuration.MemoryAllocator.Allocate<byte>(width * 3);
		Span<byte> span = buffer.GetSpan();
		for (int i = 0; i < height; i++)
		{
			Span<TPixel> span2 = pixelBuffer.DangerousGetRowSpan(i);
			PixelOperations<TPixel>.Instance.ToRgb24Bytes(configuration, span2, span, width);
			stream.Write(span);
		}
	}

	private static void WriteWideRgb<TPixel>(Configuration configuration, Stream stream, ImageFrame<TPixel> image) where TPixel : unmanaged, IPixel<TPixel>
	{
		int width = image.Width;
		int height = image.Height;
		Buffer2D<TPixel> pixelBuffer = image.PixelBuffer;
		using IMemoryOwner<byte> buffer = configuration.MemoryAllocator.Allocate<byte>(width * 6);
		Span<byte> span = buffer.GetSpan();
		for (int i = 0; i < height; i++)
		{
			Span<TPixel> span2 = pixelBuffer.DangerousGetRowSpan(i);
			PixelOperations<TPixel>.Instance.ToRgb48Bytes(configuration, span2, span, width);
			stream.Write(span);
		}
	}

	private static void WriteBlackAndWhite<TPixel>(Configuration configuration, Stream stream, ImageFrame<TPixel> image) where TPixel : unmanaged, IPixel<TPixel>
	{
		int width = image.Width;
		int height = image.Height;
		Buffer2D<TPixel> pixelBuffer = image.PixelBuffer;
		using IMemoryOwner<L8> buffer = configuration.MemoryAllocator.Allocate<L8>(width);
		Span<L8> span = buffer.GetSpan();
		for (int i = 0; i < height; i++)
		{
			Span<TPixel> span2 = pixelBuffer.DangerousGetRowSpan(i);
			PixelOperations<TPixel>.Instance.ToL8(configuration, span2, span);
			int num = 0;
			while (num < width)
			{
				int num2 = 0;
				int num3 = Math.Min(8, width - num);
				for (int j = 0; j < num3; j++)
				{
					if (span[num].PackedValue < 128)
					{
						num2 |= 128 >> j;
					}
					num++;
				}
				stream.WriteByte((byte)num2);
			}
		}
	}
}
