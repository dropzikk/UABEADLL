using System;
using System.Globalization;
using Avalonia.Controls.Utils;
using Avalonia.Data.Converters;

namespace Avalonia.Controls;

internal class DataGridValueConverter : IValueConverter
{
	public static DataGridValueConverter Instance = new DataGridValueConverter();

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return DefaultValueConverter.Instance.Convert(value, targetType, parameter, culture);
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (targetType != null && targetType.IsNullableType() && value as string == string.Empty)
		{
			return null;
		}
		return DefaultValueConverter.Instance.ConvertBack(value, targetType, parameter, culture);
	}
}
