using System;
using System.Collections;
using System.Collections.Generic;

namespace Avalonia.Controls;

internal class DataGridSelectedItemsCollection : IList, ICollection, IEnumerable
{
	private List<object> _oldSelectedItemsCache;

	private IndexToValueTable<bool> _oldSelectedSlotsTable;

	private List<object> _selectedItemsCache;

	private IndexToValueTable<bool> _selectedSlotsTable;

	public object this[int index]
	{
		get
		{
			if (index < 0 || index >= _selectedSlotsTable.IndexCount)
			{
				throw DataGridError.DataGrid.ValueMustBeBetween("index", "Index", 0, lowInclusive: true, _selectedSlotsTable.IndexCount, highInclusive: false);
			}
			int nthIndex = _selectedSlotsTable.GetNthIndex(index);
			return OwningGrid.DataConnection.GetDataItem(OwningGrid.RowIndexFromSlot(nthIndex));
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	public bool IsFixedSize => false;

	public bool IsReadOnly => false;

	public int Count => _selectedSlotsTable.IndexCount;

	public bool IsSynchronized => false;

	public object SyncRoot => this;

	internal DataGrid OwningGrid { get; private set; }

	internal List<object> SelectedItemsCache
	{
		get
		{
			return _selectedItemsCache;
		}
		set
		{
			_selectedItemsCache = value;
			UpdateIndexes();
		}
	}

	public DataGridSelectedItemsCollection(DataGrid owningGrid)
	{
		OwningGrid = owningGrid;
		_oldSelectedItemsCache = new List<object>();
		_oldSelectedSlotsTable = new IndexToValueTable<bool>();
		_selectedItemsCache = new List<object>();
		_selectedSlotsTable = new IndexToValueTable<bool>();
	}

	public int Add(object dataItem)
	{
		if (OwningGrid.SelectionMode == DataGridSelectionMode.Single)
		{
			throw DataGridError.DataGridSelectedItemsCollection.CannotChangeSelectedItemsCollectionInSingleMode();
		}
		int num = OwningGrid.DataConnection.IndexOf(dataItem);
		if (num == -1)
		{
			throw DataGridError.DataGrid.ItemIsNotContainedInTheItemsSource("dataItem");
		}
		int num2 = OwningGrid.SlotFromRowIndex(num);
		if (_selectedSlotsTable.RangeCount == 0)
		{
			OwningGrid.SelectedItem = dataItem;
		}
		else
		{
			OwningGrid.SetRowSelection(num2, isSelected: true, setAnchorSlot: false);
		}
		return _selectedSlotsTable.IndexOf(num2);
	}

	public void Clear()
	{
		if (OwningGrid.SelectionMode == DataGridSelectionMode.Single)
		{
			throw DataGridError.DataGridSelectedItemsCollection.CannotChangeSelectedItemsCollectionInSingleMode();
		}
		if (_selectedSlotsTable.RangeCount > 0 && OwningGrid.CommitEdit(DataGridEditingUnit.Row, exitEditingMode: true))
		{
			OwningGrid.ClearRowSelection(resetAnchorSlot: true);
		}
	}

	public bool Contains(object dataItem)
	{
		int num = OwningGrid.DataConnection.IndexOf(dataItem);
		if (num == -1)
		{
			return false;
		}
		return ContainsSlot(OwningGrid.SlotFromRowIndex(num));
	}

	public int IndexOf(object dataItem)
	{
		int num = OwningGrid.DataConnection.IndexOf(dataItem);
		if (num == -1)
		{
			return -1;
		}
		int index = OwningGrid.SlotFromRowIndex(num);
		return _selectedSlotsTable.IndexOf(index);
	}

	public void Insert(int index, object dataItem)
	{
		throw new NotSupportedException();
	}

	public void Remove(object dataItem)
	{
		if (OwningGrid.SelectionMode == DataGridSelectionMode.Single)
		{
			throw DataGridError.DataGridSelectedItemsCollection.CannotChangeSelectedItemsCollectionInSingleMode();
		}
		int num = OwningGrid.DataConnection.IndexOf(dataItem);
		if (num != -1 && (num != OwningGrid.CurrentSlot || OwningGrid.CommitEdit(DataGridEditingUnit.Row, exitEditingMode: true)))
		{
			OwningGrid.SetRowSelection(num, isSelected: false, setAnchorSlot: false);
		}
	}

	public void RemoveAt(int index)
	{
		if (OwningGrid.SelectionMode == DataGridSelectionMode.Single)
		{
			throw DataGridError.DataGridSelectedItemsCollection.CannotChangeSelectedItemsCollectionInSingleMode();
		}
		if (index < 0 || index >= _selectedSlotsTable.IndexCount)
		{
			throw DataGridError.DataGrid.ValueMustBeBetween("index", "Index", 0, lowInclusive: true, _selectedSlotsTable.IndexCount, highInclusive: false);
		}
		int nthIndex = _selectedSlotsTable.GetNthIndex(index);
		if (nthIndex != OwningGrid.CurrentSlot || OwningGrid.CommitEdit(DataGridEditingUnit.Row, exitEditingMode: true))
		{
			OwningGrid.SetRowSelection(nthIndex, isSelected: false, setAnchorSlot: false);
		}
	}

	public void CopyTo(Array array, int index)
	{
		throw new NotImplementedException();
	}

	public IEnumerator GetEnumerator()
	{
		foreach (int index2 in _selectedSlotsTable.GetIndexes())
		{
			int index = OwningGrid.RowIndexFromSlot(index2);
			yield return OwningGrid.DataConnection.GetDataItem(index);
		}
	}

	internal void ClearRows()
	{
		_selectedSlotsTable.Clear();
		_selectedItemsCache.Clear();
	}

	internal bool ContainsSlot(int slot)
	{
		return _selectedSlotsTable.Contains(slot);
	}

	internal bool ContainsAll(int startSlot, int endSlot)
	{
		int nextGap = OwningGrid.RowGroupHeadersTable.GetNextGap(startSlot - 1);
		while (nextGap <= endSlot)
		{
			int nextIndex = OwningGrid.RowGroupHeadersTable.GetNextIndex(nextGap);
			int num = ((nextIndex == -1) ? endSlot : Math.Min(endSlot, nextIndex - 1));
			if (!_selectedSlotsTable.ContainsAll(nextGap, num))
			{
				return false;
			}
			nextGap = OwningGrid.RowGroupHeadersTable.GetNextGap(num);
		}
		return true;
	}

	internal void Delete(int slot, object item)
	{
		if (_oldSelectedSlotsTable.Contains(slot))
		{
			OwningGrid.SelectionHasChanged = true;
		}
		DeleteSlot(slot);
		_selectedItemsCache.Remove(item);
	}

	internal void DeleteSlot(int slot)
	{
		_selectedSlotsTable.RemoveIndex(slot);
		_oldSelectedSlotsTable.RemoveIndex(slot);
	}

	internal int GetIndexCount(int lowerBound, int upperBound)
	{
		return _selectedSlotsTable.GetIndexCount(lowerBound, upperBound, value: true);
	}

	internal IEnumerable<int> GetIndexes()
	{
		return _selectedSlotsTable.GetIndexes();
	}

	internal IEnumerable<int> GetSlots(int startSlot)
	{
		return _selectedSlotsTable.GetIndexes(startSlot);
	}

	internal SelectionChangedEventArgs GetSelectionChangedEventArgs()
	{
		List<object> list = new List<object>();
		List<object> list2 = new List<object>();
		foreach (int index in _selectedSlotsTable.GetIndexes())
		{
			object dataItem = OwningGrid.DataConnection.GetDataItem(OwningGrid.RowIndexFromSlot(index));
			if (_oldSelectedSlotsTable.Contains(index))
			{
				_oldSelectedSlotsTable.RemoveValue(index);
				_oldSelectedItemsCache.Remove(dataItem);
			}
			else
			{
				list.Add(dataItem);
			}
		}
		foreach (object item in _oldSelectedItemsCache)
		{
			list2.Add(item);
		}
		_oldSelectedSlotsTable = _selectedSlotsTable.Copy();
		_oldSelectedItemsCache = new List<object>(_selectedItemsCache);
		return new SelectionChangedEventArgs(DataGrid.SelectionChangedEvent, list2, list)
		{
			Source = OwningGrid
		};
	}

	internal void InsertIndex(int slot)
	{
		_selectedSlotsTable.InsertIndex(slot);
		_oldSelectedSlotsTable.InsertIndex(slot);
		int num = OwningGrid.RowIndexFromSlot(slot);
		if (num != -1)
		{
			object dataItem = OwningGrid.DataConnection.GetDataItem(num);
			if (dataItem != null && _oldSelectedItemsCache.Contains(dataItem))
			{
				_oldSelectedSlotsTable.AddValue(slot, value: true);
			}
		}
	}

	internal void SelectSlot(int slot, bool select)
	{
		if (OwningGrid.RowGroupHeadersTable.Contains(slot))
		{
			return;
		}
		if (select)
		{
			if (!_selectedSlotsTable.Contains(slot))
			{
				_selectedItemsCache.Add(OwningGrid.DataConnection.GetDataItem(OwningGrid.RowIndexFromSlot(slot)));
			}
			_selectedSlotsTable.AddValue(slot, value: true);
		}
		else
		{
			if (_selectedSlotsTable.Contains(slot))
			{
				_selectedItemsCache.Remove(OwningGrid.DataConnection.GetDataItem(OwningGrid.RowIndexFromSlot(slot)));
			}
			_selectedSlotsTable.RemoveValue(slot);
		}
	}

	internal void SelectSlots(int startSlot, int endSlot, bool select)
	{
		int nextGap = OwningGrid.RowGroupHeadersTable.GetNextGap(startSlot - 1);
		int previousGap = OwningGrid.RowGroupHeadersTable.GetPreviousGap(endSlot + 1);
		if (select)
		{
			while (nextGap <= previousGap)
			{
				int nextIndex = OwningGrid.RowGroupHeadersTable.GetNextIndex(nextGap);
				int num = ((nextIndex == -1) ? previousGap : Math.Min(previousGap, nextIndex - 1));
				for (int i = nextGap; i <= num; i++)
				{
					if (!_selectedSlotsTable.Contains(i))
					{
						_selectedItemsCache.Add(OwningGrid.DataConnection.GetDataItem(OwningGrid.RowIndexFromSlot(i)));
					}
				}
				_selectedSlotsTable.AddValues(nextGap, num - nextGap + 1, value: true);
				nextGap = OwningGrid.RowGroupHeadersTable.GetNextGap(num);
			}
			return;
		}
		while (nextGap <= previousGap)
		{
			int nextIndex2 = OwningGrid.RowGroupHeadersTable.GetNextIndex(nextGap);
			int num2 = ((nextIndex2 == -1) ? previousGap : Math.Min(previousGap, nextIndex2 - 1));
			for (int j = nextGap; j <= num2; j++)
			{
				if (_selectedSlotsTable.Contains(j))
				{
					_selectedItemsCache.Remove(OwningGrid.DataConnection.GetDataItem(OwningGrid.RowIndexFromSlot(j)));
				}
			}
			_selectedSlotsTable.RemoveValues(nextGap, num2 - nextGap + 1);
			nextGap = OwningGrid.RowGroupHeadersTable.GetNextGap(num2);
		}
	}

	internal void UpdateIndexes()
	{
		_oldSelectedSlotsTable.Clear();
		_selectedSlotsTable.Clear();
		if (OwningGrid.DataConnection.DataSource == null)
		{
			if (SelectedItemsCache.Count > 0)
			{
				OwningGrid.SelectionHasChanged = true;
				SelectedItemsCache.Clear();
			}
			return;
		}
		List<object> list = new List<object>();
		foreach (object item in _selectedItemsCache)
		{
			int num = OwningGrid.DataConnection.IndexOf(item);
			if (num != -1)
			{
				list.Add(item);
				_selectedSlotsTable.AddValue(OwningGrid.SlotFromRowIndex(num), value: true);
			}
		}
		foreach (object item2 in _oldSelectedItemsCache)
		{
			int num2 = OwningGrid.DataConnection.IndexOf(item2);
			if (num2 == -1)
			{
				OwningGrid.SelectionHasChanged = true;
			}
			else
			{
				_oldSelectedSlotsTable.AddValue(OwningGrid.SlotFromRowIndex(num2), value: true);
			}
		}
		_selectedItemsCache = list;
	}
}
