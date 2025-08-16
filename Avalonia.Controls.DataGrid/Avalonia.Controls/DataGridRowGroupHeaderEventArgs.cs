using System;

namespace Avalonia.Controls;

public class DataGridRowGroupHeaderEventArgs : EventArgs
{
	public DataGridRowGroupHeader RowGroupHeader { get; private set; }

	public DataGridRowGroupHeaderEventArgs(DataGridRowGroupHeader rowGroupHeader)
	{
		RowGroupHeader = rowGroupHeader;
	}
}
