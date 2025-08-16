using System;
using System.Runtime.CompilerServices;

namespace SixLabors.ImageSharp.ColorSpaces.Conversion;

internal static class HslAndRgbConverter
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Rgb Convert(in Hsl input)
	{
		float num = input.H / 360f;
		float r = 0f;
		float g = 0f;
		float b = 0f;
		float s = input.S;
		float l = input.L;
		if (MathF.Abs(l) > Constants.Epsilon)
		{
			if (MathF.Abs(s) < Constants.Epsilon)
			{
				r = (g = (b = l));
			}
			else
			{
				float num2 = ((l < 0.5f) ? (l * (1f + s)) : (l + s - l * s));
				float first = 2f * l - num2;
				r = GetColorComponent(first, num2, num + 0.3333333f);
				g = GetColorComponent(first, num2, num);
				b = GetColorComponent(first, num2, num - 0.3333333f);
			}
		}
		return new Rgb(r, g, b);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Hsl Convert(in Rgb input)
	{
		float r = input.R;
		float g = input.G;
		float b = input.B;
		float num = MathF.Max(r, MathF.Max(g, b));
		float num2 = MathF.Min(r, MathF.Min(g, b));
		float num3 = num - num2;
		float num4 = 0f;
		float s = 0f;
		float num5 = (num + num2) / 2f;
		if (MathF.Abs(num3) < Constants.Epsilon)
		{
			return new Hsl(0f, s, num5);
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
		if (num4 < 0f)
		{
			num4 += 360f;
		}
		s = ((!(num5 <= 0.5f)) ? (num3 / (2f - num - num2)) : (num3 / (num + num2)));
		return new Hsl(num4, s, num5);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static float GetColorComponent(float first, float second, float third)
	{
		third = MoveIntoRange(third);
		if (third < 0.1666667f)
		{
			return first + (second - first) * 6f * third;
		}
		if (third < 0.5f)
		{
			return second;
		}
		if (third < 2f / 3f)
		{
			return first + (second - first) * (2f / 3f - third) * 6f;
		}
		return first;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static float MoveIntoRange(float value)
	{
		if (value < 0f)
		{
			value += 1f;
		}
		else if (value > 1f)
		{
			value -= 1f;
		}
		return value;
	}
}
