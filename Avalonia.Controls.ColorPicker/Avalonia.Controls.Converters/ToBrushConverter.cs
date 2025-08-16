using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Avalonia.Controls.Converters;

public class ToBrushConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value is IBrush result)
		{
			return result;
		}
		if (value is Color color)
		{
			return new SolidColorBrush(color);
		}
		if (value is HslColor hslColor)
		{
			return new SolidColorBrush(hslColor.ToRgb());
		}
		if (value is HsvColor hsvColor)
		{
			return new SolidColorBrush(hsvColor.ToRgb());
		}
		return AvaloniaProperty.UnsetValue;
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		return AvaloniaProperty.UnsetValue;
	}
}
