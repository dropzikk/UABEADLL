using System;

namespace Avalonia.Controls;

public class DataGridRowDetailsEventArgs : EventArgs
{
	public Control DetailsElement { get; private set; }

	public DataGridRow Row { get; private set; }

	public DataGridRowDetailsEventArgs(DataGridRow row, Control detailsElement)
	{
		Row = row;
		DetailsElement = detailsElement;
	}
}
