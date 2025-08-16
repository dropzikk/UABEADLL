using System;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors.Transforms;

internal class SwizzleProcessor<TSwizzler, TPixel> : TransformProcessor<TPixel> where TSwizzler : struct, ISwizzler where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly TSwizzler swizzler;

	private readonly Size destinationSize;

	public SwizzleProcessor(Configuration configuration, TSwizzler swizzler, Image<TPixel> source, Rectangle sourceRectangle)
		: base(configuration, source, sourceRectangle)
	{
		this.swizzler = swizzler;
		destinationSize = swizzler.DestinationSize;
	}

	protected override Size GetDestinationSize()
	{
		return destinationSize;
	}

	protected override void OnFrameApply(ImageFrame<TPixel> source, ImageFrame<TPixel> destination)
	{
		Point point = default(Point);
		Buffer2D<TPixel> pixelBuffer = source.PixelBuffer;
		point.Y = 0;
		while (point.Y < source.Height)
		{
			Span<TPixel> span = pixelBuffer.DangerousGetRowSpan(point.Y);
			point.X = 0;
			while (point.X < source.Width)
			{
				Point point2 = swizzler.Transform(point);
				destination[point2.X, point2.Y] = span[point.X];
				point.X++;
			}
			point.Y++;
		}
	}
}
public sealed class SwizzleProcessor<TSwizzler> : IImageProcessor where TSwizzler : struct, ISwizzler
{
	public TSwizzler Swizzler { get; }

	public SwizzleProcessor(TSwizzler swizzler)
	{
		Swizzler = swizzler;
	}

	public IImageProcessor<TPixel> CreatePixelSpecificProcessor<TPixel>(Configuration configuration, Image<TPixel> source, Rectangle sourceRectangle) where TPixel : unmanaged, IPixel<TPixel>
	{
		return new SwizzleProcessor<TSwizzler, TPixel>(configuration, Swizzler, source, sourceRectangle);
	}
}
