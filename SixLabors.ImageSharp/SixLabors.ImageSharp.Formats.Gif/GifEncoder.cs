using System.IO;
using System.Threading;

namespace SixLabors.ImageSharp.Formats.Gif;

public sealed class GifEncoder : QuantizingImageEncoder
{
	public GifColorTableMode? ColorTableMode { get; init; }

	protected override void Encode<TPixel>(Image<TPixel> image, Stream stream, CancellationToken cancellationToken)
	{
		new GifEncoderCore(image.Configuration, this).Encode(image, stream, cancellationToken);
	}
}
