using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Advanced;

internal interface IPixelSource
{
	Buffer2D<byte> PixelBuffer { get; }
}
internal interface IPixelSource<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	Buffer2D<TPixel> PixelBuffer { get; }
}
