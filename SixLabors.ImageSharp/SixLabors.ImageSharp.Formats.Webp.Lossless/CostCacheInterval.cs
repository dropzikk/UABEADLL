using System.Diagnostics;

namespace SixLabors.ImageSharp.Formats.Webp.Lossless;

[DebuggerDisplay("Start: {Start}, End: {End}, Cost: {Cost}")]
internal class CostCacheInterval
{
	public double Cost { get; set; }

	public int Start { get; set; }

	public int End { get; set; }
}
