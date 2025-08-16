using System;
using System.IO;
using System.Threading;
using SixLabors.ImageSharp.Formats.Tiff.Constants;
using SixLabors.ImageSharp.IO;
using SixLabors.ImageSharp.Memory;

namespace SixLabors.ImageSharp.Formats.Tiff.Compression;

internal abstract class TiffBaseDecompressor : TiffBaseCompression
{
	protected TiffBaseDecompressor(MemoryAllocator memoryAllocator, int width, int bitsPerPixel, TiffPredictor predictor = TiffPredictor.None)
		: base(memoryAllocator, width, bitsPerPixel, predictor)
	{
	}

	public void Decompress(BufferedReadStream stream, ulong stripOffset, ulong stripByteCount, int stripHeight, Span<byte> buffer, CancellationToken cancellationToken)
	{
		stream.Seek((long)stripOffset, SeekOrigin.Begin);
		Decompress(stream, (int)stripByteCount, stripHeight, buffer, cancellationToken);
		if ((long)(stripOffset + stripByteCount) < stream.Position)
		{
			TiffThrowHelper.ThrowImageFormatException("Out of range when reading a strip.");
		}
	}

	protected abstract void Decompress(BufferedReadStream stream, int byteCount, int stripHeight, Span<byte> buffer, CancellationToken cancellationToken);
}
