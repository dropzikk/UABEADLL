using System;

namespace SixLabors.ImageSharp.Formats.Webp.Lossless;

internal class Vp8LStreaks
{
	public int[] Counts { get; }

	public int[][] Streaks { get; }

	public Vp8LStreaks()
	{
		Counts = new int[2];
		Streaks = new int[2][];
		Streaks[0] = new int[2];
		Streaks[1] = new int[2];
	}

	public void Clear()
	{
		Counts.AsSpan().Clear();
		Streaks[0].AsSpan().Clear();
		Streaks[1].AsSpan().Clear();
	}

	public double FinalHuffmanCost()
	{
		return InitialHuffmanCost() + ((double)Counts[0] * 1.5625 + 15.0 / 64.0 * (double)Streaks[0][1]) + ((double)Counts[1] * 2.578125 + 45.0 / 64.0 * (double)Streaks[1][1]) + 1.796875 * (double)Streaks[0][0] + 3.28125 * (double)Streaks[1][0];
	}

	private static double InitialHuffmanCost()
	{
		double num = 9.1;
		return 57.0 - num;
	}
}
