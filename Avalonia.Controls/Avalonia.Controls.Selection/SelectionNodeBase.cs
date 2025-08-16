using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Avalonia.Controls.Selection;

public abstract class SelectionNodeBase<T>
{
	protected class CollectionChangeState
	{
		public int ShiftIndex { get; set; }

		public int ShiftDelta { get; set; }

		public List<T>? RemovedItems { get; set; }
	}

	private IEnumerable? _source;

	private bool _rangesEnabled;

	private List<IndexRange>? _ranges;

	protected IEnumerable? Source
	{
		get
		{
			return _source;
		}
		set
		{
			if (_source != value)
			{
				if (ItemsView != null)
				{
					ItemsView.PreCollectionChanged -= OnPreChanged;
					ItemsView.CollectionChanged -= OnChanged;
					ItemsView.PostCollectionChanged -= OnPostChanged;
				}
				_source = value;
				ItemsView = ((value != null) ? ItemsSourceView.GetOrCreate<T>(value) : null);
				if (ItemsView != null)
				{
					ItemsView.PreCollectionChanged += OnPreChanged;
					ItemsView.CollectionChanged += OnChanged;
					ItemsView.PostCollectionChanged += OnPostChanged;
				}
			}
			void OnChanged(object? sender, NotifyCollectionChangedEventArgs e)
			{
				OnSourceCollectionChanged(e);
			}
			void OnPostChanged(object? sender, NotifyCollectionChangedEventArgs e)
			{
				OnSourceCollectionChangeFinished();
			}
			void OnPreChanged(object? sender, NotifyCollectionChangedEventArgs e)
			{
				OnSourceCollectionChangeStarted();
			}
		}
	}

	protected internal ItemsSourceView<T>? ItemsView { get; set; }

	protected bool RangesEnabled
	{
		get
		{
			return _rangesEnabled;
		}
		set
		{
			if (_rangesEnabled != value)
			{
				_rangesEnabled = value;
				if (!_rangesEnabled)
				{
					_ranges = null;
				}
			}
		}
	}

	internal IReadOnlyList<IndexRange> Ranges
	{
		get
		{
			if (!RangesEnabled)
			{
				throw new InvalidOperationException("Ranges not enabled.");
			}
			return _ranges ?? (_ranges = new List<IndexRange>());
		}
	}

	protected virtual void OnSourceCollectionChangeStarted()
	{
	}

	protected virtual void OnSourceCollectionChanged(NotifyCollectionChangedEventArgs e)
	{
		int num = 0;
		int num2 = -1;
		List<T> list = null;
		if (IsValidCollectionChange(e))
		{
			switch (e.Action)
			{
			case NotifyCollectionChangedAction.Add:
			{
				CollectionChangeState collectionChangeState4 = OnItemsAdded(e.NewStartingIndex, e.NewItems);
				num2 = collectionChangeState4.ShiftIndex;
				num = collectionChangeState4.ShiftDelta;
				break;
			}
			case NotifyCollectionChangedAction.Remove:
			{
				CollectionChangeState collectionChangeState3 = OnItemsRemoved(e.OldStartingIndex, e.OldItems);
				num2 = collectionChangeState3.ShiftIndex;
				num = collectionChangeState3.ShiftDelta;
				list = collectionChangeState3.RemovedItems;
				break;
			}
			case NotifyCollectionChangedAction.Replace:
			case NotifyCollectionChangedAction.Move:
			{
				CollectionChangeState collectionChangeState = OnItemsRemoved(e.OldStartingIndex, e.OldItems);
				CollectionChangeState collectionChangeState2 = OnItemsAdded(e.NewStartingIndex, e.NewItems);
				num2 = collectionChangeState.ShiftIndex;
				num = collectionChangeState.ShiftDelta + collectionChangeState2.ShiftDelta;
				list = collectionChangeState.RemovedItems;
				break;
			}
			case NotifyCollectionChangedAction.Reset:
				OnSourceReset();
				break;
			}
			if (num != 0)
			{
				OnIndexesChanged(num2, num);
			}
			if (list != null)
			{
				OnSelectionRemoved(num2, -num, list);
			}
		}
	}

	protected virtual void OnSourceCollectionChangeFinished()
	{
	}

	protected virtual void OnIndexesChanged(int shiftIndex, int shiftDelta)
	{
	}

	protected abstract void OnSourceReset();

	protected virtual void OnSelectionRemoved(int index, int count, IReadOnlyList<T> deselectedItems)
	{
	}

	protected int CommitSelect(int begin, int end)
	{
		if (RangesEnabled)
		{
			if (_ranges == null)
			{
				_ranges = new List<IndexRange>();
			}
			return IndexRange.Add(_ranges, new IndexRange(begin, end));
		}
		return 0;
	}

	protected int CommitDeselect(int begin, int end)
	{
		if (RangesEnabled)
		{
			if (_ranges == null)
			{
				_ranges = new List<IndexRange>();
			}
			return IndexRange.Remove(_ranges, new IndexRange(begin, end));
		}
		return 0;
	}

	protected virtual CollectionChangeState OnItemsAdded(int index, IList items)
	{
		int count = items.Count;
		bool flag = false;
		if (_ranges != null)
		{
			List<IndexRange> list = null;
			for (int i = 0; i < Ranges.Count; i++)
			{
				IndexRange indexRange = Ranges[i];
				if (indexRange.End >= index)
				{
					int num = indexRange.Begin;
					if (indexRange.Contains(index - 1))
					{
						indexRange.Split(index - 1, out var before, out var _);
						(list ?? (list = new List<IndexRange>())).Add(before);
						num = index;
					}
					_ranges[i] = new IndexRange(num + count, indexRange.End + count);
					flag = true;
				}
			}
			if (list != null)
			{
				foreach (IndexRange item in list)
				{
					IndexRange.Add(_ranges, item);
				}
			}
		}
		return new CollectionChangeState
		{
			ShiftIndex = index,
			ShiftDelta = (flag ? count : 0)
		};
	}

	private protected virtual CollectionChangeState OnItemsRemoved(int index, IList items)
	{
		int count = items.Count;
		IndexRange range = new IndexRange(index, index + count - 1);
		bool flag = false;
		List<T> list = null;
		if (_ranges != null)
		{
			List<IndexRange> list2 = new List<IndexRange>();
			if (IndexRange.Remove(_ranges, range, list2) > 0)
			{
				list = new List<T>();
				foreach (IndexRange item in list2)
				{
					for (int i = item.Begin; i <= item.End; i++)
					{
						list.Add((T)items[i - index]);
					}
				}
			}
			for (int j = 0; j < Ranges.Count; j++)
			{
				IndexRange indexRange = Ranges[j];
				if (indexRange.End > range.Begin)
				{
					_ranges[j] = new IndexRange(indexRange.Begin - count, indexRange.End - count);
					flag = true;
				}
			}
		}
		return new CollectionChangeState
		{
			ShiftIndex = index,
			ShiftDelta = (flag ? (-count) : 0),
			RemovedItems = list
		};
	}

	private protected virtual bool IsValidCollectionChange(NotifyCollectionChangedEventArgs e)
	{
		if (ItemsView != null && RangesEnabled && Ranges.Count > 0 && e.Action == NotifyCollectionChangedAction.Add)
		{
			int end = Ranges[Ranges.Count - 1].End;
			if (e.NewStartingIndex <= end)
			{
				return end + e.NewItems.Count < ItemsView.Count;
			}
		}
		return true;
	}
}
