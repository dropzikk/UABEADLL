using System;
using System.IO;
using SixLabors.ImageSharp.Formats.Tiff.Constants;
using SixLabors.ImageSharp.Memory;

namespace SixLabors.ImageSharp.Formats.Tiff.Compression.Compressors;

internal sealed class LzwCompressor : TiffBaseCompressor
{
	private TiffLzwEncoder lzwEncoder;

	public override TiffCompression Method => TiffCompression.Lzw;

	public LzwCompressor(Stream output, MemoryAllocator allocator, int width, int bitsPerPixel, TiffPredictor predictor)
		: base(output, allocator, width, bitsPerPixel, predictor)
	{
	}

	public override void Initialize(int rowsPerStrip)
	{
		lzwEncoder = new TiffLzwEncoder(base.Allocator);
	}

	public override void CompressStrip(Span<byte> rows, int height)
	{
		if (base.Predictor == TiffPredictor.Horizontal)
		{
			HorizontalPredictor.ApplyHorizontalPrediction(rows, base.BytesPerRow, base.BitsPerPixel);
		}
		lzwEncoder.Encode(rows, base.Output);
	}

	protected override void Dispose(bool disposing)
	{
		lzwEncoder?.Dispose();
	}
}
