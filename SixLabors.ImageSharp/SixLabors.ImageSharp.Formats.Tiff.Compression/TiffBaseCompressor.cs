using System;
using System.IO;
using SixLabors.ImageSharp.Formats.Tiff.Constants;
using SixLabors.ImageSharp.Memory;

namespace SixLabors.ImageSharp.Formats.Tiff.Compression;

internal abstract class TiffBaseCompressor : TiffBaseCompression
{
	public abstract TiffCompression Method { get; }

	public Stream Output { get; }

	protected TiffBaseCompressor(Stream output, MemoryAllocator allocator, int width, int bitsPerPixel, TiffPredictor predictor = TiffPredictor.None)
		: base(allocator, width, bitsPerPixel, predictor)
	{
		Output = output;
	}

	public abstract void Initialize(int rowsPerStrip);

	public abstract void CompressStrip(Span<byte> rows, int height);
}
