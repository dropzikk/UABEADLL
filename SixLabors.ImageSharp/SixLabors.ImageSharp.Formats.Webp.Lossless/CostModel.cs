using System;
using SixLabors.ImageSharp.Memory;

namespace SixLabors.ImageSharp.Formats.Webp.Lossless;

internal class CostModel
{
	private readonly MemoryAllocator memoryAllocator;

	private const int ValuesInBytes = 256;

	public double[] Alpha { get; }

	public double[] Red { get; }

	public double[] Blue { get; }

	public double[] Distance { get; }

	public double[] Literal { get; }

	public CostModel(MemoryAllocator memoryAllocator, int literalArraySize)
	{
		this.memoryAllocator = memoryAllocator;
		Alpha = new double[256];
		Red = new double[256];
		Blue = new double[256];
		Distance = new double[40];
		Literal = new double[literalArraySize];
	}

	public void Build(int xSize, int cacheBits, Vp8LBackwardRefs backwardRefs)
	{
		using OwnedVp8LHistogram ownedVp8LHistogram = OwnedVp8LHistogram.Create(memoryAllocator, cacheBits);
		for (int i = 0; i < backwardRefs.Refs.Count; i++)
		{
			ownedVp8LHistogram.AddSinglePixOrCopy(backwardRefs.Refs[i], useDistanceModifier: true, xSize);
		}
		ConvertPopulationCountTableToBitEstimates(ownedVp8LHistogram.NumCodes(), ownedVp8LHistogram.Literal, Literal);
		ConvertPopulationCountTableToBitEstimates(256, ownedVp8LHistogram.Red, Red);
		ConvertPopulationCountTableToBitEstimates(256, ownedVp8LHistogram.Blue, Blue);
		ConvertPopulationCountTableToBitEstimates(256, ownedVp8LHistogram.Alpha, Alpha);
		ConvertPopulationCountTableToBitEstimates(40, ownedVp8LHistogram.Distance, Distance);
	}

	public double GetLengthCost(int length)
	{
		int extraBits = 0;
		int num = LosslessUtils.PrefixEncodeBits(length, ref extraBits);
		return Literal[256 + num] + (double)extraBits;
	}

	public double GetDistanceCost(int distance)
	{
		int extraBits = 0;
		int num = LosslessUtils.PrefixEncodeBits(distance, ref extraBits);
		return Distance[num] + (double)extraBits;
	}

	public double GetCacheCost(uint idx)
	{
		int num = (int)(280 + idx);
		return Literal[num];
	}

	public double GetLiteralCost(uint v)
	{
		return Alpha[v >> 24] + Red[(v >> 16) & 0xFF] + Literal[(v >> 8) & 0xFF] + Blue[v & 0xFF];
	}

	private static void ConvertPopulationCountTableToBitEstimates(int numSymbols, Span<uint> populationCounts, double[] output)
	{
		uint num = 0u;
		int num2 = 0;
		for (int i = 0; i < numSymbols; i++)
		{
			num += populationCounts[i];
			if (populationCounts[i] != 0)
			{
				num2++;
			}
		}
		if (num2 <= 1)
		{
			output.AsSpan(0, numSymbols).Clear();
			return;
		}
		double num3 = LosslessUtils.FastLog2(num);
		for (int j = 0; j < numSymbols; j++)
		{
			output[j] = num3 - (double)LosslessUtils.FastLog2(populationCounts[j]);
		}
	}
}
