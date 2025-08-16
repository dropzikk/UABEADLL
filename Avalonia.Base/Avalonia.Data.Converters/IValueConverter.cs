using System;
using System.Globalization;

namespace Avalonia.Data.Converters;

public interface IValueConverter
{
	object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture);

	object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture);
}
