using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace AvaloniaEdit.CodeCompletion;

public sealed class CollapseIfSingleOverloadConverter : IValueConverter
{
	public static CollapseIfSingleOverloadConverter Instance { get; } = new CollapseIfSingleOverloadConverter();

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is int num)
		{
			return num >= 2;
		}
		return AvaloniaProperty.UnsetValue;
	}

	object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
