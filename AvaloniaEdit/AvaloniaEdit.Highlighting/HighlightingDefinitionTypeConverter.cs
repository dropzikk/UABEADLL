using System;
using System.ComponentModel;
using System.Globalization;

namespace AvaloniaEdit.Highlighting;

public sealed class HighlightingDefinitionTypeConverter : TypeConverter
{
	public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
	{
		return sourceType == typeof(string);
	}

	public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
	{
		if (!(value is string name))
		{
			return null;
		}
		return HighlightingManager.Instance.GetDefinition(name);
	}

	public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
	{
		return destinationType == typeof(string);
	}

	public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
	{
		if (value is IHighlightingDefinition highlightingDefinition && destinationType == typeof(string))
		{
			return highlightingDefinition.Name;
		}
		return null;
	}
}
