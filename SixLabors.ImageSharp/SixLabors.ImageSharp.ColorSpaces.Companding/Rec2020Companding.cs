using System;
using System.Runtime.CompilerServices;

namespace SixLabors.ImageSharp.ColorSpaces.Companding;

public static class Rec2020Companding
{
	private const float Alpha = 1.0992968f;

	private const float AlphaMinusOne = 0.09929681f;

	private const float Beta = 0.01805397f;

	private const float InverseBeta = 0.08124286f;

	private const float Epsilon = 2.2222223f;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Expand(float channel)
	{
		if (!(channel < 0.08124286f))
		{
			return MathF.Pow((channel + 0.09929681f) / 1.0992968f, 2.2222223f);
		}
		return channel / 4.5f;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Compress(float channel)
	{
		if (!(channel < 0.01805397f))
		{
			return 1.0992968f * MathF.Pow(channel, 0.45f) - 0.09929681f;
		}
		return 4.5f * channel;
	}
}
