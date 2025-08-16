using System;
using System.Threading;
using SixLabors.ImageSharp.Formats.Tiff.Constants;
using SixLabors.ImageSharp.Formats.Tiff.PhotometricInterpretation;
using SixLabors.ImageSharp.IO;
using SixLabors.ImageSharp.Memory;

namespace SixLabors.ImageSharp.Formats.Tiff.Compression.Decompressors;

internal sealed class LzwTiffCompression : TiffBaseDecompressor
{
	private readonly bool isBigEndian;

	private readonly TiffColorType colorType;

	public LzwTiffCompression(MemoryAllocator memoryAllocator, int width, int bitsPerPixel, TiffColorType colorType, TiffPredictor predictor, bool isBigEndian)
		: base(memoryAllocator, width, bitsPerPixel, predictor)
	{
		this.colorType = colorType;
		this.isBigEndian = isBigEndian;
	}

	protected override void Decompress(BufferedReadStream stream, int byteCount, int stripHeight, Span<byte> buffer, CancellationToken cancellationToken)
	{
		new TiffLzwDecoder(stream).DecodePixels(buffer);
		if (base.Predictor == TiffPredictor.Horizontal)
		{
			HorizontalPredictor.Undo(buffer, base.Width, colorType, isBigEndian);
		}
	}

	protected override void Dispose(bool disposing)
	{
	}
}
