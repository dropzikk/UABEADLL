using System;
using System.Buffers.Binary;
using System.IO;
using SixLabors.ImageSharp.Formats.Webp.Lossless;

namespace SixLabors.ImageSharp.Formats.Webp.BitWriter;

internal class Vp8LBitWriter : BitWriterBase
{
	private const int MinExtraSize = 32768;

	private const int WriterBytes = 4;

	private const int WriterBits = 32;

	private ulong bits;

	private int used;

	private int cur;

	public override int NumBytes => cur + (used + 7 >> 3);

	public Vp8LBitWriter(int expectedSize)
		: base(expectedSize)
	{
	}

	private Vp8LBitWriter(byte[] buffer, ulong bits, int used, int cur)
		: base(buffer)
	{
		this.bits = bits;
		this.used = used;
		this.cur = cur;
	}

	public void PutBits(uint bits, int nBits)
	{
		if (nBits > 0)
		{
			if (used >= 32)
			{
				PutBitsFlushBits();
			}
			this.bits |= (ulong)bits << used;
			used += nBits;
		}
	}

	public void Reset(Vp8LBitWriter bwInit)
	{
		bits = bwInit.bits;
		used = bwInit.used;
		cur = bwInit.cur;
	}

	public void WriteHuffmanCode(HuffmanTreeCode code, int codeIndex)
	{
		int nBits = code.CodeLengths[codeIndex];
		int num = code.Codes[codeIndex];
		PutBits((uint)num, nBits);
	}

	public void WriteHuffmanCodeWithExtraBits(HuffmanTreeCode code, int codeIndex, int bits, int nBits)
	{
		int num = code.CodeLengths[codeIndex];
		int num2 = code.Codes[codeIndex];
		PutBits((uint)((bits << num) | num2), num + nBits);
	}

	public Vp8LBitWriter Clone()
	{
		byte[] dst = new byte[base.Buffer.Length];
		System.Buffer.BlockCopy(base.Buffer, 0, dst, 0, cur);
		return new Vp8LBitWriter(dst, bits, used, cur);
	}

	public override void Finish()
	{
		BitWriterResize(used + 7 >> 3);
		while (used > 0)
		{
			base.Buffer[cur++] = (byte)bits;
			bits >>= 8;
			used -= 8;
		}
		used = 0;
	}

	public override void WriteEncodedImageToStream(Stream stream)
	{
		uint num = (uint)(NumBytes + 1);
		uint num2 = num & 1;
		Span<byte> span = stackalloc byte[4];
		BinaryPrimitives.WriteUInt32BigEndian(span, 1448097868u);
		stream.Write(span);
		BinaryPrimitives.WriteUInt32LittleEndian(span, num);
		stream.Write(span);
		stream.WriteByte(47);
		WriteToStream(stream);
		if (num2 == 1)
		{
			stream.WriteByte(0);
		}
	}

	private void PutBitsFlushBits()
	{
		if (cur + 4 > base.Buffer.Length)
		{
			int extraSize = base.Buffer.Length - cur + 32768;
			BitWriterResize(extraSize);
		}
		Span<byte> destination = stackalloc byte[8];
		BinaryPrimitives.WriteUInt64LittleEndian(destination, bits);
		destination.Slice(0, 4).CopyTo(base.Buffer.AsSpan(cur));
		cur += 4;
		bits >>= 32;
		used -= 32;
	}

	public override void BitWriterResize(int extraSize)
	{
		int maxBytes = base.Buffer.Length + base.Buffer.Length;
		int sizeRequired = cur + extraSize;
		ResizeBuffer(maxBytes, sizeRequired);
	}
}
