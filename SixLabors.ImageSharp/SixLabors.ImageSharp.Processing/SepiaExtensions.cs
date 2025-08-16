using SixLabors.ImageSharp.Processing.Processors.Filters;

namespace SixLabors.ImageSharp.Processing;

public static class SepiaExtensions
{
	public static IImageProcessingContext Sepia(this IImageProcessingContext source)
	{
		return source.Sepia(1f);
	}

	public static IImageProcessingContext Sepia(this IImageProcessingContext source, float amount)
	{
		return source.ApplyProcessor(new SepiaProcessor(amount));
	}

	public static IImageProcessingContext Sepia(this IImageProcessingContext source, Rectangle rectangle)
	{
		return source.Sepia(1f, rectangle);
	}

	public static IImageProcessingContext Sepia(this IImageProcessingContext source, float amount, Rectangle rectangle)
	{
		return source.ApplyProcessor(new SepiaProcessor(amount), rectangle);
	}
}
