using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace Avalonia.Diagnostics.Converters;

internal class EnumToCheckedConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		return object.Equals(value, parameter);
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		bool flag = default(bool);
		int num;
		if (value is bool)
		{
			flag = (bool)value;
			num = 1;
		}
		else
		{
			num = 0;
		}
		if (((uint)num & (flag ? 1u : 0u)) != 0)
		{
			return parameter;
		}
		return BindingOperations.DoNothing;
	}
}
