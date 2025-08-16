using System;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors.Quantization;

public interface IQuantizer
{
	QuantizerOptions Options { get; }

	IQuantizer<TPixel> CreatePixelSpecificQuantizer<TPixel>(Configuration configuration) where TPixel : unmanaged, IPixel<TPixel>;

	IQuantizer<TPixel> CreatePixelSpecificQuantizer<TPixel>(Configuration configuration, QuantizerOptions options) where TPixel : unmanaged, IPixel<TPixel>;
}
public interface IQuantizer<TPixel> : IDisposable where TPixel : unmanaged, IPixel<TPixel>
{
	Configuration Configuration { get; }

	QuantizerOptions Options { get; }

	ReadOnlyMemory<TPixel> Palette { get; }

	void AddPaletteColors(Buffer2DRegion<TPixel> pixelRegion);

	IndexedImageFrame<TPixel> QuantizeFrame(ImageFrame<TPixel> source, Rectangle bounds);

	byte GetQuantizedColor(TPixel color, out TPixel match);
}
