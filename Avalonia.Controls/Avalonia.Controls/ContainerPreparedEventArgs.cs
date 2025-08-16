using System;

namespace Avalonia.Controls;

public class ContainerPreparedEventArgs : EventArgs
{
	public Control Container { get; }

	public int Index { get; }

	public ContainerPreparedEventArgs(Control container, int index)
	{
		Container = container;
		Index = index;
	}
}
