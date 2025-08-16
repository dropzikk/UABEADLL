using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Utilities;

namespace Avalonia.Controls.Converters;

public class ToColorConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value is Color color)
		{
			return color;
		}
		if (value is HslColor hslColor)
		{
			return hslColor.ToRgb();
		}
		if (value is HsvColor hsvColor)
		{
			return hsvColor.ToRgb();
		}
		if (value is SolidColorBrush { Color: var color2 } solidColorBrush)
		{
			return new Color((byte)MathUtilities.Clamp((double)(int)color2.A * solidColorBrush.Opacity, 0.0, 255.0), solidColorBrush.Color.R, solidColorBrush.Color.G, solidColorBrush.Color.B);
		}
		return AvaloniaProperty.UnsetValue;
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		return AvaloniaProperty.UnsetValue;
	}
}
