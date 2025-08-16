using SixLabors.ImageSharp.Processing.Processors.Transforms;

namespace SixLabors.ImageSharp.Processing;

public static class EntropyCropExtensions
{
	public static IImageProcessingContext EntropyCrop(this IImageProcessingContext source)
	{
		return source.ApplyProcessor(new EntropyCropProcessor());
	}

	public static IImageProcessingContext EntropyCrop(this IImageProcessingContext source, float threshold)
	{
		return source.ApplyProcessor(new EntropyCropProcessor(threshold));
	}
}
