using System;
using System.Runtime.CompilerServices;

namespace SixLabors.ImageSharp.ColorSpaces.Companding;

public static class Rec709Companding
{
	private const float Epsilon = 2.2222223f;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Expand(float channel)
	{
		if (!(channel < 0.081f))
		{
			return MathF.Pow((channel + 0.099f) / 1.099f, 2.2222223f);
		}
		return channel / 4.5f;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Compress(float channel)
	{
		if (!(channel < 0.018f))
		{
			return 1.099f * MathF.Pow(channel, 0.45f) - 0.099f;
		}
		return 4.5f * channel;
	}
}
