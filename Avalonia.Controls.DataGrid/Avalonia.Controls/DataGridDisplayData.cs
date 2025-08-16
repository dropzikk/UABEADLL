using System;
using System.Collections.Generic;
using Avalonia.Media;

namespace Avalonia.Controls;

internal class DataGridDisplayData
{
	private Stack<DataGridRow> _fullyRecycledRows;

	private int _headScrollingElements;

	private DataGrid _owner;

	private Stack<DataGridRow> _recyclableRows;

	private List<Control> _scrollingElements;

	private Stack<DataGridRowGroupHeader> _fullyRecycledGroupHeaders;

	private Stack<DataGridRowGroupHeader> _recyclableGroupHeaders;

	public int FirstDisplayedScrollingCol { get; set; }

	public int FirstScrollingSlot { get; set; }

	public int LastScrollingSlot { get; set; }

	public int LastTotallyDisplayedScrollingCol { get; set; }

	public int NumDisplayedScrollingElements => _scrollingElements.Count;

	public int NumTotallyDisplayedScrollingElements { get; set; }

	internal double PendingVerticalScrollHeight { get; set; }

	public DataGridDisplayData(DataGrid owner)
	{
		_owner = owner;
		ResetSlotIndexes();
		FirstDisplayedScrollingCol = -1;
		LastTotallyDisplayedScrollingCol = -1;
		_scrollingElements = new List<Control>();
		_recyclableRows = new Stack<DataGridRow>();
		_fullyRecycledRows = new Stack<DataGridRow>();
		_recyclableGroupHeaders = new Stack<DataGridRowGroupHeader>();
		_fullyRecycledGroupHeaders = new Stack<DataGridRowGroupHeader>();
	}

	internal void AddRecyclableRow(DataGridRow row)
	{
		row.DetachFromDataGrid(recycle: true);
		_recyclableRows.Push(row);
	}

	internal DataGridRowGroupHeader GetUsedGroupHeader()
	{
		if (_recyclableGroupHeaders.Count > 0)
		{
			return _recyclableGroupHeaders.Pop();
		}
		if (_fullyRecycledGroupHeaders.Count > 0)
		{
			DataGridRowGroupHeader dataGridRowGroupHeader = _fullyRecycledGroupHeaders.Pop();
			dataGridRowGroupHeader.IsVisible = true;
			return dataGridRowGroupHeader;
		}
		return null;
	}

	internal void AddRecylableRowGroupHeader(DataGridRowGroupHeader groupHeader)
	{
		groupHeader.IsRecycled = true;
		_recyclableGroupHeaders.Push(groupHeader);
	}

	internal void ClearElements(bool recycle)
	{
		ResetSlotIndexes();
		if (recycle)
		{
			foreach (Control scrollingElement in _scrollingElements)
			{
				if (scrollingElement is DataGridRow dataGridRow)
				{
					if (dataGridRow.IsRecyclable)
					{
						AddRecyclableRow(dataGridRow);
					}
					else
					{
						dataGridRow.Clip = new RectangleGeometry();
					}
				}
				else if (scrollingElement is DataGridRowGroupHeader groupHeader)
				{
					AddRecylableRowGroupHeader(groupHeader);
				}
			}
		}
		else
		{
			_recyclableRows.Clear();
			_fullyRecycledRows.Clear();
			_recyclableGroupHeaders.Clear();
			_fullyRecycledGroupHeaders.Clear();
		}
		_scrollingElements.Clear();
	}

	internal void CorrectSlotsAfterDeletion(int slot, bool wasCollapsed)
	{
		if (wasCollapsed)
		{
			if (slot > FirstScrollingSlot)
			{
				LastScrollingSlot--;
			}
		}
		else if (_owner.IsSlotVisible(slot))
		{
			UnloadScrollingElement(slot, updateSlotInformation: true, wasDeleted: true);
		}
		if (slot < FirstScrollingSlot)
		{
			FirstScrollingSlot--;
			LastScrollingSlot--;
		}
	}

	internal void CorrectSlotsAfterInsertion(int slot, Control element, bool isCollapsed)
	{
		if (slot < FirstScrollingSlot)
		{
			FirstScrollingSlot++;
			LastScrollingSlot++;
		}
		else if (isCollapsed && slot <= LastScrollingSlot)
		{
			LastScrollingSlot++;
		}
		else if (_owner.GetPreviousVisibleSlot(slot) <= LastScrollingSlot || LastScrollingSlot == -1)
		{
			LoadScrollingSlot(slot, element, updateSlotInformation: true);
		}
	}

	private int GetCircularListIndex(int slot, bool wrap)
	{
		int num = slot - FirstScrollingSlot - _headScrollingElements - _owner.GetCollapsedSlotCount(FirstScrollingSlot, slot);
		if (!wrap)
		{
			return num;
		}
		return num % _scrollingElements.Count;
	}

	internal void FullyRecycleElements()
	{
		while (_recyclableRows.Count > 0)
		{
			DataGridRow dataGridRow = _recyclableRows.Pop();
			dataGridRow.IsVisible = false;
			_fullyRecycledRows.Push(dataGridRow);
		}
		while (_recyclableGroupHeaders.Count > 0)
		{
			DataGridRowGroupHeader dataGridRowGroupHeader = _recyclableGroupHeaders.Pop();
			dataGridRowGroupHeader.IsVisible = false;
			_fullyRecycledGroupHeaders.Push(dataGridRowGroupHeader);
		}
	}

	internal Control GetDisplayedElement(int slot)
	{
		return _scrollingElements[GetCircularListIndex(slot, wrap: true)];
	}

	internal DataGridRow GetDisplayedRow(int rowIndex)
	{
		return GetDisplayedElement(_owner.SlotFromRowIndex(rowIndex)) as DataGridRow;
	}

	internal IEnumerable<Control> GetScrollingElements()
	{
		return GetScrollingElements(null);
	}

	internal IEnumerable<Control> GetScrollingElements(Predicate<object> filter)
	{
		for (int i = 0; i < _scrollingElements.Count; i++)
		{
			Control control = _scrollingElements[(_headScrollingElements + i) % _scrollingElements.Count];
			if (filter == null || filter(control))
			{
				yield return control;
			}
		}
	}

	internal IEnumerable<Control> GetScrollingRows()
	{
		return GetScrollingElements((object element) => element is DataGridRow);
	}

	internal DataGridRow GetUsedRow()
	{
		if (_recyclableRows.Count > 0)
		{
			return _recyclableRows.Pop();
		}
		if (_fullyRecycledRows.Count > 0)
		{
			DataGridRow dataGridRow = _fullyRecycledRows.Pop();
			dataGridRow.IsVisible = true;
			return dataGridRow;
		}
		return null;
	}

	internal void LoadScrollingSlot(int slot, Control element, bool updateSlotInformation)
	{
		if (_scrollingElements.Count == 0)
		{
			SetScrollingSlots(slot);
			_scrollingElements.Add(element);
			return;
		}
		if (updateSlotInformation)
		{
			if (slot < FirstScrollingSlot)
			{
				FirstScrollingSlot = slot;
			}
			else
			{
				LastScrollingSlot = _owner.GetNextVisibleSlot(LastScrollingSlot);
			}
		}
		int num = GetCircularListIndex(slot, wrap: false);
		if (num > _scrollingElements.Count)
		{
			num -= _scrollingElements.Count;
			_headScrollingElements++;
		}
		_scrollingElements.Insert(num, element);
	}

	private void ResetSlotIndexes()
	{
		SetScrollingSlots(-1);
		NumTotallyDisplayedScrollingElements = 0;
		_headScrollingElements = 0;
	}

	private void SetScrollingSlots(int newValue)
	{
		FirstScrollingSlot = newValue;
		LastScrollingSlot = newValue;
	}

	internal void UnloadScrollingElement(int slot, bool updateSlotInformation, bool wasDeleted)
	{
		int num = GetCircularListIndex(slot, wrap: false);
		if (num > _scrollingElements.Count)
		{
			num -= _scrollingElements.Count;
			_headScrollingElements--;
		}
		_scrollingElements.RemoveAt(num);
		if (updateSlotInformation)
		{
			if (slot == FirstScrollingSlot && !wasDeleted)
			{
				FirstScrollingSlot = _owner.GetNextVisibleSlot(FirstScrollingSlot);
			}
			else
			{
				LastScrollingSlot = _owner.GetPreviousVisibleSlot(LastScrollingSlot);
			}
			if (LastScrollingSlot < FirstScrollingSlot)
			{
				ResetSlotIndexes();
			}
		}
	}
}
