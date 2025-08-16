using SixLabors.ImageSharp.Processing.Processors.Filters;

namespace SixLabors.ImageSharp.Processing;

public static class BlackWhiteExtensions
{
	public static IImageProcessingContext BlackWhite(this IImageProcessingContext source)
	{
		return source.ApplyProcessor(new BlackWhiteProcessor());
	}

	public static IImageProcessingContext BlackWhite(this IImageProcessingContext source, Rectangle rectangle)
	{
		return source.ApplyProcessor(new BlackWhiteProcessor(), rectangle);
	}
}
