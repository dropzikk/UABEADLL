using System;
using System.ComponentModel;

namespace Avalonia.Controls;

public class DataGridColumnEventArgs : HandledEventArgs
{
	public DataGridColumn Column { get; private set; }

	public DataGridColumnEventArgs(DataGridColumn column)
	{
		Column = column ?? throw new ArgumentNullException("column");
	}
}
