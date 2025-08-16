using Avalonia.Interactivity;

namespace Avalonia.Controls;

public class ScrollChangedEventArgs : RoutedEventArgs
{
	public Vector ExtentDelta { get; }

	public Vector OffsetDelta { get; }

	public Vector ViewportDelta { get; }

	public ScrollChangedEventArgs(Vector extentDelta, Vector offsetDelta, Vector viewportDelta)
		: this(ScrollViewer.ScrollChangedEvent, extentDelta, offsetDelta, viewportDelta)
	{
	}

	public ScrollChangedEventArgs(RoutedEvent routedEvent, Vector extentDelta, Vector offsetDelta, Vector viewportDelta)
		: base(routedEvent)
	{
		ExtentDelta = extentDelta;
		OffsetDelta = offsetDelta;
		ViewportDelta = viewportDelta;
	}
}
