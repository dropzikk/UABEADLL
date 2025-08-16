using System;
using System.Diagnostics.CodeAnalysis;

namespace Avalonia.Data.Core.Plugins;

public interface IDataValidationPlugin
{
	[RequiresUnreferencedCode("DataValidationPlugin might require unreferenced code.")]
	bool Match(WeakReference<object?> reference, string memberName);

	[RequiresUnreferencedCode("DataValidationPlugin might require unreferenced code.")]
	IPropertyAccessor Start(WeakReference<object?> reference, string propertyName, IPropertyAccessor inner);
}
