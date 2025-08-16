using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors.Transforms;

public readonly struct LanczosResampler : IResampler
{
	public static readonly LanczosResampler Lanczos2 = new LanczosResampler(2f);

	public static readonly LanczosResampler Lanczos3 = new LanczosResampler(3f);

	public static readonly LanczosResampler Lanczos5 = new LanczosResampler(5f);

	public static readonly LanczosResampler Lanczos8 = new LanczosResampler(8f);

	public float Radius { get; }

	public LanczosResampler(float radius)
	{
		Radius = radius;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public float GetValue(float x)
	{
		if (x < 0f)
		{
			x = 0f - x;
		}
		float radius = Radius;
		if (x < radius)
		{
			return Numerics.SinC(x) * Numerics.SinC(x / radius);
		}
		return 0f;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ApplyTransform<TPixel>(IResamplingTransformImageProcessor<TPixel> processor) where TPixel : unmanaged, IPixel<TPixel>
	{
		processor.ApplyTransform(in this);
	}
}
