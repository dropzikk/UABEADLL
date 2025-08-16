using SixLabors.ImageSharp.Processing.Processors.Transforms;

namespace SixLabors.ImageSharp.Processing;

public static class SkewExtensions
{
	public static IImageProcessingContext Skew(this IImageProcessingContext source, float degreesX, float degreesY)
	{
		return source.Skew(degreesX, degreesY, KnownResamplers.Bicubic);
	}

	public static IImageProcessingContext Skew(this IImageProcessingContext source, float degreesX, float degreesY, IResampler sampler)
	{
		return source.ApplyProcessor(new SkewProcessor(degreesX, degreesY, sampler, source.GetCurrentSize()));
	}
}
