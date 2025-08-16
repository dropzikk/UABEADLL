using System;

namespace Avalonia.Utilities;

public sealed class WeakEventSubscriber<TEventArgs> : IWeakEventSubscriber<TEventArgs> where TEventArgs : EventArgs
{
	public event Action<object?, WeakEvent, TEventArgs>? Event;

	void IWeakEventSubscriber<TEventArgs>.OnEvent(object? sender, WeakEvent ev, TEventArgs e)
	{
		this.Event?.Invoke(sender, ev, e);
	}
}
