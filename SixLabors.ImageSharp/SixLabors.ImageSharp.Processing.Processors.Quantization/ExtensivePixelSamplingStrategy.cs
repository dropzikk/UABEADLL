using System.Collections.Generic;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors.Quantization;

public class ExtensivePixelSamplingStrategy : IPixelSamplingStrategy
{
	public IEnumerable<Buffer2DRegion<TPixel>> EnumeratePixelRegions<TPixel>(Image<TPixel> image) where TPixel : unmanaged, IPixel<TPixel>
	{
		foreach (ImageFrame<TPixel> frame in image.Frames)
		{
			yield return frame.PixelBuffer.GetRegion();
		}
	}

	public IEnumerable<Buffer2DRegion<TPixel>> EnumeratePixelRegions<TPixel>(ImageFrame<TPixel> frame) where TPixel : unmanaged, IPixel<TPixel>
	{
		yield return frame.PixelBuffer.GetRegion();
	}
}
