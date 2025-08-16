using System.Numerics;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors.Convolution;

public sealed class MedianBlurProcessor : IImageProcessor
{
	public int Radius { get; }

	public bool PreserveAlpha { get; }

	public BorderWrappingMode BorderWrapModeX { get; }

	public BorderWrappingMode BorderWrapModeY { get; }

	public MedianBlurProcessor(int radius, bool preserveAlpha)
	{
		Radius = radius;
		PreserveAlpha = preserveAlpha;
	}

	public IImageProcessor<TPixel> CreatePixelSpecificProcessor<TPixel>(Configuration configuration, Image<TPixel> source, Rectangle sourceRectangle) where TPixel : unmanaged, IPixel<TPixel>
	{
		return new MedianBlurProcessor<TPixel>(configuration, this, source, sourceRectangle);
	}
}
internal sealed class MedianBlurProcessor<TPixel> : ImageProcessor<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly MedianBlurProcessor definition;

	public MedianBlurProcessor(Configuration configuration, MedianBlurProcessor definition, Image<TPixel> source, Rectangle sourceRectangle)
		: base(configuration, source, sourceRectangle)
	{
		this.definition = definition;
	}

	protected override void OnFrameApply(ImageFrame<TPixel> source)
	{
		int num = 2 * definition.Radius + 1;
		using Buffer2D<TPixel> buffer2D = base.Configuration.MemoryAllocator.Allocate2D<TPixel>(source.Width, source.Height);
		source.CopyTo(buffer2D);
		Rectangle rectangle = Rectangle.Intersect(base.SourceRectangle, source.Bounds());
		using KernelSamplingMap kernelSamplingMap = new KernelSamplingMap(base.Configuration.MemoryAllocator);
		kernelSamplingMap.BuildSamplingOffsetMap(num, num, rectangle, definition.BorderWrapModeX, definition.BorderWrapModeY);
		MedianRowOperation<TPixel> operation = new MedianRowOperation<TPixel>(rectangle, buffer2D, source.PixelBuffer, kernelSamplingMap, num, base.Configuration, definition.PreserveAlpha);
		ParallelRowIterator.IterateRows<MedianRowOperation<TPixel>, Vector4>(base.Configuration, rectangle, in operation);
		Buffer2D<TPixel>.SwapOrCopyContent(source.PixelBuffer, buffer2D);
	}
}
