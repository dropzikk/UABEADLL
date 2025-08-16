using SixLabors.ImageSharp.Processing.Processors.Binarization;

namespace SixLabors.ImageSharp.Processing;

public static class AdaptiveThresholdExtensions
{
	public static IImageProcessingContext AdaptiveThreshold(this IImageProcessingContext source)
	{
		return source.ApplyProcessor(new AdaptiveThresholdProcessor());
	}

	public static IImageProcessingContext AdaptiveThreshold(this IImageProcessingContext source, float thresholdLimit)
	{
		return source.ApplyProcessor(new AdaptiveThresholdProcessor(thresholdLimit));
	}

	public static IImageProcessingContext AdaptiveThreshold(this IImageProcessingContext source, Color upper, Color lower)
	{
		return source.ApplyProcessor(new AdaptiveThresholdProcessor(upper, lower));
	}

	public static IImageProcessingContext AdaptiveThreshold(this IImageProcessingContext source, Color upper, Color lower, float thresholdLimit)
	{
		return source.ApplyProcessor(new AdaptiveThresholdProcessor(upper, lower, thresholdLimit));
	}

	public static IImageProcessingContext AdaptiveThreshold(this IImageProcessingContext source, Color upper, Color lower, Rectangle rectangle)
	{
		return source.ApplyProcessor(new AdaptiveThresholdProcessor(upper, lower), rectangle);
	}

	public static IImageProcessingContext AdaptiveThreshold(this IImageProcessingContext source, Color upper, Color lower, float thresholdLimit, Rectangle rectangle)
	{
		return source.ApplyProcessor(new AdaptiveThresholdProcessor(upper, lower, thresholdLimit), rectangle);
	}
}
