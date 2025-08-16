using System.ComponentModel;

namespace Avalonia.Controls;

public class DataGridRowEditEndingEventArgs : CancelEventArgs
{
	public DataGridEditAction EditAction { get; private set; }

	public DataGridRow Row { get; private set; }

	public DataGridRowEditEndingEventArgs(DataGridRow row, DataGridEditAction editAction)
	{
		Row = row;
		EditAction = editAction;
	}
}
