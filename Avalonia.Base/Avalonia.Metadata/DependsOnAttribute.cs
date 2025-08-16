using System;

namespace Avalonia.Metadata;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
public sealed class DependsOnAttribute : Attribute
{
	public string Name { get; }

	public DependsOnAttribute(string propertyName)
	{
		Name = propertyName;
	}
}
