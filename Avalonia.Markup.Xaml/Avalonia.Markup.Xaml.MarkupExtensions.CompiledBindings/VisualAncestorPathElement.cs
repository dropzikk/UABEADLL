using System;

namespace Avalonia.Markup.Xaml.MarkupExtensions.CompiledBindings;

internal class VisualAncestorPathElement : ICompiledBindingPathElement, IControlSourceBindingPathElement
{
	public Type? AncestorType { get; }

	public int Level { get; }

	public VisualAncestorPathElement(Type? ancestorType, int level)
	{
		AncestorType = ancestorType;
		Level = level;
	}
}
