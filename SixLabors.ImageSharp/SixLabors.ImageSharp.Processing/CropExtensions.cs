using SixLabors.ImageSharp.Processing.Processors.Transforms;

namespace SixLabors.ImageSharp.Processing;

public static class CropExtensions
{
	public static IImageProcessingContext Crop(this IImageProcessingContext source, int width, int height)
	{
		return source.Crop(new Rectangle(0, 0, width, height));
	}

	public static IImageProcessingContext Crop(this IImageProcessingContext source, Rectangle cropRectangle)
	{
		return source.ApplyProcessor(new CropProcessor(cropRectangle, source.GetCurrentSize()));
	}
}
