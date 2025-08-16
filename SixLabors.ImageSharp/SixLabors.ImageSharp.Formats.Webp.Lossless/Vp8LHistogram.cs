using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace SixLabors.ImageSharp.Formats.Webp.Lossless;

internal abstract class Vp8LHistogram
{
	private const uint NonTrivialSym = uint.MaxValue;

	private unsafe readonly uint* red;

	private unsafe readonly uint* blue;

	private unsafe readonly uint* alpha;

	private unsafe readonly uint* distance;

	private unsafe readonly uint* literal;

	private unsafe readonly uint* isUsed;

	private const int RedSize = 256;

	private const int BlueSize = 256;

	private const int AlphaSize = 256;

	private const int DistanceSize = 40;

	public const int LiteralSize = 1305;

	private const int UsedSize = 5;

	public const int BufferSize = 2118;

	public int PaletteCodeBits { get; set; }

	public double BitCost { get; set; }

	public double LiteralCost { get; set; }

	public double RedCost { get; set; }

	public double BlueCost { get; set; }

	public unsafe Span<uint> Red => new Span<uint>(red, 256);

	public unsafe Span<uint> Blue => new Span<uint>(blue, 256);

	public unsafe Span<uint> Alpha => new Span<uint>(alpha, 256);

	public unsafe Span<uint> Distance => new Span<uint>(distance, 40);

	public unsafe Span<uint> Literal => new Span<uint>(literal, 1305);

	public uint TrivialSymbol { get; set; }

	private unsafe Span<uint> IsUsedSpan => new Span<uint>(isUsed, 5);

	private unsafe Span<uint> TotalSpan => new Span<uint>(red, 2118);

	protected unsafe Vp8LHistogram(uint* basePointer, Vp8LBackwardRefs refs, int paletteCodeBits)
		: this(basePointer, paletteCodeBits)
	{
		StoreRefs(refs);
	}

	protected unsafe Vp8LHistogram(uint* basePointer, int paletteCodeBits)
	{
		PaletteCodeBits = paletteCodeBits;
		red = basePointer;
		blue = red + 256;
		alpha = blue + 256;
		distance = alpha + 256;
		literal = distance + 40;
		isUsed = literal + 1305;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IsUsed(int index)
	{
		return IsUsedSpan[index] == 1;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void IsUsed(int index, bool value)
	{
		IsUsedSpan[index] = (value ? 1u : 0u);
	}

	public void CopyTo(Vp8LHistogram other)
	{
		Red.CopyTo(other.Red);
		Blue.CopyTo(other.Blue);
		Alpha.CopyTo(other.Alpha);
		Literal.CopyTo(other.Literal);
		Distance.CopyTo(other.Distance);
		IsUsedSpan.CopyTo(other.IsUsedSpan);
		other.LiteralCost = LiteralCost;
		other.RedCost = RedCost;
		other.BlueCost = BlueCost;
		other.BitCost = BitCost;
		other.TrivialSymbol = TrivialSymbol;
		other.PaletteCodeBits = PaletteCodeBits;
	}

	public void Clear()
	{
		TotalSpan.Clear();
		PaletteCodeBits = 0;
		BitCost = 0.0;
		LiteralCost = 0.0;
		RedCost = 0.0;
		BlueCost = 0.0;
		TrivialSymbol = 0u;
	}

	public void StoreRefs(Vp8LBackwardRefs refs)
	{
		for (int i = 0; i < refs.Refs.Count; i++)
		{
			AddSinglePixOrCopy(refs.Refs[i], useDistanceModifier: false);
		}
	}

	public void AddSinglePixOrCopy(PixOrCopy v, bool useDistanceModifier, int xSize = 0)
	{
		Span<uint> span;
		if (v.IsLiteral())
		{
			span = Alpha;
			span[v.Literal(3)]++;
			span = Red;
			span[v.Literal(2)]++;
			span = Literal;
			span[v.Literal(1)]++;
			span = Blue;
			span[v.Literal(0)]++;
		}
		else if (v.IsCacheIdx())
		{
			int index = (int)(280 + v.CacheIdx());
			span = Literal;
			span[index]++;
		}
		else
		{
			int extraBits = 0;
			int num = LosslessUtils.PrefixEncodeBits(v.Length(), ref extraBits);
			span = Literal;
			span[256 + num]++;
			num = (useDistanceModifier ? LosslessUtils.PrefixEncodeBits(BackwardReferenceEncoder.DistanceToPlaneCode(xSize, (int)v.Distance()), ref extraBits) : LosslessUtils.PrefixEncodeBits((int)v.Distance(), ref extraBits));
			span = Distance;
			span[num]++;
		}
	}

	public int NumCodes()
	{
		return 280 + ((PaletteCodeBits > 0) ? (1 << PaletteCodeBits) : 0);
	}

	public double EstimateBits(Vp8LStreaks stats, Vp8LBitEntropy bitsEntropy)
	{
		uint trivialSym = 0u;
		double num = PopulationCost(Literal, NumCodes(), ref trivialSym, 0, stats, bitsEntropy) + PopulationCost(Red, 256, ref trivialSym, 1, stats, bitsEntropy) + PopulationCost(Blue, 256, ref trivialSym, 2, stats, bitsEntropy) + PopulationCost(Alpha, 256, ref trivialSym, 3, stats, bitsEntropy) + PopulationCost(Distance, 40, ref trivialSym, 4, stats, bitsEntropy);
		Span<uint> span = Literal;
		return num + ExtraCost(span.Slice(256, span.Length - 256), 24) + ExtraCost(Distance, 40);
	}

	public void UpdateHistogramCost(Vp8LStreaks stats, Vp8LBitEntropy bitsEntropy)
	{
		uint trivialSym = 0u;
		uint trivialSym2 = 0u;
		uint trivialSym3 = 0u;
		uint trivialSym4 = 0u;
		double num = PopulationCost(Alpha, 256, ref trivialSym, 3, stats, bitsEntropy);
		double num2 = PopulationCost(Distance, 40, ref trivialSym4, 4, stats, bitsEntropy) + ExtraCost(Distance, 40);
		int length = NumCodes();
		double num3 = PopulationCost(Literal, length, ref trivialSym4, 0, stats, bitsEntropy);
		Span<uint> span = Literal;
		LiteralCost = num3 + ExtraCost(span.Slice(256, span.Length - 256), 24);
		RedCost = PopulationCost(Red, 256, ref trivialSym2, 1, stats, bitsEntropy);
		BlueCost = PopulationCost(Blue, 256, ref trivialSym3, 2, stats, bitsEntropy);
		BitCost = LiteralCost + RedCost + BlueCost + num + num2;
		if ((trivialSym | trivialSym2 | trivialSym3) == uint.MaxValue)
		{
			TrivialSymbol = uint.MaxValue;
		}
		else
		{
			TrivialSymbol = (trivialSym << 24) | (trivialSym2 << 16) | trivialSym3;
		}
	}

	public double AddEval(Vp8LHistogram b, Vp8LStreaks stats, Vp8LBitEntropy bitsEntropy, double costThreshold, Vp8LHistogram output)
	{
		double num = BitCost + b.BitCost;
		costThreshold += num;
		if (GetCombinedHistogramEntropy(b, stats, bitsEntropy, costThreshold, 0.0, out var cost))
		{
			Add(b, output);
			output.BitCost = cost;
			output.PaletteCodeBits = PaletteCodeBits;
		}
		return cost - num;
	}

	public double AddThresh(Vp8LHistogram b, Vp8LStreaks stats, Vp8LBitEntropy bitsEntropy, double costThreshold)
	{
		double costInitial = 0.0 - BitCost;
		GetCombinedHistogramEntropy(b, stats, bitsEntropy, costThreshold, costInitial, out var cost);
		return cost;
	}

	public void Add(Vp8LHistogram b, Vp8LHistogram output)
	{
		int literalSize = NumCodes();
		AddLiteral(b, output, literalSize);
		AddRed(b, output, 256);
		AddBlue(b, output, 256);
		AddAlpha(b, output, 256);
		AddDistance(b, output, 40);
		for (int i = 0; i < 5; i++)
		{
			output.IsUsed(i, IsUsed(i) | b.IsUsed(i));
		}
		output.TrivialSymbol = ((TrivialSymbol == b.TrivialSymbol) ? TrivialSymbol : uint.MaxValue);
	}

	public bool GetCombinedHistogramEntropy(Vp8LHistogram b, Vp8LStreaks stats, Vp8LBitEntropy bitEntropy, double costThreshold, double costInitial, out double cost)
	{
		bool trivialAtEnd = false;
		cost = costInitial;
		cost += GetCombinedEntropy(Literal, b.Literal, NumCodes(), IsUsed(0), b.IsUsed(0), trivialAtEnd: false, stats, bitEntropy);
		double num = cost;
		Span<uint> span = Literal;
		ref Span<uint> reference = ref span;
		Span<uint> x = reference.Slice(256, reference.Length - 256);
		Span<uint> span2 = b.Literal;
		reference = ref span2;
		cost = num + ExtraCostCombined(x, reference.Slice(256, reference.Length - 256), 24);
		if (cost > costThreshold)
		{
			return false;
		}
		if (TrivialSymbol != uint.MaxValue && TrivialSymbol == b.TrivialSymbol)
		{
			uint num2 = (TrivialSymbol >> 24) & 0xFF;
			uint num3 = (TrivialSymbol >> 16) & 0xFF;
			uint num4 = TrivialSymbol & 0xFF;
			if ((num2 == 0 || num2 == 255) && (num3 == 0 || num3 == 255) && (num4 == 0 || num4 == 255))
			{
				trivialAtEnd = true;
			}
		}
		cost += GetCombinedEntropy(Red, b.Red, 256, IsUsed(1), b.IsUsed(1), trivialAtEnd, stats, bitEntropy);
		if (cost > costThreshold)
		{
			return false;
		}
		cost += GetCombinedEntropy(Blue, b.Blue, 256, IsUsed(2), b.IsUsed(2), trivialAtEnd, stats, bitEntropy);
		if (cost > costThreshold)
		{
			return false;
		}
		cost += GetCombinedEntropy(Alpha, b.Alpha, 256, IsUsed(3), b.IsUsed(3), trivialAtEnd, stats, bitEntropy);
		if (cost > costThreshold)
		{
			return false;
		}
		cost += GetCombinedEntropy(Distance, b.Distance, 40, IsUsed(4), b.IsUsed(4), trivialAtEnd: false, stats, bitEntropy);
		if (cost > costThreshold)
		{
			return false;
		}
		cost += ExtraCostCombined(Distance, b.Distance, 40);
		return cost <= costThreshold;
	}

	private void AddLiteral(Vp8LHistogram b, Vp8LHistogram output, int literalSize)
	{
		Span<uint> span;
		if (IsUsed(0))
		{
			if (b.IsUsed(0))
			{
				AddVector(Literal, b.Literal, output.Literal, literalSize);
				return;
			}
			span = Literal;
			span.Slice(0, literalSize).CopyTo(output.Literal);
		}
		else if (b.IsUsed(0))
		{
			span = b.Literal;
			span.Slice(0, literalSize).CopyTo(output.Literal);
		}
		else
		{
			span = output.Literal;
			span.Slice(0, literalSize).Clear();
		}
	}

	private void AddRed(Vp8LHistogram b, Vp8LHistogram output, int size)
	{
		Span<uint> span;
		if (IsUsed(1))
		{
			if (b.IsUsed(1))
			{
				AddVector(Red, b.Red, output.Red, size);
				return;
			}
			span = Red;
			span.Slice(0, size).CopyTo(output.Red);
		}
		else if (b.IsUsed(1))
		{
			span = b.Red;
			span.Slice(0, size).CopyTo(output.Red);
		}
		else
		{
			span = output.Red;
			span.Slice(0, size).Clear();
		}
	}

	private void AddBlue(Vp8LHistogram b, Vp8LHistogram output, int size)
	{
		Span<uint> span;
		if (IsUsed(2))
		{
			if (b.IsUsed(2))
			{
				AddVector(Blue, b.Blue, output.Blue, size);
				return;
			}
			span = Blue;
			span.Slice(0, size).CopyTo(output.Blue);
		}
		else if (b.IsUsed(2))
		{
			span = b.Blue;
			span.Slice(0, size).CopyTo(output.Blue);
		}
		else
		{
			span = output.Blue;
			span.Slice(0, size).Clear();
		}
	}

	private void AddAlpha(Vp8LHistogram b, Vp8LHistogram output, int size)
	{
		Span<uint> span;
		if (IsUsed(3))
		{
			if (b.IsUsed(3))
			{
				AddVector(Alpha, b.Alpha, output.Alpha, size);
				return;
			}
			span = Alpha;
			span.Slice(0, size).CopyTo(output.Alpha);
		}
		else if (b.IsUsed(3))
		{
			span = b.Alpha;
			span.Slice(0, size).CopyTo(output.Alpha);
		}
		else
		{
			span = output.Alpha;
			span.Slice(0, size).Clear();
		}
	}

	private void AddDistance(Vp8LHistogram b, Vp8LHistogram output, int size)
	{
		Span<uint> span;
		if (IsUsed(4))
		{
			if (b.IsUsed(4))
			{
				AddVector(Distance, b.Distance, output.Distance, size);
				return;
			}
			span = Distance;
			span.Slice(0, size).CopyTo(output.Distance);
		}
		else if (b.IsUsed(4))
		{
			span = b.Distance;
			span.Slice(0, size).CopyTo(output.Distance);
		}
		else
		{
			span = output.Distance;
			span.Slice(0, size).Clear();
		}
	}

	private static double GetCombinedEntropy(Span<uint> x, Span<uint> y, int length, bool isXUsed, bool isYUsed, bool trivialAtEnd, Vp8LStreaks stats, Vp8LBitEntropy bitEntropy)
	{
		stats.Clear();
		bitEntropy.Init();
		if (trivialAtEnd)
		{
			stats.Streaks[1][0] = 1;
			stats.Counts[0] = 1;
			stats.Streaks[0][1] = length - 1;
			return stats.FinalHuffmanCost();
		}
		if (isXUsed)
		{
			if (isYUsed)
			{
				bitEntropy.GetCombinedEntropyUnrefined(x, y, length, stats);
			}
			else
			{
				bitEntropy.GetEntropyUnrefined(x, length, stats);
			}
		}
		else if (isYUsed)
		{
			bitEntropy.GetEntropyUnrefined(y, length, stats);
		}
		else
		{
			stats.Counts[0] = 1;
			stats.Streaks[0][(length > 3) ? 1u : 0u] = length;
			bitEntropy.Init();
		}
		return bitEntropy.BitsEntropyRefine() + stats.FinalHuffmanCost();
	}

	private static double ExtraCostCombined(Span<uint> x, Span<uint> y, int length)
	{
		double num = 0.0;
		for (int i = 2; i < length - 2; i++)
		{
			int num2 = (int)(x[i + 2] + y[i + 2]);
			num += (double)((i >> 1) * num2);
		}
		return num;
	}

	private double PopulationCost(Span<uint> population, int length, ref uint trivialSym, int isUsedIndex, Vp8LStreaks stats, Vp8LBitEntropy bitEntropy)
	{
		bitEntropy.Init();
		stats.Clear();
		bitEntropy.BitsEntropyUnrefined(population, length, stats);
		trivialSym = ((bitEntropy.NoneZeros == 1) ? bitEntropy.NoneZeroCode : uint.MaxValue);
		IsUsed(isUsedIndex, stats.Streaks[1][0] != 0 || stats.Streaks[1][1] != 0);
		return bitEntropy.BitsEntropyRefine() + stats.FinalHuffmanCost();
	}

	private static double ExtraCost(Span<uint> population, int length)
	{
		double num = 0.0;
		for (int i = 2; i < length - 2; i++)
		{
			num += (double)((i >> 1) * population[i + 2]);
		}
		return num;
	}

	private static void AddVector(Span<uint> a, Span<uint> b, Span<uint> output, int count)
	{
		if (Avx2.IsSupported && count >= 32)
		{
			ref uint reference = ref MemoryMarshal.GetReference(a);
			ref uint reference2 = ref MemoryMarshal.GetReference(b);
			ref uint reference3 = ref MemoryMarshal.GetReference(output);
			nuint num = 0u;
			do
			{
				Vector256<uint> left = Unsafe.As<uint, Vector256<uint>>(ref Unsafe.Add(ref reference, num + 0));
				Vector256<uint> left2 = Unsafe.As<uint, Vector256<uint>>(ref Unsafe.Add(ref reference, num + 8));
				Vector256<uint> left3 = Unsafe.As<uint, Vector256<uint>>(ref Unsafe.Add(ref reference, num + 16));
				Vector256<uint> left4 = Unsafe.As<uint, Vector256<uint>>(ref Unsafe.Add(ref reference, num + 24));
				Vector256<uint> right = Unsafe.As<uint, Vector256<uint>>(ref Unsafe.Add(ref reference2, num + 0));
				Vector256<uint> right2 = Unsafe.As<uint, Vector256<uint>>(ref Unsafe.Add(ref reference2, num + 8));
				Vector256<uint> right3 = Unsafe.As<uint, Vector256<uint>>(ref Unsafe.Add(ref reference2, num + 16));
				Vector256<uint> right4 = Unsafe.As<uint, Vector256<uint>>(ref Unsafe.Add(ref reference2, num + 24));
				Unsafe.As<uint, Vector256<uint>>(ref Unsafe.Add(ref reference3, num + 0)) = Avx2.Add(left, right);
				Unsafe.As<uint, Vector256<uint>>(ref Unsafe.Add(ref reference3, num + 8)) = Avx2.Add(left2, right2);
				Unsafe.As<uint, Vector256<uint>>(ref Unsafe.Add(ref reference3, num + 16)) = Avx2.Add(left3, right3);
				Unsafe.As<uint, Vector256<uint>>(ref Unsafe.Add(ref reference3, num + 24)) = Avx2.Add(left4, right4);
				num += 32;
			}
			while (num <= (uint)(count - 32));
			for (int i = (int)num; i < count; i++)
			{
				output[i] = a[i] + b[i];
			}
		}
		else
		{
			for (int j = 0; j < count; j++)
			{
				output[j] = a[j] + b[j];
			}
		}
	}
}
