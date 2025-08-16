using Avalonia.Interactivity;

namespace Avalonia.Controls;

public class TextChangingEventArgs : RoutedEventArgs
{
	public TextChangingEventArgs(RoutedEvent? routedEvent)
		: base(routedEvent)
	{
	}

	public TextChangingEventArgs(RoutedEvent? routedEvent, Interactive? source)
		: base(routedEvent, source)
	{
	}
}
