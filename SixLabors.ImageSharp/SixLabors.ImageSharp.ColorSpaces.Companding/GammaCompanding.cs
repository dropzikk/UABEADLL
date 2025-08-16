using System;
using System.Runtime.CompilerServices;

namespace SixLabors.ImageSharp.ColorSpaces.Companding;

public static class GammaCompanding
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Expand(float channel, float gamma)
	{
		return MathF.Pow(channel, gamma);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Compress(float channel, float gamma)
	{
		return MathF.Pow(channel, 1f / gamma);
	}
}
