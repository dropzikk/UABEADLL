using System;
using System.Globalization;
using System.Text;
using Avalonia.Utilities;

namespace Avalonia.Media;

public readonly struct HslColor : IEquatable<HslColor>
{
	public double A { get; }

	public double H { get; }

	public double S { get; }

	public double L { get; }

	public HslColor(double alpha, double hue, double saturation, double lightness)
	{
		A = MathUtilities.Clamp(alpha, 0.0, 1.0);
		H = MathUtilities.Clamp(hue, 0.0, 360.0);
		S = MathUtilities.Clamp(saturation, 0.0, 1.0);
		L = MathUtilities.Clamp(lightness, 0.0, 1.0);
		H = ((H == 360.0) ? 0.0 : H);
	}

	internal HslColor(double alpha, double hue, double saturation, double lightness, bool clampValues)
	{
		if (clampValues)
		{
			A = MathUtilities.Clamp(alpha, 0.0, 1.0);
			H = MathUtilities.Clamp(hue, 0.0, 360.0);
			S = MathUtilities.Clamp(saturation, 0.0, 1.0);
			L = MathUtilities.Clamp(lightness, 0.0, 1.0);
			H = ((H == 360.0) ? 0.0 : H);
		}
		else
		{
			A = alpha;
			H = hue;
			S = saturation;
			L = lightness;
		}
	}

	public HslColor(Color color)
	{
		HslColor hslColor = color.ToHsl();
		A = hslColor.A;
		H = hslColor.H;
		S = hslColor.S;
		L = hslColor.L;
	}

	public bool Equals(HslColor other)
	{
		if (other.A == A && other.H == H && other.S == S)
		{
			return other.L == L;
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is HslColor other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (((((A.GetHashCode() * 397) ^ H.GetHashCode()) * 397) ^ S.GetHashCode()) * 397) ^ L.GetHashCode();
	}

	public Color ToRgb()
	{
		return ToRgb(H, S, L, A);
	}

	public HsvColor ToHsv()
	{
		return ToHsv(H, S, L, A);
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = StringBuilderCache.Acquire();
		stringBuilder.Append("hsva(");
		stringBuilder.Append(H.ToString(CultureInfo.InvariantCulture));
		stringBuilder.Append(", ");
		stringBuilder.Append(S.ToString(CultureInfo.InvariantCulture));
		stringBuilder.Append(", ");
		stringBuilder.Append(L.ToString(CultureInfo.InvariantCulture));
		stringBuilder.Append(", ");
		stringBuilder.Append(A.ToString(CultureInfo.InvariantCulture));
		stringBuilder.Append(')');
		return StringBuilderCache.GetStringAndRelease(stringBuilder);
	}

	public static HslColor Parse(string s)
	{
		if (s == null)
		{
			throw new ArgumentNullException("s");
		}
		if (TryParse(s, out var hslColor))
		{
			return hslColor;
		}
		throw new FormatException("Invalid HSL color string: '" + s + "'.");
	}

	public static bool TryParse(string? s, out HslColor hslColor)
	{
		bool flag = false;
		hslColor = default(HslColor);
		if (s == null)
		{
			return false;
		}
		string text = s.Trim();
		if (text.Length == 0 || text.IndexOf(",", StringComparison.Ordinal) < 0)
		{
			return false;
		}
		if (text.Length >= 11 && text.StartsWith("hsla(", StringComparison.OrdinalIgnoreCase) && text.EndsWith(")", StringComparison.Ordinal))
		{
			text = text.Substring(5, text.Length - 6);
			flag = true;
		}
		if (!flag && text.Length >= 10 && text.StartsWith("hsl(", StringComparison.OrdinalIgnoreCase) && text.EndsWith(")", StringComparison.Ordinal))
		{
			text = text.Substring(4, text.Length - 5);
			flag = true;
		}
		if (!flag)
		{
			return false;
		}
		string[] array = text.Split(',');
		double value2;
		double outDouble4;
		double outDouble5;
		double outDouble6;
		if (array.Length == 3)
		{
			if (array[0].AsSpan().TryParseDouble(NumberStyles.Number, CultureInfo.InvariantCulture, out var value) && TryInternalParse(array[1].AsSpan(), out var outDouble2) && TryInternalParse(array[2].AsSpan(), out var outDouble3))
			{
				hslColor = new HslColor(1.0, value, outDouble2, outDouble3);
				return true;
			}
		}
		else if (array.Length == 4 && array[0].AsSpan().TryParseDouble(NumberStyles.Number, CultureInfo.InvariantCulture, out value2) && TryInternalParse(array[1].AsSpan(), out outDouble4) && TryInternalParse(array[2].AsSpan(), out outDouble5) && TryInternalParse(array[3].AsSpan(), out outDouble6))
		{
			hslColor = new HslColor(outDouble6, value2, outDouble4, outDouble5);
			return true;
		}
		return false;
		static bool TryInternalParse(ReadOnlySpan<char> inString, out double outDouble)
		{
			int num = inString.IndexOf("%".AsSpan(), StringComparison.Ordinal);
			if (num >= 0)
			{
				double value3;
				bool result = inString.Slice(0, num).TryParseDouble(NumberStyles.Number, CultureInfo.InvariantCulture, out value3);
				outDouble = value3 / 100.0;
				return result;
			}
			return inString.TryParseDouble(NumberStyles.Number, CultureInfo.InvariantCulture, out outDouble);
		}
	}

	public static HslColor FromAhsl(double a, double h, double s, double l)
	{
		return new HslColor(a, h, s, l);
	}

	public static HslColor FromHsl(double h, double s, double l)
	{
		return new HslColor(1.0, h, s, l);
	}

	public static Color ToRgb(double hue, double saturation, double lightness, double alpha = 1.0)
	{
		while (hue >= 360.0)
		{
			hue -= 360.0;
		}
		while (hue < 0.0)
		{
			hue += 360.0;
		}
		saturation = ((saturation < 0.0) ? 0.0 : saturation);
		saturation = ((saturation > 1.0) ? 1.0 : saturation);
		lightness = ((lightness < 0.0) ? 0.0 : lightness);
		lightness = ((lightness > 1.0) ? 1.0 : lightness);
		alpha = ((alpha < 0.0) ? 0.0 : alpha);
		alpha = ((alpha > 1.0) ? 1.0 : alpha);
		double num = (1.0 - Math.Abs(2.0 * lightness - 1.0)) * saturation;
		double num2 = hue / 60.0;
		double num3 = num * (1.0 - Math.Abs(num2 % 2.0 - 1.0));
		double num4 = lightness - 0.5 * num;
		double num5;
		double num6;
		double num7;
		if (num2 < 1.0)
		{
			num5 = num;
			num6 = num3;
			num7 = 0.0;
		}
		else if (num2 < 2.0)
		{
			num5 = num3;
			num6 = num;
			num7 = 0.0;
		}
		else if (num2 < 3.0)
		{
			num5 = 0.0;
			num6 = num;
			num7 = num3;
		}
		else if (num2 < 4.0)
		{
			num5 = 0.0;
			num6 = num3;
			num7 = num;
		}
		else if (num2 < 5.0)
		{
			num5 = num3;
			num6 = 0.0;
			num7 = num;
		}
		else
		{
			num5 = num;
			num6 = 0.0;
			num7 = num3;
		}
		return new Color((byte)Math.Round(255.0 * alpha), (byte)Math.Round(255.0 * (num5 + num4)), (byte)Math.Round(255.0 * (num6 + num4)), (byte)Math.Round(255.0 * (num7 + num4)));
	}

	public static HsvColor ToHsv(double hue, double saturation, double lightness, double alpha = 1.0)
	{
		while (hue >= 360.0)
		{
			hue -= 360.0;
		}
		while (hue < 0.0)
		{
			hue += 360.0;
		}
		saturation = ((saturation < 0.0) ? 0.0 : saturation);
		saturation = ((saturation > 1.0) ? 1.0 : saturation);
		lightness = ((lightness < 0.0) ? 0.0 : lightness);
		lightness = ((lightness > 1.0) ? 1.0 : lightness);
		alpha = ((alpha < 0.0) ? 0.0 : alpha);
		alpha = ((alpha > 1.0) ? 1.0 : alpha);
		double num = lightness + saturation * Math.Min(lightness, 1.0 - lightness);
		double saturation2 = ((!(num <= 0.0)) ? (2.0 * (1.0 - lightness / num)) : 0.0);
		return new HsvColor(alpha, hue, saturation2, num);
	}

	public static bool operator ==(HslColor left, HslColor right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(HslColor left, HslColor right)
	{
		return !(left == right);
	}

	public static explicit operator Color(HslColor hslColor)
	{
		return hslColor.ToRgb();
	}
}
