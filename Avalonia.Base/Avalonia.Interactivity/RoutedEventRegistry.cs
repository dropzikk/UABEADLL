using System;
using System.Collections.Generic;

namespace Avalonia.Interactivity;

public class RoutedEventRegistry
{
	private readonly Dictionary<Type, List<RoutedEvent>> _registeredRoutedEvents = new Dictionary<Type, List<RoutedEvent>>();

	public static RoutedEventRegistry Instance { get; } = new RoutedEventRegistry();

	public void Register(Type type, RoutedEvent @event)
	{
		type = type ?? throw new ArgumentNullException("type");
		@event = @event ?? throw new ArgumentNullException("event");
		if (!_registeredRoutedEvents.TryGetValue(type, out List<RoutedEvent> value))
		{
			value = new List<RoutedEvent>();
			_registeredRoutedEvents.Add(type, value);
		}
		value.Add(@event);
	}

	public IEnumerable<RoutedEvent> GetAllRegistered()
	{
		foreach (List<RoutedEvent> value in _registeredRoutedEvents.Values)
		{
			foreach (RoutedEvent item in value)
			{
				yield return item;
			}
		}
	}

	public IReadOnlyList<RoutedEvent> GetRegistered(Type type)
	{
		type = type ?? throw new ArgumentNullException("type");
		if (_registeredRoutedEvents.TryGetValue(type, out List<RoutedEvent> value))
		{
			return value;
		}
		return Array.Empty<RoutedEvent>();
	}

	public IReadOnlyList<RoutedEvent> GetRegistered<TOwner>()
	{
		return GetRegistered(typeof(TOwner));
	}
}
