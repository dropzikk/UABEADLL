using System.Buffers;
using System.Buffers.Binary;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.Memory;

namespace SixLabors.ImageSharp.Formats.Webp.BitReader;

internal class Vp8BitReader : BitReaderBase
{
	private const int BitsCount = 56;

	private ulong value;

	private uint range;

	private int bits;

	private uint bufferMax;

	private uint bufferEnd;

	private bool eof;

	private long pos;

	public int Pos => (int)pos;

	public uint ImageDataSize { get; }

	public uint PartitionLength { get; }

	public uint Remaining { get; set; }

	public Vp8BitReader(Stream inputStream, uint imageDataSize, MemoryAllocator memoryAllocator, uint partitionLength, int startPos = 0)
		: base(inputStream, (int)imageDataSize, memoryAllocator)
	{
		Guard.MustBeLessThan(imageDataSize, 2147483647u, "imageDataSize");
		ImageDataSize = imageDataSize;
		PartitionLength = partitionLength;
		InitBitreader(partitionLength, startPos);
	}

	public Vp8BitReader(IMemoryOwner<byte> imageData, uint partitionLength, int startPos = 0)
		: base(imageData)
	{
		ImageDataSize = (uint)imageData.Memory.Length;
		PartitionLength = partitionLength;
		InitBitreader(partitionLength, startPos);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int GetBit(int prob)
	{
		uint num = range;
		if (bits < 0)
		{
			LoadNewBytes();
		}
		int num2 = bits;
		uint num3 = (uint)(num * prob >> 8);
		bool num4 = value >> num2 > num3;
		if (num4)
		{
			num -= num3;
			value -= (ulong)(num3 + 1) << num2;
		}
		else
		{
			num = num3 + 1;
		}
		int num5 = 7 ^ BitOperations.Log2(num);
		num <<= num5;
		bits -= num5;
		range = num - 1;
		return num4 ? 1 : 0;
	}

	public int GetSigned(int v)
	{
		if (bits < 0)
		{
			LoadNewBytes();
		}
		int num = bits;
		uint num2 = range >> 1;
		ulong num3 = value >> num;
		ulong num4 = num2 - num3 >> 31;
		bits--;
		range = (uint)(((int)range + (int)num4) | 1);
		value -= ((num2 + 1) & num4) << num;
		return (v ^ (int)num4) - (int)num4;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool ReadBool()
	{
		return ReadValue(1) == 1;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public uint ReadValue(int nBits)
	{
		uint num = 0u;
		while (nBits-- > 0)
		{
			num |= (uint)(GetBit(128) << nBits);
		}
		return num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int ReadSignedValue(int nBits)
	{
		int num = (int)ReadValue(nBits);
		if (ReadValue(1) == 0)
		{
			return num;
		}
		return -num;
	}

	private void InitBitreader(uint size, int pos = 0)
	{
		long num = pos + size;
		range = 254u;
		value = 0uL;
		bits = -8;
		eof = false;
		this.pos = pos;
		bufferEnd = (uint)num;
		bufferMax = (uint)((size > 8) ? (num - 8 + 1) : pos);
		LoadNewBytes();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void LoadNewBytes()
	{
		if (pos < bufferMax)
		{
			ulong x = BinaryPrimitives.ReadUInt64LittleEndian(base.Data.Memory.Span.Slice((int)pos, 8));
			pos += 7L;
			ulong num = ByteSwap64(x);
			num >>= 8;
			value = num | (value << 56);
			bits += 56;
		}
		else
		{
			LoadFinalBytes();
		}
	}

	private void LoadFinalBytes()
	{
		if (pos < bufferEnd)
		{
			bits += 8;
			value = base.Data.Memory.Span[(int)pos++] | (value << 8);
		}
		else if (!eof)
		{
			value <<= 8;
			bits += 8;
			eof = true;
		}
		else
		{
			bits = 0;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ulong ByteSwap64(ulong x)
	{
		x = ((x & 0xFFFFFFFF00000000uL) >> 32) | ((x & 0xFFFFFFFFu) << 32);
		x = ((x & 0xFFFF0000FFFF0000uL) >> 16) | ((x & 0xFFFF0000FFFFL) << 16);
		x = ((x & 0xFF00FF00FF00FF00uL) >> 8) | ((x & 0xFF00FF00FF00FFL) << 8);
		return x;
	}
}
