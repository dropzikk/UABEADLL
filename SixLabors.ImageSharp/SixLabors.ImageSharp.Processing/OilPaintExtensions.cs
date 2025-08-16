using SixLabors.ImageSharp.Processing.Processors.Effects;

namespace SixLabors.ImageSharp.Processing;

public static class OilPaintExtensions
{
	public static IImageProcessingContext OilPaint(this IImageProcessingContext source)
	{
		return source.OilPaint(10, 15);
	}

	public static IImageProcessingContext OilPaint(this IImageProcessingContext source, Rectangle rectangle)
	{
		return source.OilPaint(10, 15, rectangle);
	}

	public static IImageProcessingContext OilPaint(this IImageProcessingContext source, int levels, int brushSize)
	{
		return source.ApplyProcessor(new OilPaintingProcessor(levels, brushSize));
	}

	public static IImageProcessingContext OilPaint(this IImageProcessingContext source, int levels, int brushSize, Rectangle rectangle)
	{
		return source.ApplyProcessor(new OilPaintingProcessor(levels, brushSize), rectangle);
	}
}
