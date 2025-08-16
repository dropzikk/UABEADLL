using Avalonia.Interactivity;

namespace Avalonia.Input;

public class PinchEventArgs : RoutedEventArgs
{
	public double Scale { get; } = 1.0;

	public Point ScaleOrigin { get; }

	public PinchEventArgs(double scale, Point scaleOrigin)
		: base(Gestures.PinchEvent)
	{
		Scale = scale;
		ScaleOrigin = scaleOrigin;
	}
}
