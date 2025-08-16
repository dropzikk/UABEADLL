using System;
using System.Globalization;

namespace Avalonia.Data.Converters;

public class StringFormatValueConverter : IValueConverter
{
	public IValueConverter? Inner { get; }

	public string Format { get; }

	public StringFormatValueConverter(string format, IValueConverter? inner)
	{
		Format = format ?? throw new ArgumentNullException("format");
		Inner = inner;
	}

	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		value = Inner?.Convert(value, targetType, parameter, culture) ?? value;
		string text = Format;
		if (!text.Contains('{'))
		{
			text = "{0:" + text + "}";
		}
		return string.Format(culture, text, value);
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		throw new NotSupportedException("Two way bindings are not supported with a string format");
	}
}
