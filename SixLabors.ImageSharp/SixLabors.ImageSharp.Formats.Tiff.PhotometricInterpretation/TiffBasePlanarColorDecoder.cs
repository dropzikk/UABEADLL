using System.Buffers;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Tiff.PhotometricInterpretation;

internal abstract class TiffBasePlanarColorDecoder<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	public abstract void Decode(IMemoryOwner<byte>[] data, Buffer2D<TPixel> pixels, int left, int top, int width, int height);
}
