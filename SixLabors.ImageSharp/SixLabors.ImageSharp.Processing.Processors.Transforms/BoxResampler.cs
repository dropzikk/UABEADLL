using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors.Transforms;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public readonly struct BoxResampler : IResampler
{
	public float Radius => 0.5f;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public float GetValue(float x)
	{
		if (x > -0.5f && x <= 0.5f)
		{
			return 1f;
		}
		return 0f;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ApplyTransform<TPixel>(IResamplingTransformImageProcessor<TPixel> processor) where TPixel : unmanaged, IPixel<TPixel>
	{
		processor.ApplyTransform(in this);
	}
}
