using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace Avalonia.Controls.Converters;

public class EnumToBoolConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value == null && parameter == null)
		{
			return true;
		}
		if (value == null || parameter == null)
		{
			return false;
		}
		return value.Equals(parameter);
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value is bool && (bool)value)
		{
			return parameter;
		}
		return BindingOperations.DoNothing;
	}
}
