using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing.Processors.Filters;

namespace SixLabors.ImageSharp.Processing.Processors.Convolution;

public sealed class EdgeDetectorProcessor : IImageProcessor
{
	public EdgeDetectorKernel Kernel { get; }

	public bool Grayscale { get; }

	public EdgeDetectorProcessor(EdgeDetectorKernel kernel, bool grayscale)
	{
		Kernel = kernel;
		Grayscale = grayscale;
	}

	public IImageProcessor<TPixel> CreatePixelSpecificProcessor<TPixel>(Configuration configuration, Image<TPixel> source, Rectangle sourceRectangle) where TPixel : unmanaged, IPixel<TPixel>
	{
		return new EdgeDetectorProcessor<TPixel>(configuration, this, source, sourceRectangle);
	}
}
internal class EdgeDetectorProcessor<TPixel> : ImageProcessor<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly bool grayscale;

	private readonly DenseMatrix<float> kernelXY;

	public EdgeDetectorProcessor(Configuration configuration, EdgeDetectorProcessor definition, Image<TPixel> source, Rectangle sourceRectangle)
		: base(configuration, source, sourceRectangle)
	{
		kernelXY = definition.Kernel.KernelXY;
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
		using ConvolutionProcessor<TPixel> convolutionProcessor = new ConvolutionProcessor<TPixel>(base.Configuration, in kernelXY, preserveAlpha: true, base.Source, base.SourceRectangle);
		convolutionProcessor.Apply(source);
	}
}
