using SixLabors.ImageSharp.Processing.Processors.Filters;

namespace SixLabors.ImageSharp.Processing;

public static class LomographExtensions
{
	public static IImageProcessingContext Lomograph(this IImageProcessingContext source)
	{
		return source.ApplyProcessor(new LomographProcessor(source.GetGraphicsOptions()));
	}

	public static IImageProcessingContext Lomograph(this IImageProcessingContext source, Rectangle rectangle)
	{
		return source.ApplyProcessor(new LomographProcessor(source.GetGraphicsOptions()), rectangle);
	}
}
