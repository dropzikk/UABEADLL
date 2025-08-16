using System;
using System.Collections.ObjectModel;
using Avalonia.Diagnostics.Models;
using Avalonia.Interactivity;

namespace Avalonia.Diagnostics.ViewModels;

internal class FiredEvent : ViewModelBase
{
	private readonly RoutedEventArgs _eventArgs;

	private EventChainLink? _handledBy;

	public DateTime TriggerTime { get; }

	public RoutedEvent Event => _eventArgs.RoutedEvent;

	public bool IsHandled => HandledBy?.Handled ?? false;

	public ObservableCollection<EventChainLink> EventChain { get; } = new ObservableCollection<EventChainLink>();

	public string DisplayText
	{
		get
		{
			if (IsHandled)
			{
				return Event.Name + " on " + Originator.HandlerName + ";" + Environment.NewLine + $"strategies: {Event.RoutingStrategies}; handled by: {HandledBy.HandlerName}";
			}
			return $"{Event.Name} on {Originator.HandlerName}; strategies: {Event.RoutingStrategies}";
		}
	}

	public EventChainLink Originator { get; }

	public EventChainLink? HandledBy
	{
		get
		{
			return _handledBy;
		}
		set
		{
			if (_handledBy != value)
			{
				_handledBy = value;
				RaisePropertyChanged("HandledBy");
				RaisePropertyChanged("IsHandled");
				RaisePropertyChanged("DisplayText");
			}
		}
	}

	public FiredEvent(RoutedEventArgs eventArgs, EventChainLink originator, DateTime triggerTime)
	{
		_eventArgs = eventArgs ?? throw new ArgumentNullException("eventArgs");
		Originator = originator ?? throw new ArgumentNullException("originator");
		AddToChain(originator);
		TriggerTime = triggerTime;
	}

	public bool IsPartOfSameEventChain(RoutedEventArgs e)
	{
		return e == _eventArgs;
	}

	public void AddToChain(EventChainLink link)
	{
		if (EventChain.Count > 0 && EventChain[EventChain.Count - 1].Route != link.Route)
		{
			link.BeginsNewRoute = true;
		}
		EventChain.Add(link);
		if (HandledBy == null && link.Handled)
		{
			HandledBy = link;
		}
	}
}
