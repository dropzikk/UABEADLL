using System;
using System.Globalization;
using System.Text;
using Avalonia.Utilities;

namespace Avalonia.Media;

public readonly struct HsvColor : IEquatable<HsvColor>
{
	public double A { get; }

	public double H { get; }

	public double S { get; }

	public double V { get; }

	public HsvColor(double alpha, double hue, double saturation, double value)
	{
		A = MathUtilities.Clamp(alpha, 0.0, 1.0);
		H = MathUtilities.Clamp(hue, 0.0, 360.0);
		S = MathUtilities.Clamp(saturation, 0.0, 1.0);
		V = MathUtilities.Clamp(value, 0.0, 1.0);
		H = ((H == 360.0) ? 0.0 : H);
	}

	internal HsvColor(double alpha, double hue, double saturation, double value, bool clampValues)
	{
		if (clampValues)
		{
			A = MathUtilities.Clamp(alpha, 0.0, 1.0);
			H = MathUtilities.Clamp(hue, 0.0, 360.0);
			S = MathUtilities.Clamp(saturation, 0.0, 1.0);
			V = MathUtilities.Clamp(value, 0.0, 1.0);
			H = ((H == 360.0) ? 0.0 : H);
		}
		else
		{
			A = alpha;
			H = hue;
			S = saturation;
			V = value;
		}
	}

	public HsvColor(Color color)
	{
		HsvColor hsvColor = color.ToHsv();
		A = hsvColor.A;
		H = hsvColor.H;
		S = hsvColor.S;
		V = hsvColor.V;
	}

	public bool Equals(HsvColor other)
	{
		if (other.A == A && other.H == H && other.S == S)
		{
			return other.V == V;
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is HsvColor other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (((((A.GetHashCode() * 397) ^ H.GetHashCode()) * 397) ^ S.GetHashCode()) * 397) ^ V.GetHashCode();
	}

	public Color ToRgb()
	{
		return ToRgb(H, S, V, A);
	}

	public HslColor ToHsl()
	{
		return ToHsl(H, S, V, A);
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = StringBuilderCache.Acquire();
		stringBuilder.Append("hsva(");
		stringBuilder.Append(H.ToString(CultureInfo.InvariantCulture));
		stringBuilder.Append(", ");
		stringBuilder.Append(S.ToString(CultureInfo.InvariantCulture));
		stringBuilder.Append(", ");
		stringBuilder.Append(V.ToString(CultureInfo.InvariantCulture));
		stringBuilder.Append(", ");
		stringBuilder.Append(A.ToString(CultureInfo.InvariantCulture));
		stringBuilder.Append(')');
		return StringBuilderCache.GetStringAndRelease(stringBuilder);
	}

	public static HsvColor Parse(string s)
	{
		if (s == null)
		{
			throw new ArgumentNullException("s");
		}
		if (TryParse(s, out var hsvColor))
		{
			return hsvColor;
		}
		throw new FormatException("Invalid HSV color string: '" + s + "'.");
	}

	public static bool TryParse(string? s, out HsvColor hsvColor)
	{
		bool flag = false;
		hsvColor = default(HsvColor);
		if (s == null)
		{
			return false;
		}
		string text = s.Trim();
		if (text.Length == 0 || text.IndexOf(",", StringComparison.Ordinal) < 0)
		{
			return false;
		}
		if (text.Length >= 11 && text.StartsWith("hsva(", StringComparison.OrdinalIgnoreCase) && text.EndsWith(")", StringComparison.Ordinal))
		{
			text = text.Substring(5, text.Length - 6);
			flag = true;
		}
		if (!flag && text.Length >= 10 && text.StartsWith("hsv(", StringComparison.OrdinalIgnoreCase) && text.EndsWith(")", StringComparison.Ordinal))
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
				hsvColor = new HsvColor(1.0, value, outDouble2, outDouble3);
				return true;
			}
		}
		else if (array.Length == 4 && array[0].AsSpan().TryParseDouble(NumberStyles.Number, CultureInfo.InvariantCulture, out value2) && TryInternalParse(array[1].AsSpan(), out outDouble4) && TryInternalParse(array[2].AsSpan(), out outDouble5) && TryInternalParse(array[3].AsSpan(), out outDouble6))
		{
			hsvColor = new HsvColor(outDouble6, value2, outDouble4, outDouble5);
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

	public static HsvColor FromAhsv(double a, double h, double s, double v)
	{
		return new HsvColor(a, h, s, v);
	}

	public static HsvColor FromHsv(double h, double s, double v)
	{
		return new HsvColor(1.0, h, s, v);
	}

	public static Color ToRgb(double hue, double saturation, double value, double alpha = 1.0)
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
		value = ((value < 0.0) ? 0.0 : value);
		value = ((value > 1.0) ? 1.0 : value);
		alpha = ((alpha < 0.0) ? 0.0 : alpha);
		alpha = ((alpha > 1.0) ? 1.0 : alpha);
		double num = saturation * value;
		double num2 = value - num;
		if (num == 0.0)
		{
			return Color.FromArgb((byte)Math.Round(alpha * 255.0), (byte)Math.Round(num2 * 255.0), (byte)Math.Round(num2 * 255.0), (byte)Math.Round(num2 * 255.0));
		}
		int num3 = (int)(hue / 60.0);
		double num4 = hue / 60.0 - (double)num3;
		double num5 = num + num2;
		double num6 = 0.0;
		double num7 = 0.0;
		double num8 = 0.0;
		switch (num3)
		{
		case 0:
			num6 = num5;
			num7 = num2 + num * num4;
			num8 = num2;
			break;
		case 1:
			num6 = num2 + num * (1.0 - num4);
			num7 = num5;
			num8 = num2;
			break;
		case 2:
			num6 = num2;
			num7 = num5;
			num8 = num2 + num * num4;
			break;
		case 3:
			num6 = num2;
			num7 = num2 + num * (1.0 - num4);
			num8 = num5;
			break;
		case 4:
			num6 = num2 + num * num4;
			num7 = num2;
			num8 = num5;
			break;
		case 5:
			num6 = num5;
			num7 = num2;
			num8 = num2 + num * (1.0 - num4);
			break;
		}
		return new Color((byte)Math.Round(alpha * 255.0), (byte)Math.Round(num6 * 255.0), (byte)Math.Round(num7 * 255.0), (byte)Math.Round(num8 * 255.0));
	}

	public static HslColor ToHsl(double hue, double saturation, double value, double alpha = 1.0)
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
		value = ((value < 0.0) ? 0.0 : value);
		value = ((value > 1.0) ? 1.0 : value);
		alpha = ((alpha < 0.0) ? 0.0 : alpha);
		alpha = ((alpha > 1.0) ? 1.0 : alpha);
		double num = value * (1.0 - saturation / 2.0);
		double saturation2 = ((!(num <= 0.0) && !(num >= 1.0)) ? ((value - num) / Math.Min(num, 1.0 - num)) : 0.0);
		return new HslColor(alpha, hue, saturation2, num);
	}

	public static bool operator ==(HsvColor left, HsvColor right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(HsvColor left, HsvColor right)
	{
		return !(left == right);
	}

	public static explicit operator Color(HsvColor hsvColor)
	{
		return hsvColor.ToRgb();
	}
}
