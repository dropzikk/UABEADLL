using Avalonia.Interactivity;

namespace Avalonia.Input;

public class ScrollGestureEventArgs : RoutedEventArgs
{
	private static int _nextId = 1;

	public int Id { get; }

	public Vector Delta { get; }

	public bool ShouldEndScrollGesture { get; set; }

	public static int GetNextFreeId()
	{
		return _nextId++;
	}

	public ScrollGestureEventArgs(int id, Vector delta)
		: base(Gestures.ScrollGestureEvent)
	{
		Id = id;
		Delta = delta;
	}
}
