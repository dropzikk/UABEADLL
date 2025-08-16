using SixLabors.ImageSharp.Processing.Processors.Convolution;

namespace SixLabors.ImageSharp.Processing;

public static class GaussianBlurExtensions
{
	public static IImageProcessingContext GaussianBlur(this IImageProcessingContext source)
	{
		return source.ApplyProcessor(new GaussianBlurProcessor());
	}

	public static IImageProcessingContext GaussianBlur(this IImageProcessingContext source, float sigma)
	{
		return source.ApplyProcessor(new GaussianBlurProcessor(sigma));
	}

	public static IImageProcessingContext GaussianBlur(this IImageProcessingContext source, float sigma, Rectangle rectangle)
	{
		return source.ApplyProcessor(new GaussianBlurProcessor(sigma), rectangle);
	}

	public static IImageProcessingContext GaussianBlur(this IImageProcessingContext source, float sigma, Rectangle rectangle, BorderWrappingMode borderWrapModeX, BorderWrappingMode borderWrapModeY)
	{
		GaussianBlurProcessor processor = new GaussianBlurProcessor(sigma, borderWrapModeX, borderWrapModeY);
		return source.ApplyProcessor(processor, rectangle);
	}
}
