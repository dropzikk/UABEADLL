using SixLabors.ImageSharp.Processing.Processors.Transforms;

namespace SixLabors.ImageSharp.Processing;

public static class FlipExtensions
{
	public static IImageProcessingContext Flip(this IImageProcessingContext source, FlipMode flipMode)
	{
		return source.ApplyProcessor(new FlipProcessor(flipMode));
	}
}
