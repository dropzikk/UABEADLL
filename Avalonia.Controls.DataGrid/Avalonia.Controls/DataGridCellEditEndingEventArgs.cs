using System.ComponentModel;

namespace Avalonia.Controls;

public class DataGridCellEditEndingEventArgs : CancelEventArgs
{
	public DataGridColumn Column { get; private set; }

	public DataGridEditAction EditAction { get; private set; }

	public Control EditingElement { get; private set; }

	public DataGridRow Row { get; private set; }

	public DataGridCellEditEndingEventArgs(DataGridColumn column, DataGridRow row, Control editingElement, DataGridEditAction editAction)
	{
		Column = column;
		Row = row;
		EditingElement = editingElement;
		EditAction = editAction;
	}
}
