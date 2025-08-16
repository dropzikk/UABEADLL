using System;

namespace Avalonia.Metadata;

[AttributeUsage(AttributeTargets.Property)]
public sealed class TemplateContentAttribute : Attribute
{
	public Type? TemplateResultType { get; set; }
}
