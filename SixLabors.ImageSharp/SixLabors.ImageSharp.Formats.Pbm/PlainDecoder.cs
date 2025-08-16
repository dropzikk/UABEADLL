using System;
using System.Buffers;
using SixLabors.ImageSharp.IO;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Pbm;

internal class PlainDecoder
{
	private static readonly L8 White = new L8(byte.MaxValue);

	private static readonly L8 Black = new L8(0);

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
		using IMemoryOwner<L8> buffer = configuration.MemoryAllocator.Allocate<L8>(width);
		Span<L8> span = buffer.GetSpan();
		bool flag = false;
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				stream.ReadDecimal(out var value);
				span[j] = new L8((byte)value);
				flag = !stream.SkipWhitespaceAndComments();
				if (flag)
				{
					break;
				}
			}
			Span<TPixel> destinationPixels = pixels.DangerousGetRowSpan(i);
			PixelOperations<TPixel>.Instance.FromL8(configuration, span, destinationPixels);
			if (flag)
			{
				break;
			}
		}
	}

	private static void ProcessWideGrayscale<TPixel>(Configuration configuration, Buffer2D<TPixel> pixels, BufferedReadStream stream) where TPixel : unmanaged, IPixel<TPixel>
	{
		int width = pixels.Width;
		int height = pixels.Height;
		using IMemoryOwner<L16> buffer = configuration.MemoryAllocator.Allocate<L16>(width);
		Span<L16> span = buffer.GetSpan();
		bool flag = false;
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				stream.ReadDecimal(out var value);
				span[j] = new L16((ushort)value);
				flag = !stream.SkipWhitespaceAndComments();
				if (flag)
				{
					break;
				}
			}
			Span<TPixel> destinationPixels = pixels.DangerousGetRowSpan(i);
			PixelOperations<TPixel>.Instance.FromL16(configuration, span, destinationPixels);
			if (flag)
			{
				break;
			}
		}
	}

	private static void ProcessRgb<TPixel>(Configuration configuration, Buffer2D<TPixel> pixels, BufferedReadStream stream) where TPixel : unmanaged, IPixel<TPixel>
	{
		int width = pixels.Width;
		int height = pixels.Height;
		using IMemoryOwner<Rgb24> buffer = configuration.MemoryAllocator.Allocate<Rgb24>(width);
		Span<Rgb24> span = buffer.GetSpan();
		bool flag = false;
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				if (!stream.ReadDecimal(out var value) || !stream.SkipWhitespaceAndComments() || !stream.ReadDecimal(out var value2) || !stream.SkipWhitespaceAndComments())
				{
					flag = true;
					break;
				}
				stream.ReadDecimal(out var value3);
				span[j] = new Rgb24((byte)value, (byte)value2, (byte)value3);
				flag = !stream.SkipWhitespaceAndComments();
				if (flag)
				{
					break;
				}
			}
			Span<TPixel> destinationPixels = pixels.DangerousGetRowSpan(i);
			PixelOperations<TPixel>.Instance.FromRgb24(configuration, span, destinationPixels);
			if (flag)
			{
				break;
			}
		}
	}

	private static void ProcessWideRgb<TPixel>(Configuration configuration, Buffer2D<TPixel> pixels, BufferedReadStream stream) where TPixel : unmanaged, IPixel<TPixel>
	{
		int width = pixels.Width;
		int height = pixels.Height;
		using IMemoryOwner<Rgb48> buffer = configuration.MemoryAllocator.Allocate<Rgb48>(width);
		Span<Rgb48> span = buffer.GetSpan();
		bool flag = false;
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				if (!stream.ReadDecimal(out var value) || !stream.SkipWhitespaceAndComments() || !stream.ReadDecimal(out var value2) || !stream.SkipWhitespaceAndComments())
				{
					flag = true;
					break;
				}
				stream.ReadDecimal(out var value3);
				span[j] = new Rgb48((ushort)value, (ushort)value2, (ushort)value3);
				flag = !stream.SkipWhitespaceAndComments();
				if (flag)
				{
					break;
				}
			}
			Span<TPixel> destinationPixels = pixels.DangerousGetRowSpan(i);
			PixelOperations<TPixel>.Instance.FromRgb48(configuration, span, destinationPixels);
			if (flag)
			{
				break;
			}
		}
	}

	private static void ProcessBlackAndWhite<TPixel>(Configuration configuration, Buffer2D<TPixel> pixels, BufferedReadStream stream) where TPixel : unmanaged, IPixel<TPixel>
	{
		int width = pixels.Width;
		int height = pixels.Height;
		using IMemoryOwner<L8> buffer = configuration.MemoryAllocator.Allocate<L8>(width);
		Span<L8> span = buffer.GetSpan();
		bool flag = false;
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				stream.ReadDecimal(out var value);
				span[j] = ((value == 0) ? White : Black);
				flag = !stream.SkipWhitespaceAndComments();
				if (flag)
				{
					break;
				}
			}
			Span<TPixel> destinationPixels = pixels.DangerousGetRowSpan(i);
			PixelOperations<TPixel>.Instance.FromL8(configuration, span, destinationPixels);
			if (flag)
			{
				break;
			}
		}
	}
}
