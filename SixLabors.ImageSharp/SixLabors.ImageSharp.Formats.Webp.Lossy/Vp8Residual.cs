using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace SixLabors.ImageSharp.Formats.Webp.Lossy;

internal class Vp8Residual
{
	public int First { get; set; }

	public int Last { get; set; }

	public int CoeffType { get; set; }

	public short[] Coeffs { get; } = new short[16];

	public Vp8BandProbas[] Prob { get; set; }

	public Vp8Stats[] Stats { get; set; }

	public Vp8Costs[] Costs { get; set; }

	public void Init(int first, int coeffType, Vp8EncProba prob)
	{
		First = first;
		CoeffType = coeffType;
		Prob = prob.Coeffs[CoeffType];
		Stats = prob.Stats[CoeffType];
		Costs = prob.RemappedCosts[CoeffType];
		Coeffs.AsSpan().Clear();
	}

	public void SetCoeffs(Span<short> coeffs)
	{
		if (Sse2.IsSupported)
		{
			ref short reference = ref MemoryMarshal.GetReference(coeffs);
			Vector128<byte> vector = Unsafe.As<short, Vector128<byte>>(ref reference);
			Vector128<byte> vector2 = Unsafe.As<short, Vector128<byte>>(ref Unsafe.Add(ref reference, 8));
			Vector128<sbyte> value = Sse2.CompareEqual(Sse2.PackSignedSaturate(vector.AsInt16(), vector2.AsInt16()), Vector128<sbyte>.Zero);
			uint num = (uint)(0xFFFF ^ Sse2.MoveMask(value));
			Last = ((num != 0) ? BitOperations.Log2(num) : (-1));
		}
		else
		{
			Last = -1;
			for (int num2 = 15; num2 >= 0; num2--)
			{
				if (coeffs[num2] != 0)
				{
					Last = num2;
					break;
				}
			}
		}
		coeffs.Slice(0, 16).CopyTo(Coeffs);
	}

	public int RecordCoeffs(int ctx)
	{
		int first = First;
		Vp8StatsArray statsArr = Stats[first].Stats[ctx];
		if (Last < 0)
		{
			RecordStats(0, statsArr, 0);
			return 0;
		}
		while (first <= Last)
		{
			RecordStats(1, statsArr, 0);
			int num;
			while ((num = Coeffs[first++]) == 0)
			{
				RecordStats(0, statsArr, 1);
				statsArr = Stats[WebpConstants.Vp8EncBands[first]].Stats[0];
			}
			RecordStats(1, statsArr, 1);
			if (RecordStats(((uint)(num + 1) > 2u) ? 1 : 0, statsArr, 2) == 0)
			{
				statsArr = Stats[WebpConstants.Vp8EncBands[first]].Stats[1];
				continue;
			}
			num = Math.Abs(num);
			if (num > 67)
			{
				num = 67;
			}
			int num2 = WebpLookupTables.Vp8LevelCodes[num - 1][1];
			int num3 = WebpLookupTables.Vp8LevelCodes[num - 1][0];
			int num4 = 0;
			while ((num3 >>= 1) != 0)
			{
				int num5 = 2 << num4;
				if ((num3 & 1) != 0)
				{
					RecordStats(((num2 & num5) != 0) ? 1 : 0, statsArr, 3 + num4);
				}
				num4++;
			}
			statsArr = Stats[WebpConstants.Vp8EncBands[first]].Stats[2];
		}
		if (first < 16)
		{
			RecordStats(0, statsArr, 0);
		}
		return 1;
	}

	public int GetResidualCost(int ctx0)
	{
		int i = First;
		int num = Prob[i].Probabilities[ctx0].Probabilities[0];
		Vp8Costs[] costs = Costs;
		Vp8CostArray vp8CostArray = costs[i].Costs[ctx0];
		int num2 = ((ctx0 == 0) ? LossyUtils.Vp8BitCost(1, (byte)num) : 0);
		if (Last < 0)
		{
			return LossyUtils.Vp8BitCost(0, (byte)num);
		}
		if (Sse2.IsSupported)
		{
			Span<byte> span = stackalloc byte[32];
			Span<byte> span2 = span.Slice(0, 16);
			Span<byte> span3 = span.Slice(16);
			Span<ushort> span4 = stackalloc ushort[16];
			ref short reference = ref MemoryMarshal.GetReference<short>(Coeffs);
			Vector128<short> vector = Unsafe.As<short, Vector128<byte>>(ref reference).AsInt16();
			Vector128<short> vector2 = Unsafe.As<short, Vector128<byte>>(ref Unsafe.Add(ref reference, 8)).AsInt16();
			Vector128<short> right = Sse2.Subtract(Vector128<short>.Zero, vector);
			Vector128<short> right2 = Sse2.Subtract(Vector128<short>.Zero, vector2);
			Vector128<short> vector3 = Sse2.Max(vector, right);
			Vector128<short> vector4 = Sse2.Max(vector2, right2);
			Vector128<sbyte> vector5 = Sse2.PackSignedSaturate(vector3, vector4);
			Vector128<byte> vector6 = Sse2.Min(vector5.AsByte(), Vector128.Create((byte)2));
			Vector128<byte> vector7 = Sse2.Min(vector5.AsByte(), Vector128.Create((byte)67));
			ref byte reference2 = ref MemoryMarshal.GetReference(span2);
			ref byte reference3 = ref MemoryMarshal.GetReference(span3);
			ref ushort reference4 = ref MemoryMarshal.GetReference(span4);
			Unsafe.As<byte, Vector128<byte>>(ref reference2) = vector6;
			Unsafe.As<byte, Vector128<byte>>(ref reference3) = vector7;
			Unsafe.As<ushort, Vector128<ushort>>(ref reference4) = vector3.AsUInt16();
			Unsafe.As<ushort, Vector128<ushort>>(ref Unsafe.Add(ref reference4, 8)) = vector4.AsUInt16();
			int num4;
			int num5;
			for (; i < Last; i++)
			{
				int num3 = span2[i];
				num4 = span3[i];
				num5 = span4[i];
				num2 += WebpLookupTables.Vp8LevelFixedCosts[num5] + vp8CostArray.Costs[num4];
				vp8CostArray = costs[i + 1].Costs[num3];
			}
			num4 = span3[i];
			num5 = span4[i];
			num2 += WebpLookupTables.Vp8LevelFixedCosts[num5] + vp8CostArray.Costs[num4];
			if (i < 15)
			{
				int num6 = WebpConstants.Vp8EncBands[i + 1];
				int num7 = span2[i];
				int num8 = Prob[num6].Probabilities[num7].Probabilities[0];
				num2 += LossyUtils.Vp8BitCost(0, (byte)num8);
			}
			return num2;
		}
		int num9;
		for (; i < Last; i++)
		{
			num9 = Math.Abs(Coeffs[i]);
			int num10 = ((num9 >= 2) ? 2 : num9);
			num2 += LevelCost(vp8CostArray.Costs, num9);
			vp8CostArray = costs[i + 1].Costs[num10];
		}
		num9 = Math.Abs(Coeffs[i]);
		num2 += LevelCost(vp8CostArray.Costs, num9);
		if (i < 15)
		{
			int num11 = WebpConstants.Vp8EncBands[i + 1];
			int num12 = ((num9 == 1) ? 1 : 2);
			int num13 = Prob[num11].Probabilities[num12].Probabilities[0];
			num2 += LossyUtils.Vp8BitCost(0, (byte)num13);
		}
		return num2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int LevelCost(Span<ushort> table, int level)
	{
		return WebpLookupTables.Vp8LevelFixedCosts[level] + table[(level > 67) ? 67 : level];
	}

	private static int RecordStats(int bit, Vp8StatsArray statsArr, int idx)
	{
		if (statsArr.Stats[idx] >= 4294836224u)
		{
			statsArr.Stats[idx] = (statsArr.Stats[idx] + 1 >> 1) & 0x7FFF7FFF;
		}
		statsArr.Stats[idx] += (uint)(65536 + bit);
		return bit;
	}
}
