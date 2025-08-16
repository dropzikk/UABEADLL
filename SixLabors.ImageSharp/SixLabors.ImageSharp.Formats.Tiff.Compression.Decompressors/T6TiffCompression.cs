using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using SixLabors.ImageSharp.Formats.Tiff.Constants;
using SixLabors.ImageSharp.IO;
using SixLabors.ImageSharp.Memory;

namespace SixLabors.ImageSharp.Formats.Tiff.Compression.Decompressors;

internal sealed class T6TiffCompression : TiffBaseDecompressor
{
	private readonly bool isWhiteZero;

	private readonly int width;

	private readonly byte white;

	private TiffFillOrder FillOrder { get; }

	public T6TiffCompression(MemoryAllocator allocator, TiffFillOrder fillOrder, int width, int bitsPerPixel, TiffPhotometricInterpretation photometricInterpretation)
		: base(allocator, width, bitsPerPixel)
	{
		FillOrder = fillOrder;
		this.width = width;
		isWhiteZero = photometricInterpretation == TiffPhotometricInterpretation.WhiteIsZero;
		white = (byte)((!isWhiteZero) ? 255u : 0u);
	}

	protected override void Decompress(BufferedReadStream stream, int byteCount, int stripHeight, Span<byte> buffer, CancellationToken cancellationToken)
	{
		buffer.Clear();
		using IMemoryOwner<byte> buffer2 = base.Allocator.Allocate<byte>(width * 2);
		Span<byte> span = buffer2.GetSpan();
		Span<byte> span2 = span.Slice(0, width);
		span = buffer2.GetSpan();
		Span<byte> span3 = span.Slice(width, width);
		T6BitReader bitReader = new T6BitReader(stream, FillOrder, byteCount);
		CcittReferenceScanline referenceScanline = new CcittReferenceScanline(isWhiteZero, width);
		nint bitsWritten = 0;
		for (int i = 0; i < stripHeight; i++)
		{
			span2.Clear();
			Decode2DScanline(bitReader, isWhiteZero, referenceScanline, span2);
			bitsWritten = WriteScanLine(buffer, span2, bitsWritten);
			span2.CopyTo(span3);
			referenceScanline = new CcittReferenceScanline(isWhiteZero, span3);
		}
	}

	private nint WriteScanLine(Span<byte> buffer, Span<byte> scanLine, nint bitsWritten)
	{
		nint num = Numerics.Modulo8(bitsWritten);
		nint num2 = bitsWritten / 8;
		ref byte reference = ref MemoryMarshal.GetReference(scanLine);
		for (nuint num3 = 0u; num3 < (uint)scanLine.Length; num3++)
		{
			if (Unsafe.Add(ref reference, num3) != white)
			{
				BitWriterUtils.WriteBit(buffer, num2, num);
			}
			num++;
			bitsWritten++;
			if (num >= 8)
			{
				num = 0;
				num2++;
			}
		}
		nint num4 = Numerics.Modulo8(bitsWritten);
		if (num4 != 0)
		{
			nint num5 = 8 - num4;
			BitWriterUtils.WriteBits(buffer, bitsWritten, num5, 0);
			bitsWritten += num5;
		}
		return bitsWritten;
	}

	private static void Decode2DScanline(T6BitReader bitReader, bool whiteIsZero, CcittReferenceScanline referenceScanline, Span<byte> scanline)
	{
		int length = scanline.Length;
		bitReader.StartNewRow();
		int a = -1;
		byte b = (byte)((!whiteIsZero) ? byte.MaxValue : 0);
		int num = 0;
		while (true)
		{
			if (bitReader.ReadNextCodeWord())
			{
				if (whiteIsZero)
				{
					scanline.Clear();
				}
				else
				{
					scanline.Fill(byte.MaxValue);
				}
				break;
			}
			int num2 = referenceScanline.FindB1(a, b);
			switch (bitReader.Code.Type)
			{
			case CcittTwoDimensionalCodeType.None:
				TiffThrowHelper.ThrowImageFormatException("ccitt compression parsing error, could not read a valid code word.");
				break;
			case CcittTwoDimensionalCodeType.Pass:
			{
				int num5 = referenceScanline.FindB2(num2);
				int num4 = num;
				scanline.Slice(num4, num5 - num4).Fill(b);
				num = num5;
				a = num5;
				break;
			}
			case CcittTwoDimensionalCodeType.Horizontal:
			{
				bitReader.ReadNextRun();
				int runLength = (int)bitReader.RunLength;
				if (runLength > (uint)(scanline.Length - num))
				{
					TiffThrowHelper.ThrowImageFormatException("ccitt compression parsing error");
				}
				scanline.Slice(num, runLength).Fill(b);
				num += runLength;
				b = (byte)(~b);
				bitReader.ReadNextRun();
				runLength = (int)bitReader.RunLength;
				if (runLength > (uint)(scanline.Length - num))
				{
					TiffThrowHelper.ThrowImageFormatException("ccitt compression parsing error");
				}
				scanline.Slice(num, runLength).Fill(b);
				num += runLength;
				b = (byte)(~b);
				a = num;
				break;
			}
			case CcittTwoDimensionalCodeType.Vertical0:
			{
				int num3 = num2;
				int num4 = num;
				scanline.Slice(num4, num3 - num4).Fill(b);
				num = num3;
				a = num3;
				b = (byte)(~b);
				bitReader.SwapColor();
				break;
			}
			case CcittTwoDimensionalCodeType.VerticalR1:
			{
				int num3 = num2 + 1;
				int num4 = num;
				scanline.Slice(num4, num3 - num4).Fill(b);
				num = num3;
				a = num3;
				b = (byte)(~b);
				bitReader.SwapColor();
				break;
			}
			case CcittTwoDimensionalCodeType.VerticalR2:
			{
				int num3 = num2 + 2;
				int num4 = num;
				scanline.Slice(num4, num3 - num4).Fill(b);
				num = num3;
				a = num3;
				b = (byte)(~b);
				bitReader.SwapColor();
				break;
			}
			case CcittTwoDimensionalCodeType.VerticalR3:
			{
				int num3 = num2 + 3;
				int num4 = num;
				scanline.Slice(num4, num3 - num4).Fill(b);
				num = num3;
				a = num3;
				b = (byte)(~b);
				bitReader.SwapColor();
				break;
			}
			case CcittTwoDimensionalCodeType.VerticalL1:
			{
				int num3 = num2 - 1;
				int num4 = num;
				scanline.Slice(num4, num3 - num4).Fill(b);
				num = num3;
				a = num3;
				b = (byte)(~b);
				bitReader.SwapColor();
				break;
			}
			case CcittTwoDimensionalCodeType.VerticalL2:
			{
				int num3 = num2 - 2;
				int num4 = num;
				scanline.Slice(num4, num3 - num4).Fill(b);
				num = num3;
				a = num3;
				b = (byte)(~b);
				bitReader.SwapColor();
				break;
			}
			case CcittTwoDimensionalCodeType.VerticalL3:
			{
				int num3 = num2 - 3;
				int num4 = num;
				scanline.Slice(num4, num3 - num4).Fill(b);
				num = num3;
				a = num3;
				b = (byte)(~b);
				bitReader.SwapColor();
				break;
			}
			default:
				throw new NotSupportedException("ccitt extensions are not supported.");
			}
			if (num != length)
			{
				if (num > length)
				{
					TiffThrowHelper.ThrowImageFormatException("ccitt compression parsing error, unpacked data > width");
				}
				continue;
			}
			break;
		}
	}

	protected override void Dispose(bool disposing)
	{
	}
}
