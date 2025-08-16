using System.Diagnostics;

namespace SixLabors.ImageSharp.Formats.Webp.Lossless;

[DebuggerDisplay("Start: {Start}, End: {End}, Cost: {Cost}")]
internal class CostInterval
{
	public float Cost { get; set; }

	public int Start { get; set; }

	public int End { get; set; }

	public int Index { get; set; }

	public CostInterval? Previous { get; set; }

	public CostInterval? Next { get; set; }
}
