using System;

namespace Avalonia.Markup.Xaml.MarkupExtensions.CompiledBindings;

internal class TypeCastPathElement<T> : ITypeCastElement, ICompiledBindingPathElement
{
	public Type Type => typeof(T);

	public Func<object?, object?> Cast => TryCast;

	private static object? TryCast(object? obj)
	{
		if (obj is T val)
		{
			return val;
		}
		return null;
	}

	public override string ToString()
	{
		return "(" + Type.FullName + ")";
	}
}
