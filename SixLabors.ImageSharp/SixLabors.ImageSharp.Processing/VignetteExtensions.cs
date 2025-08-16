using SixLabors.ImageSharp.Processing.Processors.Overlays;

namespace SixLabors.ImageSharp.Processing;

public static class VignetteExtensions
{
	public static IImageProcessingContext Vignette(this IImageProcessingContext source)
	{
		return source.Vignette(source.GetGraphicsOptions());
	}

	public static IImageProcessingContext Vignette(this IImageProcessingContext source, Color color)
	{
		return source.Vignette(source.GetGraphicsOptions(), color);
	}

	public static IImageProcessingContext Vignette(this IImageProcessingContext source, float radiusX, float radiusY)
	{
		return source.Vignette(source.GetGraphicsOptions(), radiusX, radiusY);
	}

	public static IImageProcessingContext Vignette(this IImageProcessingContext source, Rectangle rectangle)
	{
		return source.Vignette(source.GetGraphicsOptions(), rectangle);
	}

	public static IImageProcessingContext Vignette(this IImageProcessingContext source, Color color, float radiusX, float radiusY, Rectangle rectangle)
	{
		return source.Vignette(source.GetGraphicsOptions(), color, radiusX, radiusY, rectangle);
	}

	public static IImageProcessingContext Vignette(this IImageProcessingContext source, GraphicsOptions options)
	{
		return source.VignetteInternal(options, Color.Black, ValueSize.PercentageOfWidth(0.5f), ValueSize.PercentageOfHeight(0.5f));
	}

	public static IImageProcessingContext Vignette(this IImageProcessingContext source, GraphicsOptions options, Color color)
	{
		return source.VignetteInternal(options, color, ValueSize.PercentageOfWidth(0.5f), ValueSize.PercentageOfHeight(0.5f));
	}

	public static IImageProcessingContext Vignette(this IImageProcessingContext source, GraphicsOptions options, float radiusX, float radiusY)
	{
		return source.VignetteInternal(options, Color.Black, radiusX, radiusY);
	}

	public static IImageProcessingContext Vignette(this IImageProcessingContext source, GraphicsOptions options, Rectangle rectangle)
	{
		return source.VignetteInternal(options, Color.Black, ValueSize.PercentageOfWidth(0.5f), ValueSize.PercentageOfHeight(0.5f), rectangle);
	}

	public static IImageProcessingContext Vignette(this IImageProcessingContext source, GraphicsOptions options, Color color, float radiusX, float radiusY, Rectangle rectangle)
	{
		return source.VignetteInternal(options, color, radiusX, radiusY, rectangle);
	}

	private static IImageProcessingContext VignetteInternal(this IImageProcessingContext source, GraphicsOptions options, Color color, ValueSize radiusX, ValueSize radiusY, Rectangle rectangle)
	{
		return source.ApplyProcessor(new VignetteProcessor(options, color, radiusX, radiusY), rectangle);
	}

	private static IImageProcessingContext VignetteInternal(this IImageProcessingContext source, GraphicsOptions options, Color color, ValueSize radiusX, ValueSize radiusY)
	{
		return source.ApplyProcessor(new VignetteProcessor(options, color, radiusX, radiusY));
	}
}
