using System.IO;
using System.Threading;
using SixLabors.ImageSharp.Processing.Processors.Quantization;

namespace SixLabors.ImageSharp.Formats.Png;

public class PngEncoder : QuantizingImageEncoder
{
	public PngBitDepth? BitDepth { get; init; }

	public PngColorType? ColorType { get; init; }

	public PngFilterMethod? FilterMethod { get; init; }

	public PngCompressionLevel CompressionLevel { get; init; } = PngCompressionLevel.Level6;

	public int TextCompressionThreshold { get; init; } = 1024;

	public float? Gamma { get; init; }

	public byte Threshold { get; init; } = byte.MaxValue;

	public PngInterlaceMode? InterlaceMethod { get; init; }

	public PngChunkFilter? ChunkFilter { get; init; }

	public PngTransparentColorMode TransparentColorMode { get; init; }

	public PngEncoder()
	{
		base.PixelSamplingStrategy = new ExtensivePixelSamplingStrategy();
	}

	protected override void Encode<TPixel>(Image<TPixel> image, Stream stream, CancellationToken cancellationToken)
	{
		using PngEncoderCore pngEncoderCore = new PngEncoderCore(image.Configuration, this);
		pngEncoderCore.Encode(image, stream, cancellationToken);
	}
}
