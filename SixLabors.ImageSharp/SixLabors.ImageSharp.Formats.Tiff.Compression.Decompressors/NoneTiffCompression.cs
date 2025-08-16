using System;
using System.Threading;
using SixLabors.ImageSharp.IO;
using SixLabors.ImageSharp.Memory;

namespace SixLabors.ImageSharp.Formats.Tiff.Compression.Decompressors;

internal sealed class NoneTiffCompression : TiffBaseDecompressor
{
	public NoneTiffCompression(MemoryAllocator memoryAllocator, int width, int bitsPerPixel)
		: base(memoryAllocator, width, bitsPerPixel)
	{
	}

	protected override void Decompress(BufferedReadStream stream, int byteCount, int stripHeight, Span<byte> buffer, CancellationToken cancellationToken)
	{
		stream.Read(buffer, 0, Math.Min(buffer.Length, byteCount));
	}

	protected override void Dispose(bool disposing)
	{
	}
}
