using System;
using System.ComponentModel;
using System.Globalization;
using Avalonia.Utilities;

namespace Avalonia.Media;

public readonly struct Color : IEquatable<Color>
{
	private const double byteToDouble = 1.0 / 255.0;

	public byte A { get; }

	public byte R { get; }

	public byte G { get; }

	public byte B { get; }

	public Color(byte a, byte r, byte g, byte b)
	{
		A = a;
		R = r;
		G = g;
		B = b;
	}

	public static Color FromArgb(byte a, byte r, byte g, byte b)
	{
		return new Color(a, r, g, b);
	}

	public static Color FromRgb(byte r, byte g, byte b)
	{
		return new Color(byte.MaxValue, r, g, b);
	}

	public static Color FromUInt32(uint value)
	{
		return new Color((byte)((value >> 24) & 0xFF), (byte)((value >> 16) & 0xFF), (byte)((value >> 8) & 0xFF), (byte)(value & 0xFF));
	}

	public static Color Parse(string s)
	{
		if (s == null)
		{
			throw new ArgumentNullException("s");
		}
		if (TryParse(s, out var color))
		{
			return color;
		}
		throw new FormatException("Invalid color string: '" + s + "'.");
	}

	public static Color Parse(ReadOnlySpan<char> s)
	{
		if (TryParse(s, out var color))
		{
			return color;
		}
		throw new FormatException("Invalid color string: '" + s.ToString() + "'.");
	}

	public static bool TryParse(string? s, out Color color)
	{
		color = default(Color);
		if (string.IsNullOrEmpty(s))
		{
			return false;
		}
		if (s[0] == '#' && TryParseHexFormat(s.AsSpan(), out color))
		{
			return true;
		}
		if (s.Length >= 10 && (s[0] == 'r' || s[0] == 'R') && (s[1] == 'g' || s[1] == 'G') && (s[2] == 'b' || s[2] == 'B') && TryParseCssFormat(s, out color))
		{
			return true;
		}
		if (s.Length >= 10 && (s[0] == 'h' || s[0] == 'H') && (s[1] == 's' || s[1] == 'S') && (s[2] == 'l' || s[2] == 'L') && HslColor.TryParse(s, out var hslColor))
		{
			color = hslColor.ToRgb();
			return true;
		}
		if (s.Length >= 10 && (s[0] == 'h' || s[0] == 'H') && (s[1] == 's' || s[1] == 'S') && (s[2] == 'v' || s[2] == 'V') && HsvColor.TryParse(s, out var hsvColor))
		{
			color = hsvColor.ToRgb();
			return true;
		}
		KnownColor knownColor = KnownColors.GetKnownColor(s);
		if (knownColor != 0)
		{
			color = knownColor.ToColor();
			return true;
		}
		return false;
	}

	public static bool TryParse(ReadOnlySpan<char> s, out Color color)
	{
		if (s.Length == 0)
		{
			color = default(Color);
			return false;
		}
		if (s[0] == '#' && TryParseHexFormat(s, out color))
		{
			return true;
		}
		string s2 = s.ToString();
		if (s.Length >= 10 && (s[0] == 'r' || s[0] == 'R') && (s[1] == 'g' || s[1] == 'G') && (s[2] == 'b' || s[2] == 'B') && TryParseCssFormat(s2, out color))
		{
			return true;
		}
		if (s.Length >= 10 && (s[0] == 'h' || s[0] == 'H') && (s[1] == 's' || s[1] == 'S') && (s[2] == 'l' || s[2] == 'L') && HslColor.TryParse(s2, out var hslColor))
		{
			color = hslColor.ToRgb();
			return true;
		}
		if (s.Length >= 10 && (s[0] == 'h' || s[0] == 'H') && (s[1] == 's' || s[1] == 'S') && (s[2] == 'v' || s[2] == 'V') && HsvColor.TryParse(s2, out var hsvColor))
		{
			color = hsvColor.ToRgb();
			return true;
		}
		KnownColor knownColor = KnownColors.GetKnownColor(s2);
		if (knownColor != 0)
		{
			color = knownColor.ToColor();
			return true;
		}
		color = default(Color);
		return false;
	}

	private static bool TryParseHexFormat(ReadOnlySpan<char> s, out Color color)
	{
		color = default(Color);
		ReadOnlySpan<char> input2 = s.Slice(1);
		if (input2.Length == 3 || input2.Length == 4)
		{
			Span<char> span = stackalloc char[2 * input2.Length];
			for (int i = 0; i < input2.Length; i++)
			{
				span[2 * i] = input2[i];
				span[2 * i + 1] = input2[i];
			}
			return TryParseCore(span, ref color);
		}
		return TryParseCore(input2, ref color);
		static bool TryParseCore(ReadOnlySpan<char> input, ref Color color)
		{
			uint num = 0u;
			if (input.Length == 6)
			{
				num = 4278190080u;
			}
			else if (input.Length != 8)
			{
				return false;
			}
			if (!input.TryParseUInt(NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var value))
			{
				return false;
			}
			color = FromUInt32(value | num);
			return true;
		}
	}

	private static bool TryParseCssFormat(string? s, out Color color)
	{
		bool flag = false;
		color = default(Color);
		if (s == null)
		{
			return false;
		}
		string text = s.Trim();
		if (text.Length == 0 || text.IndexOf(",", StringComparison.Ordinal) < 0)
		{
			return false;
		}
		if (text.Length >= 11 && text.StartsWith("rgba(", StringComparison.OrdinalIgnoreCase) && text.EndsWith(")", StringComparison.Ordinal))
		{
			text = text.Substring(5, text.Length - 6);
			flag = true;
		}
		if (!flag && text.Length >= 10 && text.StartsWith("rgb(", StringComparison.OrdinalIgnoreCase) && text.EndsWith(")", StringComparison.Ordinal))
		{
			text = text.Substring(4, text.Length - 5);
			flag = true;
		}
		if (!flag)
		{
			return false;
		}
		string[] array = text.Split(',');
		byte outByte5;
		byte outByte6;
		byte outByte7;
		double outDouble2;
		if (array.Length == 3)
		{
			if (InternalTryParseByte(array[0].AsSpan(), out var outByte2) && InternalTryParseByte(array[1].AsSpan(), out var outByte3) && InternalTryParseByte(array[2].AsSpan(), out var outByte4))
			{
				color = new Color(byte.MaxValue, outByte2, outByte3, outByte4);
				return true;
			}
		}
		else if (array.Length == 4 && InternalTryParseByte(array[0].AsSpan(), out outByte5) && InternalTryParseByte(array[1].AsSpan(), out outByte6) && InternalTryParseByte(array[2].AsSpan(), out outByte7) && InternalTryParseDouble(array[3].AsSpan(), out outDouble2))
		{
			color = new Color((byte)Math.Round(outDouble2 * 255.0), outByte5, outByte6, outByte7);
			return true;
		}
		return false;
		static bool InternalTryParseByte(ReadOnlySpan<char> inString, out byte outByte)
		{
			int num = inString.IndexOf("%".AsSpan(), StringComparison.Ordinal);
			if (num >= 0)
			{
				double value;
				bool result = inString.Slice(0, num).TryParseDouble(NumberStyles.Number, CultureInfo.InvariantCulture, out value);
				outByte = (byte)Math.Round(value / 100.0 * 255.0);
				return result;
			}
			return inString.TryParseByte(NumberStyles.Number, CultureInfo.InvariantCulture, out outByte);
		}
		static bool InternalTryParseDouble(ReadOnlySpan<char> inString, out double outDouble)
		{
			int num2 = inString.IndexOf("%".AsSpan(), StringComparison.Ordinal);
			if (num2 >= 0)
			{
				double value2;
				bool result2 = inString.Slice(0, num2).TryParseDouble(NumberStyles.Number, CultureInfo.InvariantCulture, out value2);
				outDouble = value2 / 100.0;
				return result2;
			}
			return inString.TryParseDouble(NumberStyles.Number, CultureInfo.InvariantCulture, out outDouble);
		}
	}

	public override string ToString()
	{
		uint rgb = ToUInt32();
		return KnownColors.GetKnownColorName(rgb) ?? ("#" + rgb.ToString("x8", CultureInfo.InvariantCulture));
	}

	public uint ToUInt32()
	{
		return (uint)((A << 24) | (R << 16) | (G << 8) | B);
	}

	[Obsolete("Use Color.ToUInt32() instead.")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public uint ToUint32()
	{
		return ToUInt32();
	}

	public HslColor ToHsl()
	{
		return ToHsl(R, G, B, A);
	}

	public HsvColor ToHsv()
	{
		return ToHsv(R, G, B, A);
	}

	public bool Equals(Color other)
	{
		if (A == other.A && R == other.R && G == other.G)
		{
			return B == other.B;
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is Color other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (((((A.GetHashCode() * 397) ^ R.GetHashCode()) * 397) ^ G.GetHashCode()) * 397) ^ B.GetHashCode();
	}

	public static HslColor ToHsl(byte red, byte green, byte blue, byte alpha = byte.MaxValue)
	{
		return ToHsl(1.0 / 255.0 * (double)(int)red, 1.0 / 255.0 * (double)(int)green, 1.0 / 255.0 * (double)(int)blue, 1.0 / 255.0 * (double)(int)alpha);
	}

	internal static HslColor ToHsl(double r, double g, double b, double a = 1.0)
	{
		double num = ((!(r >= g)) ? ((g >= b) ? g : b) : ((r >= b) ? r : b));
		double num2 = ((!(r <= g)) ? ((g <= b) ? g : b) : ((r <= b) ? r : b));
		double num3 = num - num2;
		double num4 = ((num3 == 0.0) ? 0.0 : ((num == r) ? (((g - b) / num3 + 6.0) % 6.0) : ((num != g) ? (4.0 + (r - g) / num3) : (2.0 + (b - r) / num3))));
		double num5 = 0.5 * (num + num2);
		double saturation = ((num3 == 0.0) ? 0.0 : (num3 / (1.0 - Math.Abs(2.0 * num5 - 1.0))));
		return new HslColor(a, 60.0 * num4, saturation, num5, clampValues: false);
	}

	public static HsvColor ToHsv(byte red, byte green, byte blue, byte alpha = byte.MaxValue)
	{
		return ToHsv(1.0 / 255.0 * (double)(int)red, 1.0 / 255.0 * (double)(int)green, 1.0 / 255.0 * (double)(int)blue, 1.0 / 255.0 * (double)(int)alpha);
	}

	internal static HsvColor ToHsv(double r, double g, double b, double a = 1.0)
	{
		double num = ((!(r >= g)) ? ((g >= b) ? g : b) : ((r >= b) ? r : b));
		double num2 = ((!(r <= g)) ? ((g <= b) ? g : b) : ((r <= b) ? r : b));
		double num3 = num;
		double num4 = num - num2;
		double num5;
		double saturation;
		if (num4 == 0.0)
		{
			num5 = 0.0;
			saturation = 0.0;
		}
		else
		{
			num5 = ((r == num) ? (60.0 * (g - b) / num4) : ((g != num) ? (240.0 + 60.0 * (r - g) / num4) : (120.0 + 60.0 * (b - r) / num4)));
			if (num5 < 0.0)
			{
				num5 += 360.0;
			}
			saturation = num4 / num3;
		}
		return new HsvColor(a, num5, saturation, num3, clampValues: false);
	}

	public static bool operator ==(Color left, Color right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(Color left, Color right)
	{
		return !left.Equals(right);
	}
}
