using System;
using SixLabors.ImageSharp.Processing.Processors.Dithering;

namespace SixLabors.ImageSharp.Processing;

public static class DitherExtensions
{
	public static IImageProcessingContext Dither(this IImageProcessingContext source)
	{
		return source.Dither(KnownDitherings.Bayer8x8);
	}

	public static IImageProcessingContext Dither(this IImageProcessingContext source, IDither dither)
	{
		return source.ApplyProcessor(new PaletteDitherProcessor(dither));
	}

	public static IImageProcessingContext Dither(this IImageProcessingContext source, IDither dither, float ditherScale)
	{
		return source.ApplyProcessor(new PaletteDitherProcessor(dither, ditherScale));
	}

	public static IImageProcessingContext Dither(this IImageProcessingContext source, IDither dither, ReadOnlyMemory<Color> palette)
	{
		return source.ApplyProcessor(new PaletteDitherProcessor(dither, palette));
	}

	public static IImageProcessingContext Dither(this IImageProcessingContext source, IDither dither, float ditherScale, ReadOnlyMemory<Color> palette)
	{
		return source.ApplyProcessor(new PaletteDitherProcessor(dither, ditherScale, palette));
	}

	public static IImageProcessingContext Dither(this IImageProcessingContext source, Rectangle rectangle)
	{
		return source.Dither(KnownDitherings.Bayer8x8, rectangle);
	}

	public static IImageProcessingContext Dither(this IImageProcessingContext source, IDither dither, Rectangle rectangle)
	{
		return source.ApplyProcessor(new PaletteDitherProcessor(dither), rectangle);
	}

	public static IImageProcessingContext Dither(this IImageProcessingContext source, IDither dither, float ditherScale, Rectangle rectangle)
	{
		return source.ApplyProcessor(new PaletteDitherProcessor(dither, ditherScale), rectangle);
	}

	public static IImageProcessingContext Dither(this IImageProcessingContext source, IDither dither, ReadOnlyMemory<Color> palette, Rectangle rectangle)
	{
		return source.ApplyProcessor(new PaletteDitherProcessor(dither, palette), rectangle);
	}

	public static IImageProcessingContext Dither(this IImageProcessingContext source, IDither dither, float ditherScale, ReadOnlyMemory<Color> palette, Rectangle rectangle)
	{
		return source.ApplyProcessor(new PaletteDitherProcessor(dither, ditherScale, palette), rectangle);
	}
}
