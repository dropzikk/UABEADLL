using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing.Processors.Effects;

namespace SixLabors.ImageSharp.Processing;

public static class PixelRowDelegateExtensions
{
	public static IImageProcessingContext ProcessPixelRowsAsVector4(this IImageProcessingContext source, PixelRowOperation rowOperation)
	{
		return source.ProcessPixelRowsAsVector4(rowOperation, PixelConversionModifiers.None);
	}

	public static IImageProcessingContext ProcessPixelRowsAsVector4(this IImageProcessingContext source, PixelRowOperation rowOperation, PixelConversionModifiers modifiers)
	{
		return source.ApplyProcessor(new PixelRowDelegateProcessor(rowOperation, modifiers));
	}

	public static IImageProcessingContext ProcessPixelRowsAsVector4(this IImageProcessingContext source, PixelRowOperation rowOperation, Rectangle rectangle)
	{
		return source.ProcessPixelRowsAsVector4(rowOperation, rectangle, PixelConversionModifiers.None);
	}

	public static IImageProcessingContext ProcessPixelRowsAsVector4(this IImageProcessingContext source, PixelRowOperation rowOperation, Rectangle rectangle, PixelConversionModifiers modifiers)
	{
		return source.ApplyProcessor(new PixelRowDelegateProcessor(rowOperation, modifiers), rectangle);
	}

	public static IImageProcessingContext ProcessPixelRowsAsVector4(this IImageProcessingContext source, PixelRowOperation<Point> rowOperation)
	{
		return source.ProcessPixelRowsAsVector4(rowOperation, PixelConversionModifiers.None);
	}

	public static IImageProcessingContext ProcessPixelRowsAsVector4(this IImageProcessingContext source, PixelRowOperation<Point> rowOperation, PixelConversionModifiers modifiers)
	{
		return source.ApplyProcessor(new PositionAwarePixelRowDelegateProcessor(rowOperation, modifiers));
	}

	public static IImageProcessingContext ProcessPixelRowsAsVector4(this IImageProcessingContext source, PixelRowOperation<Point> rowOperation, Rectangle rectangle)
	{
		return source.ProcessPixelRowsAsVector4(rowOperation, rectangle, PixelConversionModifiers.None);
	}

	public static IImageProcessingContext ProcessPixelRowsAsVector4(this IImageProcessingContext source, PixelRowOperation<Point> rowOperation, Rectangle rectangle, PixelConversionModifiers modifiers)
	{
		return source.ApplyProcessor(new PositionAwarePixelRowDelegateProcessor(rowOperation, modifiers), rectangle);
	}
}
