using Avalonia.Interactivity;

namespace Avalonia.Input;

public class ScrollGestureInertiaStartingEventArgs : RoutedEventArgs
{
	public int Id { get; }

	public Vector Inertia { get; }

	internal ScrollGestureInertiaStartingEventArgs(int id, Vector inertia)
		: base(Gestures.ScrollGestureInertiaStartingEvent)
	{
		Id = id;
		Inertia = inertia;
	}
}
