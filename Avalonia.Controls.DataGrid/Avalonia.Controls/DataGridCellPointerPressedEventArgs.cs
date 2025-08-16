using System;
using Avalonia.Input;

namespace Avalonia.Controls;

public class DataGridCellPointerPressedEventArgs : EventArgs
{
	public DataGridCell Cell { get; }

	public DataGridRow Row { get; }

	public DataGridColumn Column { get; }

	public PointerPressedEventArgs PointerPressedEventArgs { get; }

	public DataGridCellPointerPressedEventArgs(DataGridCell cell, DataGridRow row, DataGridColumn column, PointerPressedEventArgs e)
	{
		Cell = cell;
		Row = row;
		Column = column;
		PointerPressedEventArgs = e;
	}
}
