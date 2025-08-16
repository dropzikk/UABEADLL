using Avalonia.Interactivity;

namespace Avalonia.Input;

public class ScrollGestureEndedEventArgs : RoutedEventArgs
{
	public int Id { get; }

	public ScrollGestureEndedEventArgs(int id)
		: base(Gestures.ScrollGestureEndedEvent)
	{
		Id = id;
	}
}
