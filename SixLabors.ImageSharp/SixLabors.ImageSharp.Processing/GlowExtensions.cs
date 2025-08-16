using SixLabors.ImageSharp.Processing.Processors.Overlays;

namespace SixLabors.ImageSharp.Processing;

public static class GlowExtensions
{
	public static IImageProcessingContext Glow(this IImageProcessingContext source)
	{
		return source.Glow(source.GetGraphicsOptions());
	}

	public static IImageProcessingContext Glow(this IImageProcessingContext source, Color color)
	{
		return source.Glow(source.GetGraphicsOptions(), color);
	}

	public static IImageProcessingContext Glow(this IImageProcessingContext source, float radius)
	{
		return source.Glow(source.GetGraphicsOptions(), radius);
	}

	public static IImageProcessingContext Glow(this IImageProcessingContext source, Rectangle rectangle)
	{
		return source.Glow(source.GetGraphicsOptions(), rectangle);
	}

	public static IImageProcessingContext Glow(this IImageProcessingContext source, Color color, float radius, Rectangle rectangle)
	{
		return source.Glow(source.GetGraphicsOptions(), color, ValueSize.Absolute(radius), rectangle);
	}

	public static IImageProcessingContext Glow(this IImageProcessingContext source, GraphicsOptions options)
	{
		return source.Glow(options, Color.Black, ValueSize.PercentageOfWidth(0.5f));
	}

	public static IImageProcessingContext Glow(this IImageProcessingContext source, GraphicsOptions options, Color color)
	{
		return source.Glow(options, color, ValueSize.PercentageOfWidth(0.5f));
	}

	public static IImageProcessingContext Glow(this IImageProcessingContext source, GraphicsOptions options, float radius)
	{
		return source.Glow(options, Color.Black, ValueSize.Absolute(radius));
	}

	public static IImageProcessingContext Glow(this IImageProcessingContext source, GraphicsOptions options, Rectangle rectangle)
	{
		return source.Glow(options, Color.Black, ValueSize.PercentageOfWidth(0.5f), rectangle);
	}

	public static IImageProcessingContext Glow(this IImageProcessingContext source, GraphicsOptions options, Color color, float radius, Rectangle rectangle)
	{
		return source.Glow(options, color, ValueSize.Absolute(radius), rectangle);
	}

	private static IImageProcessingContext Glow(this IImageProcessingContext source, GraphicsOptions options, Color color, ValueSize radius, Rectangle rectangle)
	{
		return source.ApplyProcessor(new GlowProcessor(options, color, radius), rectangle);
	}

	private static IImageProcessingContext Glow(this IImageProcessingContext source, GraphicsOptions options, Color color, ValueSize radius)
	{
		return source.ApplyProcessor(new GlowProcessor(options, color, radius));
	}
}
