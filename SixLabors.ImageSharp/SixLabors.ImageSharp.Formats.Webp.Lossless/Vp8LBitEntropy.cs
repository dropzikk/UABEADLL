using System;

namespace SixLabors.ImageSharp.Formats.Webp.Lossless;

internal class Vp8LBitEntropy
{
	private const uint NonTrivialSym = uint.MaxValue;

	public double Entropy { get; set; }

	public uint Sum { get; set; }

	public int NoneZeros { get; set; }

	public uint MaxVal { get; set; }

	public uint NoneZeroCode { get; set; }

	public Vp8LBitEntropy()
	{
		Entropy = 0.0;
		Sum = 0u;
		NoneZeros = 0;
		MaxVal = 0u;
		NoneZeroCode = uint.MaxValue;
	}

	public void Init()
	{
		Entropy = 0.0;
		Sum = 0u;
		NoneZeros = 0;
		MaxVal = 0u;
		NoneZeroCode = uint.MaxValue;
	}

	public double BitsEntropyRefine()
	{
		double num;
		if (NoneZeros < 5)
		{
			if (NoneZeros <= 1)
			{
				return 0.0;
			}
			if (NoneZeros == 2)
			{
				return 0.99 * (double)Sum + 0.01 * Entropy;
			}
			num = ((NoneZeros != 3) ? 0.7 : 0.95);
		}
		else
		{
			num = 0.627;
		}
		double num2 = 2 * Sum - MaxVal;
		num2 = num * num2 + (1.0 - num) * Entropy;
		if (!(Entropy < num2))
		{
			return Entropy;
		}
		return num2;
	}

	public void BitsEntropyUnrefined(Span<uint> array, int n)
	{
		Init();
		for (int i = 0; i < n; i++)
		{
			if (array[i] != 0)
			{
				Sum += array[i];
				NoneZeroCode = (uint)i;
				NoneZeros++;
				Entropy -= LosslessUtils.FastSLog2(array[i]);
				if (MaxVal < array[i])
				{
					MaxVal = array[i];
				}
			}
		}
		Entropy += LosslessUtils.FastSLog2(Sum);
	}

	public void BitsEntropyUnrefined(Span<uint> x, int length, Vp8LStreaks stats)
	{
		int iPrev = 0;
		uint valPrev = x[0];
		Init();
		int i;
		for (i = 1; i < length; i++)
		{
			uint num = x[i];
			if (num != valPrev)
			{
				GetEntropyUnrefined(num, i, ref valPrev, ref iPrev, stats);
			}
		}
		GetEntropyUnrefined(0u, i, ref valPrev, ref iPrev, stats);
		Entropy += LosslessUtils.FastSLog2(Sum);
	}

	public void GetCombinedEntropyUnrefined(Span<uint> x, Span<uint> y, int length, Vp8LStreaks stats)
	{
		int iPrev = 0;
		uint valPrev = x[0] + y[0];
		Init();
		int i;
		for (i = 1; i < length; i++)
		{
			uint num = x[i] + y[i];
			if (num != valPrev)
			{
				GetEntropyUnrefined(num, i, ref valPrev, ref iPrev, stats);
			}
		}
		GetEntropyUnrefined(0u, i, ref valPrev, ref iPrev, stats);
		Entropy += LosslessUtils.FastSLog2(Sum);
	}

	public void GetEntropyUnrefined(Span<uint> x, int length, Vp8LStreaks stats)
	{
		int iPrev = 0;
		uint valPrev = x[0];
		Init();
		int i;
		for (i = 1; i < length; i++)
		{
			uint num = x[i];
			if (num != valPrev)
			{
				GetEntropyUnrefined(num, i, ref valPrev, ref iPrev, stats);
			}
		}
		GetEntropyUnrefined(0u, i, ref valPrev, ref iPrev, stats);
		Entropy += LosslessUtils.FastSLog2(Sum);
	}

	private void GetEntropyUnrefined(uint val, int i, ref uint valPrev, ref int iPrev, Vp8LStreaks stats)
	{
		int num = i - iPrev;
		if (valPrev != 0)
		{
			Sum += (uint)(int)(valPrev * num);
			NoneZeros += num;
			NoneZeroCode = (uint)iPrev;
			Entropy -= LosslessUtils.FastSLog2(valPrev) * (float)num;
			if (MaxVal < valPrev)
			{
				MaxVal = valPrev;
			}
		}
		stats.Counts[(valPrev != 0) ? 1u : 0u] += ((num > 3) ? 1 : 0);
		stats.Streaks[(valPrev != 0) ? 1u : 0u][(num > 3) ? 1u : 0u] += num;
		valPrev = val;
		iPrev = i;
	}
}
