using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors.Transforms;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public readonly struct NearestNeighborResampler : IResampler
{
	public float Radius => 1f;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public float GetValue(float x)
	{
		return x;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ApplyTransform<TPixel>(IResamplingTransformImageProcessor<TPixel> processor) where TPixel : unmanaged, IPixel<TPixel>
	{
		processor.ApplyTransform(in this);
	}
}
