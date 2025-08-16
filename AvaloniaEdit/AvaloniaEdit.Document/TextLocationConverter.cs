using System;
using System.ComponentModel;
using System.Globalization;

namespace AvaloniaEdit.Document;

public class TextLocationConverter : TypeConverter
{
	public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
	{
		return sourceType == typeof(string);
	}

	public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
	{
		return destinationType == typeof(TextLocation);
	}

	public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
	{
		string[] array = (value as string)?.Split(';', ',');
		if (array != null && array.Length == 2)
		{
			return new TextLocation(int.Parse(array[0], culture), int.Parse(array[1], culture));
		}
		throw new InvalidOperationException();
	}

	public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
	{
		if (value is TextLocation textLocation && destinationType == typeof(string))
		{
			return textLocation.Line.ToString(culture) + ";" + textLocation.Column.ToString(culture);
		}
		throw new InvalidOperationException();
	}
}
