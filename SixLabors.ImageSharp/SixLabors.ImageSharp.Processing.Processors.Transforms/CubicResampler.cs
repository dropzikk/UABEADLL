using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors.Transforms;

public readonly struct CubicResampler : IResampler
{
	private readonly float bspline;

	private readonly float cardinal;

	public static readonly CubicResampler CatmullRom = new CubicResampler(2f, 0f, 0.5f);

	public static readonly CubicResampler Hermite = new CubicResampler(2f, 0f, 0f);

	public static readonly CubicResampler MitchellNetravali = new CubicResampler(2f, 0.3333333f, 0.3333333f);

	public static readonly CubicResampler Robidoux = new CubicResampler(2f, 0.37821576f, 0.31089213f);

	public static readonly CubicResampler RobidouxSharp = new CubicResampler(2f, 0.2620145f, 0.36899275f);

	public static readonly CubicResampler Spline = new CubicResampler(2f, 1f, 0f);

	public float Radius { get; }

	public CubicResampler(float radius, float bspline, float cardinal)
	{
		Radius = radius;
		this.bspline = bspline;
		this.cardinal = cardinal;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public float GetValue(float x)
	{
		float num = bspline;
		float num2 = cardinal;
		if (x < 0f)
		{
			x = 0f - x;
		}
		float num3 = x * x;
		if (x < 1f)
		{
			x = (12f - 9f * num - 6f * num2) * (x * num3) + (-18f + 12f * num + 6f * num2) * num3 + (6f - 2f * num);
			return x / 6f;
		}
		if (x < 2f)
		{
			x = (0f - num - 6f * num2) * (x * num3) + (6f * num + 30f * num2) * num3 + (-12f * num - 48f * num2) * x + (8f * num + 24f * num2);
			return x / 6f;
		}
		return 0f;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ApplyTransform<TPixel>(IResamplingTransformImageProcessor<TPixel> processor) where TPixel : unmanaged, IPixel<TPixel>
	{
		processor.ApplyTransform(in this);
	}
}
