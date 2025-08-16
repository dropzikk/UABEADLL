using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Utilities;

namespace Avalonia.Controls.Converters;

public class ColorToHexConverter : IValueConverter
{
	public bool IsAlphaVisible { get; set; } = true;

	public AlphaComponentPosition AlphaPosition { get; set; }

	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		bool valueOrDefault = parameter as bool? == true;
		Color color2;
		if (value is Color color)
		{
			color2 = color;
		}
		else if (value is HslColor hslColor)
		{
			color2 = hslColor.ToRgb();
		}
		else if (value is HsvColor hsvColor)
		{
			color2 = hsvColor.ToRgb();
		}
		else
		{
			if (!(value is SolidColorBrush solidColorBrush))
			{
				return AvaloniaProperty.UnsetValue;
			}
			color2 = solidColorBrush.Color;
		}
		return ToHexString(color2, AlphaPosition, IsAlphaVisible, valueOrDefault);
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		Color? color = ParseHexString(value?.ToString() ?? string.Empty, AlphaPosition);
		if (!color.HasValue)
		{
			return AvaloniaProperty.UnsetValue;
		}
		return color.GetValueOrDefault();
	}

	public static string ToHexString(Color color, AlphaComponentPosition alphaPosition, bool includeAlpha = true, bool includeSymbol = false)
	{
		string text = ((!includeAlpha) ? ((uint)((color.R << 16) | (color.G << 8) | color.B)).ToString("x6", CultureInfo.InvariantCulture).ToUpperInvariant() : ((uint)((alphaPosition != AlphaComponentPosition.Trailing) ? ((color.A << 24) | (color.R << 16) | (color.G << 8) | color.B) : ((color.R << 24) | (color.G << 16) | (color.B << 8) | color.A))).ToString("x8", CultureInfo.InvariantCulture).ToUpperInvariant());
		if (includeSymbol)
		{
			text = "#" + text;
		}
		return text;
	}

	public static Color? ParseHexString(string hexColor, AlphaComponentPosition alphaPosition)
	{
		hexColor = hexColor.Trim();
		if (!hexColor.StartsWith("#", StringComparison.Ordinal))
		{
			hexColor = "#" + hexColor;
		}
		if (TryParseHexFormat(hexColor.AsSpan(), alphaPosition, out var color))
		{
			return color;
		}
		return null;
	}

	private static bool TryParseHexFormat(ReadOnlySpan<char> s, AlphaComponentPosition alphaPosition, out Color color)
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
			return TryParseCore(span, alphaPosition, ref color);
		}
		return TryParseCore(input2, alphaPosition, ref color);
		static bool TryParseCore(ReadOnlySpan<char> input, AlphaComponentPosition alphaPosition, ref Color color)
		{
			uint num = 0u;
			if (input.Length == 6)
			{
				num = ((alphaPosition != AlphaComponentPosition.Trailing) ? 4278190080u : 255u);
			}
			else if (input.Length != 8)
			{
				return false;
			}
			if (!input.TryParseUInt(NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var value))
			{
				return false;
			}
			if (num != 0)
			{
				value = ((alphaPosition != AlphaComponentPosition.Trailing) ? (value | num) : ((value << 8) | num));
			}
			if (alphaPosition == AlphaComponentPosition.Trailing)
			{
				color = new Color((byte)(value & 0xFF), (byte)((value >> 24) & 0xFF), (byte)((value >> 16) & 0xFF), (byte)((value >> 8) & 0xFF));
			}
			else
			{
				color = new Color((byte)((value >> 24) & 0xFF), (byte)((value >> 16) & 0xFF), (byte)((value >> 8) & 0xFF), (byte)(value & 0xFF));
			}
			return true;
		}
	}
}
