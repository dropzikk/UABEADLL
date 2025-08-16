using System.IO;
using System.Threading;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Webp;

public sealed class WebpDecoder : SpecializedImageDecoder<WebpDecoderOptions>
{
	public static WebpDecoder Instance { get; } = new WebpDecoder();

	private WebpDecoder()
	{
	}

	protected override ImageInfo Identify(DecoderOptions options, Stream stream, CancellationToken cancellationToken)
	{
		Guard.NotNull(options, "options");
		Guard.NotNull(stream, "stream");
		using WebpDecoderCore decoder = new WebpDecoderCore(new WebpDecoderOptions
		{
			GeneralOptions = options
		});
		return decoder.Identify(options.Configuration, stream, cancellationToken);
	}

	protected override Image<TPixel> Decode<TPixel>(WebpDecoderOptions options, Stream stream, CancellationToken cancellationToken)
	{
		Guard.NotNull(options, "options");
		Guard.NotNull(stream, "stream");
		using WebpDecoderCore decoder = new WebpDecoderCore(options);
		Image<TPixel> image = decoder.Decode<TPixel>(options.GeneralOptions.Configuration, stream, cancellationToken);
		ImageDecoder.ScaleToTargetSize(options.GeneralOptions, image);
		return image;
	}

	protected override Image Decode(WebpDecoderOptions options, Stream stream, CancellationToken cancellationToken)
	{
		return Decode<Rgba32>(options, stream, cancellationToken);
	}

	protected override Image Decode(DecoderOptions options, Stream stream, CancellationToken cancellationToken)
	{
		return Decode<Rgba32>(options, stream, cancellationToken);
	}

	protected override WebpDecoderOptions CreateDefaultSpecializedOptions(DecoderOptions options)
	{
		return new WebpDecoderOptions
		{
			GeneralOptions = options
		};
	}
}
