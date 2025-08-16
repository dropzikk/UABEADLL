using SixLabors.ImageSharp.Processing.Processors.Filters;

namespace SixLabors.ImageSharp.Processing;

public static class HueExtensions
{
	public static IImageProcessingContext Hue(this IImageProcessingContext source, float degrees)
	{
		return source.ApplyProcessor(new HueProcessor(degrees));
	}

	public static IImageProcessingContext Hue(this IImageProcessingContext source, float degrees, Rectangle rectangle)
	{
		return source.ApplyProcessor(new HueProcessor(degrees), rectangle);
	}
}
