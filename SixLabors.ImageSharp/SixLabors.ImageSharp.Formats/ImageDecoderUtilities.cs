using System;
using System.IO;
using System.Threading;
using SixLabors.ImageSharp.IO;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats;

internal static class ImageDecoderUtilities
{
	internal static ImageInfo Identify(this IImageDecoderInternals decoder, Configuration configuration, Stream stream, CancellationToken cancellationToken)
	{
		using BufferedReadStream stream2 = new BufferedReadStream(configuration, stream, cancellationToken);
		try
		{
			return decoder.Identify(stream2, cancellationToken);
		}
		catch (InvalidMemoryOperationException memoryException)
		{
			throw new InvalidImageContentException(decoder.Dimensions, memoryException);
		}
		catch (Exception)
		{
			throw;
		}
	}

	internal static Image<TPixel> Decode<TPixel>(this IImageDecoderInternals decoder, Configuration configuration, Stream stream, CancellationToken cancellationToken) where TPixel : unmanaged, IPixel<TPixel>
	{
		return decoder.Decode<TPixel>(configuration, stream, DefaultLargeImageExceptionFactory, cancellationToken);
	}

	internal static Image<TPixel> Decode<TPixel>(this IImageDecoderInternals decoder, Configuration configuration, Stream stream, Func<InvalidMemoryOperationException, Size, InvalidImageContentException> largeImageExceptionFactory, CancellationToken cancellationToken) where TPixel : unmanaged, IPixel<TPixel>
	{
		BufferedReadStream bufferedReadStream = (stream as BufferedReadStream) ?? new BufferedReadStream(configuration, stream, cancellationToken);
		try
		{
			return decoder.Decode<TPixel>(bufferedReadStream, cancellationToken);
		}
		catch (InvalidMemoryOperationException arg)
		{
			throw largeImageExceptionFactory(arg, decoder.Dimensions);
		}
		catch (Exception)
		{
			throw;
		}
		finally
		{
			if (bufferedReadStream != stream)
			{
				bufferedReadStream.Dispose();
			}
		}
	}

	private static InvalidImageContentException DefaultLargeImageExceptionFactory(InvalidMemoryOperationException memoryOperationException, Size dimensions)
	{
		return new InvalidImageContentException(dimensions, memoryOperationException);
	}
}
