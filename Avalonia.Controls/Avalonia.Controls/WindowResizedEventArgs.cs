using System;

namespace Avalonia.Controls;

public class WindowResizedEventArgs : EventArgs
{
	public Size ClientSize { get; }

	public WindowResizeReason Reason { get; }

	internal WindowResizedEventArgs(Size clientSize, WindowResizeReason reason)
	{
		ClientSize = clientSize;
		Reason = reason;
	}
}
