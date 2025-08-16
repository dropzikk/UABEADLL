using SixLabors.ImageSharp.Processing.Processors.Convolution;

namespace SixLabors.ImageSharp.Processing;

public static class BoxBlurExtensions
{
	public static IImageProcessingContext BoxBlur(this IImageProcessingContext source)
	{
		return source.ApplyProcessor(new BoxBlurProcessor());
	}

	public static IImageProcessingContext BoxBlur(this IImageProcessingContext source, int radius)
	{
		return source.ApplyProcessor(new BoxBlurProcessor(radius));
	}

	public static IImageProcessingContext BoxBlur(this IImageProcessingContext source, int radius, Rectangle rectangle)
	{
		return source.ApplyProcessor(new BoxBlurProcessor(radius), rectangle);
	}

	public static IImageProcessingContext BoxBlur(this IImageProcessingContext source, int radius, Rectangle rectangle, BorderWrappingMode borderWrapModeX, BorderWrappingMode borderWrapModeY)
	{
		BoxBlurProcessor processor = new BoxBlurProcessor(radius, borderWrapModeX, borderWrapModeY);
		return source.ApplyProcessor(processor, rectangle);
	}
}
