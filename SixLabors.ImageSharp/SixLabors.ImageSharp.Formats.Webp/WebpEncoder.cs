using System.IO;
using System.Threading;

namespace SixLabors.ImageSharp.Formats.Webp;

public sealed class WebpEncoder : ImageEncoder
{
	public WebpFileFormatType? FileFormat { get; init; }

	public int Quality { get; init; } = 75;

	public WebpEncodingMethod Method { get; init; } = WebpEncodingMethod.Level4;

	public bool UseAlphaCompression { get; init; } = true;

	public int EntropyPasses { get; init; } = 1;

	public int SpatialNoiseShaping { get; init; } = 50;

	public int FilterStrength { get; init; } = 60;

	public WebpTransparentColorMode TransparentColorMode { get; init; }

	public bool NearLossless { get; init; }

	public int NearLosslessQuality { get; init; } = 100;

	protected override void Encode<TPixel>(Image<TPixel> image, Stream stream, CancellationToken cancellationToken)
	{
		new WebpEncoderCore(this, image.Configuration).Encode(image, stream, cancellationToken);
	}
}
