using SixLabors.ImageSharp.Processing.Processors.Dithering;

namespace SixLabors.ImageSharp.Processing;

public static class KnownDitherings
{
	public static IDither Bayer2x2 { get; } = OrderedDither.Bayer2x2;

	public static IDither Ordered3x3 { get; } = OrderedDither.Ordered3x3;

	public static IDither Bayer4x4 { get; } = OrderedDither.Bayer4x4;

	public static IDither Bayer8x8 { get; } = OrderedDither.Bayer8x8;

	public static IDither Bayer16x16 { get; } = OrderedDither.Bayer16x16;

	public static IDither Atkinson { get; } = ErrorDither.Atkinson;

	public static IDither Burks { get; } = ErrorDither.Burkes;

	public static IDither FloydSteinberg { get; } = ErrorDither.FloydSteinberg;

	public static IDither JarvisJudiceNinke { get; } = ErrorDither.JarvisJudiceNinke;

	public static IDither Sierra2 { get; } = ErrorDither.Sierra2;

	public static IDither Sierra3 { get; } = ErrorDither.Sierra3;

	public static IDither SierraLite { get; } = ErrorDither.SierraLite;

	public static IDither StevensonArce { get; } = ErrorDither.StevensonArce;

	public static IDither Stucki { get; } = ErrorDither.Stucki;
}
