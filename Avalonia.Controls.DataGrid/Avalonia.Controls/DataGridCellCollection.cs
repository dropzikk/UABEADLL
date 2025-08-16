using System;
using System.Collections;
using System.Collections.Generic;

namespace Avalonia.Controls;

internal class DataGridCellCollection
{
	private List<DataGridCell> _cells;

	private DataGridRow _owningRow;

	public int Count => _cells.Count;

	public DataGridCell this[int index]
	{
		get
		{
			if (index < 0 || index >= _cells.Count)
			{
				throw DataGridError.DataGrid.ValueMustBeBetween("index", "Index", 0, lowInclusive: true, _cells.Count, highInclusive: false);
			}
			return _cells[index];
		}
	}

	internal event EventHandler<DataGridCellEventArgs> CellAdded;

	internal event EventHandler<DataGridCellEventArgs> CellRemoved;

	public DataGridCellCollection(DataGridRow owningRow)
	{
		_owningRow = owningRow;
		_cells = new List<DataGridCell>();
	}

	public IEnumerator GetEnumerator()
	{
		return _cells.GetEnumerator();
	}

	public void Insert(int cellIndex, DataGridCell cell)
	{
		cell.OwningRow = _owningRow;
		_cells.Insert(cellIndex, cell);
		this.CellAdded?.Invoke(this, new DataGridCellEventArgs(cell));
	}

	public void RemoveAt(int cellIndex)
	{
		DataGridCell dataGridCell = _cells[cellIndex];
		_cells.RemoveAt(cellIndex);
		dataGridCell.OwningRow = null;
		this.CellRemoved?.Invoke(this, new DataGridCellEventArgs(dataGridCell));
	}
}
