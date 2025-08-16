using System;

namespace Avalonia.Controls.Primitives;

public class ScrollEventArgs : EventArgs
{
	public double NewValue { get; private set; }

	public ScrollEventType ScrollEventType { get; private set; }

	public ScrollEventArgs(ScrollEventType eventType, double newValue)
	{
		ScrollEventType = eventType;
		NewValue = newValue;
	}
}
