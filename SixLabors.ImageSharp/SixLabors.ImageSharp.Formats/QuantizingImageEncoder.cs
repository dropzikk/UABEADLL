using SixLabors.ImageSharp.Processing.Processors.Quantization;

namespace SixLabors.ImageSharp.Formats;

public abstract class QuantizingImageEncoder : ImageEncoder
{
	public IQuantizer? Quantizer { get; init; }

	public IPixelSamplingStrategy PixelSamplingStrategy { get; init; } = new DefaultPixelSamplingStrategy();
}
