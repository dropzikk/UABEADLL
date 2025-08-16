using System.IO;
using System.Threading;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Jpeg;

public sealed class JpegDecoder : SpecializedImageDecoder<JpegDecoderOptions>
{
	public static JpegDecoder Instance { get; } = new JpegDecoder();

	private JpegDecoder()
	{
	}

	protected override ImageInfo Identify(DecoderOptions options, Stream stream, CancellationToken cancellationToken)
	{
		Guard.NotNull(options, "options");
		Guard.NotNull(stream, "stream");
		using JpegDecoderCore decoder = new JpegDecoderCore(new JpegDecoderOptions
		{
			GeneralOptions = options
		});
		return decoder.Identify(options.Configuration, stream, cancellationToken);
	}

	protected override Image<TPixel> Decode<TPixel>(JpegDecoderOptions options, Stream stream, CancellationToken cancellationToken)
	{
		Guard.NotNull(options, "options");
		Guard.NotNull(stream, "stream");
		using JpegDecoderCore decoder = new JpegDecoderCore(options);
		Image<TPixel> image = decoder.Decode<TPixel>(options.GeneralOptions.Configuration, stream, cancellationToken);
		if (options.ResizeMode != JpegDecoderResizeMode.IdctOnly)
		{
			ImageDecoder.ScaleToTargetSize(options.GeneralOptions, image);
		}
		return image;
	}

	protected override Image Decode(JpegDecoderOptions options, Stream stream, CancellationToken cancellationToken)
	{
		return Decode<Rgb24>(options, stream, cancellationToken);
	}

	protected override JpegDecoderOptions CreateDefaultSpecializedOptions(DecoderOptions options)
	{
		return new JpegDecoderOptions
		{
			GeneralOptions = options
		};
	}
}
