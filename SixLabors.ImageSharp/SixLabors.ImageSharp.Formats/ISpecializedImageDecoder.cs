using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats;

public interface ISpecializedImageDecoder<T> : IImageDecoder where T : ISpecializedDecoderOptions
{
	Image<TPixel> Decode<TPixel>(T options, Stream stream) where TPixel : unmanaged, IPixel<TPixel>;

	Image Decode(T options, Stream stream);

	Task<Image<TPixel>> DecodeAsync<TPixel>(T options, Stream stream, CancellationToken cancellationToken = default(CancellationToken)) where TPixel : unmanaged, IPixel<TPixel>;

	Task<Image> DecodeAsync(T options, Stream stream, CancellationToken cancellationToken = default(CancellationToken));
}
