using SixLabors.ImageSharp.Processing.Processors.Binarization;

namespace SixLabors.ImageSharp.Processing;

public static class BinaryThresholdExtensions
{
	public static IImageProcessingContext BinaryThreshold(this IImageProcessingContext source, float threshold)
	{
		return source.ApplyProcessor(new BinaryThresholdProcessor(threshold, BinaryThresholdMode.Luminance));
	}

	public static IImageProcessingContext BinaryThreshold(this IImageProcessingContext source, float threshold, BinaryThresholdMode mode)
	{
		return source.ApplyProcessor(new BinaryThresholdProcessor(threshold, mode));
	}

	public static IImageProcessingContext BinaryThreshold(this IImageProcessingContext source, float threshold, Rectangle rectangle)
	{
		return source.ApplyProcessor(new BinaryThresholdProcessor(threshold, BinaryThresholdMode.Luminance), rectangle);
	}

	public static IImageProcessingContext BinaryThreshold(this IImageProcessingContext source, float threshold, BinaryThresholdMode mode, Rectangle rectangle)
	{
		return source.ApplyProcessor(new BinaryThresholdProcessor(threshold, mode), rectangle);
	}

	public static IImageProcessingContext BinaryThreshold(this IImageProcessingContext source, float threshold, Color upperColor, Color lowerColor)
	{
		return source.ApplyProcessor(new BinaryThresholdProcessor(threshold, upperColor, lowerColor, BinaryThresholdMode.Luminance));
	}

	public static IImageProcessingContext BinaryThreshold(this IImageProcessingContext source, float threshold, Color upperColor, Color lowerColor, BinaryThresholdMode mode)
	{
		return source.ApplyProcessor(new BinaryThresholdProcessor(threshold, upperColor, lowerColor, mode));
	}

	public static IImageProcessingContext BinaryThreshold(this IImageProcessingContext source, float threshold, Color upperColor, Color lowerColor, Rectangle rectangle)
	{
		return source.ApplyProcessor(new BinaryThresholdProcessor(threshold, upperColor, lowerColor, BinaryThresholdMode.Luminance), rectangle);
	}

	public static IImageProcessingContext BinaryThreshold(this IImageProcessingContext source, float threshold, Color upperColor, Color lowerColor, BinaryThresholdMode mode, Rectangle rectangle)
	{
		return source.ApplyProcessor(new BinaryThresholdProcessor(threshold, upperColor, lowerColor, mode), rectangle);
	}
}
