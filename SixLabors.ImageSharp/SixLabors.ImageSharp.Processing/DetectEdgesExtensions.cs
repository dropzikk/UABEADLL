using SixLabors.ImageSharp.Processing.Processors.Convolution;

namespace SixLabors.ImageSharp.Processing;

public static class DetectEdgesExtensions
{
	public static IImageProcessingContext DetectEdges(this IImageProcessingContext source)
	{
		return source.DetectEdges(KnownEdgeDetectorKernels.Sobel);
	}

	public static IImageProcessingContext DetectEdges(this IImageProcessingContext source, Rectangle rectangle)
	{
		return source.DetectEdges(KnownEdgeDetectorKernels.Sobel, rectangle);
	}

	public static IImageProcessingContext DetectEdges(this IImageProcessingContext source, EdgeDetector2DKernel kernel)
	{
		return source.DetectEdges(kernel, grayscale: true);
	}

	public static IImageProcessingContext DetectEdges(this IImageProcessingContext source, EdgeDetector2DKernel kernel, bool grayscale)
	{
		EdgeDetector2DProcessor processor = new EdgeDetector2DProcessor(kernel, grayscale);
		source.ApplyProcessor(processor);
		return source;
	}

	public static IImageProcessingContext DetectEdges(this IImageProcessingContext source, EdgeDetector2DKernel kernel, Rectangle rectangle)
	{
		return source.DetectEdges(kernel, grayscale: true, rectangle);
	}

	public static IImageProcessingContext DetectEdges(this IImageProcessingContext source, EdgeDetector2DKernel kernel, bool grayscale, Rectangle rectangle)
	{
		EdgeDetector2DProcessor processor = new EdgeDetector2DProcessor(kernel, grayscale);
		source.ApplyProcessor(processor, rectangle);
		return source;
	}

	public static IImageProcessingContext DetectEdges(this IImageProcessingContext source, EdgeDetectorKernel kernel)
	{
		return source.DetectEdges(kernel, grayscale: true);
	}

	public static IImageProcessingContext DetectEdges(this IImageProcessingContext source, EdgeDetectorKernel kernel, bool grayscale)
	{
		EdgeDetectorProcessor processor = new EdgeDetectorProcessor(kernel, grayscale);
		source.ApplyProcessor(processor);
		return source;
	}

	public static IImageProcessingContext DetectEdges(this IImageProcessingContext source, EdgeDetectorKernel kernel, Rectangle rectangle)
	{
		return source.DetectEdges(kernel, grayscale: true, rectangle);
	}

	public static IImageProcessingContext DetectEdges(this IImageProcessingContext source, EdgeDetectorKernel kernel, bool grayscale, Rectangle rectangle)
	{
		EdgeDetectorProcessor processor = new EdgeDetectorProcessor(kernel, grayscale);
		source.ApplyProcessor(processor, rectangle);
		return source;
	}

	public static IImageProcessingContext DetectEdges(this IImageProcessingContext source, EdgeDetectorCompassKernel kernel)
	{
		return source.DetectEdges(kernel, grayscale: true);
	}

	public static IImageProcessingContext DetectEdges(this IImageProcessingContext source, EdgeDetectorCompassKernel kernel, bool grayscale)
	{
		EdgeDetectorCompassProcessor processor = new EdgeDetectorCompassProcessor(kernel, grayscale);
		source.ApplyProcessor(processor);
		return source;
	}

	public static IImageProcessingContext DetectEdges(this IImageProcessingContext source, EdgeDetectorCompassKernel kernel, Rectangle rectangle)
	{
		return source.DetectEdges(kernel, grayscale: true, rectangle);
	}

	public static IImageProcessingContext DetectEdges(this IImageProcessingContext source, EdgeDetectorCompassKernel kernel, bool grayscale, Rectangle rectangle)
	{
		EdgeDetectorCompassProcessor processor = new EdgeDetectorCompassProcessor(kernel, grayscale);
		source.ApplyProcessor(processor, rectangle);
		return source;
	}
}
