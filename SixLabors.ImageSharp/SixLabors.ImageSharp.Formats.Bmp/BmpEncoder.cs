using System.IO;
using System.Threading;
using SixLabors.ImageSharp.Processing;

namespace SixLabors.ImageSharp.Formats.Bmp;

public sealed class BmpEncoder : QuantizingImageEncoder
{
	public BmpBitsPerPixel? BitsPerPixel { get; init; }

	public bool SupportTransparency { get; init; }

	public BmpEncoder()
	{
		base.Quantizer = KnownQuantizers.Octree;
	}

	protected override void Encode<TPixel>(Image<TPixel> image, Stream stream, CancellationToken cancellationToken)
	{
		new BmpEncoderCore(this, image.Configuration.MemoryAllocator).Encode(image, stream, cancellationToken);
	}
}
