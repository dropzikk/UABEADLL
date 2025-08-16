using System.Threading;
using SixLabors.ImageSharp.IO;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats;

internal interface IImageDecoderInternals
{
	DecoderOptions Options { get; }

	Size Dimensions { get; }

	Image<TPixel> Decode<TPixel>(BufferedReadStream stream, CancellationToken cancellationToken) where TPixel : unmanaged, IPixel<TPixel>;

	ImageInfo Identify(BufferedReadStream stream, CancellationToken cancellationToken);
}
