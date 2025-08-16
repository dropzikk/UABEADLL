using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing.Processors.Drawing;

namespace SixLabors.ImageSharp.Processing;

public static class DrawImageExtensions
{
	public static IImageProcessingContext DrawImage(this IImageProcessingContext source, Image foreground, float opacity)
	{
		GraphicsOptions graphicsOptions = source.GetGraphicsOptions();
		return source.DrawImage(foreground, graphicsOptions.ColorBlendingMode, graphicsOptions.AlphaCompositionMode, opacity);
	}

	public static IImageProcessingContext DrawImage(this IImageProcessingContext source, Image foreground, Rectangle foregroundRectangle, float opacity)
	{
		GraphicsOptions graphicsOptions = source.GetGraphicsOptions();
		return source.DrawImage(foreground, foregroundRectangle, graphicsOptions.ColorBlendingMode, graphicsOptions.AlphaCompositionMode, opacity);
	}

	public static IImageProcessingContext DrawImage(this IImageProcessingContext source, Image foreground, PixelColorBlendingMode colorBlending, float opacity)
	{
		return source.DrawImage(foreground, Point.Empty, colorBlending, opacity);
	}

	public static IImageProcessingContext DrawImage(this IImageProcessingContext source, Image foreground, Rectangle foregroundRectangle, PixelColorBlendingMode colorBlending, float opacity)
	{
		return source.DrawImage(foreground, foregroundRectangle, colorBlending, source.GetGraphicsOptions().AlphaCompositionMode, opacity);
	}

	public static IImageProcessingContext DrawImage(this IImageProcessingContext source, Image foreground, PixelColorBlendingMode colorBlending, PixelAlphaCompositionMode alphaComposition, float opacity)
	{
		return source.DrawImage(foreground, Point.Empty, colorBlending, alphaComposition, opacity);
	}

	public static IImageProcessingContext DrawImage(this IImageProcessingContext source, Image foreground, Rectangle foregroundRectangle, PixelColorBlendingMode colorBlending, PixelAlphaCompositionMode alphaComposition, float opacity)
	{
		return source.DrawImage(foreground, Point.Empty, foregroundRectangle, colorBlending, alphaComposition, opacity);
	}

	public static IImageProcessingContext DrawImage(this IImageProcessingContext source, Image foreground, GraphicsOptions options)
	{
		return source.DrawImage(foreground, Point.Empty, options);
	}

	public static IImageProcessingContext DrawImage(this IImageProcessingContext source, Image foreground, Rectangle foregroundRectangle, GraphicsOptions options)
	{
		return source.DrawImage(foreground, Point.Empty, foregroundRectangle, options);
	}

	public static IImageProcessingContext DrawImage(this IImageProcessingContext source, Image foreground, Point backgroundLocation, float opacity)
	{
		GraphicsOptions graphicsOptions = source.GetGraphicsOptions();
		return source.DrawImage(foreground, backgroundLocation, graphicsOptions.ColorBlendingMode, graphicsOptions.AlphaCompositionMode, opacity);
	}

	public static IImageProcessingContext DrawImage(this IImageProcessingContext source, Image foreground, Point backgroundLocation, Rectangle foregroundRectangle, float opacity)
	{
		GraphicsOptions graphicsOptions = source.GetGraphicsOptions();
		return source.DrawImage(foreground, backgroundLocation, foregroundRectangle, graphicsOptions.ColorBlendingMode, graphicsOptions.AlphaCompositionMode, opacity);
	}

	public static IImageProcessingContext DrawImage(this IImageProcessingContext source, Image foreground, Point backgroundLocation, PixelColorBlendingMode colorBlending, float opacity)
	{
		return source.DrawImage(foreground, backgroundLocation, colorBlending, source.GetGraphicsOptions().AlphaCompositionMode, opacity);
	}

	public static IImageProcessingContext DrawImage(this IImageProcessingContext source, Image foreground, Point backgroundLocation, Rectangle foregroundRectangle, PixelColorBlendingMode colorBlending, float opacity)
	{
		return source.DrawImage(foreground, backgroundLocation, foregroundRectangle, colorBlending, source.GetGraphicsOptions().AlphaCompositionMode, opacity);
	}

	public static IImageProcessingContext DrawImage(this IImageProcessingContext source, Image foreground, Point backgroundLocation, GraphicsOptions options)
	{
		return source.DrawImage(foreground, backgroundLocation, options.ColorBlendingMode, options.AlphaCompositionMode, options.BlendPercentage);
	}

	public static IImageProcessingContext DrawImage(this IImageProcessingContext source, Image foreground, Point backgroundLocation, Rectangle foregroundRectangle, GraphicsOptions options)
	{
		return source.DrawImage(foreground, backgroundLocation, foregroundRectangle, options.ColorBlendingMode, options.AlphaCompositionMode, options.BlendPercentage);
	}

	public static IImageProcessingContext DrawImage(this IImageProcessingContext source, Image foreground, Point backgroundLocation, PixelColorBlendingMode colorBlending, PixelAlphaCompositionMode alphaComposition, float opacity)
	{
		return source.ApplyProcessor(new DrawImageProcessor(foreground, backgroundLocation, foreground.Bounds, colorBlending, alphaComposition, opacity));
	}

	public static IImageProcessingContext DrawImage(this IImageProcessingContext source, Image foreground, Point backgroundLocation, Rectangle foregroundRectangle, PixelColorBlendingMode colorBlending, PixelAlphaCompositionMode alphaComposition, float opacity)
	{
		return source.ApplyProcessor(new DrawImageProcessor(foreground, backgroundLocation, foregroundRectangle, colorBlending, alphaComposition, opacity), foregroundRectangle);
	}
}
