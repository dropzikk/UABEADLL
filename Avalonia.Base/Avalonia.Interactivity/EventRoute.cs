using System;
using Avalonia.Collections.Pooled;

namespace Avalonia.Interactivity;

public class EventRoute : IDisposable
{
	private readonly struct RouteItem
	{
		public Interactive Target { get; }

		public Delegate? Handler { get; }

		public Action<Delegate, object, RoutedEventArgs>? Adapter { get; }

		public RoutingStrategies Routes { get; }

		public bool HandledEventsToo { get; }

		public RouteItem(Interactive target, Delegate? handler, Action<Delegate, object, RoutedEventArgs>? adapter, RoutingStrategies routes, bool handledEventsToo)
		{
			Target = target;
			Handler = handler;
			Adapter = adapter;
			Routes = routes;
			HandledEventsToo = handledEventsToo;
		}
	}

	private readonly RoutedEvent _event;

	private PooledList<RouteItem>? _route;

	public bool HasHandlers
	{
		get
		{
			PooledList<RouteItem>? route = _route;
			if (route == null)
			{
				return false;
			}
			return route.Count > 0;
		}
	}

	public EventRoute(RoutedEvent e)
	{
		e = e ?? throw new ArgumentNullException("e");
		_event = e;
		_route = null;
	}

	public void Add(Interactive target, Delegate handler, RoutingStrategies routes, bool handledEventsToo = false, Action<Delegate, object, RoutedEventArgs>? adapter = null)
	{
		target = target ?? throw new ArgumentNullException("target");
		handler = handler ?? throw new ArgumentNullException("handler");
		if (_route == null)
		{
			_route = new PooledList<RouteItem>(16);
		}
		_route.Add(new RouteItem(target, handler, adapter, routes, handledEventsToo));
	}

	public void AddClassHandler(Interactive target)
	{
		target = target ?? throw new ArgumentNullException("target");
		if (_route == null)
		{
			_route = new PooledList<RouteItem>(16);
		}
		_route.Add(new RouteItem(target, null, null, (RoutingStrategies)0, handledEventsToo: false));
	}

	public void RaiseEvent(Interactive source, RoutedEventArgs e)
	{
		source = source ?? throw new ArgumentNullException("source");
		e = e ?? throw new ArgumentNullException("e");
		e.Source = source;
		if (_event.RoutingStrategies == RoutingStrategies.Direct)
		{
			e.Route = RoutingStrategies.Direct;
			RaiseEventImpl(e);
			_event.InvokeRouteFinished(e);
			return;
		}
		if (_event.RoutingStrategies.HasAllFlags(RoutingStrategies.Tunnel))
		{
			e.Route = RoutingStrategies.Tunnel;
			RaiseEventImpl(e);
			_event.InvokeRouteFinished(e);
		}
		if (_event.RoutingStrategies.HasAllFlags(RoutingStrategies.Bubble))
		{
			e.Route = RoutingStrategies.Bubble;
			RaiseEventImpl(e);
			_event.InvokeRouteFinished(e);
		}
	}

	public void Dispose()
	{
		_route?.Dispose();
		_route = null;
	}

	private void RaiseEventImpl(RoutedEventArgs e)
	{
		if (_route == null)
		{
			return;
		}
		Interactive interactive = null;
		int num = 0;
		int num2 = _route.Count;
		int num3 = 1;
		if (e.Route == RoutingStrategies.Tunnel)
		{
			num = num2 - 1;
			num3 = (num2 = -1);
		}
		for (int i = num; i != num2; i += num3)
		{
			RouteItem routeItem = _route[i];
			if (routeItem.Target != interactive)
			{
				_event.InvokeRaised(routeItem.Target, e);
				if (e.Route == RoutingStrategies.Direct && interactive != null)
				{
					break;
				}
				interactive = routeItem.Target;
			}
			if ((object)routeItem.Handler != null && routeItem.Routes.HasAllFlags(e.Route) && (!e.Handled || routeItem.HandledEventsToo))
			{
				if (routeItem.Adapter != null)
				{
					routeItem.Adapter(routeItem.Handler, routeItem.Target, e);
					continue;
				}
				routeItem.Handler.DynamicInvoke(routeItem.Target, e);
			}
		}
	}
}
