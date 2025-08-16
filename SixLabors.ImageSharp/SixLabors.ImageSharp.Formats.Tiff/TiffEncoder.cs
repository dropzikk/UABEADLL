using System.IO;
using System.Threading;
using SixLabors.ImageSharp.Compression.Zlib;
using SixLabors.ImageSharp.Formats.Tiff.Constants;
using SixLabors.ImageSharp.Processing;

namespace SixLabors.ImageSharp.Formats.Tiff;

public class TiffEncoder : QuantizingImageEncoder
{
	public TiffBitsPerPixel? BitsPerPixel { get; init; }

	public TiffCompression? Compression { get; init; }

	public DeflateCompressionLevel? CompressionLevel { get; init; }

	public TiffPhotometricInterpretation? PhotometricInterpretation { get; init; }

	public TiffPredictor? HorizontalPredictor { get; init; }

	public TiffEncoder()
	{
		base.Quantizer = KnownQuantizers.Octree;
	}

	protected override void Encode<TPixel>(Image<TPixel> image, Stream stream, CancellationToken cancellationToken)
	{
		new TiffEncoderCore(this, image.Configuration.MemoryAllocator).Encode(image, stream, cancellationToken);
	}
}
