using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.Memory;

namespace SixLabors.ImageSharp.Formats.Webp.Lossless;

internal sealed class Vp8LHashChain : IDisposable
{
	private const uint HashMultiplierHi = 3332679571u;

	private const uint HashMultiplierLo = 1540483478u;

	private const int HashBits = 18;

	private const int HashSize = 262144;

	private const int WindowSizeBits = 20;

	private const int WindowSize = 1048456;

	private readonly MemoryAllocator memoryAllocator;

	public IMemoryOwner<uint> OffsetLength { get; }

	public int Size { get; }

	public Vp8LHashChain(MemoryAllocator memoryAllocator, int size)
	{
		this.memoryAllocator = memoryAllocator;
		OffsetLength = this.memoryAllocator.Allocate<uint>(size, AllocationOptions.Clean);
		Size = size;
	}

	public void Fill(ReadOnlySpan<uint> bgra, uint quality, int xSize, int ySize, bool lowEffort)
	{
		int num = xSize * ySize;
		int maxItersForQuality = GetMaxItersForQuality(quality);
		int windowSizeForHashChain = GetWindowSizeForHashChain(quality, xSize);
		if (num <= 2)
		{
			OffsetLength.GetSpan()[0] = 0u;
			return;
		}
		using IMemoryOwner<int> buffer = memoryAllocator.Allocate<int>(262144);
		using IMemoryOwner<int> buffer2 = memoryAllocator.Allocate<int>(num, AllocationOptions.Clean);
		Span<int> span = buffer.GetSpan();
		Span<int> span2 = buffer2.GetSpan();
		span.Fill(-1);
		bool flag = bgra.Length > 1 && bgra[0] == bgra[1];
		Span<uint> span3 = stackalloc uint[2];
		int num2 = 0;
		ref ReadOnlySpan<uint> reference;
		int num4;
		while (num2 < num - 2)
		{
			bool flag2 = bgra[num2 + 1] == bgra[num2 + 2];
			if (flag && flag2)
			{
				span3.Clear();
				uint num3 = 1u;
				span3[0] = bgra[num2];
				for (; num2 + (int)num3 + 2 < num && bgra[(int)(num2 + num3 + 2)] == bgra[num2]; num3++)
				{
				}
				if (num3 > 4095)
				{
					num2 += (int)(num3 - 4095);
					num3 = 4095u;
				}
				while (num3 != 0)
				{
					span3[1] = num3--;
					uint pixPairHash = GetPixPairHash64(span3);
					span2[num2] = span[(int)pixPairHash];
					span[(int)pixPairHash] = num2++;
				}
				flag = false;
			}
			else
			{
				reference = ref bgra;
				num4 = num2;
				uint pixPairHash = GetPixPairHash64(reference.Slice(num4, reference.Length - num4));
				span2[num2] = span[(int)pixPairHash];
				span[(int)pixPairHash] = num2++;
				flag = flag2;
			}
		}
		ref int reference2 = ref span2[num2];
		reference = ref bgra;
		num4 = num2;
		reference2 = span[(int)GetPixPairHash64(reference.Slice(num4, reference.Length - num4))];
		Span<uint> span4 = OffsetLength.GetSpan();
		span4[0] = (span4[num - 1] = 0u);
		int num5 = num - 2;
		while (num5 > 0)
		{
			int num6 = LosslessUtils.MaxFindCopyLength(num - 1 - num5);
			int num7 = num5;
			int num8 = maxItersForQuality;
			int num9 = 0;
			uint num10 = 0u;
			int num11 = ((num5 > windowSizeForHashChain) ? (num5 - windowSizeForHashChain) : 0);
			int num12 = ((num6 < 256) ? num6 : 256);
			num2 = span2[num5];
			if (!lowEffort)
			{
				int num13;
				if (num5 >= (uint)xSize)
				{
					reference = ref bgra;
					num4 = num7 - xSize;
					ReadOnlySpan<uint> array = reference.Slice(num4, reference.Length - num4);
					reference = ref bgra;
					num4 = num7;
					num13 = LosslessUtils.FindMatchLength(array, reference.Slice(num4, reference.Length - num4), num9, num6);
					if (num13 > num9)
					{
						num9 = num13;
						num10 = (uint)xSize;
					}
					num8--;
				}
				reference = ref bgra;
				num4 = num7 - 1;
				ReadOnlySpan<uint> array2 = reference.Slice(num4, reference.Length - num4);
				reference = ref bgra;
				num4 = num7;
				num13 = LosslessUtils.FindMatchLength(array2, reference.Slice(num4, reference.Length - num4), num9, num6);
				if (num13 > num9)
				{
					num9 = num13;
					num10 = 1u;
				}
				num8--;
				if (num9 == 4095)
				{
					num2 = num11 - 1;
				}
			}
			reference = ref bgra;
			num4 = num7;
			uint num14 = reference.Slice(num4, reference.Length - num4)[num9];
			while (num2 >= num11 && --num8 > 0)
			{
				if (bgra[num2 + num9] == num14)
				{
					reference = ref bgra;
					num4 = num2;
					ReadOnlySpan<uint> array3 = reference.Slice(num4, reference.Length - num4);
					reference = ref bgra;
					num4 = num7;
					int num13 = LosslessUtils.VectorMismatch(array3, reference.Slice(num4, reference.Length - num4), num6);
					if (num9 < num13)
					{
						num9 = num13;
						num10 = (uint)(num5 - num2);
						reference = ref bgra;
						num4 = num7;
						num14 = reference.Slice(num4, reference.Length - num4)[num9];
						if (num9 >= num12)
						{
							break;
						}
					}
				}
				num2 = span2[num2];
			}
			uint num15 = (uint)num5;
			while (true)
			{
				span4[num5] = (num10 << 12) | (uint)num9;
				num5--;
				if (num10 == 0 || num5 == 0 || num5 < num10 || bgra[(int)(num5 - num10)] != bgra[num5] || (num9 == 4095 && num10 != 1 && num5 + 4095 < num15))
				{
					break;
				}
				if (num9 < 4095)
				{
					num9++;
					num15 = (uint)num5;
				}
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int FindLength(int basePosition)
	{
		return (int)(OffsetLength.GetSpan()[basePosition] & 0xFFF);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int FindOffset(int basePosition)
	{
		return (int)(OffsetLength.GetSpan()[basePosition] >> 12);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static uint GetPixPairHash64(ReadOnlySpan<uint> bgra)
	{
		return (uint)((int)bgra[1] * -962287725 + (int)(bgra[0] * 1540483478)) >> 14;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int GetMaxItersForQuality(uint quality)
	{
		return (int)(8 + quality * quality / 128);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int GetWindowSizeForHashChain(uint quality, int xSize)
	{
		int num;
		switch (quality)
		{
		case 0u:
		case 1u:
		case 2u:
		case 3u:
		case 4u:
		case 5u:
		case 6u:
		case 7u:
		case 8u:
		case 9u:
		case 10u:
		case 11u:
		case 12u:
		case 13u:
		case 14u:
		case 15u:
		case 16u:
		case 17u:
		case 18u:
		case 19u:
		case 20u:
		case 21u:
		case 22u:
		case 23u:
		case 24u:
		case 25u:
			num = xSize << 4;
			break;
		case 26u:
		case 27u:
		case 28u:
		case 29u:
		case 30u:
		case 31u:
		case 32u:
		case 33u:
		case 34u:
		case 35u:
		case 36u:
		case 37u:
		case 38u:
		case 39u:
		case 40u:
		case 41u:
		case 42u:
		case 43u:
		case 44u:
		case 45u:
		case 46u:
		case 47u:
		case 48u:
		case 49u:
		case 50u:
			num = xSize << 6;
			break;
		case 51u:
		case 52u:
		case 53u:
		case 54u:
		case 55u:
		case 56u:
		case 57u:
		case 58u:
		case 59u:
		case 60u:
		case 61u:
		case 62u:
		case 63u:
		case 64u:
		case 65u:
		case 66u:
		case 67u:
		case 68u:
		case 69u:
		case 70u:
		case 71u:
		case 72u:
		case 73u:
		case 74u:
		case 75u:
			num = xSize << 8;
			break;
		default:
			num = 1048456;
			break;
		}
		int num2 = num;
		if (num2 <= 1048456)
		{
			return num2;
		}
		return 1048456;
	}

	public void Dispose()
	{
		OffsetLength.Dispose();
	}
}
