using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing.Processors.Filters;

namespace SixLabors.ImageSharp.Processing.Processors.Convolution;

public sealed class EdgeDetector2DProcessor : IImageProcessor
{
	public EdgeDetector2DKernel Kernel { get; }

	public bool Grayscale { get; }

	public EdgeDetector2DProcessor(EdgeDetector2DKernel kernel, bool grayscale)
	{
		Kernel = kernel;
		Grayscale = grayscale;
	}

	public IImageProcessor<TPixel> CreatePixelSpecificProcessor<TPixel>(Configuration configuration, Image<TPixel> source, Rectangle sourceRectangle) where TPixel : unmanaged, IPixel<TPixel>
	{
		return new EdgeDetector2DProcessor<TPixel>(configuration, this, source, sourceRectangle);
	}
}
internal class EdgeDetector2DProcessor<TPixel> : ImageProcessor<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly DenseMatrix<float> kernelX;

	private readonly DenseMatrix<float> kernelY;

	private readonly bool grayscale;

	public EdgeDetector2DProcessor(Configuration configuration, EdgeDetector2DProcessor definition, Image<TPixel> source, Rectangle sourceRectangle)
		: base(configuration, source, sourceRectangle)
	{
		kernelX = definition.Kernel.KernelX;
		kernelY = definition.Kernel.KernelY;
		grayscale = definition.Grayscale;
	}

	protected override void BeforeImageApply()
	{
		using (IImageProcessor<TPixel> imageProcessor = new OpaqueProcessor<TPixel>(base.Configuration, base.Source, base.SourceRectangle))
		{
			imageProcessor.Execute();
		}
		if (grayscale)
		{
			new GrayscaleBt709Processor(1f).Execute(base.Configuration, base.Source, base.SourceRectangle);
		}
		base.BeforeImageApply();
	}

	protected override void OnFrameApply(ImageFrame<TPixel> source)
	{
		using Convolution2DProcessor<TPixel> convolution2DProcessor = new Convolution2DProcessor<TPixel>(base.Configuration, in kernelX, in kernelY, preserveAlpha: true, base.Source, base.SourceRectangle);
		convolution2DProcessor.Apply(source);
	}
}
