using SixLabors.ImageSharp.Processing.Processors.Filters;

namespace SixLabors.ImageSharp.Processing;

public static class OpacityExtensions
{
	public static IImageProcessingContext Opacity(this IImageProcessingContext source, float amount)
	{
		return source.ApplyProcessor(new OpacityProcessor(amount));
	}

	public static IImageProcessingContext Opacity(this IImageProcessingContext source, float amount, Rectangle rectangle)
	{
		return source.ApplyProcessor(new OpacityProcessor(amount), rectangle);
	}
}
