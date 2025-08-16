using System.IO;
using System.Threading;

namespace SixLabors.ImageSharp.Formats.Qoi;

public class QoiEncoder : ImageEncoder
{
	public QoiChannels? Channels { get; init; }

	public QoiColorSpace? ColorSpace { get; init; }

	protected override void Encode<TPixel>(Image<TPixel> image, Stream stream, CancellationToken cancellationToken)
	{
		new QoiEncoderCore(this, image.Configuration).Encode(image, stream, cancellationToken);
	}
}
