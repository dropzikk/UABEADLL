namespace Avalonia.Controls;

internal class DataGridCellCoordinates
{
	public int ColumnIndex { get; set; }

	public int Slot { get; set; }

	public DataGridCellCoordinates(int columnIndex, int slot)
	{
		ColumnIndex = columnIndex;
		Slot = slot;
	}

	public DataGridCellCoordinates(DataGridCellCoordinates dataGridCellCoordinates)
		: this(dataGridCellCoordinates.ColumnIndex, dataGridCellCoordinates.Slot)
	{
	}

	public override bool Equals(object o)
	{
		if (o is DataGridCellCoordinates dataGridCellCoordinates)
		{
			if (dataGridCellCoordinates.ColumnIndex == ColumnIndex)
			{
				return dataGridCellCoordinates.Slot == Slot;
			}
			return false;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}
}
