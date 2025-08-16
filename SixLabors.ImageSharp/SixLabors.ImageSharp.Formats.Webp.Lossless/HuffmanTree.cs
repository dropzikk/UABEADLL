using System.Diagnostics;

namespace SixLabors.ImageSharp.Formats.Webp.Lossless;

[DebuggerDisplay("TotalCount = {TotalCount}, Value = {Value}, Left = {PoolIndexLeft}, Right = {PoolIndexRight}")]
internal struct HuffmanTree
{
	public int TotalCount { get; set; }

	public int Value { get; set; }

	public int PoolIndexLeft { get; set; }

	public int PoolIndexRight { get; set; }

	private HuffmanTree(HuffmanTree other)
	{
		TotalCount = other.TotalCount;
		Value = other.Value;
		PoolIndexLeft = other.PoolIndexLeft;
		PoolIndexRight = other.PoolIndexRight;
	}

	public static int Compare(HuffmanTree t1, HuffmanTree t2)
	{
		if (t1.TotalCount > t2.TotalCount)
		{
			return -1;
		}
		if (t1.TotalCount < t2.TotalCount)
		{
			return 1;
		}
		if (t1.Value >= t2.Value)
		{
			return 1;
		}
		return -1;
	}
}
