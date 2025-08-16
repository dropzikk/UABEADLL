using System;
using System.Globalization;
using Avalonia.Controls.Primitives;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Avalonia.Controls.Converters;

public class ColorToDisplayNameConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
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
		if (color2.A == 0)
		{
			return AvaloniaProperty.UnsetValue;
		}
		return ColorHelper.ToDisplayName(color2);
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		return AvaloniaProperty.UnsetValue;
	}
}
