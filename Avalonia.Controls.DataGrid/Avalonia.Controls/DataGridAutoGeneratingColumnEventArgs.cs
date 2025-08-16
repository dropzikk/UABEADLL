using System;
using System.ComponentModel;

namespace Avalonia.Controls;

public class DataGridAutoGeneratingColumnEventArgs : CancelEventArgs
{
	public DataGridColumn Column { get; set; }

	public string PropertyName { get; private set; }

	public Type PropertyType { get; private set; }

	public DataGridAutoGeneratingColumnEventArgs(string propertyName, Type propertyType, DataGridColumn column)
	{
		Column = column;
		PropertyName = propertyName;
		PropertyType = propertyType;
	}
}
