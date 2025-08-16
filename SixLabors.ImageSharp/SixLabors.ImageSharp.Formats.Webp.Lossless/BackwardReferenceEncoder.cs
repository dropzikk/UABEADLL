using System;
using System.Buffers;
using System.Collections.Generic;
using SixLabors.ImageSharp.Memory;

namespace SixLabors.ImageSharp.Formats.Webp.Lossless;

internal static class BackwardReferenceEncoder
{
	public const int MaxLengthBits = 12;

	private const float MaxEntropy = 1E+30f;

	private const int WindowOffsetsSizeMax = 32;

	public const int MaxLength = 4095;

	private const int MinLength = 4;

	public static Vp8LBackwardRefs GetBackwardReferences(int width, int height, ReadOnlySpan<uint> bgra, uint quality, int lz77TypesToTry, ref int cacheBits, MemoryAllocator memoryAllocator, Vp8LHashChain hashChain, Vp8LBackwardRefs best, Vp8LBackwardRefs worst)
	{
		int num = 0;
		double num2 = -1.0;
		int num3 = cacheBits;
		Vp8LHashChain vp8LHashChain = null;
		Vp8LStreaks stats = new Vp8LStreaks();
		Vp8LBitEntropy bitsEntropy = new Vp8LBitEntropy();
		ColorCache[] array = new ColorCache[11];
		int num4 = 1;
		while (lz77TypesToTry > 0)
		{
			int bestCacheBits = num3;
			if ((lz77TypesToTry & num4) != 0)
			{
				switch ((Vp8LLz77Type)num4)
				{
				case Vp8LLz77Type.Lz77Rle:
					BackwardReferencesRle(width, height, bgra, 0, worst);
					break;
				case Vp8LLz77Type.Lz77Standard:
					BackwardReferencesLz77(width, height, bgra, 0, hashChain, worst);
					break;
				case Vp8LLz77Type.Lz77Box:
					vp8LHashChain = new Vp8LHashChain(memoryAllocator, width * height);
					BackwardReferencesLz77Box(width, height, bgra, 0, hashChain, vp8LHashChain, worst);
					break;
				}
				bestCacheBits = CalculateBestCacheSize(memoryAllocator, array, bgra, quality, worst, bestCacheBits);
				if (bestCacheBits > 0)
				{
					BackwardRefsWithLocalCache(bgra, bestCacheBits, worst);
				}
				using OwnedVp8LHistogram ownedVp8LHistogram = OwnedVp8LHistogram.Create(memoryAllocator, worst, bestCacheBits);
				double num5 = ownedVp8LHistogram.EstimateBits(stats, bitsEntropy);
				if (num == 0 || num5 < num2)
				{
					Vp8LBackwardRefs vp8LBackwardRefs = worst;
					worst = best;
					best = vp8LBackwardRefs;
					num2 = num5;
					cacheBits = bestCacheBits;
					num = num4;
				}
			}
			lz77TypesToTry &= ~num4;
			num4 <<= 1;
		}
		if ((num == 1 || num == 4) && quality >= 25)
		{
			Vp8LHashChain hashChain2 = ((num == 1) ? hashChain : vp8LHashChain);
			BackwardReferencesTraceBackwards(width, height, memoryAllocator, bgra, cacheBits, hashChain2, best, worst);
			using OwnedVp8LHistogram ownedVp8LHistogram2 = OwnedVp8LHistogram.Create(memoryAllocator, worst, cacheBits);
			if (ownedVp8LHistogram2.EstimateBits(stats, bitsEntropy) < num2)
			{
				best = worst;
			}
		}
		BackwardReferences2DLocality(width, best);
		vp8LHashChain?.Dispose();
		return best;
	}

	private static int CalculateBestCacheSize(MemoryAllocator memoryAllocator, Span<ColorCache> colorCache, ReadOnlySpan<uint> bgra, uint quality, Vp8LBackwardRefs refs, int bestCacheBits)
	{
		int num = ((quality > 25) ? bestCacheBits : 0);
		if (num == 0)
		{
			return 0;
		}
		double num2 = 1.0000000150474662E+30;
		int num3 = 0;
		using Vp8LHistogramSet vp8LHistogramSet = new Vp8LHistogramSet(memoryAllocator, colorCache.Length, 0);
		for (int i = 0; i < colorCache.Length; i++)
		{
			vp8LHistogramSet[i].PaletteCodeBits = i;
			colorCache[i] = new ColorCache(i);
		}
		for (int j = 0; j < refs.Refs.Count; j++)
		{
			PixOrCopy pixOrCopy = refs.Refs[j];
			Span<uint> span;
			if (pixOrCopy.IsLiteral())
			{
				uint num4 = bgra[num3++];
				int index = (int)((num4 >> 24) & 0xFF);
				int index2 = (int)((num4 >> 16) & 0xFF);
				int index3 = (int)((num4 >> 8) & 0xFF);
				int index4 = (int)(num4 & 0xFF);
				int num5 = ColorCache.HashPix(num4, 32 - num);
				span = vp8LHistogramSet[0].Blue;
				span[index4]++;
				span = vp8LHistogramSet[0].Literal;
				span[index3]++;
				span = vp8LHistogramSet[0].Red;
				span[index2]++;
				span = vp8LHistogramSet[0].Alpha;
				span[index]++;
				int num6 = num;
				while (num6 >= 1)
				{
					if (colorCache[num6].Lookup(num5) == num4)
					{
						span = vp8LHistogramSet[num6].Literal;
						span[280 + num5]++;
					}
					else
					{
						colorCache[num6].Set((uint)num5, num4);
						span = vp8LHistogramSet[num6].Blue;
						span[index4]++;
						span = vp8LHistogramSet[num6].Literal;
						span[index3]++;
						span = vp8LHistogramSet[num6].Red;
						span[index2]++;
						span = vp8LHistogramSet[num6].Alpha;
						span[index]++;
					}
					num6--;
					num5 >>= 1;
				}
				continue;
			}
			int num7 = pixOrCopy.Len;
			uint num8 = bgra[num3] ^ 0xFFFFFFFFu;
			int extraBits = 0;
			int extraBitsValue = 0;
			int num9 = LosslessUtils.PrefixEncode(num7, ref extraBits, ref extraBitsValue);
			for (int k = 0; k <= num; k++)
			{
				span = vp8LHistogramSet[k].Literal;
				span[256 + num9]++;
			}
			do
			{
				if (bgra[num3] != num8)
				{
					int num10 = ColorCache.HashPix(bgra[num3], 32 - num);
					int num11 = num;
					while (num11 >= 1)
					{
						colorCache[num11].Colors[num10] = bgra[num3];
						num11--;
						num10 >>= 1;
					}
					num8 = bgra[num3];
				}
				num3++;
			}
			while (--num7 != 0);
		}
		Vp8LStreaks stats = new Vp8LStreaks();
		Vp8LBitEntropy bitsEntropy = new Vp8LBitEntropy();
		for (int l = 0; l <= num; l++)
		{
			double num12 = vp8LHistogramSet[l].EstimateBits(stats, bitsEntropy);
			if (l == 0 || num12 < num2)
			{
				num2 = num12;
				bestCacheBits = l;
			}
		}
		return bestCacheBits;
	}

	private static void BackwardReferencesTraceBackwards(int xSize, int ySize, MemoryAllocator memoryAllocator, ReadOnlySpan<uint> bgra, int cacheBits, Vp8LHashChain hashChain, Vp8LBackwardRefs refsSrc, Vp8LBackwardRefs refsDst)
	{
		int num = xSize * ySize;
		using IMemoryOwner<ushort> memoryOwner = memoryAllocator.Allocate<ushort>(num);
		Span<ushort> span = memoryOwner.GetSpan();
		BackwardReferencesHashChainDistanceOnly(xSize, ySize, memoryAllocator, bgra, cacheBits, hashChain, refsSrc, memoryOwner);
		int num2 = TraceBackwards(span, num);
		int num3 = num - num2;
		Span<ushort> chosenPath = span.Slice(num3, span.Length - num3);
		BackwardReferencesHashChainFollowChosenPath(bgra, cacheBits, chosenPath, num2, hashChain, refsDst);
	}

	private static void BackwardReferencesHashChainDistanceOnly(int xSize, int ySize, MemoryAllocator memoryAllocator, ReadOnlySpan<uint> bgra, int cacheBits, Vp8LHashChain hashChain, Vp8LBackwardRefs refs, IMemoryOwner<ushort> distArrayBuffer)
	{
		int num = xSize * ySize;
		bool flag = cacheBits > 0;
		int literalArraySize = 280 + ((cacheBits > 0) ? (1 << cacheBits) : 0);
		CostModel costModel = new CostModel(memoryAllocator, literalArraySize);
		int num2 = -1;
		int num3 = -1;
		double num4 = -1.0;
		int num5 = -1;
		int num6 = 0;
		ColorCache colorCache = null;
		if (flag)
		{
			colorCache = new ColorCache(cacheBits);
		}
		costModel.Build(xSize, cacheBits, refs);
		using CostManager costManager = new CostManager(memoryAllocator, distArrayBuffer, num, costModel);
		Span<float> span = costManager.Costs.GetSpan();
		Span<ushort> span2 = distArrayBuffer.GetSpan();
		span2[0] = 0;
		AddSingleLiteralWithCostModel(bgra, colorCache, costModel, 0, flag, 0f, span, span2);
		for (int i = 1; i < num; i++)
		{
			float num7 = span[i - 1];
			int num8 = hashChain.FindOffset(i);
			int num9 = hashChain.FindLength(i);
			AddSingleLiteralWithCostModel(bgra, colorCache, costModel, i, flag, num7, span, span2);
			if (num9 >= 2)
			{
				if (num8 != num2)
				{
					int distance = DistanceToPlaneCode(xSize, num8);
					num4 = costModel.GetDistanceCost(distance);
					num5 = 1;
					costManager.PushInterval((double)num7 + num4, i, num9);
				}
				else
				{
					if (num5 != 0)
					{
						num6 = i - 1 + num3 - 1;
						num5 = 0;
					}
					if (i + num9 - 1 > num6)
					{
						int num10 = 0;
						int j;
						for (j = i; j <= num6; j++)
						{
							int num11 = hashChain.FindOffset(j + 1);
							num10 = hashChain.FindLength(j + 1);
							if (num11 != num8)
							{
								num10 = hashChain.FindLength(j);
								break;
							}
						}
						costManager.UpdateCostAtIndex(j - 1, doCleanIntervals: false);
						costManager.UpdateCostAtIndex(j, doCleanIntervals: false);
						costManager.PushInterval((double)span[j - 1] + num4, j, num10);
						num6 = j + num10 - 1;
					}
				}
			}
			costManager.UpdateCostAtIndex(i, doCleanIntervals: true);
			num2 = num8;
			num3 = num9;
		}
	}

	private static int TraceBackwards(Span<ushort> distArray, int distArraySize)
	{
		int num = 0;
		int num2 = distArraySize;
		int num3 = distArraySize - 1;
		while (num3 >= 0)
		{
			ushort num4 = distArray[num3];
			num2--;
			num++;
			distArray[num2] = num4;
			num3 -= num4;
		}
		return num;
	}

	private static void BackwardReferencesHashChainFollowChosenPath(ReadOnlySpan<uint> bgra, int cacheBits, Span<ushort> chosenPath, int chosenPathSize, Vp8LHashChain hashChain, Vp8LBackwardRefs backwardRefs)
	{
		bool flag = cacheBits > 0;
		ColorCache colorCache = null;
		int num = 0;
		if (flag)
		{
			colorCache = new ColorCache(cacheBits);
		}
		backwardRefs.Refs.Clear();
		for (int i = 0; i < chosenPathSize; i++)
		{
			int num2 = chosenPath[i];
			if (num2 != 1)
			{
				int distance = hashChain.FindOffset(num);
				backwardRefs.Add(PixOrCopy.CreateCopy((uint)distance, (ushort)num2));
				if (flag)
				{
					for (int j = 0; j < num2; j++)
					{
						colorCache.Insert(bgra[num + j]);
					}
				}
				num += num2;
				continue;
			}
			int num3 = (flag ? colorCache.Contains(bgra[num]) : (-1));
			PixOrCopy pixOrCopy;
			if (num3 >= 0)
			{
				pixOrCopy = PixOrCopy.CreateCacheIdx(num3);
			}
			else
			{
				if (flag)
				{
					colorCache.Insert(bgra[num]);
				}
				pixOrCopy = PixOrCopy.CreateLiteral(bgra[num]);
			}
			backwardRefs.Add(pixOrCopy);
			num++;
		}
	}

	private static void AddSingleLiteralWithCostModel(ReadOnlySpan<uint> bgra, ColorCache? colorCache, CostModel costModel, int idx, bool useColorCache, float prevCost, Span<float> cost, Span<ushort> distArray)
	{
		double num = prevCost;
		uint num2 = bgra[idx];
		int num3 = (useColorCache ? colorCache.Contains(num2) : (-1));
		if (num3 >= 0)
		{
			num += costModel.GetCacheCost((uint)num3) * 0.68;
		}
		else
		{
			if (useColorCache)
			{
				colorCache.Insert(num2);
			}
			num += costModel.GetLiteralCost(num2) * 0.82;
		}
		if ((double)cost[idx] > num)
		{
			cost[idx] = (float)num;
			distArray[idx] = 1;
		}
	}

	private static void BackwardReferencesLz77(int xSize, int ySize, ReadOnlySpan<uint> bgra, int cacheBits, Vp8LHashChain hashChain, Vp8LBackwardRefs refs)
	{
		int num = -1;
		bool flag = cacheBits > 0;
		int num2 = xSize * ySize;
		ColorCache colorCache = null;
		if (flag)
		{
			colorCache = new ColorCache(cacheBits);
		}
		refs.Refs.Clear();
		int num3;
		for (int i = 0; i < num2; i += num3)
		{
			int distance = hashChain.FindOffset(i);
			num3 = hashChain.FindLength(i);
			if (num3 >= 4)
			{
				int num4 = num3;
				int num5 = 0;
				int num6 = ((i + num4 >= num2) ? (num2 - 1) : (i + num4));
				num = ((i > num) ? i : num);
				for (int j = num + 1; j <= num6; j++)
				{
					int num7 = hashChain.FindLength(j);
					int num8 = j + ((num7 < 4) ? 1 : num7);
					if (num8 > num5)
					{
						num3 = j - i;
						num5 = num8;
						if (num5 >= num2)
						{
							break;
						}
					}
				}
			}
			else
			{
				num3 = 1;
			}
			if (num3 == 1)
			{
				AddSingleLiteral(bgra[i], flag, colorCache, refs);
				continue;
			}
			refs.Add(PixOrCopy.CreateCopy((uint)distance, (ushort)num3));
			if (flag)
			{
				for (int j = i; j < i + num3; j++)
				{
					colorCache.Insert(bgra[j]);
				}
			}
		}
	}

	private static void BackwardReferencesLz77Box(int xSize, int ySize, ReadOnlySpan<uint> bgra, int cacheBits, Vp8LHashChain hashChainBest, Vp8LHashChain hashChain, Vp8LBackwardRefs refs)
	{
		int num = xSize * ySize;
		int[] array = new int[32];
		int[] array2 = new int[32];
		int num2 = 0;
		int num3 = 0;
		short[] array3 = new short[xSize * ySize];
		int num4 = -1;
		int num5 = -1;
		int num6 = num - 2;
		int num7 = num6;
		array3[num7 + 1] = 1;
		while (num6 >= 0)
		{
			if (bgra[num6] == bgra[num6 + 1])
			{
				array3[num7] = array3[num7 + 1];
				if (array3[num7 + 1] != 4095)
				{
					array3[num7]++;
				}
			}
			else
			{
				array3[num7] = 1;
			}
			num6--;
			num7--;
		}
		for (int i = 0; i <= 6; i++)
		{
			for (int j = -6; j <= 6; j++)
			{
				int num8 = i * xSize + j;
				if (num8 > 0)
				{
					int num9 = DistanceToPlaneCode(xSize, num8) - 1;
					if (num9 < 32)
					{
						array[num9] = num8;
					}
				}
			}
		}
		for (num6 = 0; num6 < 32; num6++)
		{
			if (array[num6] != 0)
			{
				array[num2++] = array[num6];
			}
		}
		for (num6 = 0; num6 < num2; num6++)
		{
			bool flag = false;
			for (int k = 0; k < num2; k++)
			{
				if (flag)
				{
					break;
				}
				flag |= array[num6] == array[k] + 1;
			}
			if (!flag)
			{
				array2[num3] = array[num6];
				num3++;
			}
		}
		Span<uint> span = hashChain.OffsetLength.GetSpan();
		span[0] = 0u;
		for (num6 = 1; num6 < num; num6++)
		{
			int num10 = hashChainBest.FindLength(num6);
			int num11 = 0;
			bool flag2 = true;
			if (num10 >= 4095)
			{
				num11 = hashChainBest.FindOffset(num6);
				for (int l = 0; l < num2; l++)
				{
					if (num11 == array[l])
					{
						flag2 = false;
						break;
					}
				}
			}
			if (flag2)
			{
				bool flag3 = num5 > 1 && num5 < 4095;
				int num12 = (flag3 ? num3 : num2);
				num10 = (flag3 ? (num5 - 1) : 0);
				num11 = (flag3 ? num4 : 0);
				for (int l = 0; l < num12; l++)
				{
					int num13 = 0;
					int num14 = num6;
					int num15 = (flag3 ? (num6 - array2[l]) : (num6 - array[l]));
					if (num15 < 0 || bgra[num15] != bgra[num6])
					{
						continue;
					}
					do
					{
						int num16 = array3[num15];
						int num17 = array3[num14];
						if (num16 != num17)
						{
							num13 += ((num16 < num17) ? num16 : num17);
							break;
						}
						num13 += num16;
						num15 += num16;
						num14 += num16;
					}
					while (num13 <= 4095 && num14 < num && bgra[num15] == bgra[num14]);
					if (num10 < num13)
					{
						num11 = (flag3 ? array2[l] : array[l]);
						if (num13 >= 4095)
						{
							num10 = 4095;
							break;
						}
						num10 = num13;
					}
				}
			}
			if (num10 <= 4)
			{
				span[num6] = 0u;
				num4 = 0;
				num5 = 0;
			}
			else
			{
				span[num6] = (uint)((num11 << 12) | num10);
				num4 = num11;
				num5 = num10;
			}
		}
		span[0] = 0u;
		BackwardReferencesLz77(xSize, ySize, bgra, cacheBits, hashChain, refs);
	}

	private static void BackwardReferencesRle(int xSize, int ySize, ReadOnlySpan<uint> bgra, int cacheBits, Vp8LBackwardRefs refs)
	{
		int num = xSize * ySize;
		bool flag = cacheBits > 0;
		ColorCache colorCache = null;
		if (flag)
		{
			colorCache = new ColorCache(cacheBits);
		}
		refs.Refs.Clear();
		AddSingleLiteral(bgra[0], flag, colorCache, refs);
		int num2 = 1;
		while (num2 < num)
		{
			int maxLimit = LosslessUtils.MaxFindCopyLength(num - num2);
			ref ReadOnlySpan<uint> reference = ref bgra;
			int num3 = num2;
			ReadOnlySpan<uint> array = reference.Slice(num3, reference.Length - num3);
			reference = ref bgra;
			num3 = num2 - 1;
			int num4 = LosslessUtils.FindMatchLength(array, reference.Slice(num3, reference.Length - num3), 0, maxLimit);
			int num5;
			if (num2 >= xSize)
			{
				reference = ref bgra;
				num3 = num2;
				ReadOnlySpan<uint> array2 = reference.Slice(num3, reference.Length - num3);
				reference = ref bgra;
				num3 = num2 - xSize;
				num5 = LosslessUtils.FindMatchLength(array2, reference.Slice(num3, reference.Length - num3), 0, maxLimit);
			}
			else
			{
				num5 = 0;
			}
			int num6 = num5;
			if (num4 >= num6 && num4 >= 4)
			{
				refs.Add(PixOrCopy.CreateCopy(1u, (ushort)num4));
				num2 += num4;
			}
			else if (num6 >= 4)
			{
				refs.Add(PixOrCopy.CreateCopy((uint)xSize, (ushort)num6));
				if (flag)
				{
					for (int i = 0; i < num6; i++)
					{
						colorCache.Insert(bgra[num2 + i]);
					}
				}
				num2 += num6;
			}
			else
			{
				AddSingleLiteral(bgra[num2], flag, colorCache, refs);
				num2++;
			}
		}
	}

	private static void BackwardRefsWithLocalCache(ReadOnlySpan<uint> bgra, int cacheBits, Vp8LBackwardRefs refs)
	{
		int num = 0;
		ColorCache colorCache = new ColorCache(cacheBits);
		for (int i = 0; i < refs.Refs.Count; i++)
		{
			PixOrCopy pixOrCopy = refs.Refs[i];
			if (pixOrCopy.IsLiteral())
			{
				uint bgraOrDistance = pixOrCopy.BgraOrDistance;
				int num2 = colorCache.Contains(bgraOrDistance);
				if (num2 >= 0)
				{
					pixOrCopy.Mode = PixOrCopyMode.CacheIdx;
					pixOrCopy.BgraOrDistance = (uint)num2;
					pixOrCopy.Len = 1;
				}
				else
				{
					colorCache.Insert(bgraOrDistance);
				}
				num++;
			}
			else
			{
				for (int j = 0; j < pixOrCopy.Len; j++)
				{
					colorCache.Insert(bgra[num++]);
				}
			}
		}
	}

	private static void BackwardReferences2DLocality(int xSize, Vp8LBackwardRefs refs)
	{
		using List<PixOrCopy>.Enumerator enumerator = refs.Refs.GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (enumerator.Current.IsCopy())
			{
				int bgraOrDistance = (int)enumerator.Current.BgraOrDistance;
				int bgraOrDistance2 = DistanceToPlaneCode(xSize, bgraOrDistance);
				enumerator.Current.BgraOrDistance = (uint)bgraOrDistance2;
			}
		}
	}

	private static void AddSingleLiteral(uint pixel, bool useColorCache, ColorCache? colorCache, Vp8LBackwardRefs refs)
	{
		PixOrCopy pixOrCopy;
		if (useColorCache)
		{
			int index = colorCache.GetIndex(pixel);
			if (colorCache.Lookup(index) == pixel)
			{
				pixOrCopy = PixOrCopy.CreateCacheIdx(index);
			}
			else
			{
				pixOrCopy = PixOrCopy.CreateLiteral(pixel);
				colorCache.Set((uint)index, pixel);
			}
		}
		else
		{
			pixOrCopy = PixOrCopy.CreateLiteral(pixel);
		}
		refs.Add(pixOrCopy);
	}

	public static int DistanceToPlaneCode(int xSize, int dist)
	{
		int num = dist / xSize;
		int num2 = dist - num * xSize;
		if (num2 <= 8 && num < 8)
		{
			return (int)(WebpLookupTables.PlaneToCodeLut[num * 16 + 8 - num2] + 1);
		}
		if (num2 > xSize - 8 && num < 7)
		{
			return (int)(WebpLookupTables.PlaneToCodeLut[(num + 1) * 16 + 8 + (xSize - num2)] + 1);
		}
		return dist + 120;
	}
}
