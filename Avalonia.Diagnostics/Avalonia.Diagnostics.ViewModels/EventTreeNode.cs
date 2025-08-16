using System;
using Avalonia.Diagnostics.Models;
using Avalonia.Diagnostics.Views;
using Avalonia.Interactivity;
using Avalonia.Reactive;
using Avalonia.Threading;

namespace Avalonia.Diagnostics.ViewModels;

internal class EventTreeNode : EventTreeNodeBase
{
	private readonly EventsPageViewModel _parentViewModel;

	private bool _isRegistered;

	private FiredEvent? _currentEvent;

	public RoutedEvent Event { get; }

	public override bool? IsEnabled
	{
		get
		{
			return base.IsEnabled;
		}
		set
		{
			if (base.IsEnabled == value)
			{
				return;
			}
			base.IsEnabled = value;
			UpdateTracker();
			if (base.Parent != null && _updateParent)
			{
				try
				{
					base.Parent._updateChildren = false;
					base.Parent.UpdateChecked();
				}
				finally
				{
					base.Parent._updateChildren = true;
				}
			}
		}
	}

	public EventTreeNode(EventOwnerTreeNode parent, RoutedEvent @event, EventsPageViewModel vm)
		: base(parent, @event.Name)
	{
		Event = @event ?? throw new ArgumentNullException("event");
		_parentViewModel = vm ?? throw new ArgumentNullException("vm");
	}

	private void UpdateTracker()
	{
		if (IsEnabled == true && !_isRegistered)
		{
			RoutingStrategies routes = RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble;
			Event.AddClassHandler(typeof(object), HandleEvent, routes, handledEventsToo: true);
			Event.RouteFinished.Subscribe(HandleRouteFinished);
			_isRegistered = true;
		}
	}

	private void HandleEvent(object? sender, RoutedEventArgs e)
	{
		object s;
		bool handled;
		RoutingStrategies route;
		DateTime triggerTime;
		if (_isRegistered && IsEnabled != false && (!(sender is Visual v) || !BelongsToDevTool(v)))
		{
			s = sender;
			handled = e.Handled;
			route = e.Route;
			triggerTime = DateTime.Now;
			if (!Dispatcher.UIThread.CheckAccess())
			{
				Dispatcher.UIThread.Post(handler);
			}
			else
			{
				handler();
			}
		}
		void handler()
		{
			if (_currentEvent == null || !_currentEvent.IsPartOfSameEventChain(e))
			{
				_currentEvent = new FiredEvent(e, new EventChainLink(s, handled, route), triggerTime);
				_parentViewModel.RecordedEvents.Add(_currentEvent);
				while (_parentViewModel.RecordedEvents.Count > 100)
				{
					_parentViewModel.RecordedEvents.RemoveAt(0);
				}
			}
			else
			{
				_currentEvent.AddToChain(new EventChainLink(s, handled, route));
			}
		}
	}

	private void HandleRouteFinished(RoutedEventArgs e)
	{
		bool handled;
		if (_isRegistered && IsEnabled != false && (!(e.Source is Visual v) || !BelongsToDevTool(v)))
		{
			_ = e.Source;
			handled = e.Handled;
			_ = e.Route;
			if (!Dispatcher.UIThread.CheckAccess())
			{
				Dispatcher.UIThread.Post(handler);
			}
			else
			{
				handler();
			}
		}
		void handler()
		{
			if (_currentEvent != null && handled)
			{
				int index = _currentEvent.EventChain.Count - 1;
				EventChainLink eventChainLink = _currentEvent.EventChain[index];
				eventChainLink.Handled = true;
				FiredEvent currentEvent = _currentEvent;
				if (currentEvent.HandledBy == null)
				{
					EventChainLink eventChainLink3 = (currentEvent.HandledBy = eventChainLink);
				}
			}
		}
	}

	private static bool BelongsToDevTool(Visual v)
	{
		for (Visual visual = v; visual != null; visual = visual.VisualParent)
		{
			if (visual is MainView || visual is MainWindow)
			{
				return true;
			}
		}
		return false;
	}
}
