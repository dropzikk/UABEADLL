using SixLabors.ImageSharp.Processing.Processors.Filters;

namespace SixLabors.ImageSharp.Processing;

public static class KodachromeExtensions
{
	public static IImageProcessingContext Kodachrome(this IImageProcessingContext source)
	{
		return source.ApplyProcessor(new KodachromeProcessor());
	}

	public static IImageProcessingContext Kodachrome(this IImageProcessingContext source, Rectangle rectangle)
	{
		return source.ApplyProcessor(new KodachromeProcessor(), rectangle);
	}
}
