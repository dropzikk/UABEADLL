using System.IO;
using System.Threading;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Png;

public sealed class PngDecoder : SpecializedImageDecoder<PngDecoderOptions>
{
	public static PngDecoder Instance { get; } = new PngDecoder();

	private PngDecoder()
	{
	}

	protected override ImageInfo Identify(DecoderOptions options, Stream stream, CancellationToken cancellationToken)
	{
		Guard.NotNull(options, "options");
		Guard.NotNull(stream, "stream");
		return new PngDecoderCore(new PngDecoderOptions
		{
			GeneralOptions = options
		}).Identify(options.Configuration, stream, cancellationToken);
	}

	protected override Image<TPixel> Decode<TPixel>(PngDecoderOptions options, Stream stream, CancellationToken cancellationToken)
	{
		Guard.NotNull(options, "options");
		Guard.NotNull(stream, "stream");
		Image<TPixel> image = new PngDecoderCore(options).Decode<TPixel>(options.GeneralOptions.Configuration, stream, cancellationToken);
		ImageDecoder.ScaleToTargetSize(options.GeneralOptions, image);
		return image;
	}

	protected override Image Decode(PngDecoderOptions options, Stream stream, CancellationToken cancellationToken)
	{
		Guard.NotNull(options, "options");
		Guard.NotNull(stream, "stream");
		ImageInfo imageInfo = new PngDecoderCore(options, colorMetadataOnly: true).Identify(options.GeneralOptions.Configuration, stream, cancellationToken);
		stream.Position = 0L;
		PngMetadata pngMetadata = imageInfo.Metadata.GetPngMetadata();
		PngColorType valueOrDefault = pngMetadata.ColorType.GetValueOrDefault();
		PngBitDepth valueOrDefault2 = pngMetadata.BitDepth.GetValueOrDefault();
		switch (valueOrDefault)
		{
		case PngColorType.Grayscale:
			if (valueOrDefault2 == PngBitDepth.Bit16)
			{
				if (pngMetadata.TransparentColor.HasValue)
				{
					return Decode<La32>(options, stream, cancellationToken);
				}
				return Decode<L16>(options, stream, cancellationToken);
			}
			if (pngMetadata.TransparentColor.HasValue)
			{
				return Decode<La16>(options, stream, cancellationToken);
			}
			return Decode<L8>(options, stream, cancellationToken);
		case PngColorType.Rgb:
			if (valueOrDefault2 == PngBitDepth.Bit16)
			{
				if (pngMetadata.TransparentColor.HasValue)
				{
					return Decode<Rgba64>(options, stream, cancellationToken);
				}
				return Decode<Rgb48>(options, stream, cancellationToken);
			}
			if (pngMetadata.TransparentColor.HasValue)
			{
				return Decode<Rgba32>(options, stream, cancellationToken);
			}
			return Decode<Rgb24>(options, stream, cancellationToken);
		case PngColorType.Palette:
			return Decode<Rgba32>(options, stream, cancellationToken);
		case PngColorType.GrayscaleWithAlpha:
			if (valueOrDefault2 != PngBitDepth.Bit16)
			{
				return Decode<La16>(options, stream, cancellationToken);
			}
			return Decode<La32>(options, stream, cancellationToken);
		case PngColorType.RgbWithAlpha:
			if (valueOrDefault2 != PngBitDepth.Bit16)
			{
				return Decode<Rgba32>(options, stream, cancellationToken);
			}
			return Decode<Rgba64>(options, stream, cancellationToken);
		default:
			return Decode<Rgba32>(options, stream, cancellationToken);
		}
	}

	protected override PngDecoderOptions CreateDefaultSpecializedOptions(DecoderOptions options)
	{
		return new PngDecoderOptions
		{
			GeneralOptions = options
		};
	}
}
