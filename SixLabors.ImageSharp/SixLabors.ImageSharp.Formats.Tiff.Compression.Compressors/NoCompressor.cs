using System;
using System.IO;
using SixLabors.ImageSharp.Formats.Tiff.Constants;
using SixLabors.ImageSharp.Memory;

namespace SixLabors.ImageSharp.Formats.Tiff.Compression.Compressors;

internal sealed class NoCompressor : TiffBaseCompressor
{
	public override TiffCompression Method => TiffCompression.None;

	public NoCompressor(Stream output, MemoryAllocator memoryAllocator, int width, int bitsPerPixel)
		: base(output, memoryAllocator, width, bitsPerPixel)
	{
	}

	public override void Initialize(int rowsPerStrip)
	{
	}

	public override void CompressStrip(Span<byte> rows, int height)
	{
		base.Output.Write(rows);
	}

	protected override void Dispose(bool disposing)
	{
	}
}
