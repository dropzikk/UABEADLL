using SixLabors.ImageSharp.Processing.Processors.Filters;

namespace SixLabors.ImageSharp.Processing;

public static class PolaroidExtensions
{
	public static IImageProcessingContext Polaroid(this IImageProcessingContext source)
	{
		return source.ApplyProcessor(new PolaroidProcessor(source.GetGraphicsOptions()));
	}

	public static IImageProcessingContext Polaroid(this IImageProcessingContext source, Rectangle rectangle)
	{
		return source.ApplyProcessor(new PolaroidProcessor(source.GetGraphicsOptions()), rectangle);
	}
}
