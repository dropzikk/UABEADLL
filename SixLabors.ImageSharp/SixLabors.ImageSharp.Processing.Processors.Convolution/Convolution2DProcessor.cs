using System.Numerics;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors.Convolution;

internal class Convolution2DProcessor<TPixel> : ImageProcessor<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	public DenseMatrix<float> KernelX { get; }

	public DenseMatrix<float> KernelY { get; }

	public bool PreserveAlpha { get; }

	public Convolution2DProcessor(Configuration configuration, in DenseMatrix<float> kernelX, in DenseMatrix<float> kernelY, bool preserveAlpha, Image<TPixel> source, Rectangle sourceRectangle)
		: base(configuration, source, sourceRectangle)
	{
		Guard.IsTrue(kernelX.Size.Equals(kernelY.Size), "kernelX kernelY", "Kernel sizes must be the same.");
		KernelX = kernelX;
		KernelY = kernelY;
		PreserveAlpha = preserveAlpha;
	}

	protected override void OnFrameApply(ImageFrame<TPixel> source)
	{
		MemoryAllocator memoryAllocator = base.Configuration.MemoryAllocator;
		using Buffer2D<TPixel> buffer2D = memoryAllocator.Allocate2D<TPixel>(source.Width, source.Height);
		source.CopyTo(buffer2D);
		Rectangle rectangle = Rectangle.Intersect(base.SourceRectangle, source.Bounds());
		using (KernelSamplingMap kernelSamplingMap = new KernelSamplingMap(memoryAllocator))
		{
			kernelSamplingMap.BuildSamplingOffsetMap(KernelY, rectangle);
			Convolution2DRowOperation<TPixel> operation = new Convolution2DRowOperation<TPixel>(rectangle, buffer2D, source.PixelBuffer, kernelSamplingMap, KernelY, KernelX, base.Configuration, PreserveAlpha);
			ParallelRowIterator.IterateRows<Convolution2DRowOperation<TPixel>, Vector4>(base.Configuration, rectangle, in operation);
		}
		Buffer2D<TPixel>.SwapOrCopyContent(source.PixelBuffer, buffer2D);
	}
}
