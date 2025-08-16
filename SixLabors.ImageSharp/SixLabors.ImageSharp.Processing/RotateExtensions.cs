using SixLabors.ImageSharp.Processing.Processors.Transforms;

namespace SixLabors.ImageSharp.Processing;

public static class RotateExtensions
{
	public static IImageProcessingContext Rotate(this IImageProcessingContext source, RotateMode rotateMode)
	{
		return source.Rotate((float)rotateMode);
	}

	public static IImageProcessingContext Rotate(this IImageProcessingContext source, float degrees)
	{
		return source.Rotate(degrees, KnownResamplers.Bicubic);
	}

	public static IImageProcessingContext Rotate(this IImageProcessingContext source, float degrees, IResampler sampler)
	{
		return source.ApplyProcessor(new RotateProcessor(degrees, sampler, source.GetCurrentSize()));
	}
}
