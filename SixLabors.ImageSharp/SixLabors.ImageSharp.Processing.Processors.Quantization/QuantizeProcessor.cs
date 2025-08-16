using System;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors.Quantization;

public class QuantizeProcessor : IImageProcessor
{
	public IQuantizer Quantizer { get; }

	public QuantizeProcessor(IQuantizer quantizer)
	{
		Quantizer = quantizer;
	}

	public IImageProcessor<TPixel> CreatePixelSpecificProcessor<TPixel>(Configuration configuration, Image<TPixel> source, Rectangle sourceRectangle) where TPixel : unmanaged, IPixel<TPixel>
	{
		return new QuantizeProcessor<TPixel>(configuration, Quantizer, source, sourceRectangle);
	}
}
internal class QuantizeProcessor<TPixel> : ImageProcessor<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly IQuantizer quantizer;

	public QuantizeProcessor(Configuration configuration, IQuantizer quantizer, Image<TPixel> source, Rectangle sourceRectangle)
		: base(configuration, source, sourceRectangle)
	{
		Guard.NotNull(quantizer, "quantizer");
		this.quantizer = quantizer;
	}

	protected override void OnFrameApply(ImageFrame<TPixel> source)
	{
		Rectangle bounds = Rectangle.Intersect(source.Bounds(), base.SourceRectangle);
		Configuration configuration = base.Configuration;
		using IQuantizer<TPixel> quantizer = this.quantizer.CreatePixelSpecificQuantizer<TPixel>(configuration);
		using IndexedImageFrame<TPixel> indexedImageFrame = quantizer.BuildPaletteAndQuantizeFrame(source, bounds);
		ReadOnlySpan<TPixel> span = indexedImageFrame.Palette.Span;
		int top = bounds.Top;
		int left = bounds.Left;
		Buffer2D<TPixel> pixelBuffer = source.PixelBuffer;
		for (int i = 0; i < indexedImageFrame.Height; i++)
		{
			ReadOnlySpan<byte> readOnlySpan = indexedImageFrame.DangerousGetRowSpan(i);
			Span<TPixel> span2 = pixelBuffer.DangerousGetRowSpan(i + top);
			for (int j = 0; j < indexedImageFrame.Width; j++)
			{
				span2[j + left] = span[readOnlySpan[j]];
			}
		}
	}
}
