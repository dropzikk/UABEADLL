using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors.Transforms;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public readonly struct BicubicResampler : IResampler
{
	public float Radius => 2f;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public float GetValue(float x)
	{
		if (x < 0f)
		{
			x = 0f - x;
		}
		if (x <= 1f)
		{
			return (1.5f * x - 2.5f) * x * x + 1f;
		}
		if (x < 2f)
		{
			return ((-0.5f * x + 2.5f) * x - 4f) * x + 2f;
		}
		return 0f;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ApplyTransform<TPixel>(IResamplingTransformImageProcessor<TPixel> processor) where TPixel : unmanaged, IPixel<TPixel>
	{
		processor.ApplyTransform(in this);
	}
}
