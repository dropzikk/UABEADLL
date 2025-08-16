using System;

namespace Avalonia.LogicalTree;

public interface IChildIndexProvider
{
	event EventHandler<ChildIndexChangedEventArgs>? ChildIndexChanged;

	int GetChildIndex(ILogical child);

	bool TryGetTotalCount(out int count);
}
