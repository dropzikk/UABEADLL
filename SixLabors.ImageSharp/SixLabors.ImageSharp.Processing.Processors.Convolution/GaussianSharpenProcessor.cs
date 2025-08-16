using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors.Convolution;

public sealed class GaussianSharpenProcessor : IImageProcessor
{
	public const float DefaultSigma = 3f;

	public float Sigma { get; }

	public int Radius { get; }

	public BorderWrappingMode BorderWrapModeX { get; }

	public BorderWrappingMode BorderWrapModeY { get; }

	public GaussianSharpenProcessor()
		: this(3f, ConvolutionProcessorHelpers.GetDefaultGaussianRadius(3f))
	{
	}

	public GaussianSharpenProcessor(float sigma)
		: this(sigma, ConvolutionProcessorHelpers.GetDefaultGaussianRadius(sigma))
	{
	}

	public GaussianSharpenProcessor(float sigma, BorderWrappingMode borderWrapModeX, BorderWrappingMode borderWrapModeY)
		: this(sigma, ConvolutionProcessorHelpers.GetDefaultGaussianRadius(sigma), borderWrapModeX, borderWrapModeY)
	{
	}

	public GaussianSharpenProcessor(int radius)
		: this((float)radius / 3f, radius)
	{
	}

	public GaussianSharpenProcessor(float sigma, int radius)
		: this(sigma, radius, BorderWrappingMode.Repeat, BorderWrappingMode.Repeat)
	{
	}

	public GaussianSharpenProcessor(float sigma, int radius, BorderWrappingMode borderWrapModeX, BorderWrappingMode borderWrapModeY)
	{
		Sigma = sigma;
		Radius = radius;
		BorderWrapModeX = borderWrapModeX;
		BorderWrapModeY = borderWrapModeY;
	}

	public IImageProcessor<TPixel> CreatePixelSpecificProcessor<TPixel>(Configuration configuration, Image<TPixel> source, Rectangle sourceRectangle) where TPixel : unmanaged, IPixel<TPixel>
	{
		return new GaussianSharpenProcessor<TPixel>(configuration, this, source, sourceRectangle, BorderWrapModeX, BorderWrapModeY);
	}
}
internal class GaussianSharpenProcessor<TPixel> : ImageProcessor<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	public float[] Kernel { get; }

	public BorderWrappingMode BorderWrapModeX { get; }

	public BorderWrappingMode BorderWrapModeY { get; }

	public GaussianSharpenProcessor(Configuration configuration, GaussianSharpenProcessor definition, Image<TPixel> source, Rectangle sourceRectangle)
		: this(configuration, definition, source, sourceRectangle, BorderWrappingMode.Repeat, BorderWrappingMode.Repeat)
	{
	}

	public GaussianSharpenProcessor(Configuration configuration, GaussianSharpenProcessor definition, Image<TPixel> source, Rectangle sourceRectangle, BorderWrappingMode borderWrapModeX, BorderWrappingMode borderWrapModeY)
		: base(configuration, source, sourceRectangle)
	{
		int size = definition.Radius * 2 + 1;
		Kernel = ConvolutionProcessorHelpers.CreateGaussianSharpenKernel(size, definition.Sigma);
		BorderWrapModeX = borderWrapModeX;
		BorderWrapModeY = borderWrapModeY;
	}

	protected override void OnFrameApply(ImageFrame<TPixel> source)
	{
		using Convolution2PassProcessor<TPixel> convolution2PassProcessor = new Convolution2PassProcessor<TPixel>(base.Configuration, Kernel, preserveAlpha: false, base.Source, base.SourceRectangle, BorderWrapModeX, BorderWrapModeY);
		convolution2PassProcessor.Apply(source);
	}
}
