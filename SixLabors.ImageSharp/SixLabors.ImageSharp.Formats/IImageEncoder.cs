using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats;

public interface IImageEncoder
{
	bool SkipMetadata { get; init; }

	void Encode<TPixel>(Image<TPixel> image, Stream stream) where TPixel : unmanaged, IPixel<TPixel>;

	Task EncodeAsync<TPixel>(Image<TPixel> image, Stream stream, CancellationToken cancellationToken) where TPixel : unmanaged, IPixel<TPixel>;
}
