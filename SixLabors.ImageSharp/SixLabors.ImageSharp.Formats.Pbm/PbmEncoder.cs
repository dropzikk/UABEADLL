using System.IO;
using System.Threading;

namespace SixLabors.ImageSharp.Formats.Pbm;

public sealed class PbmEncoder : ImageEncoder
{
	public PbmEncoding? Encoding { get; init; }

	public PbmColorType? ColorType { get; init; }

	public PbmComponentType? ComponentType { get; init; }

	protected override void Encode<TPixel>(Image<TPixel> image, Stream stream, CancellationToken cancellationToken)
	{
		new PbmEncoderCore(image.Configuration, this).Encode(image, stream, cancellationToken);
	}
}
