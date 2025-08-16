using SixLabors.ImageSharp.Processing.Processors.Transforms;

namespace SixLabors.ImageSharp.Processing;

public static class AutoOrientExtensions
{
	public static IImageProcessingContext AutoOrient(this IImageProcessingContext source)
	{
		return source.ApplyProcessor(new AutoOrientProcessor());
	}
}
