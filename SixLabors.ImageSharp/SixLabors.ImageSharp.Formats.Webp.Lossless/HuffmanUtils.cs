using System;
using System.Runtime.CompilerServices;

namespace SixLabors.ImageSharp.Formats.Webp.Lossless;

internal static class HuffmanUtils
{
	public const int HuffmanTableBits = 8;

	public const int HuffmanPackedBits = 6;

	public const int HuffmanTableMask = 255;

	public const uint HuffmanPackedTableSize = 64u;

	private static readonly byte[] ReversedBits = new byte[16]
	{
		0, 8, 4, 12, 2, 10, 6, 14, 1, 9,
		5, 13, 3, 11, 7, 15
	};

	public static void CreateHuffmanTree(Span<uint> histogram, int treeDepthLimit, bool[] bufRle, Span<HuffmanTree> huffTree, HuffmanTreeCode huffCode)
	{
		int numSymbols = huffCode.NumSymbols;
		bufRle.AsSpan().Clear();
		OptimizeHuffmanForRle(numSymbols, bufRle, histogram);
		GenerateOptimalTree(huffTree, histogram, numSymbols, treeDepthLimit, huffCode.CodeLengths);
		ConvertBitDepthsToSymbols(huffCode);
	}

	public static void OptimizeHuffmanForRle(int length, bool[] goodForRle, Span<uint> counts)
	{
		while (length >= 0)
		{
			if (length == 0)
			{
				return;
			}
			if (counts[length - 1] != 0)
			{
				break;
			}
			length--;
		}
		uint num = counts[0];
		int num2 = 0;
		for (int i = 0; i < length + 1; i++)
		{
			if (i == length || counts[i] != num)
			{
				if ((num == 0 && num2 >= 5) || (num != 0 && num2 >= 7))
				{
					for (int j = 0; j < num2; j++)
					{
						goodForRle[i - j - 1] = true;
					}
				}
				num2 = 1;
				if (i != length)
				{
					num = counts[i];
				}
			}
			else
			{
				num2++;
			}
		}
		num2 = 0;
		uint b = counts[0];
		uint num3 = 0u;
		for (int k = 0; k < length + 1; k++)
		{
			if (k == length || goodForRle[k] || (k != 0 && goodForRle[k - 1]) || !ValuesShouldBeCollapsedToStrideAverage((int)counts[k], (int)b))
			{
				if (num2 >= 4 || (num2 >= 3 && num3 == 0))
				{
					uint num4 = (num3 + (uint)num2 / 2u) / (uint)num2;
					if (num4 < 1)
					{
						num4 = 1u;
					}
					if (num3 == 0)
					{
						num4 = 0u;
					}
					for (uint num5 = 0u; num5 < num2; num5++)
					{
						counts[(int)(k - num5 - 1)] = num4;
					}
				}
				num2 = 0;
				num3 = 0u;
				b = ((k < length - 3) ? ((counts[k] + counts[k + 1] + counts[k + 2] + counts[k + 3] + 2) / 4) : ((k < length) ? counts[k] : 0u));
			}
			num2++;
			if (k != length)
			{
				num3 += counts[k];
				if (num2 >= 4)
				{
					b = (num3 + (uint)num2 / 2u) / (uint)num2;
				}
			}
		}
	}

	public static void GenerateOptimalTree(Span<HuffmanTree> tree, Span<uint> histogram, int histogramSize, int treeDepthLimit, byte[] bitDepths)
	{
		int num = 0;
		for (int i = 0; i < histogramSize; i++)
		{
			if (histogram[i] != 0)
			{
				num++;
			}
		}
		if (num == 0)
		{
			return;
		}
		int num2 = num;
		Span<HuffmanTree> pool = tree.Slice(num2, tree.Length - num2);
		uint num3 = 1u;
		while (true)
		{
			int num4 = num;
			int num5 = 0;
			for (int j = 0; j < histogramSize; j++)
			{
				if (histogram[j] != 0)
				{
					uint totalCount = ((histogram[j] < num3) ? num3 : histogram[j]);
					tree[num5].TotalCount = (int)totalCount;
					tree[num5].Value = j;
					tree[num5].PoolIndexLeft = -1;
					tree[num5].PoolIndexRight = -1;
					num5++;
				}
			}
			tree.Slice(0, num4).Sort(HuffmanTree.Compare);
			if (num4 > 1)
			{
				int num6 = 0;
				while (num4 > 1)
				{
					pool[num6++] = tree[num4 - 1];
					pool[num6++] = tree[num4 - 2];
					int num7 = pool[num6 - 1].TotalCount + pool[num6 - 2].TotalCount;
					num4 -= 2;
					int k;
					for (k = 0; k < num4 && tree[k].TotalCount > num7; k++)
					{
					}
					int num8 = k + 1;
					int num9 = num4 - k;
					for (int num10 = num8 + num9 - 1; num10 >= num8; num10--)
					{
						tree[num10] = tree[num10 - 1];
					}
					tree[k].TotalCount = num7;
					tree[k].Value = -1;
					tree[k].PoolIndexLeft = num6 - 1;
					tree[k].PoolIndexRight = num6 - 2;
					num4++;
				}
				SetBitDepths(tree, pool, bitDepths, 0);
			}
			else if (num4 == 1)
			{
				bitDepths[tree[0].Value] = 1;
			}
			int num11 = bitDepths[0];
			for (int l = 1; l < histogramSize; l++)
			{
				if (num11 < bitDepths[l])
				{
					num11 = bitDepths[l];
				}
			}
			if (num11 > treeDepthLimit)
			{
				num3 *= 2;
				continue;
			}
			break;
		}
	}

	public static int CreateCompressedHuffmanTree(HuffmanTreeCode tree, HuffmanTreeToken[] tokensArray)
	{
		int numSymbols = tree.NumSymbols;
		int prevValue = 8;
		int i = 0;
		int num = 0;
		int num3;
		for (; i < numSymbols; i += num3)
		{
			int num2 = tree.CodeLengths[i];
			int j;
			for (j = i + 1; j < numSymbols && tree.CodeLengths[j] == num2; j++)
			{
			}
			num3 = j - i;
			if (num2 == 0)
			{
				num += CodeRepeatedZeros(num3, tokensArray.AsSpan(num));
				continue;
			}
			num += CodeRepeatedValues(num3, tokensArray.AsSpan(num), num2, prevValue);
			prevValue = num2;
		}
		return num;
	}

	public static int BuildHuffmanTable(Span<HuffmanCode> table, int rootBits, int[] codeLengths, int codeLengthsSize)
	{
		Span<int> span = ((codeLengthsSize > 64) ? ((Span<int>)new int[codeLengthsSize]) : stackalloc int[codeLengthsSize]);
		Span<int> span2 = span;
		int num = 1 << rootBits;
		Span<int> span3 = stackalloc int[16];
		Span<int> span4 = stackalloc int[16];
		int i;
		for (i = 0; i < codeLengthsSize; i++)
		{
			int num2 = codeLengths[i];
			if (num2 > 15)
			{
				return 0;
			}
			span3[num2]++;
		}
		if (span3[0] == codeLengthsSize)
		{
			return 0;
		}
		span4[1] = 0;
		int j;
		for (j = 1; j < 15; j++)
		{
			int num3 = span3[j];
			if (num3 > 1 << j)
			{
				return 0;
			}
			span4[j + 1] = span4[j] + num3;
		}
		for (i = 0; i < codeLengthsSize; i++)
		{
			int num4 = codeLengths[i];
			if (num4 > 0)
			{
				span2[span4[num4]++] = i;
			}
		}
		if (span4[15] == 1)
		{
			HuffmanCode huffmanCode = default(HuffmanCode);
			huffmanCode.BitsUsed = 0;
			huffmanCode.Value = (uint)span2[0];
			HuffmanCode code = huffmanCode;
			ReplicateValue(table, 1, num, code);
			return num;
		}
		int num5 = -1;
		int num6 = num - 1;
		int num7 = 0;
		int num8 = 1;
		int num9 = 1;
		int num10 = rootBits;
		int num11 = 1 << num10;
		i = 0;
		j = 1;
		int num12 = 2;
		while (j <= rootBits)
		{
			int num13 = span3[j];
			num9 <<= 1;
			num8 += num9;
			num9 -= span3[j];
			if (num9 < 0)
			{
				return 0;
			}
			while (num13 > 0)
			{
				HuffmanCode huffmanCode = default(HuffmanCode);
				huffmanCode.BitsUsed = j;
				huffmanCode.Value = (uint)span2[i++];
				HuffmanCode code2 = huffmanCode;
				ref Span<HuffmanCode> reference = ref table;
				int num14 = num7;
				ReplicateValue(reference.Slice(num14, reference.Length - num14), num12, num11, code2);
				num7 = GetNextKey(num7, j);
				num13--;
			}
			span3[j] = num13;
			j++;
			num12 <<= 1;
		}
		Span<HuffmanCode> span5 = table;
		int num15 = 0;
		j = rootBits + 1;
		num12 = 2;
		while (j <= 15)
		{
			num9 <<= 1;
			num8 += num9;
			num9 -= span3[j];
			if (num9 < 0)
			{
				return 0;
			}
			while (span3[j] > 0)
			{
				ref Span<HuffmanCode> reference;
				int num14;
				if ((num7 & num6) != num5)
				{
					reference = ref span5;
					num14 = num11;
					span5 = reference.Slice(num14, reference.Length - num14);
					num15 += num11;
					num10 = NextTableBitSize(span3, j, rootBits);
					num11 = 1 << num10;
					num += num11;
					num5 = num7 & num6;
					table[num5] = new HuffmanCode
					{
						BitsUsed = num10 + rootBits,
						Value = (uint)(num15 - num5)
					};
				}
				HuffmanCode huffmanCode = default(HuffmanCode);
				huffmanCode.BitsUsed = j - rootBits;
				huffmanCode.Value = (uint)span2[i++];
				HuffmanCode code3 = huffmanCode;
				reference = ref span5;
				num14 = num7 >> rootBits;
				ReplicateValue(reference.Slice(num14, reference.Length - num14), num12, num11, code3);
				num7 = GetNextKey(num7, j);
				span3[j]--;
			}
			j++;
			num12 <<= 1;
		}
		return num;
	}

	private static int CodeRepeatedZeros(int repetitions, Span<HuffmanTreeToken> tokens)
	{
		int num = 0;
		while (repetitions >= 1)
		{
			if (repetitions < 3)
			{
				for (int i = 0; i < repetitions; i++)
				{
					tokens[num].Code = 0;
					tokens[num].ExtraBits = 0;
					num++;
				}
				break;
			}
			if (repetitions < 11)
			{
				tokens[num].Code = 17;
				tokens[num].ExtraBits = (byte)(repetitions - 3);
				num++;
				break;
			}
			if (repetitions < 139)
			{
				tokens[num].Code = 18;
				tokens[num].ExtraBits = (byte)(repetitions - 11);
				num++;
				break;
			}
			tokens[num].Code = 18;
			tokens[num].ExtraBits = 127;
			num++;
			repetitions -= 138;
		}
		return num;
	}

	private static int CodeRepeatedValues(int repetitions, Span<HuffmanTreeToken> tokens, int value, int prevValue)
	{
		int num = 0;
		if (value != prevValue)
		{
			tokens[num].Code = (byte)value;
			tokens[num].ExtraBits = 0;
			num++;
			repetitions--;
		}
		while (repetitions >= 1)
		{
			if (repetitions < 3)
			{
				for (int i = 0; i < repetitions; i++)
				{
					tokens[num].Code = (byte)value;
					tokens[num].ExtraBits = 0;
					num++;
				}
				break;
			}
			if (repetitions < 7)
			{
				tokens[num].Code = 16;
				tokens[num].ExtraBits = (byte)(repetitions - 3);
				num++;
				break;
			}
			tokens[num].Code = 16;
			tokens[num].ExtraBits = 3;
			num++;
			repetitions -= 6;
		}
		return num;
	}

	private static void ConvertBitDepthsToSymbols(HuffmanTreeCode tree)
	{
		Span<uint> span = stackalloc uint[16];
		Span<int> span2 = stackalloc int[16];
		int numSymbols = tree.NumSymbols;
		for (int i = 0; i < numSymbols; i++)
		{
			span2[tree.CodeLengths[i]]++;
		}
		span2[0] = 0;
		span[0] = 0u;
		uint num = 0u;
		for (int j = 1; j <= 15; j++)
		{
			num = (uint)(num + span2[j - 1] << 1);
			span[j] = num;
		}
		for (int k = 0; k < numSymbols; k++)
		{
			int num2 = tree.CodeLengths[k];
			tree.Codes[k] = (short)ReverseBits(num2, span[num2]++);
		}
	}

	private static void SetBitDepths(Span<HuffmanTree> tree, Span<HuffmanTree> pool, byte[] bitDepths, int level)
	{
		if (tree[0].PoolIndexLeft >= 0)
		{
			ref Span<HuffmanTree> reference = ref pool;
			int poolIndexLeft = tree[0].PoolIndexLeft;
			SetBitDepths(reference.Slice(poolIndexLeft, reference.Length - poolIndexLeft), pool, bitDepths, level + 1);
			reference = ref pool;
			poolIndexLeft = tree[0].PoolIndexRight;
			SetBitDepths(reference.Slice(poolIndexLeft, reference.Length - poolIndexLeft), pool, bitDepths, level + 1);
		}
		else
		{
			bitDepths[tree[0].Value] = (byte)level;
		}
	}

	private static uint ReverseBits(int numBits, uint bits)
	{
		uint num = 0u;
		int num2 = 0;
		while (num2 < numBits)
		{
			num2 += 4;
			num |= (uint)(ReversedBits[bits & 0xF] << 16 - num2);
			bits >>= 4;
		}
		return num >> 16 - numBits;
	}

	private static int NextTableBitSize(ReadOnlySpan<int> count, int len, int rootBits)
	{
		int num = 1 << len - rootBits;
		while (len < 15)
		{
			num -= count[len];
			if (num <= 0)
			{
				break;
			}
			len++;
			num <<= 1;
		}
		return len - rootBits;
	}

	private static void ReplicateValue(Span<HuffmanCode> table, int step, int end, HuffmanCode code)
	{
		do
		{
			end -= step;
			table[end] = code;
		}
		while (end > 0);
	}

	private static int GetNextKey(int key, int len)
	{
		int num = 1 << len - 1;
		while ((key & num) != 0)
		{
			num >>= 1;
		}
		if (num == 0)
		{
			return key;
		}
		return (key & (num - 1)) + num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool ValuesShouldBeCollapsedToStrideAverage(int a, int b)
	{
		return Math.Abs(a - b) < 4;
	}
}
