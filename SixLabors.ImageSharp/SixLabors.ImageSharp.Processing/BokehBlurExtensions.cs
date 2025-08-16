using SixLabors.ImageSharp.Processing.Processors.Convolution;

namespace SixLabors.ImageSharp.Processing;

public static class BokehBlurExtensions
{
	public static IImageProcessingContext BokehBlur(this IImageProcessingContext source)
	{
		return source.ApplyProcessor(new BokehBlurProcessor());
	}

	public static IImageProcessingContext BokehBlur(this IImageProcessingContext source, int radius, int components, float gamma)
	{
		return source.ApplyProcessor(new BokehBlurProcessor(radius, components, gamma));
	}

	public static IImageProcessingContext BokehBlur(this IImageProcessingContext source, Rectangle rectangle)
	{
		return source.ApplyProcessor(new BokehBlurProcessor(), rectangle);
	}

	public static IImageProcessingContext BokehBlur(this IImageProcessingContext source, int radius, int components, float gamma, Rectangle rectangle)
	{
		return source.ApplyProcessor(new BokehBlurProcessor(radius, components, gamma), rectangle);
	}
}
