using System;
using System.Runtime.CompilerServices;

namespace SixLabors.ImageSharp.ColorSpaces.Companding;

public static class LCompanding
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Expand(float channel)
	{
		if (!(channel <= 0.08f))
		{
			return Numerics.Pow3((channel + 0.16f) / 1.16f);
		}
		return 100f * channel / 903.2963f;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Compress(float channel)
	{
		if (!(channel <= 0.008856452f))
		{
			return 1.16f * MathF.Pow(channel, 0.3333333f) - 0.16f;
		}
		return channel * 903.2963f / 100f;
	}
}
