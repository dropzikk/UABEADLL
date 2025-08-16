using System;
using System.Runtime.CompilerServices;

namespace SixLabors.ImageSharp.Processing.Processors.Transforms;

internal static class LinearTransformUtility
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float GetSamplingRadius<TResampler>(in TResampler sampler, int sourceSize, int destinationSize) where TResampler : struct, IResampler
	{
		float num = (float)sourceSize / (float)destinationSize;
		if (num < 1f)
		{
			num = 1f;
		}
		return MathF.Ceiling(sampler.Radius * num);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetRangeStart(float radius, float center, int min, int max)
	{
		return Numerics.Clamp((int)MathF.Ceiling(center - radius), min, max);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetRangeEnd(float radius, float center, int min, int max)
	{
		return Numerics.Clamp((int)MathF.Floor(center + radius), min, max);
	}
}
