using System;
using System.Runtime.CompilerServices;

namespace SixLabors.ImageSharp;

public static class GeometryUtilities
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float DegreeToRadian(float degree)
	{
		return degree * ((float)Math.PI / 180f);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float RadianToDegree(float radian)
	{
		return radian / ((float)Math.PI / 180f);
	}
}
