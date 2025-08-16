using System;
using System.Buffers;
using System.Threading;
using SixLabors.ImageSharp.IO;
using SixLabors.ImageSharp.Memory;

namespace SixLabors.ImageSharp.Formats.Tiff.Compression.Decompressors;

internal sealed class PackBitsTiffCompression : TiffBaseDecompressor
{
	private IMemoryOwner<byte> compressedDataMemory;

	public PackBitsTiffCompression(MemoryAllocator memoryAllocator, int width, int bitsPerPixel)
		: base(memoryAllocator, width, bitsPerPixel)
	{
	}

	protected override void Decompress(BufferedReadStream stream, int byteCount, int stripHeight, Span<byte> buffer, CancellationToken cancellationToken)
	{
		if (compressedDataMemory == null)
		{
			compressedDataMemory = base.Allocator.Allocate<byte>(byteCount);
		}
		else if (compressedDataMemory.Length() < byteCount)
		{
			compressedDataMemory.Dispose();
			compressedDataMemory = base.Allocator.Allocate<byte>(byteCount);
		}
		Span<byte> span = compressedDataMemory.GetSpan();
		stream.Read(span, 0, byteCount);
		int num = 0;
		int num2 = 0;
		while (num < byteCount)
		{
			byte b = span[num];
			if (b <= 127)
			{
				int num3 = num + 1;
				int num4 = span[num] + 1;
				if (num3 + num4 > span.Length)
				{
					TiffThrowHelper.ThrowImageFormatException("Tiff packbits compression error: not enough data.");
				}
				Span<byte> span2 = span.Slice(num3, num4);
				int num5 = num2;
				span2.CopyTo(buffer.Slice(num5, buffer.Length - num5));
				num += num4 + 1;
				num2 += num4;
			}
			else if (b == 128)
			{
				num++;
			}
			else
			{
				byte value = span[num + 1];
				int num6 = 257 - b;
				buffer.Slice(num2, num6).Fill(value);
				num += 2;
				num2 += num6;
			}
		}
	}

	protected override void Dispose(bool disposing)
	{
		compressedDataMemory?.Dispose();
	}
}
