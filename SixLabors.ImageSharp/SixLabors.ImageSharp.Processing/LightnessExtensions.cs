using SixLabors.ImageSharp.Processing.Processors.Filters;

namespace SixLabors.ImageSharp.Processing;

public static class LightnessExtensions
{
	public static IImageProcessingContext Lightness(this IImageProcessingContext source, float amount)
	{
		return source.ApplyProcessor(new LightnessProcessor(amount));
	}

	public static IImageProcessingContext Lightness(this IImageProcessingContext source, float amount, Rectangle rectangle)
	{
		return source.ApplyProcessor(new LightnessProcessor(amount), rectangle);
	}
}
