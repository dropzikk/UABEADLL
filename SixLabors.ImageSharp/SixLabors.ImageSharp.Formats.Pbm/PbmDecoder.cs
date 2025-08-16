using System.IO;
using System.Threading;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Pbm;

public sealed class PbmDecoder : ImageDecoder
{
	public static PbmDecoder Instance { get; } = new PbmDecoder();

	private PbmDecoder()
	{
	}

	protected override ImageInfo Identify(DecoderOptions options, Stream stream, CancellationToken cancellationToken)
	{
		Guard.NotNull(options, "options");
		Guard.NotNull(stream, "stream");
		return new PbmDecoderCore(options).Identify(options.Configuration, stream, cancellationToken);
	}

	protected override Image<TPixel> Decode<TPixel>(DecoderOptions options, Stream stream, CancellationToken cancellationToken)
	{
		Guard.NotNull(options, "options");
		Guard.NotNull(stream, "stream");
		Image<TPixel> image = new PbmDecoderCore(options).Decode<TPixel>(options.Configuration, stream, cancellationToken);
		ImageDecoder.ScaleToTargetSize(options, image);
		return image;
	}

	protected override Image Decode(DecoderOptions options, Stream stream, CancellationToken cancellationToken)
	{
		return Decode<Rgb24>(options, stream, cancellationToken);
	}
}
