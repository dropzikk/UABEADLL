using System;

namespace Avalonia.LogicalTree;

public class ChildIndexChangedEventArgs : EventArgs
{
	public ChildIndexChangedAction Action { get; }

	public ILogical? Child { get; }

	public int Index { get; }

	public static ChildIndexChangedEventArgs ChildIndexesReset { get; } = new ChildIndexChangedEventArgs(ChildIndexChangedAction.ChildIndexesReset);

	public static ChildIndexChangedEventArgs TotalCountChanged { get; } = new ChildIndexChangedEventArgs(ChildIndexChangedAction.TotalCountChanged);

	public ChildIndexChangedEventArgs(ILogical child, int index)
	{
		Action = ChildIndexChangedAction.ChildIndexChanged;
		Child = child;
		Index = index;
	}

	private ChildIndexChangedEventArgs(ChildIndexChangedAction action)
	{
		Action = action;
		Index = -1;
	}
}
