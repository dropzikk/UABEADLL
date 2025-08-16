namespace SixLabors.ImageSharp.Formats.Webp.Lossless;

internal class DominantCostRange
{
	public double LiteralMax { get; set; }

	public double LiteralMin { get; set; }

	public double RedMax { get; set; }

	public double RedMin { get; set; }

	public double BlueMax { get; set; }

	public double BlueMin { get; set; }

	public DominantCostRange()
	{
		LiteralMax = 0.0;
		LiteralMin = double.MaxValue;
		RedMax = 0.0;
		RedMin = double.MaxValue;
		BlueMax = 0.0;
		BlueMin = double.MaxValue;
	}

	public void UpdateDominantCostRange(Vp8LHistogram h)
	{
		if (LiteralMax < h.LiteralCost)
		{
			LiteralMax = h.LiteralCost;
		}
		if (LiteralMin > h.LiteralCost)
		{
			LiteralMin = h.LiteralCost;
		}
		if (RedMax < h.RedCost)
		{
			RedMax = h.RedCost;
		}
		if (RedMin > h.RedCost)
		{
			RedMin = h.RedCost;
		}
		if (BlueMax < h.BlueCost)
		{
			BlueMax = h.BlueCost;
		}
		if (BlueMin > h.BlueCost)
		{
			BlueMin = h.BlueCost;
		}
	}

	public int GetHistoBinIndex(Vp8LHistogram h, int numPartitions)
	{
		return (GetBinIdForEntropy(LiteralMin, LiteralMax, h.LiteralCost, numPartitions) * numPartitions + GetBinIdForEntropy(RedMin, RedMax, h.RedCost, numPartitions)) * numPartitions + GetBinIdForEntropy(BlueMin, BlueMax, h.BlueCost, numPartitions);
	}

	private static int GetBinIdForEntropy(double min, double max, double val, int numPartitions)
	{
		double num = max - min;
		if (num > 0.0)
		{
			double num2 = val - min;
			return (int)(((double)numPartitions - 1E-06) * num2 / num);
		}
		return 0;
	}
}
