using SixLabors.ImageSharp.Processing.Processors.Quantization;

namespace SixLabors.ImageSharp.Processing;

public static class KnownQuantizers
{
	public static IQuantizer Octree { get; } = new OctreeQuantizer();

	public static IQuantizer Wu { get; } = new WuQuantizer();

	public static IQuantizer WebSafe { get; } = new WebSafePaletteQuantizer();

	public static IQuantizer Werner { get; } = new WernerPaletteQuantizer();
}
