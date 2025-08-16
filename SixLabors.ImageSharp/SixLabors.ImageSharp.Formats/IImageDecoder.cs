using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats;

public interface IImageDecoder
{
	ImageInfo Identify(DecoderOptions options, Stream stream);

	Task<ImageInfo> IdentifyAsync(DecoderOptions options, Stream stream, CancellationToken cancellationToken = default(CancellationToken));

	Image<TPixel> Decode<TPixel>(DecoderOptions options, Stream stream) where TPixel : unmanaged, IPixel<TPixel>;

	Image Decode(DecoderOptions options, Stream stream);

	Task<Image<TPixel>> DecodeAsync<TPixel>(DecoderOptions options, Stream stream, CancellationToken cancellationToken = default(CancellationToken)) where TPixel : unmanaged, IPixel<TPixel>;

	Task<Image> DecodeAsync(DecoderOptions options, Stream stream, CancellationToken cancellationToken = default(CancellationToken));
}
