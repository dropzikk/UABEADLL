using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors.Transforms;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public readonly struct WelchResampler : IResampler
{
	public float Radius => 3f;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public float GetValue(float x)
	{
		if (x < 0f)
		{
			x = 0f - x;
		}
		if (x < 3f)
		{
			return Numerics.SinC(x) * (1f - x * x / 9f);
		}
		return 0f;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ApplyTransform<TPixel>(IResamplingTransformImageProcessor<TPixel> processor) where TPixel : unmanaged, IPixel<TPixel>
	{
		processor.ApplyTransform(in this);
	}
}
