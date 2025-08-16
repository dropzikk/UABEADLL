using Avalonia.Interactivity;

namespace Avalonia.Controls;

public class TextChangedEventArgs : RoutedEventArgs
{
	public TextChangedEventArgs(RoutedEvent? routedEvent)
		: base(routedEvent)
	{
	}

	public TextChangedEventArgs(RoutedEvent? routedEvent, Interactive? source)
		: base(routedEvent, source)
	{
	}
}
