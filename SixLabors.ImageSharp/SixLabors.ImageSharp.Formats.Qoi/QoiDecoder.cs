using System.IO;
using System.Threading;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Qoi;

internal class QoiDecoder : ImageDecoder
{
	public static QoiDecoder Instance { get; } = new QoiDecoder();

	private QoiDecoder()
	{
	}

	protected override Image<TPixel> Decode<TPixel>(DecoderOptions options, Stream stream, CancellationToken cancellationToken)
	{
		Guard.NotNull(options, "options");
		Guard.NotNull(stream, "stream");
		Image<TPixel> image = new QoiDecoderCore(options).Decode<TPixel>(options.Configuration, stream, cancellationToken);
		ImageDecoder.ScaleToTargetSize(options, image);
		return image;
	}

	protected override Image Decode(DecoderOptions options, Stream stream, CancellationToken cancellationToken)
	{
		Guard.NotNull(options, "options");
		Guard.NotNull(stream, "stream");
		return Decode<Rgba32>(options, stream, cancellationToken);
	}

	protected override ImageInfo Identify(DecoderOptions options, Stream stream, CancellationToken cancellationToken)
	{
		Guard.NotNull(options, "options");
		Guard.NotNull(stream, "stream");
		return new QoiDecoderCore(options).Identify(options.Configuration, stream, cancellationToken);
	}
}
