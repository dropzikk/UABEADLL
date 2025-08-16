using System;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors.Dithering;

public interface IPaletteDitherImageProcessor<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	Configuration Configuration { get; }

	ReadOnlyMemory<TPixel> Palette { get; }

	float DitherScale { get; }

	TPixel GetPaletteColor(TPixel color);
}
