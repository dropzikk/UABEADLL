using System;
using System.ComponentModel;
using System.Globalization;
using Avalonia.Media.Transformation;

namespace Avalonia.Media;

public class TransformConverter : TypeConverter
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
		return TransformOperations.Parse(s);
	}
}
