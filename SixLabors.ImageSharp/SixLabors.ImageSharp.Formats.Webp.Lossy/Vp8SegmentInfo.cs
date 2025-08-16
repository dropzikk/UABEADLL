using System;

namespace SixLabors.ImageSharp.Formats.Webp.Lossy;

internal class Vp8SegmentInfo
{
	public Vp8Matrix Y1;

	public Vp8Matrix Y2;

	public Vp8Matrix Uv;

	public int Alpha { get; set; }

	public int Beta { get; set; }

	public int Quant { get; set; }

	public int FStrength { get; set; }

	public int MaxEdge { get; set; }

	public long I4Penalty { get; set; }

	public int MinDisto { get; set; }

	public int LambdaI16 { get; set; }

	public int LambdaI4 { get; set; }

	public int TLambda { get; set; }

	public int LambdaUv { get; set; }

	public int LambdaMode { get; set; }

	public void StoreMaxDelta(Span<short> dcs)
	{
		int num = Math.Abs(dcs[1]);
		int num2 = Math.Abs(dcs[2]);
		int num3 = Math.Abs(dcs[4]);
		int num4 = ((num2 > num) ? num2 : num);
		num4 = ((num3 > num4) ? num3 : num4);
		if (num4 > MaxEdge)
		{
			MaxEdge = num4;
		}
	}
}
