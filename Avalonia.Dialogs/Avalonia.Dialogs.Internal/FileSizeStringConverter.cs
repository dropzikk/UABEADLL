using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Utilities;

namespace Avalonia.Dialogs.Internal;

public class FileSizeStringConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value is long num && num > 0)
		{
			return ByteSizeHelper.ToString((ulong)num, separate: true);
		}
		return "";
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
