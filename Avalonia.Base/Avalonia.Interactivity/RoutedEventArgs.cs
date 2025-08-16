using System;

namespace Avalonia.Interactivity;

public class RoutedEventArgs : EventArgs
{
	public bool Handled { get; set; }

	public RoutedEvent? RoutedEvent { get; set; }

	public RoutingStrategies Route { get; set; }

	public object? Source { get; set; }

	public RoutedEventArgs()
	{
	}

	public RoutedEventArgs(RoutedEvent? routedEvent)
	{
		RoutedEvent = routedEvent;
	}

	public RoutedEventArgs(RoutedEvent? routedEvent, object? source)
	{
		RoutedEvent = routedEvent;
		Source = source;
	}
}
