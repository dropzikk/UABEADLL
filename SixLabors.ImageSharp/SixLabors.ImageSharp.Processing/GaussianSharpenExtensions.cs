using SixLabors.ImageSharp.Processing.Processors.Convolution;

namespace SixLabors.ImageSharp.Processing;

public static class GaussianSharpenExtensions
{
	public static IImageProcessingContext GaussianSharpen(this IImageProcessingContext source)
	{
		return source.ApplyProcessor(new GaussianSharpenProcessor());
	}

	public static IImageProcessingContext GaussianSharpen(this IImageProcessingContext source, float sigma)
	{
		return source.ApplyProcessor(new GaussianSharpenProcessor(sigma));
	}

	public static IImageProcessingContext GaussianSharpen(this IImageProcessingContext source, float sigma, Rectangle rectangle)
	{
		return source.ApplyProcessor(new GaussianSharpenProcessor(sigma), rectangle);
	}

	public static IImageProcessingContext GaussianSharpen(this IImageProcessingContext source, float sigma, Rectangle rectangle, BorderWrappingMode borderWrapModeX, BorderWrappingMode borderWrapModeY)
	{
		GaussianSharpenProcessor processor = new GaussianSharpenProcessor(sigma, borderWrapModeX, borderWrapModeY);
		return source.ApplyProcessor(processor, rectangle);
	}
}
