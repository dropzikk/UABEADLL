using System;
using System.Buffers;
using SixLabors.ImageSharp.IO;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Pbm;

internal class BinaryDecoder
{
	private static L8 white = new L8(byte.MaxValue);

	private static L8 black = new L8(0);

	public static void Process<TPixel>(Configuration configuration, Buffer2D<TPixel> pixels, BufferedReadStream stream, PbmColorType colorType, PbmComponentType componentType) where TPixel : unmanaged, IPixel<TPixel>
	{
		switch (colorType)
		{
		case PbmColorType.Grayscale:
			if (componentType == PbmComponentType.Byte)
			{
				ProcessGrayscale(configuration, pixels, stream);
			}
			else
			{
				ProcessWideGrayscale(configuration, pixels, stream);
			}
			break;
		case PbmColorType.Rgb:
			if (componentType == PbmComponentType.Byte)
			{
				ProcessRgb(configuration, pixels, stream);
			}
			else
			{
				ProcessWideRgb(configuration, pixels, stream);
			}
			break;
		default:
			ProcessBlackAndWhite(configuration, pixels, stream);
			break;
		}
	}

	private static void ProcessGrayscale<TPixel>(Configuration configuration, Buffer2D<TPixel> pixels, BufferedReadStream stream) where TPixel : unmanaged, IPixel<TPixel>
	{
		int width = pixels.Width;
		int height = pixels.Height;
		using IMemoryOwner<byte> buffer = configuration.MemoryAllocator.Allocate<byte>(width);
		Span<byte> span = buffer.GetSpan();
		for (int i = 0; i < height; i++)
		{
			if (stream.Read(span) < span.Length)
			{
				break;
			}
			Span<TPixel> destinationPixels = pixels.DangerousGetRowSpan(i);
			PixelOperations<TPixel>.Instance.FromL8Bytes(configuration, span, destinationPixels, width);
		}
	}

	private static void ProcessWideGrayscale<TPixel>(Configuration configuration, Buffer2D<TPixel> pixels, BufferedReadStream stream) where TPixel : unmanaged, IPixel<TPixel>
	{
		int width = pixels.Width;
		int height = pixels.Height;
		using IMemoryOwner<byte> buffer = configuration.MemoryAllocator.Allocate<byte>(width * 2);
		Span<byte> span = buffer.GetSpan();
		for (int i = 0; i < height; i++)
		{
			if (stream.Read(span) < span.Length)
			{
				break;
			}
			Span<TPixel> destinationPixels = pixels.DangerousGetRowSpan(i);
			PixelOperations<TPixel>.Instance.FromL16Bytes(configuration, span, destinationPixels, width);
		}
	}

	private static void ProcessRgb<TPixel>(Configuration configuration, Buffer2D<TPixel> pixels, BufferedReadStream stream) where TPixel : unmanaged, IPixel<TPixel>
	{
		int width = pixels.Width;
		int height = pixels.Height;
		using IMemoryOwner<byte> buffer = configuration.MemoryAllocator.Allocate<byte>(width * 3);
		Span<byte> span = buffer.GetSpan();
		for (int i = 0; i < height; i++)
		{
			if (stream.Read(span) < span.Length)
			{
				break;
			}
			Span<TPixel> destinationPixels = pixels.DangerousGetRowSpan(i);
			PixelOperations<TPixel>.Instance.FromRgb24Bytes(configuration, span, destinationPixels, width);
		}
	}

	private static void ProcessWideRgb<TPixel>(Configuration configuration, Buffer2D<TPixel> pixels, BufferedReadStream stream) where TPixel : unmanaged, IPixel<TPixel>
	{
		int width = pixels.Width;
		int height = pixels.Height;
		using IMemoryOwner<byte> buffer = configuration.MemoryAllocator.Allocate<byte>(width * 6);
		Span<byte> span = buffer.GetSpan();
		for (int i = 0; i < height; i++)
		{
			if (stream.Read(span) < span.Length)
			{
				break;
			}
			Span<TPixel> destinationPixels = pixels.DangerousGetRowSpan(i);
			PixelOperations<TPixel>.Instance.FromRgb48Bytes(configuration, span, destinationPixels, width);
		}
	}

	private static void ProcessBlackAndWhite<TPixel>(Configuration configuration, Buffer2D<TPixel> pixels, BufferedReadStream stream) where TPixel : unmanaged, IPixel<TPixel>
	{
		int width = pixels.Width;
		int height = pixels.Height;
		using IMemoryOwner<L8> buffer = configuration.MemoryAllocator.Allocate<L8>(width);
		Span<L8> span = buffer.GetSpan();
		for (int i = 0; i < height; i++)
		{
			int num = 0;
			while (num < width)
			{
				int num2 = stream.ReadByte();
				if (num2 < 0)
				{
					return;
				}
				int num3 = Math.Min(8, width - num);
				for (int j = 0; j < num3; j++)
				{
					bool flag = (num2 & (128 >> j)) != 0;
					span[num] = (flag ? black : white);
					num++;
				}
			}
			Span<TPixel> destinationPixels = pixels.DangerousGetRowSpan(i);
			PixelOperations<TPixel>.Instance.FromL8(configuration, span, destinationPixels);
		}
	}
}
