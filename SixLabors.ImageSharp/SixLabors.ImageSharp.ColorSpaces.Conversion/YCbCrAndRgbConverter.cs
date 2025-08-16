using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SixLabors.ImageSharp.ColorSpaces.Conversion;

internal static class YCbCrAndRgbConverter
{
	private static readonly Vector3 MaxBytes = new Vector3(255f);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Rgb Convert(in YCbCr input)
	{
		float y = input.Y;
		float num = input.Cb - 128f;
		float num2 = input.Cr - 128f;
		float x = MathF.Round(y + 1.402f * num2, MidpointRounding.AwayFromZero);
		float y2 = MathF.Round(y - 0.344136f * num - 0.714136f * num2, MidpointRounding.AwayFromZero);
		float z = MathF.Round(y + 1.772f * num, MidpointRounding.AwayFromZero);
		return new Rgb(new Vector3(x, y2, z) / MaxBytes);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static YCbCr Convert(in Rgb input)
	{
		Vector3 vector = input.ToVector3() * MaxBytes;
		float x = vector.X;
		float y = vector.Y;
		float z = vector.Z;
		float y2 = 0.299f * x + 0.587f * y + 0.114f * z;
		float cb = 128f + (-0.168736f * x - 0.331264f * y + 0.5f * z);
		float cr = 128f + (0.5f * x - 0.418688f * y - 0.081312f * z);
		return new YCbCr(y2, cb, cr);
	}
}
