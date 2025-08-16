using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats;

public abstract class SpecializedImageDecoder<T> : ImageDecoder, ISpecializedImageDecoder<T>, IImageDecoder where T : ISpecializedDecoderOptions
{
	public Image<TPixel> Decode<TPixel>(T options, Stream stream) where TPixel : unmanaged, IPixel<TPixel>
	{
		Image<TPixel> image = ImageDecoder.WithSeekableStream(options.GeneralOptions, stream, (Stream s) => Decode<TPixel>(options, s, default(CancellationToken)));
		SetDecoderFormat(options.GeneralOptions.Configuration, image);
		return image;
	}

	public Image Decode(T options, Stream stream)
	{
		Image image = ImageDecoder.WithSeekableStream(options.GeneralOptions, stream, (Stream s) => Decode(options, s, default(CancellationToken)));
		SetDecoderFormat(options.GeneralOptions.Configuration, image);
		return image;
	}

	public async Task<Image<TPixel>> DecodeAsync<TPixel>(T options, Stream stream, CancellationToken cancellationToken = default(CancellationToken)) where TPixel : unmanaged, IPixel<TPixel>
	{
		Image<TPixel> image = await ImageDecoder.WithSeekableMemoryStreamAsync(options.GeneralOptions, stream, (Stream s, CancellationToken ct) => Decode<TPixel>(options, s, ct), cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		SetDecoderFormat(options.GeneralOptions.Configuration, image);
		return image;
	}

	public async Task<Image> DecodeAsync(T options, Stream stream, CancellationToken cancellationToken = default(CancellationToken))
	{
		Image image = await ImageDecoder.WithSeekableMemoryStreamAsync(options.GeneralOptions, stream, (Stream s, CancellationToken ct) => Decode(options, s, ct), cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		SetDecoderFormat(options.GeneralOptions.Configuration, image);
		return image;
	}

	protected abstract Image<TPixel> Decode<TPixel>(T options, Stream stream, CancellationToken cancellationToken) where TPixel : unmanaged, IPixel<TPixel>;

	protected abstract Image Decode(T options, Stream stream, CancellationToken cancellationToken);

	protected override Image<TPixel> Decode<TPixel>(DecoderOptions options, Stream stream, CancellationToken cancellationToken)
	{
		return Decode<TPixel>(CreateDefaultSpecializedOptions(options), stream, cancellationToken);
	}

	protected override Image Decode(DecoderOptions options, Stream stream, CancellationToken cancellationToken)
	{
		return Decode(CreateDefaultSpecializedOptions(options), stream, cancellationToken);
	}

	protected abstract T CreateDefaultSpecializedOptions(DecoderOptions options);
}
