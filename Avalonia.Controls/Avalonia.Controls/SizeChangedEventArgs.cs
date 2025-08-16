using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Utilities;

namespace Avalonia.Controls;

public class SizeChangedEventArgs : RoutedEventArgs
{
	public bool HeightChanged => !MathUtilities.AreClose(NewSize.Height, PreviousSize.Height, LayoutHelper.LayoutEpsilon);

	public Size NewSize { get; init; }

	public Size PreviousSize { get; init; }

	public bool WidthChanged => !MathUtilities.AreClose(NewSize.Width, PreviousSize.Width, LayoutHelper.LayoutEpsilon);

	public SizeChangedEventArgs(RoutedEvent? routedEvent)
		: base(routedEvent)
	{
	}

	public SizeChangedEventArgs(RoutedEvent? routedEvent, object? source)
		: base(routedEvent, source)
	{
	}

	public SizeChangedEventArgs(RoutedEvent? routedEvent, object? source, Size previousSize, Size newSize)
		: base(routedEvent, source)
	{
		PreviousSize = previousSize;
		NewSize = newSize;
	}
}
