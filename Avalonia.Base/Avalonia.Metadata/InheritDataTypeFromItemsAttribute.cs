using System;

namespace Avalonia.Metadata;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class InheritDataTypeFromItemsAttribute : Attribute
{
	public string AncestorItemsProperty { get; }

	public Type? AncestorType { get; set; }

	public InheritDataTypeFromItemsAttribute(string ancestorItemsProperty)
	{
		AncestorItemsProperty = ancestorItemsProperty;
	}
}
