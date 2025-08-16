using SixLabors.ImageSharp.Processing.Processors.Filters;

namespace SixLabors.ImageSharp.Processing;

public static class InvertExtensions
{
	public static IImageProcessingContext Invert(this IImageProcessingContext source)
	{
		return source.ApplyProcessor(new InvertProcessor(1f));
	}

	public static IImageProcessingContext Invert(this IImageProcessingContext source, Rectangle rectangle)
	{
		return source.ApplyProcessor(new InvertProcessor(1f), rectangle);
	}
}
