using SixLabors.ImageSharp.Processing.Processors.Normalization;

namespace SixLabors.ImageSharp.Processing;

public static class HistogramEqualizationExtensions
{
	public static IImageProcessingContext HistogramEqualization(this IImageProcessingContext source)
	{
		return source.HistogramEqualization(new HistogramEqualizationOptions());
	}

	public static IImageProcessingContext HistogramEqualization(this IImageProcessingContext source, HistogramEqualizationOptions options)
	{
		return source.ApplyProcessor(HistogramEqualizationProcessor.FromOptions(options));
	}
}
