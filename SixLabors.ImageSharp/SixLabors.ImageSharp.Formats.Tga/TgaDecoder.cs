using System.IO;
using System.Threading;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Tga;

public sealed class TgaDecoder : ImageDecoder
{
	public static TgaDecoder Instance { get; } = new TgaDecoder();

	private TgaDecoder()
	{
	}

	protected override ImageInfo Identify(DecoderOptions options, Stream stream, CancellationToken cancellationToken)
	{
		Guard.NotNull(options, "options");
		Guard.NotNull(stream, "stream");
		return new TgaDecoderCore(options).Identify(options.Configuration, stream, cancellationToken);
	}

	protected override Image<TPixel> Decode<TPixel>(DecoderOptions options, Stream stream, CancellationToken cancellationToken)
	{
		Guard.NotNull(options, "options");
		Guard.NotNull(stream, "stream");
		Image<TPixel> image = new TgaDecoderCore(options).Decode<TPixel>(options.Configuration, stream, cancellationToken);
		ImageDecoder.ScaleToTargetSize(options, image);
		return image;
	}

	protected override Image Decode(DecoderOptions options, Stream stream, CancellationToken cancellationToken)
	{
		return Decode<Rgba32>(options, stream, cancellationToken);
	}
}
