using System.Collections.Generic;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors.Quantization;

public interface IPixelSamplingStrategy
{
	IEnumerable<Buffer2DRegion<TPixel>> EnumeratePixelRegions<TPixel>(Image<TPixel> image) where TPixel : unmanaged, IPixel<TPixel>;

	IEnumerable<Buffer2DRegion<TPixel>> EnumeratePixelRegions<TPixel>(ImageFrame<TPixel> frame) where TPixel : unmanaged, IPixel<TPixel>;
}
