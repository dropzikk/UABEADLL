using SixLabors.ImageSharp.Processing.Processors.Filters;

namespace SixLabors.ImageSharp.Processing;

public static class FilterExtensions
{
	public static IImageProcessingContext Filter(this IImageProcessingContext source, ColorMatrix matrix)
	{
		return source.ApplyProcessor(new FilterProcessor(matrix));
	}

	public static IImageProcessingContext Filter(this IImageProcessingContext source, ColorMatrix matrix, Rectangle rectangle)
	{
		return source.ApplyProcessor(new FilterProcessor(matrix), rectangle);
	}
}
