using SixLabors.ImageSharp.Processing.Processors.Dithering;

namespace SixLabors.ImageSharp.Processing;

public static class BinaryDitherExtensions
{
	public static IImageProcessingContext BinaryDither(this IImageProcessingContext source, IDither dither)
	{
		return source.BinaryDither(dither, Color.White, Color.Black);
	}

	public static IImageProcessingContext BinaryDither(this IImageProcessingContext source, IDither dither, Color upperColor, Color lowerColor)
	{
		return source.ApplyProcessor(new PaletteDitherProcessor(dither, new Color[2] { upperColor, lowerColor }));
	}

	public static IImageProcessingContext BinaryDither(this IImageProcessingContext source, IDither dither, Rectangle rectangle)
	{
		return source.BinaryDither(dither, Color.White, Color.Black, rectangle);
	}

	public static IImageProcessingContext BinaryDither(this IImageProcessingContext source, IDither dither, Color upperColor, Color lowerColor, Rectangle rectangle)
	{
		return source.ApplyProcessor(new PaletteDitherProcessor(dither, new Color[2] { upperColor, lowerColor }), rectangle);
	}
}
