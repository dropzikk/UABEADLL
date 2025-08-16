using SixLabors.ImageSharp.Processing.Processors;
using SixLabors.ImageSharp.Processing.Processors.Filters;

namespace SixLabors.ImageSharp.Processing;

public static class GrayscaleExtensions
{
	public static IImageProcessingContext Grayscale(this IImageProcessingContext source)
	{
		return source.Grayscale(GrayscaleMode.Bt709);
	}

	public static IImageProcessingContext Grayscale(this IImageProcessingContext source, float amount)
	{
		return source.Grayscale(GrayscaleMode.Bt709, amount);
	}

	public static IImageProcessingContext Grayscale(this IImageProcessingContext source, GrayscaleMode mode)
	{
		return source.Grayscale(mode, 1f);
	}

	public static IImageProcessingContext Grayscale(this IImageProcessingContext source, GrayscaleMode mode, float amount)
	{
		IImageProcessor imageProcessor2;
		if (mode != 0)
		{
			IImageProcessor imageProcessor = new GrayscaleBt601Processor(amount);
			imageProcessor2 = imageProcessor;
		}
		else
		{
			IImageProcessor imageProcessor = new GrayscaleBt709Processor(amount);
			imageProcessor2 = imageProcessor;
		}
		IImageProcessor processor = imageProcessor2;
		source.ApplyProcessor(processor);
		return source;
	}

	public static IImageProcessingContext Grayscale(this IImageProcessingContext source, Rectangle rectangle)
	{
		return source.Grayscale(1f, rectangle);
	}

	public static IImageProcessingContext Grayscale(this IImageProcessingContext source, float amount, Rectangle rectangle)
	{
		return source.Grayscale(GrayscaleMode.Bt709, amount, rectangle);
	}

	public static IImageProcessingContext Grayscale(this IImageProcessingContext source, GrayscaleMode mode, Rectangle rectangle)
	{
		return source.Grayscale(mode, 1f, rectangle);
	}

	public static IImageProcessingContext Grayscale(this IImageProcessingContext source, GrayscaleMode mode, float amount, Rectangle rectangle)
	{
		IImageProcessor imageProcessor2;
		if (mode != 0)
		{
			IImageProcessor imageProcessor = new GrayscaleBt601Processor(amount);
			imageProcessor2 = imageProcessor;
		}
		else
		{
			IImageProcessor imageProcessor = new GrayscaleBt709Processor(amount);
			imageProcessor2 = imageProcessor;
		}
		IImageProcessor processor = imageProcessor2;
		source.ApplyProcessor(processor, rectangle);
		return source;
	}
}
