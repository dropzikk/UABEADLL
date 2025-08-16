using System;

namespace Avalonia.Controls;

public class DataGridRowEditEndedEventArgs : EventArgs
{
	public DataGridEditAction EditAction { get; private set; }

	public DataGridRow Row { get; private set; }

	public DataGridRowEditEndedEventArgs(DataGridRow row, DataGridEditAction editAction)
	{
		Row = row;
		EditAction = editAction;
	}
}
