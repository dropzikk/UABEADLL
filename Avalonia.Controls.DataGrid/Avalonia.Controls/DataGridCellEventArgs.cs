using System;

namespace Avalonia.Controls;

internal class DataGridCellEventArgs : EventArgs
{
	internal DataGridCell Cell { get; private set; }

	internal DataGridCellEventArgs(DataGridCell dataGridCell)
	{
		Cell = dataGridCell;
	}
}
