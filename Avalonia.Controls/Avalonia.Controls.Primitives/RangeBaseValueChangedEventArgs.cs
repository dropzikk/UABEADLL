using Avalonia.Interactivity;

namespace Avalonia.Controls.Primitives;

public class RangeBaseValueChangedEventArgs : RoutedEventArgs
{
	public double OldValue { get; init; }

	public double NewValue { get; init; }

	public RangeBaseValueChangedEventArgs(double oldValue, double newValue, RoutedEvent? routedEvent)
		: base(routedEvent)
	{
		OldValue = oldValue;
		NewValue = newValue;
	}

	public RangeBaseValueChangedEventArgs(double oldValue, double newValue, RoutedEvent? routedEvent, object? source)
		: base(routedEvent, source)
	{
		OldValue = oldValue;
		NewValue = newValue;
	}
}
