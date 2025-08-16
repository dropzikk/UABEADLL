using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace SixLabors.ImageSharp.Formats.Webp.Lossy;

internal sealed class Vp8Histogram
{
	private const int MaxCoeffThresh = 31;

	private int maxValue;

	private int lastNonZero;

	public Vp8Histogram()
	{
		maxValue = 0;
		lastNonZero = 1;
	}

	public int GetAlpha()
	{
		int num = maxValue;
		int num2 = lastNonZero;
		if (num <= 1)
		{
			return 0;
		}
		return 510 * num2 / num;
	}

	public void CollectHistogram(Span<byte> reference, Span<byte> pred, int startBlock, int endBlock)
	{
		Span<int> scratch = stackalloc int[16];
		Span<short> span = stackalloc short[16];
		Span<int> span2 = stackalloc int[32];
		for (int i = startBlock; i < endBlock; i++)
		{
			ref Span<byte> reference2 = ref reference;
			int num = WebpLookupTables.Vp8DspScan[i];
			Span<byte> src = reference2.Slice(num, reference2.Length - num);
			reference2 = ref pred;
			num = WebpLookupTables.Vp8DspScan[i];
			Vp8Encoding.FTransform(src, reference2.Slice(num, reference2.Length - num), span, scratch);
			if (Avx2.IsSupported)
			{
				ref short reference3 = ref MemoryMarshal.GetReference(span);
				Vector256<short> vector = Avx2.Min(Avx2.ShiftRightArithmetic(Avx2.Abs(Unsafe.As<short, Vector256<byte>>(ref reference3).AsInt16()).AsInt16(), 3), Vector256.Create((short)31));
				Unsafe.As<short, Vector256<short>>(ref reference3) = vector;
				for (int j = 0; j < 16; j++)
				{
					span2[span[j]]++;
				}
			}
			else
			{
				for (int k = 0; k < 16; k++)
				{
					span2[ClipMax(Math.Abs(span[k]) >> 3, 31)]++;
				}
			}
		}
		SetHistogramData(span2);
	}

	public void Merge(Vp8Histogram other)
	{
		if (maxValue > other.maxValue)
		{
			other.maxValue = maxValue;
		}
		if (lastNonZero > other.lastNonZero)
		{
			other.lastNonZero = lastNonZero;
		}
	}

	private void SetHistogramData(ReadOnlySpan<int> distribution)
	{
		int num = 0;
		int num2 = 1;
		for (int i = 0; i <= 31; i++)
		{
			int num3 = distribution[i];
			if (num3 > 0)
			{
				if (num3 > num)
				{
					num = num3;
				}
				num2 = i;
			}
		}
		maxValue = num;
		lastNonZero = num2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int ClipMax(int v, int max)
	{
		if (v <= max)
		{
			return v;
		}
		return max;
	}
}
