using SixLabors.ImageSharp.Processing.Processors.Transforms;

namespace SixLabors.ImageSharp.Processing;

public static class KnownResamplers
{
	public static IResampler Bicubic { get; } = default(BicubicResampler);

	public static IResampler Box { get; } = default(BoxResampler);

	public static IResampler CatmullRom { get; } = CubicResampler.CatmullRom;

	public static IResampler Hermite { get; } = CubicResampler.Hermite;

	public static IResampler Lanczos2 { get; } = LanczosResampler.Lanczos2;

	public static IResampler Lanczos3 { get; } = LanczosResampler.Lanczos3;

	public static IResampler Lanczos5 { get; } = LanczosResampler.Lanczos5;

	public static IResampler Lanczos8 { get; } = LanczosResampler.Lanczos8;

	public static IResampler MitchellNetravali { get; } = CubicResampler.MitchellNetravali;

	public static IResampler NearestNeighbor { get; } = default(NearestNeighborResampler);

	public static IResampler Robidoux { get; } = CubicResampler.Robidoux;

	public static IResampler RobidouxSharp { get; } = CubicResampler.RobidouxSharp;

	public static IResampler Spline { get; } = CubicResampler.Spline;

	public static IResampler Triangle { get; } = default(TriangleResampler);

	public static IResampler Welch { get; } = default(WelchResampler);
}
