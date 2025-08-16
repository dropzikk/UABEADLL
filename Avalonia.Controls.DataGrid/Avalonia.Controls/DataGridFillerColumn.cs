using Avalonia.Controls.Utils;
using Avalonia.Interactivity;

namespace Avalonia.Controls;

internal class DataGridFillerColumn : DataGridColumn
{
	internal double FillerWidth { get; set; }

	internal bool IsActive => FillerWidth > 0.0;

	internal bool IsRepresented { get; set; }

	public DataGridFillerColumn(DataGrid owningGrid)
	{
		IsReadOnly = true;
		base.OwningGrid = owningGrid;
		base.MinWidth = 0.0;
		base.MaxWidth = 2147483647.0;
	}

	internal override DataGridColumnHeader CreateHeader()
	{
		DataGridColumnHeader dataGridColumnHeader = base.CreateHeader();
		if (dataGridColumnHeader != null)
		{
			dataGridColumnHeader.IsEnabled = false;
		}
		return dataGridColumnHeader;
	}

	protected override Control GenerateElement(DataGridCell cell, object dataItem)
	{
		return null;
	}

	protected override Control GenerateEditingElement(DataGridCell cell, object dataItem, out ICellEditBinding editBinding)
	{
		editBinding = null;
		return null;
	}

	protected override object PrepareCellForEdit(Control editingElement, RoutedEventArgs editingEventArgs)
	{
		return null;
	}
}
