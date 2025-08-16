using System;

namespace Avalonia.Markup.Xaml;

[AttributeUsage(AttributeTargets.Property)]
public sealed class ConstructorArgumentAttribute : Attribute
{
	public ConstructorArgumentAttribute(string name)
	{
	}
}
