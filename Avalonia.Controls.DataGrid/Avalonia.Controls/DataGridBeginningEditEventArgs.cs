using System.ComponentModel;
using Avalonia.Interactivity;

namespace Avalonia.Controls;

public class DataGridBeginningEditEventArgs : CancelEventArgs
{
	public DataGridColumn Column { get; private set; }

	public RoutedEventArgs EditingEventArgs { get; private set; }

	public DataGridRow Row { get; private set; }

	public DataGridBeginningEditEventArgs(DataGridColumn column, DataGridRow row, RoutedEventArgs editingEventArgs)
	{
		Column = column;
		Row = row;
		EditingEventArgs = editingEventArgs;
	}
}
