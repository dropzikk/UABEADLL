using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.Memory;

namespace SixLabors.ImageSharp.Formats.Webp.Lossless;

internal static class HistogramEncoder
{
	private const int NumPartitions = 4;

	private const int BinSize = 64;

	private const int MaxHistoGreedy = 100;

	private const uint NonTrivialSym = uint.MaxValue;

	private const ushort InvalidHistogramSymbol = ushort.MaxValue;

	public static void GetHistoImageSymbols(MemoryAllocator memoryAllocator, int xSize, int ySize, Vp8LBackwardRefs refs, uint quality, int histoBits, int cacheBits, Vp8LHistogramSet imageHisto, Vp8LHistogram tmpHisto, Span<ushort> histogramSymbols)
	{
		int num = ((histoBits <= 0) ? 1 : LosslessUtils.SubSampleSize(xSize, histoBits));
		int num2 = ((histoBits <= 0) ? 1 : LosslessUtils.SubSampleSize(ySize, histoBits));
		int num3 = num * num2;
		using IMemoryOwner<ushort> buffer = memoryAllocator.Allocate<ushort>(num3 * 2, AllocationOptions.Clean);
		Span<ushort> span = buffer.Slice(0, num3);
		Span<ushort> clusterMappings = buffer.Slice(num3, num3);
		using Vp8LHistogramSet vp8LHistogramSet = new Vp8LHistogramSet(memoryAllocator, num3, cacheBits);
		HistogramBuild(xSize, histoBits, refs, vp8LHistogramSet);
		int num4 = HistogramCopyAndAnalyze(vp8LHistogramSet, imageHisto, histogramSymbols);
		if (num4 > 128 && quality < 100)
		{
			int numClusters = num4;
			double combineCostFactor = GetCombineCostFactor(num3, quality);
			HistogramAnalyzeEntropyBin(imageHisto, span);
			HistogramCombineEntropyBin(imageHisto, histogramSymbols, clusterMappings, tmpHisto, span, 64, combineCostFactor);
			OptimizeHistogramSymbols(clusterMappings, numClusters, span, histogramSymbols);
		}
		float num5 = (float)quality / 100f;
		int minClusterSize = (int)(1f + num5 * num5 * num5 * 99f);
		if (HistogramCombineStochastic(imageHisto, minClusterSize))
		{
			RemoveEmptyHistograms(imageHisto);
			HistogramCombineGreedy(imageHisto);
		}
		RemoveEmptyHistograms(imageHisto);
		HistogramRemap(vp8LHistogramSet, imageHisto, histogramSymbols);
	}

	private static void RemoveEmptyHistograms(Vp8LHistogramSet histograms)
	{
		for (int num = histograms.Count - 1; num >= 0; num--)
		{
			if (histograms[num] == null)
			{
				histograms.RemoveAt(num);
			}
		}
	}

	private static void HistogramBuild(int xSize, int histoBits, Vp8LBackwardRefs backwardRefs, Vp8LHistogramSet histograms)
	{
		int num = 0;
		int num2 = 0;
		int num3 = LosslessUtils.SubSampleSize(xSize, histoBits);
		foreach (PixOrCopy @ref in backwardRefs.Refs)
		{
			int index = (num2 >> histoBits) * num3 + (num >> histoBits);
			histograms[index].AddSinglePixOrCopy(@ref, useDistanceModifier: false);
			num += @ref.Len;
			while (num >= xSize)
			{
				num -= xSize;
				num2++;
			}
		}
	}

	private static void HistogramAnalyzeEntropyBin(Vp8LHistogramSet histograms, Span<ushort> binMap)
	{
		int count = histograms.Count;
		DominantCostRange dominantCostRange = new DominantCostRange();
		for (int i = 0; i < count; i++)
		{
			if (histograms[i] != null)
			{
				dominantCostRange.UpdateDominantCostRange(histograms[i]);
			}
		}
		for (int j = 0; j < count; j++)
		{
			if (histograms[j] != null)
			{
				binMap[j] = (ushort)dominantCostRange.GetHistoBinIndex(histograms[j], 4);
			}
		}
	}

	private static int HistogramCopyAndAnalyze(Vp8LHistogramSet origHistograms, Vp8LHistogramSet histograms, Span<ushort> histogramSymbols)
	{
		Vp8LStreaks stats = new Vp8LStreaks();
		Vp8LBitEntropy bitsEntropy = new Vp8LBitEntropy();
		int num = 0;
		for (int i = 0; i < origHistograms.Count; i++)
		{
			Vp8LHistogram vp8LHistogram = origHistograms[i];
			vp8LHistogram.UpdateHistogramCost(stats, bitsEntropy);
			if (!vp8LHistogram.IsUsed(0) && !vp8LHistogram.IsUsed(1) && !vp8LHistogram.IsUsed(2) && !vp8LHistogram.IsUsed(3) && !vp8LHistogram.IsUsed(4))
			{
				origHistograms[i] = null;
				histograms[i] = null;
				histogramSymbols[i] = ushort.MaxValue;
			}
			else
			{
				vp8LHistogram.CopyTo(histograms[i]);
				histogramSymbols[i] = (ushort)num++;
			}
		}
		int num2 = 0;
		Span<ushort> span = histogramSymbols;
		for (int j = 0; j < span.Length; j++)
		{
			if (span[j] != ushort.MaxValue)
			{
				num2++;
			}
		}
		return num2;
	}

	private static void HistogramCombineEntropyBin(Vp8LHistogramSet histograms, Span<ushort> clusters, Span<ushort> clusterMappings, Vp8LHistogram curCombo, ReadOnlySpan<ushort> binMap, int numBins, double combineCostFactor)
	{
		Span<HistogramBinInfo> span = stackalloc HistogramBinInfo[64];
		for (int i = 0; i < numBins; i++)
		{
			span[i].First = -1;
			span[i].NumCombineFailures = 0;
		}
		for (int j = 0; j < histograms.Count; j++)
		{
			clusterMappings[j] = (ushort)j;
		}
		List<int> list = new List<int>();
		Vp8LStreaks stats = new Vp8LStreaks();
		Vp8LBitEntropy bitsEntropy = new Vp8LBitEntropy();
		for (int k = 0; k < histograms.Count; k++)
		{
			if (histograms[k] == null)
			{
				continue;
			}
			int index = binMap[k];
			int first = span[index].First;
			if (first == -1)
			{
				span[index].First = (short)k;
				continue;
			}
			double num = (0.0 - histograms[k].BitCost) * combineCostFactor;
			if (histograms[first].AddEval(histograms[k], stats, bitsEntropy, num, curCombo) < num)
			{
				if (curCombo.TrivialSymbol != uint.MaxValue || (histograms[k].TrivialSymbol == uint.MaxValue && histograms[first].TrivialSymbol == uint.MaxValue) || span[index].NumCombineFailures >= 32)
				{
					int index2 = first;
					Vp8LHistogram value = curCombo;
					Vp8LHistogram vp8LHistogram = histograms[first];
					histograms[index2] = value;
					curCombo = vp8LHistogram;
					histograms[k] = null;
					list.Add(k);
					clusterMappings[clusters[k]] = clusters[first];
				}
				else
				{
					span[index].NumCombineFailures++;
				}
			}
		}
		for (int num2 = list.Count - 1; num2 >= 0; num2--)
		{
			histograms.RemoveAt(list[num2]);
		}
	}

	private static void OptimizeHistogramSymbols(Span<ushort> clusterMappings, int numClusters, Span<ushort> clusterMappingsTmp, Span<ushort> symbols)
	{
		bool flag = true;
		while (flag)
		{
			flag = false;
			for (int i = 0; i < numClusters; i++)
			{
				int num;
				for (num = clusterMappings[i]; num != clusterMappings[num]; num = clusterMappings[num])
				{
					clusterMappings[num] = clusterMappings[clusterMappings[num]];
				}
				if (num != clusterMappings[i])
				{
					flag = true;
					clusterMappings[i] = (ushort)num;
				}
			}
		}
		int num2 = 0;
		clusterMappingsTmp.Clear();
		for (int j = 0; j < symbols.Length; j++)
		{
			if (symbols[j] != ushort.MaxValue)
			{
				int num3 = clusterMappings[symbols[j]];
				if (num3 > 0 && clusterMappingsTmp[num3] == 0)
				{
					num2++;
					clusterMappingsTmp[num3] = (ushort)num2;
				}
				symbols[j] = clusterMappingsTmp[num3];
			}
		}
	}

	private static bool HistogramCombineStochastic(Vp8LHistogramSet histograms, int minClusterSize)
	{
		uint seed = 1u;
		int num = 0;
		int num2 = histograms.Count((Vp8LHistogram h) => h != null);
		int num3 = num2;
		int num4 = (int)((uint)num3 / 2u);
		Vp8LStreaks stats = new Vp8LStreaks();
		Vp8LBitEntropy bitsEntropy = new Vp8LBitEntropy();
		if (num2 < minClusterSize)
		{
			return true;
		}
		List<HistogramPair> list = new List<HistogramPair>();
		Span<int> span = ((histograms.Count > 64) ? ((Span<int>)new int[histograms.Count]) : stackalloc int[histograms.Count]);
		Span<int> span2 = span;
		int num5 = 0;
		for (int i = 0; i < histograms.Count; i++)
		{
			if (histograms[i] != null)
			{
				span2[num5++] = i;
			}
		}
		for (int j = 0; j < num3; j++)
		{
			if (num2 < minClusterSize)
			{
				break;
			}
			if (++num >= num4)
			{
				break;
			}
			double threshold = ((list.Count == 0) ? 0.0 : list[0].CostDiff);
			int num6 = (int)((uint)num2 / 2u);
			uint num7 = (uint)((num2 - 1) * num2);
			int num8 = 0;
			while (num2 >= 2 && num8 < num6)
			{
				uint num9 = MyRand(ref seed) % num7;
				int num10 = (int)(num9 / (num2 - 1));
				int num11 = (int)(num9 % (num2 - 1));
				if (num11 >= num10)
				{
					num11++;
				}
				num10 = span2[num10];
				num11 = span2[num11];
				double num12 = HistoPriorityListPush(list, 9, histograms, num10, num11, threshold, stats, bitsEntropy);
				if (num12 < 0.0)
				{
					threshold = num12;
					if (list.Count == 9)
					{
						break;
					}
				}
				num8++;
			}
			if (list.Count == 0)
			{
				continue;
			}
			int idx = list[0].Idx1;
			int idx2 = list[0].Idx2;
			int num13 = span2.IndexOf(idx2);
			Span<int> span3 = span2.Slice(num13 + 1, num2 - num13 - 1);
			int num14 = num13;
			Span<int> destination = span2.Slice(num14, span2.Length - num14);
			span3.CopyTo(destination);
			HistogramAdd(histograms[idx2], histograms[idx], histograms[idx]);
			histograms[idx].BitCost = list[0].CostCombo;
			histograms[idx2] = null;
			num2--;
			int num15 = 0;
			while (num15 < list.Count)
			{
				HistogramPair histogramPair = list[num15];
				bool flag = histogramPair.Idx1 == idx || histogramPair.Idx1 == idx2;
				bool flag2 = histogramPair.Idx2 == idx || histogramPair.Idx2 == idx2;
				bool flag3 = false;
				if (flag && flag2)
				{
					list[num15] = list[list.Count - 1];
					list.RemoveAt(list.Count - 1);
					continue;
				}
				if (flag)
				{
					histogramPair.Idx1 = idx;
					flag3 = true;
				}
				else if (flag2)
				{
					histogramPair.Idx2 = idx;
					flag3 = true;
				}
				if (histogramPair.Idx1 > histogramPair.Idx2)
				{
					HistogramPair histogramPair2 = histogramPair;
					num14 = histogramPair.Idx2;
					int idx3 = histogramPair.Idx1;
					histogramPair.Idx1 = num14;
					histogramPair2.Idx2 = idx3;
				}
				if (flag3)
				{
					HistoListUpdatePair(histograms[histogramPair.Idx1], histograms[histogramPair.Idx2], stats, bitsEntropy, 0.0, histogramPair);
					if (histogramPair.CostDiff >= 0.0)
					{
						list[num15] = list[list.Count - 1];
						list.RemoveAt(list.Count - 1);
						continue;
					}
				}
				HistoListUpdateHead(list, histogramPair);
				num15++;
			}
			num = 0;
		}
		return num2 <= minClusterSize;
	}

	private static void HistogramCombineGreedy(Vp8LHistogramSet histograms)
	{
		int num = histograms.Count((Vp8LHistogram h) => h != null);
		List<HistogramPair> list = new List<HistogramPair>();
		int maxSize = num * num;
		Vp8LStreaks stats = new Vp8LStreaks();
		Vp8LBitEntropy bitsEntropy = new Vp8LBitEntropy();
		for (int i = 0; i < num; i++)
		{
			if (histograms[i] == null)
			{
				continue;
			}
			for (int j = i + 1; j < num; j++)
			{
				if (histograms[j] != null)
				{
					HistoPriorityListPush(list, maxSize, histograms, i, j, 0.0, stats, bitsEntropy);
				}
			}
		}
		while (list.Count > 0)
		{
			int idx = list[0].Idx1;
			int idx2 = list[0].Idx2;
			HistogramAdd(histograms[idx2], histograms[idx], histograms[idx]);
			histograms[idx].BitCost = list[0].CostCombo;
			histograms[idx2] = null;
			int num2 = 0;
			while (num2 < list.Count)
			{
				HistogramPair histogramPair = list[num2];
				if (histogramPair.Idx1 == idx || histogramPair.Idx2 == idx || histogramPair.Idx1 == idx2 || histogramPair.Idx2 == idx2)
				{
					list[num2] = list[list.Count - 1];
					list.RemoveAt(list.Count - 1);
				}
				else
				{
					HistoListUpdateHead(list, histogramPair);
					num2++;
				}
			}
			for (int k = 0; k < num; k++)
			{
				if (k != idx && histograms[k] != null)
				{
					HistoPriorityListPush(list, maxSize, histograms, idx, k, 0.0, stats, bitsEntropy);
				}
			}
		}
	}

	private static void HistogramRemap(Vp8LHistogramSet input, Vp8LHistogramSet output, Span<ushort> symbols)
	{
		int count = input.Count;
		int count2 = output.Count;
		Vp8LStreaks stats = new Vp8LStreaks();
		Vp8LBitEntropy bitsEntropy = new Vp8LBitEntropy();
		if (count2 > 1)
		{
			for (int i = 0; i < count; i++)
			{
				if (input[i] == null)
				{
					symbols[i] = symbols[i - 1];
					continue;
				}
				int num = 0;
				double num2 = double.MaxValue;
				for (int j = 0; j < count2; j++)
				{
					double num3 = output[j].AddThresh(input[i], stats, bitsEntropy, num2);
					if (j == 0 || num3 < num2)
					{
						num2 = num3;
						num = j;
					}
				}
				symbols[i] = (ushort)num;
			}
		}
		else
		{
			for (int k = 0; k < count; k++)
			{
				symbols[k] = 0;
			}
		}
		int paletteCodeBits = output[0].PaletteCodeBits;
		for (int l = 0; l < count2; l++)
		{
			output[l].Clear();
			output[l].PaletteCodeBits = paletteCodeBits;
		}
		for (int m = 0; m < count; m++)
		{
			if (input[m] != null)
			{
				int index = symbols[m];
				input[m].Add(output[index], output[index]);
			}
		}
	}

	private static double HistoPriorityListPush(List<HistogramPair> histoList, int maxSize, Vp8LHistogramSet histograms, int idx1, int idx2, double threshold, Vp8LStreaks stats, Vp8LBitEntropy bitsEntropy)
	{
		HistogramPair histogramPair = new HistogramPair();
		if (histoList.Count == maxSize)
		{
			return 0.0;
		}
		if (idx1 > idx2)
		{
			int num = idx2;
			idx2 = idx1;
			idx1 = num;
		}
		histogramPair.Idx1 = idx1;
		histogramPair.Idx2 = idx2;
		Vp8LHistogram h = histograms[idx1];
		Vp8LHistogram h2 = histograms[idx2];
		HistoListUpdatePair(h, h2, stats, bitsEntropy, threshold, histogramPair);
		if (histogramPair.CostDiff >= threshold)
		{
			return 0.0;
		}
		histoList.Add(histogramPair);
		HistoListUpdateHead(histoList, histogramPair);
		return histogramPair.CostDiff;
	}

	private static void HistoListUpdatePair(Vp8LHistogram h1, Vp8LHistogram h2, Vp8LStreaks stats, Vp8LBitEntropy bitsEntropy, double threshold, HistogramPair pair)
	{
		double num = h1.BitCost + h2.BitCost;
		pair.CostCombo = 0.0;
		h1.GetCombinedHistogramEntropy(h2, stats, bitsEntropy, num + threshold, pair.CostCombo, out var cost);
		pair.CostCombo = cost;
		pair.CostDiff = pair.CostCombo - num;
	}

	private static void HistoListUpdateHead(List<HistogramPair> histoList, HistogramPair pair)
	{
		if (pair.CostDiff < histoList[0].CostDiff)
		{
			int index = histoList.IndexOf(pair);
			histoList[index] = histoList[0];
			histoList[0] = pair;
		}
	}

	private static void HistogramAdd(Vp8LHistogram a, Vp8LHistogram b, Vp8LHistogram output)
	{
		a.Add(b, output);
		output.TrivialSymbol = ((a.TrivialSymbol == b.TrivialSymbol) ? a.TrivialSymbol : uint.MaxValue);
	}

	private static double GetCombineCostFactor(int histoSize, uint quality)
	{
		double num = 0.16;
		if (quality < 90)
		{
			if (histoSize > 256)
			{
				num /= 2.0;
			}
			if (histoSize > 512)
			{
				num /= 2.0;
			}
			if (histoSize > 1024)
			{
				num /= 2.0;
			}
			if (quality <= 50)
			{
				num /= 2.0;
			}
		}
		return num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static uint MyRand(ref uint seed)
	{
		seed = (uint)((ulong)((long)seed * 48271L) % 2147483647uL);
		return seed;
	}
}
