using System.ComponentModel;

namespace Avalonia.Controls;

public class WindowClosingEventArgs : CancelEventArgs
{
	public WindowCloseReason CloseReason { get; }

	public bool IsProgrammatic { get; }

	internal WindowClosingEventArgs(WindowCloseReason reason, bool isProgrammatic)
	{
		CloseReason = reason;
		IsProgrammatic = isProgrammatic;
	}
}
