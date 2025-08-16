using SixLabors.ImageSharp.Processing.Processors.Filters;

namespace SixLabors.ImageSharp.Processing;

public static class SaturateExtensions
{
	public static IImageProcessingContext Saturate(this IImageProcessingContext source, float amount)
	{
		return source.ApplyProcessor(new SaturateProcessor(amount));
	}

	public static IImageProcessingContext Saturate(this IImageProcessingContext source, float amount, Rectangle rectangle)
	{
		return source.ApplyProcessor(new SaturateProcessor(amount), rectangle);
	}
}
