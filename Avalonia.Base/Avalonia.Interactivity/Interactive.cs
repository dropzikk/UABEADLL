using System;
using System.Collections.Generic;
using Avalonia.Layout;

namespace Avalonia.Interactivity;

public class Interactive : Layoutable
{
	private readonly struct EventSubscription
	{
		public Action<Delegate, object, RoutedEventArgs>? InvokeAdapter { get; }

		public Delegate Handler { get; }

		public RoutingStrategies Routes { get; }

		public bool HandledEventsToo { get; }

		public EventSubscription(Delegate handler, RoutingStrategies routes, bool handledEventsToo, Action<Delegate, object, RoutedEventArgs>? invokeAdapter = null)
		{
			Handler = handler;
			Routes = routes;
			HandledEventsToo = handledEventsToo;
			InvokeAdapter = invokeAdapter;
		}
	}

	private Dictionary<RoutedEvent, List<EventSubscription>>? _eventHandlers;

	internal virtual Interactive? InteractiveParent => base.VisualParent as Interactive;

	public void AddHandler(RoutedEvent routedEvent, Delegate handler, RoutingStrategies routes = RoutingStrategies.Direct | RoutingStrategies.Bubble, bool handledEventsToo = false)
	{
		routedEvent = routedEvent ?? throw new ArgumentNullException("routedEvent");
		handler = handler ?? throw new ArgumentNullException("handler");
		EventSubscription subscription = new EventSubscription(handler, routes, handledEventsToo);
		AddEventSubscription(routedEvent, subscription);
	}

	public void AddHandler<TEventArgs>(RoutedEvent<TEventArgs> routedEvent, EventHandler<TEventArgs>? handler, RoutingStrategies routes = RoutingStrategies.Direct | RoutingStrategies.Bubble, bool handledEventsToo = false) where TEventArgs : RoutedEventArgs
	{
		routedEvent = routedEvent ?? throw new ArgumentNullException("routedEvent");
		if (handler != null)
		{
			EventSubscription subscription = new EventSubscription(handler, routes, handledEventsToo, delegate(Delegate baseHandler, object sender, RoutedEventArgs args)
			{
				InvokeAdapter(baseHandler, sender, args);
			});
			AddEventSubscription(routedEvent, subscription);
		}
		static void InvokeAdapter(Delegate baseHandler, object sender, RoutedEventArgs args)
		{
			EventHandler<TEventArgs> obj = (EventHandler<TEventArgs>)baseHandler;
			TEventArgs e = (TEventArgs)args;
			obj(sender, e);
		}
	}

	public void RemoveHandler(RoutedEvent routedEvent, Delegate handler)
	{
		routedEvent = routedEvent ?? throw new ArgumentNullException("routedEvent");
		handler = handler ?? throw new ArgumentNullException("handler");
		if (_eventHandlers == null || !_eventHandlers.TryGetValue(routedEvent, out List<EventSubscription> value))
		{
			return;
		}
		for (int num = value.Count - 1; num >= 0; num--)
		{
			if (value[num].Handler == handler)
			{
				value.RemoveAt(num);
			}
		}
	}

	public void RemoveHandler<TEventArgs>(RoutedEvent<TEventArgs> routedEvent, EventHandler<TEventArgs>? handler) where TEventArgs : RoutedEventArgs
	{
		if (handler != null)
		{
			RemoveHandler((RoutedEvent)routedEvent, (Delegate)handler);
		}
	}

	public void RaiseEvent(RoutedEventArgs e)
	{
		e = e ?? throw new ArgumentNullException("e");
		if (e.RoutedEvent == null)
		{
			throw new ArgumentException("Cannot raise an event whose RoutedEvent is null.");
		}
		using EventRoute eventRoute = BuildEventRoute(e.RoutedEvent);
		eventRoute.RaiseEvent(this, e);
	}

	protected EventRoute BuildEventRoute(RoutedEvent e)
	{
		e = e ?? throw new ArgumentNullException("e");
		EventRoute eventRoute = new EventRoute(e);
		bool hasRaisedSubscriptions = e.HasRaisedSubscriptions;
		if (e.RoutingStrategies.HasAllFlags(RoutingStrategies.Bubble) || e.RoutingStrategies.HasAllFlags(RoutingStrategies.Tunnel))
		{
			for (Interactive interactive = this; interactive != null; interactive = interactive.InteractiveParent)
			{
				if (hasRaisedSubscriptions)
				{
					eventRoute.AddClassHandler(interactive);
				}
				interactive.AddToEventRoute(e, eventRoute);
			}
		}
		else
		{
			if (hasRaisedSubscriptions)
			{
				eventRoute.AddClassHandler(this);
			}
			AddToEventRoute(e, eventRoute);
		}
		return eventRoute;
	}

	private void AddEventSubscription(RoutedEvent routedEvent, EventSubscription subscription)
	{
		if (_eventHandlers == null)
		{
			_eventHandlers = new Dictionary<RoutedEvent, List<EventSubscription>>();
		}
		if (!_eventHandlers.TryGetValue(routedEvent, out List<EventSubscription> value))
		{
			value = new List<EventSubscription>();
			_eventHandlers.Add(routedEvent, value);
		}
		value.Add(subscription);
	}

	private void AddToEventRoute(RoutedEvent routedEvent, EventRoute route)
	{
		routedEvent = routedEvent ?? throw new ArgumentNullException("routedEvent");
		route = route ?? throw new ArgumentNullException("route");
		if (_eventHandlers == null || !_eventHandlers.TryGetValue(routedEvent, out List<EventSubscription> value))
		{
			return;
		}
		foreach (EventSubscription item in value)
		{
			route.Add(this, item.Handler, item.Routes, item.HandledEventsToo, item.InvokeAdapter);
		}
	}
}
