using System;
using System.Diagnostics.CodeAnalysis;

namespace Avalonia.Data.Core.Plugins;

public interface IStreamPlugin
{
	[RequiresUnreferencedCode("StreamPlugin might require unreferenced code.")]
	bool Match(WeakReference<object?> reference);

	[RequiresUnreferencedCode("StreamPlugin might require unreferenced code.")]
	IObservable<object?> Start(WeakReference<object?> reference);
}
