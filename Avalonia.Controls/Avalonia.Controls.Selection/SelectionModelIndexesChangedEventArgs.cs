using System;

namespace Avalonia.Controls.Selection;

public class SelectionModelIndexesChangedEventArgs : EventArgs
{
	public int StartIndex { get; }

	public int Delta { get; }

	public SelectionModelIndexesChangedEventArgs(int startIndex, int delta)
	{
		StartIndex = startIndex;
		Delta = delta;
	}
}
