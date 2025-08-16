using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SixLabors.ImageSharp.IO;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats;

public abstract class ImageEncoder : IImageEncoder
{
	public bool SkipMetadata { get; init; }

	public void Encode<TPixel>(Image<TPixel> image, Stream stream) where TPixel : unmanaged, IPixel<TPixel>
	{
		EncodeWithSeekableStream(image, stream, default(CancellationToken));
	}

	public Task EncodeAsync<TPixel>(Image<TPixel> image, Stream stream, CancellationToken cancellationToken = default(CancellationToken)) where TPixel : unmanaged, IPixel<TPixel>
	{
		return EncodeWithSeekableStreamAsync(image, stream, cancellationToken);
	}

	protected abstract void Encode<TPixel>(Image<TPixel> image, Stream stream, CancellationToken cancellationToken) where TPixel : unmanaged, IPixel<TPixel>;

	private void EncodeWithSeekableStream<TPixel>(Image<TPixel> image, Stream stream, CancellationToken cancellationToken) where TPixel : unmanaged, IPixel<TPixel>
	{
		Configuration configuration = image.Configuration;
		if (stream.CanSeek)
		{
			Encode(image, stream, cancellationToken);
			return;
		}
		using ChunkedMemoryStream chunkedMemoryStream = new ChunkedMemoryStream(configuration.MemoryAllocator);
		Encode(image, stream, cancellationToken);
		chunkedMemoryStream.Position = 0L;
		chunkedMemoryStream.CopyTo(stream, configuration.StreamProcessingBufferSize);
	}

	private async Task EncodeWithSeekableStreamAsync<TPixel>(Image<TPixel> image, Stream stream, CancellationToken cancellationToken) where TPixel : unmanaged, IPixel<TPixel>
	{
		Configuration configuration = image.Configuration;
		if (stream.CanSeek)
		{
			await DoEncodeAsync(stream).ConfigureAwait(continueOnCapturedContext: false);
			return;
		}
		using (ChunkedMemoryStream ms = new ChunkedMemoryStream(configuration.MemoryAllocator))
		{
			await DoEncodeAsync(ms);
			ms.Position = 0L;
			await ms.CopyToAsync(stream, configuration.StreamProcessingBufferSize, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
		Task DoEncodeAsync(Stream innerStream)
		{
			try
			{
				Encode(image, innerStream, cancellationToken);
				return Task.CompletedTask;
			}
			catch (OperationCanceledException)
			{
				return Task.FromCanceled(cancellationToken);
			}
			catch (Exception exception)
			{
				return Task.FromException(exception);
			}
		}
	}
}
