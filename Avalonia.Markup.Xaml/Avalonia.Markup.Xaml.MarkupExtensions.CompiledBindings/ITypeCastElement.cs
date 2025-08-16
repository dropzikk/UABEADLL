using System;

namespace Avalonia.Markup.Xaml.MarkupExtensions.CompiledBindings;

internal interface ITypeCastElement : ICompiledBindingPathElement
{
	Type Type { get; }

	Func<object?, object?> Cast { get; }
}
