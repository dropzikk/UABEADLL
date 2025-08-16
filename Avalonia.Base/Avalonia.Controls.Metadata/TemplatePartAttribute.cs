using System;

namespace Avalonia.Controls.Metadata;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class TemplatePartAttribute : Attribute
{
	public string Name { get; set; }

	public Type Type { get; set; }

	public TemplatePartAttribute()
	{
		Name = string.Empty;
		Type = typeof(object);
	}

	public TemplatePartAttribute(string name, Type type)
	{
		Name = name;
		Type = type;
	}
}
