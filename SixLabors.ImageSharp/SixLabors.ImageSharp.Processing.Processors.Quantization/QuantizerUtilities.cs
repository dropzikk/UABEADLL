using System;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing.Processors.Dithering;

namespace SixLabors.ImageSharp.Processing.Processors.Quantization;

public static class QuantizerUtilities
{
	public static void CheckPaletteState<TPixel>(in ReadOnlyMemory<TPixel> palette) where TPixel : unmanaged, IPixel<TPixel>
	{
		if (palette.Equals(default(ReadOnlyMemory<TPixel>)))
		{
			throw new InvalidOperationException("Frame Quantizer palette has not been built.");
		}
	}

	public static IndexedImageFrame<TPixel> BuildPaletteAndQuantizeFrame<TPixel>(this IQuantizer<TPixel> quantizer, ImageFrame<TPixel> source, Rectangle bounds) where TPixel : unmanaged, IPixel<TPixel>
	{
		Guard.NotNull(quantizer, "quantizer");
		Guard.NotNull(source, "source");
		Rectangle rectangle = Rectangle.Intersect(source.Bounds(), bounds);
		Buffer2DRegion<TPixel> region = source.PixelBuffer.GetRegion(rectangle);
		quantizer.AddPaletteColors(region);
		return quantizer.QuantizeFrame(source, bounds);
	}

	public static IndexedImageFrame<TPixel> QuantizeFrame<TFrameQuantizer, TPixel>(ref TFrameQuantizer quantizer, ImageFrame<TPixel> source, Rectangle bounds) where TFrameQuantizer : struct, IQuantizer<TPixel> where TPixel : unmanaged, IPixel<TPixel>
	{
		Guard.NotNull(source, "source");
		Rectangle bounds2 = Rectangle.Intersect(source.Bounds(), bounds);
		IndexedImageFrame<TPixel> indexedImageFrame = new IndexedImageFrame<TPixel>(quantizer.Configuration, bounds2.Width, bounds2.Height, quantizer.Palette);
		if (quantizer.Options.Dither == null)
		{
			SecondPass(ref quantizer, source, indexedImageFrame, bounds2);
		}
		else
		{
			using ImageFrame<TPixel> source2 = source.Clone();
			SecondPass(ref quantizer, source2, indexedImageFrame, bounds2);
		}
		return indexedImageFrame;
	}

	public static void BuildPalette<TPixel>(this IQuantizer<TPixel> quantizer, IPixelSamplingStrategy pixelSamplingStrategy, Image<TPixel> source) where TPixel : unmanaged, IPixel<TPixel>
	{
		foreach (Buffer2DRegion<TPixel> item in pixelSamplingStrategy.EnumeratePixelRegions(source))
		{
			quantizer.AddPaletteColors(item);
		}
	}

	public static void BuildPalette<TPixel>(this IQuantizer<TPixel> quantizer, IPixelSamplingStrategy pixelSamplingStrategy, ImageFrame<TPixel> source) where TPixel : unmanaged, IPixel<TPixel>
	{
		foreach (Buffer2DRegion<TPixel> item in pixelSamplingStrategy.EnumeratePixelRegions(source))
		{
			quantizer.AddPaletteColors(item);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void SecondPass<TFrameQuantizer, TPixel>(ref TFrameQuantizer quantizer, ImageFrame<TPixel> source, IndexedImageFrame<TPixel> destination, Rectangle bounds) where TFrameQuantizer : struct, IQuantizer<TPixel> where TPixel : unmanaged, IPixel<TPixel>
	{
		IDither dither = quantizer.Options.Dither;
		Buffer2D<TPixel> pixelBuffer = source.PixelBuffer;
		if (dither == null)
		{
			int top = bounds.Top;
			int left = bounds.Left;
			for (int i = 0; i < destination.Height; i++)
			{
				ReadOnlySpan<TPixel> readOnlySpan = pixelBuffer.DangerousGetRowSpan(i + top);
				Span<byte> writablePixelRowSpanUnsafe = destination.GetWritablePixelRowSpanUnsafe(i);
				for (int j = 0; j < writablePixelRowSpanUnsafe.Length; j++)
				{
					writablePixelRowSpanUnsafe[j] = Unsafe.AsRef(in quantizer).GetQuantizedColor(readOnlySpan[j + left], out var _);
				}
			}
		}
		else
		{
			dither.ApplyQuantizationDither(ref quantizer, source, destination, bounds);
		}
	}
}
