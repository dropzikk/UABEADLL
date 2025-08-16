using System;
using System.Buffers;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Formats.Webp.Lossless;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Webp;

internal static class AlphaEncoder
{
	public static IMemoryOwner<byte> EncodeAlpha<TPixel>(Buffer2DRegion<TPixel> frame, Configuration configuration, MemoryAllocator memoryAllocator, bool skipMetadata, bool compress, out int size) where TPixel : unmanaged, IPixel<TPixel>
	{
		IMemoryOwner<byte> memoryOwner = ExtractAlphaChannel(frame, configuration, memoryAllocator);
		if (compress)
		{
			using (Vp8LEncoder vp8LEncoder = new Vp8LEncoder(memoryAllocator, configuration, frame.Width, frame.Height, 32u, skipMetadata, WebpEncodingMethod.Level4, WebpTransparentColorMode.Preserve, nearLossless: false, 0))
			{
				using ImageFrame<Bgra32> imageFrame = DispatchAlphaToGreen(configuration, frame, memoryOwner.GetSpan());
				size = vp8LEncoder.EncodeAlphaImageData(imageFrame.PixelBuffer.GetRegion(), memoryOwner);
				return memoryOwner;
			}
		}
		size = frame.Width * frame.Height;
		return memoryOwner;
	}

	private static ImageFrame<Bgra32> DispatchAlphaToGreen<TPixel>(Configuration configuration, Buffer2DRegion<TPixel> frame, Span<byte> alphaData) where TPixel : unmanaged, IPixel<TPixel>
	{
		int width = frame.Width;
		int height = frame.Height;
		ImageFrame<Bgra32> imageFrame = new ImageFrame<Bgra32>(configuration, width, height);
		for (int i = 0; i < height; i++)
		{
			Span<Bgra32> span = imageFrame.DangerousGetPixelRowMemory(i).Span;
			Span<byte> span2 = alphaData.Slice(i * width, width);
			for (int j = 0; j < width; j++)
			{
				span[j] = new Bgra32(0, span2[j], 0, 0);
			}
		}
		return imageFrame;
	}

	private static IMemoryOwner<byte> ExtractAlphaChannel<TPixel>(Buffer2DRegion<TPixel> frame, Configuration configuration, MemoryAllocator memoryAllocator) where TPixel : unmanaged, IPixel<TPixel>
	{
		int width = frame.Width;
		int height = frame.Height;
		IMemoryOwner<byte> memoryOwner = memoryAllocator.Allocate<byte>(width * height);
		Span<byte> span = memoryOwner.GetSpan();
		using IMemoryOwner<Rgba32> buffer = memoryAllocator.Allocate<Rgba32>(width);
		Span<Rgba32> span2 = buffer.GetSpan();
		for (int i = 0; i < height; i++)
		{
			Span<TPixel> span3 = frame.DangerousGetRowSpan(i);
			PixelOperations<TPixel>.Instance.ToRgba32(configuration, span3, span2);
			int num = i * width;
			for (int j = 0; j < width; j++)
			{
				span[num + j] = span2[j].A;
			}
		}
		return memoryOwner;
	}
}
