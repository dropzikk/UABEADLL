using SixLabors.ImageSharp.Processing.Processors.Filters;

namespace SixLabors.ImageSharp.Processing;

public static class ContrastExtensions
{
	public static IImageProcessingContext Contrast(this IImageProcessingContext source, float amount)
	{
		return source.ApplyProcessor(new ContrastProcessor(amount));
	}

	public static IImageProcessingContext Contrast(this IImageProcessingContext source, float amount, Rectangle rectangle)
	{
		return source.ApplyProcessor(new ContrastProcessor(amount), rectangle);
	}
}
