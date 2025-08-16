using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Avalonia.Controls.Primitives.Converters;

public class AccentColorConverter : IValueConverter
{
	public const double ValueDelta = 0.1;

	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		Color? color = null;
		HsvColor? hsvColor = null;
		if (value is Color value2)
		{
			color = value2;
		}
		else if (value is HslColor hslColor)
		{
			color = hslColor.ToRgb();
		}
		else if (value is HsvColor value3)
		{
			hsvColor = value3;
		}
		else
		{
			if (!(value is SolidColorBrush solidColorBrush))
			{
				return AvaloniaProperty.UnsetValue;
			}
			color = solidColorBrush.Color;
		}
		int accentStep;
		try
		{
			accentStep = int.Parse(parameter?.ToString() ?? "", CultureInfo.InvariantCulture);
		}
		catch
		{
			return AvaloniaProperty.UnsetValue;
		}
		if (!hsvColor.HasValue && color.HasValue)
		{
			hsvColor = color.Value.ToHsv();
		}
		if (hsvColor.HasValue)
		{
			return new SolidColorBrush(GetAccent(hsvColor.Value, accentStep).ToRgb());
		}
		return AvaloniaProperty.UnsetValue;
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		return AvaloniaProperty.UnsetValue;
	}

	public static HsvColor GetAccent(HsvColor hsvColor, int accentStep)
	{
		if (accentStep != 0)
		{
			double v = hsvColor.V;
			v += (double)accentStep * 0.1;
			v = Math.Round(v, 2);
			return new HsvColor(hsvColor.A, hsvColor.H, hsvColor.S, v);
		}
		return hsvColor;
	}
}
