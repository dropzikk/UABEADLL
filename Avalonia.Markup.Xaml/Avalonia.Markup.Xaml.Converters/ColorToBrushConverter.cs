using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace Avalonia.Markup.Xaml.Converters;

public class ColorToBrushConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		return Convert(value, targetType);
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		return ConvertBack(value, targetType);
	}

	public static object? Convert(object? value, Type? targetType)
	{
		if (targetType == typeof(IBrush) && value is Color color)
		{
			return new ImmutableSolidColorBrush(color);
		}
		return value;
	}

	public static object? ConvertBack(object? value, Type? targetType)
	{
		if (targetType == typeof(Color) && value is ISolidColorBrush solidColorBrush)
		{
			return solidColorBrush.Color;
		}
		return value;
	}
}
