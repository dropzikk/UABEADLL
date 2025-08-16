using System;
using System.IO;
using SixLabors.ImageSharp.Compression.Zlib;
using SixLabors.ImageSharp.Formats.Tiff.Constants;
using SixLabors.ImageSharp.Memory;

namespace SixLabors.ImageSharp.Formats.Tiff.Compression.Compressors;

internal sealed class DeflateCompressor : TiffBaseCompressor
{
	private readonly DeflateCompressionLevel compressionLevel;

	private readonly MemoryStream memoryStream = new MemoryStream();

	public override TiffCompression Method => TiffCompression.Deflate;

	public DeflateCompressor(Stream output, MemoryAllocator allocator, int width, int bitsPerPixel, TiffPredictor predictor, DeflateCompressionLevel compressionLevel)
		: base(output, allocator, width, bitsPerPixel, predictor)
	{
		this.compressionLevel = compressionLevel;
	}

	public override void Initialize(int rowsPerStrip)
	{
	}

	public override void CompressStrip(Span<byte> rows, int height)
	{
		memoryStream.Seek(0L, SeekOrigin.Begin);
		using (ZlibDeflateStream zlibDeflateStream = new ZlibDeflateStream(base.Allocator, memoryStream, compressionLevel))
		{
			if (base.Predictor == TiffPredictor.Horizontal)
			{
				HorizontalPredictor.ApplyHorizontalPrediction(rows, base.BytesPerRow, base.BitsPerPixel);
			}
			zlibDeflateStream.Write(rows);
			zlibDeflateStream.Flush();
		}
		int count = (int)memoryStream.Position;
		byte[] buffer = memoryStream.GetBuffer();
		base.Output.Write(buffer, 0, count);
	}

	protected override void Dispose(bool disposing)
	{
	}
}
