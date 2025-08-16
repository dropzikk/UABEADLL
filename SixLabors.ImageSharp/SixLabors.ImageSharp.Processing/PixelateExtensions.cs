using SixLabors.ImageSharp.Processing.Processors.Effects;

namespace SixLabors.ImageSharp.Processing;

public static class PixelateExtensions
{
	public static IImageProcessingContext Pixelate(this IImageProcessingContext source)
	{
		return source.Pixelate(4);
	}

	public static IImageProcessingContext Pixelate(this IImageProcessingContext source, int size)
	{
		return source.ApplyProcessor(new PixelateProcessor(size));
	}

	public static IImageProcessingContext Pixelate(this IImageProcessingContext source, int size, Rectangle rectangle)
	{
		return source.ApplyProcessor(new PixelateProcessor(size), rectangle);
	}
}
