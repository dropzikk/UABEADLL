using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SixLabors.ImageSharp.IO;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace SixLabors.ImageSharp.Formats;

public abstract class ImageDecoder : IImageDecoder
{
	public Image<TPixel> Decode<TPixel>(DecoderOptions options, Stream stream) where TPixel : unmanaged, IPixel<TPixel>
	{
		Image<TPixel> image = WithSeekableStream(options, stream, (Stream s) => Decode<TPixel>(options, s, default(CancellationToken)));
		SetDecoderFormat(options.Configuration, image);
		return image;
	}

	public Image Decode(DecoderOptions options, Stream stream)
	{
		Image image = WithSeekableStream(options, stream, (Stream s) => Decode(options, s, default(CancellationToken)));
		SetDecoderFormat(options.Configuration, image);
		return image;
	}

	public async Task<Image<TPixel>> DecodeAsync<TPixel>(DecoderOptions options, Stream stream, CancellationToken cancellationToken = default(CancellationToken)) where TPixel : unmanaged, IPixel<TPixel>
	{
		Image<TPixel> image = await WithSeekableMemoryStreamAsync(options, stream, (Stream s, CancellationToken ct) => Decode<TPixel>(options, s, ct), cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		SetDecoderFormat(options.Configuration, image);
		return image;
	}

	public async Task<Image> DecodeAsync(DecoderOptions options, Stream stream, CancellationToken cancellationToken = default(CancellationToken))
	{
		Image image = await WithSeekableMemoryStreamAsync(options, stream, (Stream s, CancellationToken ct) => Decode(options, s, ct), cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		SetDecoderFormat(options.Configuration, image);
		return image;
	}

	public ImageInfo Identify(DecoderOptions options, Stream stream)
	{
		ImageInfo imageInfo = WithSeekableStream(options, stream, (Stream s) => Identify(options, s, default(CancellationToken)));
		SetDecoderFormat(options.Configuration, imageInfo);
		return imageInfo;
	}

	public async Task<ImageInfo> IdentifyAsync(DecoderOptions options, Stream stream, CancellationToken cancellationToken = default(CancellationToken))
	{
		ImageInfo imageInfo = await WithSeekableMemoryStreamAsync(options, stream, (Stream s, CancellationToken ct) => Identify(options, s, ct), cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		SetDecoderFormat(options.Configuration, imageInfo);
		return imageInfo;
	}

	protected abstract Image<TPixel> Decode<TPixel>(DecoderOptions options, Stream stream, CancellationToken cancellationToken) where TPixel : unmanaged, IPixel<TPixel>;

	protected abstract Image Decode(DecoderOptions options, Stream stream, CancellationToken cancellationToken);

	protected abstract ImageInfo Identify(DecoderOptions options, Stream stream, CancellationToken cancellationToken);

	protected static void ScaleToTargetSize(DecoderOptions options, Image image)
	{
		if (ShouldResize(options, image))
		{
			ResizeOptions resizeOptions = new ResizeOptions
			{
				Size = options.TargetSize.Value,
				Sampler = options.Sampler,
				Mode = ResizeMode.Max
			};
			image.Mutate(delegate(IImageProcessingContext x)
			{
				x.Resize(resizeOptions);
			});
		}
	}

	private static bool ShouldResize(DecoderOptions options, Image image)
	{
		if (!options.TargetSize.HasValue)
		{
			return false;
		}
		Size value = options.TargetSize.Value;
		Size size = image.Size;
		if (size.Width != value.Width)
		{
			return size.Height != value.Height;
		}
		return false;
	}

	internal static T WithSeekableStream<T>(DecoderOptions options, Stream stream, Func<Stream, T> action)
	{
		Guard.NotNull(options, "options");
		Guard.NotNull(stream, "stream");
		if (!stream.CanRead)
		{
			throw new NotSupportedException("Cannot read from the stream.");
		}
		if (stream.CanSeek)
		{
			return PeformActionAndResetPosition(stream, stream.Position);
		}
		Configuration configuration = options.Configuration;
		using (ChunkedMemoryStream chunkedMemoryStream = new ChunkedMemoryStream(configuration.MemoryAllocator))
		{
			stream.CopyTo(chunkedMemoryStream, configuration.StreamProcessingBufferSize);
			chunkedMemoryStream.Position = 0L;
			return action(chunkedMemoryStream);
		}
		T PeformActionAndResetPosition(Stream s, long position)
		{
			T result = action(s);
			if (stream.Position != s.Position && s.Position != s.Length)
			{
				stream.Position = position + s.Position;
			}
			return result;
		}
	}

	internal static Task<T> WithSeekableMemoryStreamAsync<T>(DecoderOptions options, Stream stream, Func<Stream, CancellationToken, T> action, CancellationToken cancellationToken)
	{
		Guard.NotNull(options, "options");
		Guard.NotNull(stream, "stream");
		if (!stream.CanRead)
		{
			throw new NotSupportedException("Cannot read from the stream.");
		}
		if (stream is MemoryStream memoryStream)
		{
			return PeformActionAndResetPosition(memoryStream, memoryStream.Position, cancellationToken);
		}
		if (stream is ChunkedMemoryStream chunkedMemoryStream)
		{
			return PeformActionAndResetPosition(chunkedMemoryStream, chunkedMemoryStream.Position, cancellationToken);
		}
		return CopyToMemoryStreamAndActionAsync(options, stream, PeformActionAndResetPosition, cancellationToken);
		Task<T> PeformActionAndResetPosition(Stream s, long position, CancellationToken ct)
		{
			try
			{
				T result = action(s, ct);
				if (stream.CanSeek && stream.Position != s.Position && s.Position != s.Length)
				{
					stream.Position = position + s.Position;
				}
				return Task.FromResult(result);
			}
			catch (OperationCanceledException)
			{
				return Task.FromCanceled<T>(cancellationToken);
			}
			catch (Exception exception)
			{
				return Task.FromException<T>(exception);
			}
		}
	}

	private static async Task<T> CopyToMemoryStreamAndActionAsync<T>(DecoderOptions options, Stream stream, Func<Stream, long, CancellationToken, Task<T>> action, CancellationToken cancellationToken)
	{
		long position = (stream.CanSeek ? stream.Position : 0);
		Configuration configuration = options.Configuration;
		using ChunkedMemoryStream memoryStream = new ChunkedMemoryStream(configuration.MemoryAllocator);
		await stream.CopyToAsync(memoryStream, configuration.StreamProcessingBufferSize, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		memoryStream.Position = 0L;
		return await action(memoryStream, position, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
	}

	internal void SetDecoderFormat(Configuration configuration, Image image)
	{
		if (configuration.ImageFormatsManager.TryFindFormatByDecoder(this, out IImageFormat format))
		{
			image.Metadata.DecodedImageFormat = format;
		}
	}

	internal void SetDecoderFormat(Configuration configuration, ImageInfo info)
	{
		if (configuration.ImageFormatsManager.TryFindFormatByDecoder(this, out IImageFormat format))
		{
			info.Metadata.DecodedImageFormat = format;
		}
	}
}
