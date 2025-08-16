using System.ComponentModel;

namespace Avalonia.Controls;

public class DataGridColumnReorderingEventArgs : CancelEventArgs
{
	public DataGridColumn Column { get; private set; }

	public Control DragIndicator { get; set; }

	public Control DropLocationIndicator { get; set; }

	public DataGridColumnReorderingEventArgs(DataGridColumn dataGridColumn)
	{
		Column = dataGridColumn;
	}
}
