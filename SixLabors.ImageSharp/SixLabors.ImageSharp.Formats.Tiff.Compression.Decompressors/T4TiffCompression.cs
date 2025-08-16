using System;
using System.Threading;
using SixLabors.ImageSharp.Formats.Tiff.Constants;
using SixLabors.ImageSharp.IO;
using SixLabors.ImageSharp.Memory;

namespace SixLabors.ImageSharp.Formats.Tiff.Compression.Decompressors;

internal sealed class T4TiffCompression : TiffBaseDecompressor
{
	private readonly FaxCompressionOptions faxCompressionOptions;

	private readonly byte whiteValue;

	private readonly byte blackValue;

	private readonly int width;

	private TiffFillOrder FillOrder { get; }

	public T4TiffCompression(MemoryAllocator allocator, TiffFillOrder fillOrder, int width, int bitsPerPixel, FaxCompressionOptions faxOptions, TiffPhotometricInterpretation photometricInterpretation)
		: base(allocator, width, bitsPerPixel)
	{
		faxCompressionOptions = faxOptions;
		FillOrder = fillOrder;
		this.width = width;
		bool flag = photometricInterpretation == TiffPhotometricInterpretation.WhiteIsZero;
		whiteValue = ((!flag) ? ((byte)1) : ((byte)0));
		blackValue = (flag ? ((byte)1) : ((byte)0));
	}

	protected override void Decompress(BufferedReadStream stream, int byteCount, int stripHeight, Span<byte> buffer, CancellationToken cancellationToken)
	{
		if (faxCompressionOptions.HasFlag(FaxCompressionOptions.TwoDimensionalCoding))
		{
			TiffThrowHelper.ThrowNotSupported("TIFF CCITT 2D compression is not yet supported");
		}
		bool eolPadding = faxCompressionOptions.HasFlag(FaxCompressionOptions.EolPadding);
		T4BitReader t4BitReader = new T4BitReader(stream, FillOrder, byteCount, eolPadding);
		buffer.Clear();
		nint num = 0;
		nuint num2 = 0u;
		nint num3 = 0;
		while (t4BitReader.HasMoreData)
		{
			t4BitReader.ReadNextRun();
			if (t4BitReader.RunLength != 0)
			{
				WritePixelRun(buffer, t4BitReader, num);
				num += (int)t4BitReader.RunLength;
				num2 += t4BitReader.RunLength;
			}
			if (t4BitReader.IsEndOfScanLine)
			{
				nint num4 = 8 - Numerics.Modulo8(num);
				if (num4 != 8)
				{
					BitWriterUtils.WriteBits(buffer, num, num4, 0);
					num += num4;
				}
				num2 = 0u;
				num3++;
				if (num3 >= stripHeight)
				{
					break;
				}
			}
		}
		if (num2 != 0 && (ulong)num2 < (ulong)width)
		{
			t4BitReader.ReadNextRun();
			WritePixelRun(buffer, t4BitReader, num);
		}
	}

	private void WritePixelRun(Span<byte> buffer, T4BitReader bitReader, nint bitsWritten)
	{
		if (bitReader.IsWhiteRun)
		{
			BitWriterUtils.WriteBits(buffer, bitsWritten, (int)bitReader.RunLength, whiteValue);
		}
		else
		{
			BitWriterUtils.WriteBits(buffer, bitsWritten, (int)bitReader.RunLength, blackValue);
		}
	}

	protected override void Dispose(bool disposing)
	{
	}
}
