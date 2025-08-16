using SixLabors.ImageSharp.Processing.Processors.Convolution;

namespace SixLabors.ImageSharp.Processing;

public static class MedianBlurExtensions
{
	public static IImageProcessingContext MedianBlur(this IImageProcessingContext source, int radius, bool preserveAlpha)
	{
		return source.ApplyProcessor(new MedianBlurProcessor(radius, preserveAlpha));
	}

	public static IImageProcessingContext MedianBlur(this IImageProcessingContext source, int radius, bool preserveAlpha, Rectangle rectangle)
	{
		return source.ApplyProcessor(new MedianBlurProcessor(radius, preserveAlpha), rectangle);
	}
}
