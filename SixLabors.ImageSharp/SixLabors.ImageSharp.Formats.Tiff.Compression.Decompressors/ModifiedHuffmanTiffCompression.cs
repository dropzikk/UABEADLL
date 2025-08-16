using System;
using System.Threading;
using SixLabors.ImageSharp.Formats.Tiff.Constants;
using SixLabors.ImageSharp.IO;
using SixLabors.ImageSharp.Memory;

namespace SixLabors.ImageSharp.Formats.Tiff.Compression.Decompressors;

internal sealed class ModifiedHuffmanTiffCompression : TiffBaseDecompressor
{
	private readonly byte whiteValue;

	private readonly byte blackValue;

	private TiffFillOrder FillOrder { get; }

	public ModifiedHuffmanTiffCompression(MemoryAllocator allocator, TiffFillOrder fillOrder, int width, int bitsPerPixel, TiffPhotometricInterpretation photometricInterpretation)
		: base(allocator, width, bitsPerPixel)
	{
		FillOrder = fillOrder;
		bool flag = photometricInterpretation == TiffPhotometricInterpretation.WhiteIsZero;
		whiteValue = ((!flag) ? ((byte)1) : ((byte)0));
		blackValue = (flag ? ((byte)1) : ((byte)0));
	}

	protected override void Decompress(BufferedReadStream stream, int byteCount, int stripHeight, Span<byte> buffer, CancellationToken cancellationToken)
	{
		ModifiedHuffmanBitReader modifiedHuffmanBitReader = new ModifiedHuffmanBitReader(stream, FillOrder, byteCount);
		buffer.Clear();
		nint num = 0;
		nuint num2 = 0u;
		nint num3 = 0;
		while (modifiedHuffmanBitReader.HasMoreData)
		{
			modifiedHuffmanBitReader.ReadNextRun();
			if (modifiedHuffmanBitReader.RunLength != 0)
			{
				if (modifiedHuffmanBitReader.IsWhiteRun)
				{
					BitWriterUtils.WriteBits(buffer, num, (int)modifiedHuffmanBitReader.RunLength, whiteValue);
				}
				else
				{
					BitWriterUtils.WriteBits(buffer, num, (int)modifiedHuffmanBitReader.RunLength, blackValue);
				}
				num += (int)modifiedHuffmanBitReader.RunLength;
				num2 += modifiedHuffmanBitReader.RunLength;
			}
			if ((ulong)num2 == (ulong)base.Width)
			{
				num3++;
				num2 = 0u;
				nint num4 = 8 - Numerics.Modulo8(num);
				if (num4 != 8)
				{
					BitWriterUtils.WriteBits(buffer, num, num4, 0);
					num += num4;
				}
				if (num3 >= stripHeight)
				{
					break;
				}
				modifiedHuffmanBitReader.StartNewRow();
			}
			if ((ulong)num2 > (ulong)base.Width)
			{
				TiffThrowHelper.ThrowImageFormatException("ccitt compression parsing error, decoded more pixels then image width");
			}
		}
	}

	protected override void Dispose(bool disposing)
	{
	}
}
