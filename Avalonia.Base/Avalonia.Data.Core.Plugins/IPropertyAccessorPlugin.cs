using System;
using System.Diagnostics.CodeAnalysis;

namespace Avalonia.Data.Core.Plugins;

public interface IPropertyAccessorPlugin
{
	[RequiresUnreferencedCode("PropertyAccessors might require unreferenced code.")]
	bool Match(object obj, string propertyName);

	[RequiresUnreferencedCode("PropertyAccessors might require unreferenced code.")]
	IPropertyAccessor? Start(WeakReference<object?> reference, string propertyName);
}
