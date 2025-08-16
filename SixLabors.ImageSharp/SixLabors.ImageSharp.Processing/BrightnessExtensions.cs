using SixLabors.ImageSharp.Processing.Processors.Filters;

namespace SixLabors.ImageSharp.Processing;

public static class BrightnessExtensions
{
	public static IImageProcessingContext Brightness(this IImageProcessingContext source, float amount)
	{
		return source.ApplyProcessor(new BrightnessProcessor(amount));
	}

	public static IImageProcessingContext Brightness(this IImageProcessingContext source, float amount, Rectangle rectangle)
	{
		return source.ApplyProcessor(new BrightnessProcessor(amount), rectangle);
	}
}
