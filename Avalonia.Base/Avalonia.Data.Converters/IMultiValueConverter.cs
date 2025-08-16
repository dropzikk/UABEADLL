using System;
using System.Collections.Generic;
using System.Globalization;

namespace Avalonia.Data.Converters;

public interface IMultiValueConverter
{
	object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture);
}
