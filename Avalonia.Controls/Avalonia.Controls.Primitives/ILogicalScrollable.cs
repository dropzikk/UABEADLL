using System;
using Avalonia.Input;

namespace Avalonia.Controls.Primitives;

public interface ILogicalScrollable : IScrollable
{
	bool CanHorizontallyScroll { get; set; }

	bool CanVerticallyScroll { get; set; }

	bool IsLogicalScrollEnabled { get; }

	Size ScrollSize { get; }

	Size PageScrollSize { get; }

	event EventHandler? ScrollInvalidated;

	bool BringIntoView(Control target, Rect targetRect);

	Control? GetControlInDirection(NavigationDirection direction, Control? from);

	void RaiseScrollInvalidated(EventArgs e);
}
