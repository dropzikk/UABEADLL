using System;
using System.ComponentModel;
using System.Globalization;

namespace Avalonia.Media;

public class BrushConverter : TypeConverter
{
	public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
	{
		return sourceType == typeof(string);
	}

	public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object? value)
	{
		if (!(value is string s))
		{
			return null;
		}
		return Brush.Parse(s);
	}
}
