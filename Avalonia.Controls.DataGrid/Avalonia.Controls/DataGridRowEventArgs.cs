using System;

namespace Avalonia.Controls;

public class DataGridRowEventArgs : EventArgs
{
	public DataGridRow Row { get; private set; }

	public DataGridRowEventArgs(DataGridRow dataGridRow)
	{
		Row = dataGridRow;
	}
}
