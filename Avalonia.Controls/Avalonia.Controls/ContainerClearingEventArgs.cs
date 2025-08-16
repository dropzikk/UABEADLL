using System;

namespace Avalonia.Controls;

public class ContainerClearingEventArgs : EventArgs
{
	public Control Container { get; }

	public ContainerClearingEventArgs(Control container)
	{
		Container = container;
	}
}
