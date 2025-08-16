using System;
using System.Globalization;
using Avalonia.Controls.Converters;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Avalonia.Controls.Primitives.Converters;

public class ContrastBrushConverter : IValueConverter
{
	private ToColorConverter toColorConverter = new ToColorConverter();

	public byte AlphaThreshold { get; set; } = 128;

	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		Color? color = null;
		if (toColorConverter.Convert(value, targetType, parameter, culture) is Color color2)
		{
			Color color3 = color2;
			if (toColorConverter.Convert(parameter, targetType, parameter, culture) is Color value2)
			{
				color = value2;
			}
			if (color3.A < AlphaThreshold && color.HasValue)
			{
				return new SolidColorBrush(color.Value);
			}
			if (ColorHelper.GetRelativeLuminance(color3) <= 0.5)
			{
				return new SolidColorBrush(Colors.White);
			}
			return new SolidColorBrush(Colors.Black);
		}
		return AvaloniaProperty.UnsetValue;
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		return AvaloniaProperty.UnsetValue;
	}
}
