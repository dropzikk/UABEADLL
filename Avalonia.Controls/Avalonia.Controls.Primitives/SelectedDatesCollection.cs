using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Threading;

namespace Avalonia.Controls.Primitives;

public sealed class SelectedDatesCollection : ObservableCollection<DateTime>
{
	private Collection<DateTime> _addedItems;

	private bool _isCleared;

	private bool _isRangeAdded;

	private Calendar _owner;

	public SelectedDatesCollection(Calendar owner)
	{
		_owner = owner;
		_addedItems = new Collection<DateTime>();
	}

	private void InvokeCollectionChanged(IList removedItems, IList addedItems)
	{
		_owner.OnSelectedDatesCollectionChanged(new SelectionChangedEventArgs(SelectingItemsControl.SelectionChangedEvent, removedItems, addedItems));
	}

	public void AddRange(DateTime start, DateTime end)
	{
		int num = ((DateTime.Compare(end, start) >= 0) ? 1 : (-1));
		_addedItems.Clear();
		DateTime? value = start;
		_isRangeAdded = true;
		if (_owner.IsMouseSelection)
		{
			while (value.HasValue && DateTime.Compare(end, value.Value) != -num)
			{
				if (Calendar.IsValidDateSelection(_owner, value))
				{
					Add(value.Value);
				}
				else if (_owner.SelectionMode == CalendarSelectionMode.SingleRange)
				{
					_owner.HoverEnd = value.Value.AddDays(-num);
					break;
				}
				value = DateTimeHelper.AddDays(value.Value, num);
			}
		}
		else
		{
			if (_owner.SelectionMode == CalendarSelectionMode.SingleRange && base.Count > 0)
			{
				using (IEnumerator<DateTime> enumerator = GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						DateTime current = enumerator.Current;
						_owner.RemovedItems.Add(current);
					}
				}
				ClearInternal();
			}
			while (value.HasValue && DateTime.Compare(end, value.Value) != -num)
			{
				Add(value.Value);
				value = DateTimeHelper.AddDays(value.Value, num);
			}
		}
		_owner.OnSelectedDatesCollectionChanged(new SelectionChangedEventArgs(SelectingItemsControl.SelectionChangedEvent, _owner.RemovedItems, _addedItems));
		_owner.RemovedItems.Clear();
		_owner.UpdateMonths();
		_isRangeAdded = false;
	}

	protected override void ClearItems()
	{
		EnsureValidThread();
		Collection<DateTime> addedItems = new Collection<DateTime>();
		Collection<DateTime> collection = new Collection<DateTime>();
		using (IEnumerator<DateTime> enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				DateTime current = enumerator.Current;
				collection.Add(current);
			}
		}
		base.ClearItems();
		if (_owner.SelectionMode != CalendarSelectionMode.None && _owner.SelectedDate.HasValue)
		{
			_owner.SelectedDate = null;
		}
		if (collection.Count != 0)
		{
			InvokeCollectionChanged(collection, addedItems);
		}
		_owner.UpdateMonths();
	}

	protected override void InsertItem(int index, DateTime item)
	{
		EnsureValidThread();
		if (Contains(item))
		{
			return;
		}
		Collection<DateTime> collection = new Collection<DateTime>();
		if (!CheckSelectionMode())
		{
			return;
		}
		if (Calendar.IsValidDateSelection(_owner, item))
		{
			if (_isCleared)
			{
				index = 0;
				_isCleared = false;
			}
			base.InsertItem(index, item);
			if (index == 0 && (!_owner.SelectedDate.HasValue || DateTime.Compare(_owner.SelectedDate.Value, item) != 0))
			{
				_owner.SelectedDate = item;
			}
			if (!_isRangeAdded)
			{
				collection.Add(item);
				InvokeCollectionChanged(_owner.RemovedItems, collection);
				_owner.RemovedItems.Clear();
				int num = DateTimeHelper.CompareYearMonth(item, _owner.DisplayDateInternal);
				if (num < 2 && num > -2)
				{
					_owner.UpdateMonths();
				}
			}
			else
			{
				_addedItems.Add(item);
			}
			return;
		}
		throw new ArgumentOutOfRangeException("SelectedDate value is not valid.");
	}

	protected override void RemoveItem(int index)
	{
		EnsureValidThread();
		if (index >= base.Count)
		{
			base.RemoveItem(index);
			return;
		}
		Collection<DateTime> addedItems = new Collection<DateTime>();
		Collection<DateTime> collection = new Collection<DateTime>();
		int num = DateTimeHelper.CompareYearMonth(base[index], _owner.DisplayDateInternal);
		collection.Add(base[index]);
		base.RemoveItem(index);
		if (index == 0)
		{
			if (base.Count > 0)
			{
				_owner.SelectedDate = base[0];
			}
			else
			{
				_owner.SelectedDate = null;
			}
		}
		InvokeCollectionChanged(collection, addedItems);
		if (num < 2 && num > -2)
		{
			_owner.UpdateMonths();
		}
	}

	protected override void SetItem(int index, DateTime item)
	{
		EnsureValidThread();
		if (Contains(item))
		{
			return;
		}
		Collection<DateTime> collection = new Collection<DateTime>();
		Collection<DateTime> collection2 = new Collection<DateTime>();
		if (index >= base.Count)
		{
			base.SetItem(index, item);
		}
		else if (DateTime.Compare(base[index], item) != 0 && Calendar.IsValidDateSelection(_owner, item))
		{
			collection2.Add(base[index]);
			base.SetItem(index, item);
			collection.Add(item);
			if (index == 0 && (!_owner.SelectedDate.HasValue || DateTime.Compare(_owner.SelectedDate.Value, item) != 0))
			{
				_owner.SelectedDate = item;
			}
			InvokeCollectionChanged(collection2, collection);
			int num = DateTimeHelper.CompareYearMonth(item, _owner.DisplayDateInternal);
			if (num < 2 && num > -2)
			{
				_owner.UpdateMonths();
			}
		}
	}

	internal void ClearInternal()
	{
		base.ClearItems();
	}

	private bool CheckSelectionMode()
	{
		if (_owner.SelectionMode == CalendarSelectionMode.None)
		{
			throw new InvalidOperationException("The SelectedDate property cannot be set when the selection mode is None.");
		}
		if (_owner.SelectionMode == CalendarSelectionMode.SingleDate && base.Count > 0)
		{
			throw new InvalidOperationException("The SelectedDates collection can be changed only in a multiple selection mode. Use the SelectedDate in a single selection mode.");
		}
		if (_owner.SelectionMode == CalendarSelectionMode.SingleRange && !_isRangeAdded && base.Count > 0)
		{
			using (IEnumerator<DateTime> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					DateTime current = enumerator.Current;
					_owner.RemovedItems.Add(current);
				}
			}
			ClearInternal();
			_isCleared = true;
		}
		return true;
	}

	private static void EnsureValidThread()
	{
		Dispatcher.UIThread.VerifyAccess();
	}
}
