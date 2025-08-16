using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Data.Core.Plugins;
using Avalonia.Reactive;

namespace Avalonia.Markup.Xaml.MarkupExtensions.CompiledBindings;

internal class ObservableStreamPlugin<T> : IStreamPlugin
{
	[RequiresUnreferencedCode("StreamPlugin might require unreferenced code.")]
	public bool Match(WeakReference<object?> reference)
	{
		if (reference.TryGetTarget(out object target))
		{
			return target is IObservable<T>;
		}
		return false;
	}

	[RequiresUnreferencedCode("StreamPlugin might require unreferenced code.")]
	public IObservable<object?> Start(WeakReference<object?> reference)
	{
		if (!reference.TryGetTarget(out object target) || !(target is IObservable<T> source))
		{
			return Observable.Empty<object>();
		}
		if (target is IObservable<object> result)
		{
			return result;
		}
		return source.Select((Func<T, object>)((T x) => x));
	}
}
