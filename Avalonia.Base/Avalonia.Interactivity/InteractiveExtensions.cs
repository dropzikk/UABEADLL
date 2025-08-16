using System;
using Avalonia.Reactive;

namespace Avalonia.Interactivity;

public static class InteractiveExtensions
{
	public static IDisposable AddDisposableHandler<TEventArgs>(this Interactive o, RoutedEvent<TEventArgs> routedEvent, EventHandler<TEventArgs> handler, RoutingStrategies routes = RoutingStrategies.Direct | RoutingStrategies.Bubble, bool handledEventsToo = false) where TEventArgs : RoutedEventArgs
	{
		o.AddHandler(routedEvent, handler, routes, handledEventsToo);
		return Disposable.Create((o, handler, routedEvent), delegate((Interactive instance, EventHandler<TEventArgs> handler, RoutedEvent<TEventArgs> routedEvent) state)
		{
			state.instance.RemoveHandler(state.routedEvent, state.handler);
		});
	}

	public static Interactive? GetInteractiveParent(this Interactive o)
	{
		return o.InteractiveParent;
	}

	public static IObservable<TEventArgs> GetObservable<TEventArgs>(this Interactive o, RoutedEvent<TEventArgs> routedEvent, RoutingStrategies routes = RoutingStrategies.Direct | RoutingStrategies.Bubble, bool handledEventsToo = false) where TEventArgs : RoutedEventArgs
	{
		o = o ?? throw new ArgumentNullException("o");
		routedEvent = routedEvent ?? throw new ArgumentNullException("routedEvent");
		return Observable.Create((IObserver<TEventArgs> x) => o.AddDisposableHandler(routedEvent, delegate(object? _, TEventArgs e)
		{
			x.OnNext(e);
		}, routes, handledEventsToo));
	}
}
