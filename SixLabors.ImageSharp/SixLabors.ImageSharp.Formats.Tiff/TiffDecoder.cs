using System.IO;
using System.Threading;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Tiff;

public class TiffDecoder : ImageDecoder
{
	public static TiffDecoder Instance { get; } = new TiffDecoder();

	private TiffDecoder()
	{
	}

	protected override ImageInfo Identify(DecoderOptions options, Stream stream, CancellationToken cancellationToken)
	{
		Guard.NotNull(options, "options");
		Guard.NotNull(stream, "stream");
		return new TiffDecoderCore(options).Identify(options.Configuration, stream, cancellationToken);
	}

	protected override Image<TPixel> Decode<TPixel>(DecoderOptions options, Stream stream, CancellationToken cancellationToken)
	{
		Guard.NotNull(options, "options");
		Guard.NotNull(stream, "stream");
		Image<TPixel> image = new TiffDecoderCore(options).Decode<TPixel>(options.Configuration, stream, cancellationToken);
		ImageDecoder.ScaleToTargetSize(options, image);
		return image;
	}

	protected override Image Decode(DecoderOptions options, Stream stream, CancellationToken cancellationToken)
	{
		return Decode<Rgba32>(options, stream, cancellationToken);
	}
}
