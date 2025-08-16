using System;
using Avalonia.Interactivity;

namespace Avalonia.Controls;

public class DataGridPreparingCellForEditEventArgs : EventArgs
{
	public DataGridColumn Column { get; private set; }

	public Control EditingElement { get; private set; }

	public RoutedEventArgs EditingEventArgs { get; private set; }

	public DataGridRow Row { get; private set; }

	public DataGridPreparingCellForEditEventArgs(DataGridColumn column, DataGridRow row, RoutedEventArgs editingEventArgs, Control editingElement)
	{
		Column = column;
		Row = row;
		EditingEventArgs = editingEventArgs;
		EditingElement = editingElement;
	}
}
