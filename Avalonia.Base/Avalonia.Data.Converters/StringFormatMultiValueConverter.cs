using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Avalonia.Data.Converters;

public class StringFormatMultiValueConverter : IMultiValueConverter
{
	public IMultiValueConverter? Inner { get; }

	public string Format { get; }

	public StringFormatMultiValueConverter(string format, IMultiValueConverter? inner)
	{
		Format = format ?? throw new ArgumentNullException("format");
		Inner = inner;
	}

	public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
	{
		if (Inner != null)
		{
			return string.Format(culture, Format, Inner.Convert(values, targetType, parameter, culture));
		}
		return string.Format(culture, Format, values.ToArray());
	}
}
