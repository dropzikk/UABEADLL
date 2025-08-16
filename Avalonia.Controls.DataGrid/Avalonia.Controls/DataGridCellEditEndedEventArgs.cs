using System;

namespace Avalonia.Controls;

public class DataGridCellEditEndedEventArgs : EventArgs
{
	public DataGridColumn Column { get; private set; }

	public DataGridEditAction EditAction { get; private set; }

	public DataGridRow Row { get; private set; }

	public DataGridCellEditEndedEventArgs(DataGridColumn column, DataGridRow row, DataGridEditAction editAction)
	{
		Column = column;
		Row = row;
		EditAction = editAction;
	}
}
