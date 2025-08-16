using System;

namespace Avalonia.Controls;

public class ContainerIndexChangedEventArgs : EventArgs
{
	public Control Container { get; }

	public int NewIndex { get; }

	public int OldIndex { get; }

	public ContainerIndexChangedEventArgs(Control container, int oldIndex, int newIndex)
	{
		Container = container;
		OldIndex = oldIndex;
		NewIndex = newIndex;
	}
}
