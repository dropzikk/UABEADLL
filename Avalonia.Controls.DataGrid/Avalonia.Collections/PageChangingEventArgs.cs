using System.ComponentModel;

namespace Avalonia.Collections;

public sealed class PageChangingEventArgs : CancelEventArgs
{
	public int NewPageIndex { get; private set; }

	public PageChangingEventArgs(int newPageIndex)
	{
		NewPageIndex = newPageIndex;
	}
}
