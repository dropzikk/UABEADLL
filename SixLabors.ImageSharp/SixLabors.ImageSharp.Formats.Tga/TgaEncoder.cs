using System.IO;
using System.Threading;

namespace SixLabors.ImageSharp.Formats.Tga;

public sealed class TgaEncoder : ImageEncoder
{
	public TgaBitsPerPixel? BitsPerPixel { get; init; }

	public TgaCompression Compression { get; init; } = TgaCompression.RunLength;

	protected override void Encode<TPixel>(Image<TPixel> image, Stream stream, CancellationToken cancellationToken)
	{
		new TgaEncoderCore(this, image.Configuration.MemoryAllocator).Encode(image, stream, cancellationToken);
	}
}
