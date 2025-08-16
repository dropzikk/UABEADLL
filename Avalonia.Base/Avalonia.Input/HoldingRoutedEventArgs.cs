using Avalonia.Interactivity;

namespace Avalonia.Input;

public class HoldingRoutedEventArgs : RoutedEventArgs
{
	public HoldingState HoldingState { get; }

	public Point Position { get; }

	public PointerType PointerType { get; }

	public HoldingRoutedEventArgs(HoldingState holdingState, Point position, PointerType pointerType)
		: base(Gestures.HoldingEvent)
	{
		HoldingState = holdingState;
		Position = position;
		PointerType = pointerType;
	}
}
