using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors.Convolution;

public sealed class GaussianBlurProcessor : IImageProcessor
{
	public const float DefaultSigma = 3f;

	public float Sigma { get; }

	public int Radius { get; }

	public BorderWrappingMode BorderWrapModeX { get; }

	public BorderWrappingMode BorderWrapModeY { get; }

	public GaussianBlurProcessor()
		: this(3f, ConvolutionProcessorHelpers.GetDefaultGaussianRadius(3f))
	{
	}

	public GaussianBlurProcessor(float sigma)
		: this(sigma, ConvolutionProcessorHelpers.GetDefaultGaussianRadius(sigma))
	{
	}

	public GaussianBlurProcessor(float sigma, BorderWrappingMode borderWrapModeX, BorderWrappingMode borderWrapModeY)
		: this(sigma, ConvolutionProcessorHelpers.GetDefaultGaussianRadius(sigma), borderWrapModeX, borderWrapModeY)
	{
	}

	public GaussianBlurProcessor(int radius)
		: this((float)radius / 3f, radius)
	{
	}

	public GaussianBlurProcessor(float sigma, int radius)
		: this(sigma, radius, BorderWrappingMode.Repeat, BorderWrappingMode.Repeat)
	{
	}

	public GaussianBlurProcessor(float sigma, int radius, BorderWrappingMode borderWrapModeX, BorderWrappingMode borderWrapModeY)
	{
		Sigma = sigma;
		Radius = radius;
		BorderWrapModeX = borderWrapModeX;
		BorderWrapModeY = borderWrapModeY;
	}

	public IImageProcessor<TPixel> CreatePixelSpecificProcessor<TPixel>(Configuration configuration, Image<TPixel> source, Rectangle sourceRectangle) where TPixel : unmanaged, IPixel<TPixel>
	{
		return new GaussianBlurProcessor<TPixel>(configuration, this, source, sourceRectangle, BorderWrapModeX, BorderWrapModeY);
	}
}
internal class GaussianBlurProcessor<TPixel> : ImageProcessor<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	public float[] Kernel { get; }

	public BorderWrappingMode BorderWrapModeX { get; }

	public BorderWrappingMode BorderWrapModeY { get; }

	public GaussianBlurProcessor(Configuration configuration, GaussianBlurProcessor definition, Image<TPixel> source, Rectangle sourceRectangle)
		: base(configuration, source, sourceRectangle)
	{
		int size = definition.Radius * 2 + 1;
		Kernel = ConvolutionProcessorHelpers.CreateGaussianBlurKernel(size, definition.Sigma);
	}

	public GaussianBlurProcessor(Configuration configuration, GaussianBlurProcessor definition, Image<TPixel> source, Rectangle sourceRectangle, BorderWrappingMode borderWrapModeX, BorderWrappingMode borderWrapModeY)
		: base(configuration, source, sourceRectangle)
	{
		int size = definition.Radius * 2 + 1;
		Kernel = ConvolutionProcessorHelpers.CreateGaussianBlurKernel(size, definition.Sigma);
		BorderWrapModeX = borderWrapModeX;
		BorderWrapModeY = borderWrapModeY;
	}

	protected override void OnFrameApply(ImageFrame<TPixel> source)
	{
		using Convolution2PassProcessor<TPixel> convolution2PassProcessor = new Convolution2PassProcessor<TPixel>(base.Configuration, Kernel, preserveAlpha: false, base.Source, base.SourceRectangle, BorderWrapModeX, BorderWrapModeY);
		convolution2PassProcessor.Apply(source);
	}
}
