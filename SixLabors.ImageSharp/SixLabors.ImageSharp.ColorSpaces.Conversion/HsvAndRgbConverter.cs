using System;
using System.Runtime.CompilerServices;

namespace SixLabors.ImageSharp.ColorSpaces.Conversion;

internal static class HsvAndRgbConverter
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Rgb Convert(in Hsv input)
	{
		float s = input.S;
		float v = input.V;
		if (MathF.Abs(s) < Constants.Epsilon)
		{
			return new Rgb(v, v, v);
		}
		float num = ((MathF.Abs(input.H - 360f) < Constants.Epsilon) ? 0f : (input.H / 60f));
		int num2 = (int)Math.Truncate(num);
		float num3 = num - (float)num2;
		float num4 = v * (1f - s);
		float num5 = v * (1f - s * num3);
		float num6 = v * (1f - s * (1f - num3));
		float r;
		float g;
		float b;
		switch (num2)
		{
		case 0:
			r = v;
			g = num6;
			b = num4;
			break;
		case 1:
			r = num5;
			g = v;
			b = num4;
			break;
		case 2:
			r = num4;
			g = v;
			b = num6;
			break;
		case 3:
			r = num4;
			g = num5;
			b = v;
			break;
		case 4:
			r = num6;
			g = num4;
			b = v;
			break;
		default:
			r = v;
			g = num4;
			b = num5;
			break;
		}
		return new Rgb(r, g, b);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Hsv Convert(in Rgb input)
	{
		float r = input.R;
		float g = input.G;
		float b = input.B;
		float num = MathF.Max(r, MathF.Max(g, b));
		float num2 = MathF.Min(r, MathF.Min(g, b));
		float num3 = num - num2;
		float num4 = 0f;
		float s = 0f;
		float num5 = num;
		if (MathF.Abs(num3) < Constants.Epsilon)
		{
			return new Hsv(0f, s, num5);
		}
		if (MathF.Abs(r - num) < Constants.Epsilon)
		{
			num4 = (g - b) / num3;
		}
		else if (MathF.Abs(g - num) < Constants.Epsilon)
		{
			num4 = 2f + (b - r) / num3;
		}
		else if (MathF.Abs(b - num) < Constants.Epsilon)
		{
			num4 = 4f + (r - g) / num3;
		}
		num4 *= 60f;
		if ((double)num4 < 0.0)
		{
			num4 += 360f;
		}
		s = num3 / num5;
		return new Hsv(num4, s, num5);
	}
}
