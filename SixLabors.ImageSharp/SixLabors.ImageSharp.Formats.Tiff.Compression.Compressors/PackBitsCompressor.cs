using System;
using System.Buffers;
using System.IO;
using SixLabors.ImageSharp.Formats.Tiff.Constants;
using SixLabors.ImageSharp.Memory;

namespace SixLabors.ImageSharp.Formats.Tiff.Compression.Compressors;

internal sealed class PackBitsCompressor : TiffBaseCompressor
{
	private IMemoryOwner<byte> pixelData;

	public override TiffCompression Method => TiffCompression.PackBits;

	public PackBitsCompressor(Stream output, MemoryAllocator allocator, int width, int bitsPerPixel)
		: base(output, allocator, width, bitsPerPixel)
	{
	}

	public override void Initialize(int rowsPerStrip)
	{
		int num = (base.BytesPerRow + 126) / 127 + 1;
		pixelData = base.Allocator.Allocate<byte>(base.BytesPerRow + num);
	}

	public override void CompressStrip(Span<byte> rows, int height)
	{
		Span<byte> span = pixelData.GetSpan();
		for (int i = 0; i < height; i++)
		{
			int length = PackBitsWriter.PackBits(rows.Slice(i * base.BytesPerRow, base.BytesPerRow), span);
			base.Output.Write(span.Slice(0, length));
		}
	}

	protected override void Dispose(bool disposing)
	{
		pixelData?.Dispose();
	}
}
