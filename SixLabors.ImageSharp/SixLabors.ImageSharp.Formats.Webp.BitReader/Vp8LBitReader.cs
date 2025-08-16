using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.Memory;

namespace SixLabors.ImageSharp.Formats.Webp.BitReader;

internal class Vp8LBitReader : BitReaderBase
{
	private const int Vp8LMaxNumBitRead = 24;

	private const int Lbits = 64;

	private const int Wbits = 32;

	private static readonly uint[] BitMask = new uint[25]
	{
		0u, 1u, 3u, 7u, 15u, 31u, 63u, 127u, 255u, 511u,
		1023u, 2047u, 4095u, 8191u, 16383u, 32767u, 65535u, 131071u, 262143u, 524287u,
		1048575u, 2097151u, 4194303u, 8388607u, 16777215u
	};

	private ulong value;

	private readonly long len;

	private long pos;

	private int bitPos;

	public bool Eos { get; set; }

	public Vp8LBitReader(IMemoryOwner<byte> data)
		: base(data)
	{
		Memory<byte> memory = data.Memory;
		len = memory.Length;
		value = 0uL;
		bitPos = 0;
		Eos = false;
		ulong num = 0uL;
		memory = base.Data.Memory;
		Span<byte> span = memory.Span;
		for (int i = 0; i < 8; i++)
		{
			num |= (ulong)span[i] << 8 * i;
		}
		value = num;
		pos = 8L;
	}

	public Vp8LBitReader(Stream inputStream, uint imageDataSize, MemoryAllocator memoryAllocator)
		: base(inputStream, (int)imageDataSize, memoryAllocator)
	{
		long num = (len = imageDataSize);
		value = 0uL;
		bitPos = 0;
		Eos = false;
		if (num > 8)
		{
			num = 8L;
		}
		ulong num2 = 0uL;
		Span<byte> span = base.Data.Memory.Span;
		for (int i = 0; i < num; i++)
		{
			num2 |= (ulong)span[i] << 8 * i;
		}
		value = num2;
		pos = num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public uint ReadValue(int nBits)
	{
		if (!Eos && nBits <= 24)
		{
			ulong num = PrefetchBits() & BitMask[nBits];
			bitPos += nBits;
			ShiftBytes();
			return (uint)num;
		}
		return 0u;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool ReadBit()
	{
		return ReadValue(1) != 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AdvanceBitPosition(int numberOfBits)
	{
		bitPos += numberOfBits;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ulong PrefetchBits()
	{
		return value >> (bitPos & 0x3F);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void FillBitWindow()
	{
		if (bitPos >= 32)
		{
			DoFillBitWindow();
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IsEndOfStream()
	{
		if (!Eos)
		{
			if (pos == len)
			{
				return bitPos > 64;
			}
			return false;
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void DoFillBitWindow()
	{
		ShiftBytes();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void ShiftBytes()
	{
		Span<byte> span = base.Data.Memory.Span;
		while (bitPos >= 8 && pos < len)
		{
			value >>= 8;
			value |= (ulong)span[(int)pos] << 56;
			pos++;
			bitPos -= 8;
		}
	}
}
