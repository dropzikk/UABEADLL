using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SixLabors.ImageSharp.ColorSpaces.Conversion;

internal static class CmykAndRgbConverter
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Rgb Convert(in Cmyk input)
	{
		return new Rgb((Vector3.One - new Vector3(input.C, input.M, input.Y)) * (Vector3.One - new Vector3(input.K)));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Cmyk Convert(in Rgb input)
	{
		Vector3 vector = Vector3.One - input.ToVector3();
		Vector3 vector2 = new Vector3(MathF.Min(vector.X, MathF.Min(vector.Y, vector.Z)));
		if (MathF.Abs(vector2.X - 1f) < Constants.Epsilon)
		{
			return new Cmyk(0f, 0f, 0f, 1f);
		}
		vector = (vector - vector2) / (Vector3.One - vector2);
		return new Cmyk(vector.X, vector.Y, vector.Z, vector2.X);
	}
}
