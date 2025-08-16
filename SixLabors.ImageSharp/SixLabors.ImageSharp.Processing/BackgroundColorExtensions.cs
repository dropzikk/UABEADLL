using SixLabors.ImageSharp.Processing.Processors.Overlays;

namespace SixLabors.ImageSharp.Processing;

public static class BackgroundColorExtensions
{
	public static IImageProcessingContext BackgroundColor(this IImageProcessingContext source, Color color)
	{
		return source.BackgroundColor(source.GetGraphicsOptions(), color);
	}

	public static IImageProcessingContext BackgroundColor(this IImageProcessingContext source, Color color, Rectangle rectangle)
	{
		return source.BackgroundColor(source.GetGraphicsOptions(), color, rectangle);
	}

	public static IImageProcessingContext BackgroundColor(this IImageProcessingContext source, GraphicsOptions options, Color color)
	{
		return source.ApplyProcessor(new BackgroundColorProcessor(options, color));
	}

	public static IImageProcessingContext BackgroundColor(this IImageProcessingContext source, GraphicsOptions options, Color color, Rectangle rectangle)
	{
		return source.ApplyProcessor(new BackgroundColorProcessor(options, color), rectangle);
	}
}
