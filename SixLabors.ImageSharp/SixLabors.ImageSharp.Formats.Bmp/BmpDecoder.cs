using System.IO;
using System.Threading;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Bmp;

public sealed class BmpDecoder : SpecializedImageDecoder<BmpDecoderOptions>
{
	public static BmpDecoder Instance { get; } = new BmpDecoder();

	private BmpDecoder()
	{
	}

	protected override ImageInfo Identify(DecoderOptions options, Stream stream, CancellationToken cancellationToken)
	{
		Guard.NotNull(options, "options");
		Guard.NotNull(stream, "stream");
		return new BmpDecoderCore(new BmpDecoderOptions
		{
			GeneralOptions = options
		}).Identify(options.Configuration, stream, cancellationToken);
	}

	protected override Image<TPixel> Decode<TPixel>(BmpDecoderOptions options, Stream stream, CancellationToken cancellationToken)
	{
		Guard.NotNull(options, "options");
		Guard.NotNull(stream, "stream");
		Image<TPixel> image = new BmpDecoderCore(options).Decode<TPixel>(options.GeneralOptions.Configuration, stream, cancellationToken);
		ImageDecoder.ScaleToTargetSize(options.GeneralOptions, image);
		return image;
	}

	protected override Image Decode(BmpDecoderOptions options, Stream stream, CancellationToken cancellationToken)
	{
		return Decode<Rgba32>(options, stream, cancellationToken);
	}

	protected override BmpDecoderOptions CreateDefaultSpecializedOptions(DecoderOptions options)
	{
		return new BmpDecoderOptions
		{
			GeneralOptions = options
		};
	}
}
