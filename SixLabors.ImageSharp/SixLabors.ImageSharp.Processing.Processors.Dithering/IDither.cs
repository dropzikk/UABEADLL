using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing.Processors.Quantization;

namespace SixLabors.ImageSharp.Processing.Processors.Dithering;

public interface IDither
{
	void ApplyQuantizationDither<TFrameQuantizer, TPixel>(ref TFrameQuantizer quantizer, ImageFrame<TPixel> source, IndexedImageFrame<TPixel> destination, Rectangle bounds) where TFrameQuantizer : struct, IQuantizer<TPixel> where TPixel : unmanaged, IPixel<TPixel>;

	void ApplyPaletteDither<TPaletteDitherImageProcessor, TPixel>(in TPaletteDitherImageProcessor processor, ImageFrame<TPixel> source, Rectangle bounds) where TPaletteDitherImageProcessor : struct, IPaletteDitherImageProcessor<TPixel> where TPixel : unmanaged, IPixel<TPixel>;
}
