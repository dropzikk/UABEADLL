using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Avalonia.Controls.Selection;

public class SelectionModel<T> : SelectionNodeBase<T>, ISelectionModel, INotifyPropertyChanged
{
	public record struct BatchUpdateOperation(SelectionModel<T> owner) : IDisposable
	{
		internal Operation Operation => _owner._operation;

		private readonly SelectionModel<T> _owner;

		private bool _isDisposed;

		public BatchUpdateOperation(SelectionModel<T> owner)
		{
			_isDisposed = false;
			owner.BeginBatchUpdate();
		}

		public void Dispose()
		{
			if (!_isDisposed)
			{
				_owner?.EndBatchUpdate();
				_isDisposed = true;
			}
		}

		[CompilerGenerated]
		private readonly bool PrintMembers(StringBuilder builder)
		{
			return false;
		}
	}

	internal class Operation
	{
		public int UpdateCount { get; set; }

		public bool IsSourceUpdate { get; set; }

		public bool SkipLostSelection { get; set; }

		public int AnchorIndex { get; set; }

		public int SelectedIndex { get; set; }

		public List<IndexRange>? SelectedRanges { get; set; }

		public List<IndexRange>? DeselectedRanges { get; set; }

		public IReadOnlyList<T>? DeselectedItems { get; set; }

		public Operation(SelectionModel<T> owner)
		{
			AnchorIndex = owner.AnchorIndex;
			SelectedIndex = owner.SelectedIndex;
		}
	}

	private bool _singleSelect = true;

	private int _anchorIndex = -1;

	private int _selectedIndex = -1;

	private Operation? _operation;

	private SelectedIndexes<T>? _selectedIndexes;

	private SelectedItems<T>? _selectedItems;

	private SelectedItems<T>.Untyped? _selectedItemsUntyped;

	private EventHandler<SelectionModelSelectionChangedEventArgs>? _untypedSelectionChanged;

	private IList? _initSelectedItems;

	private bool _isSourceCollectionChanging;

	public new IEnumerable? Source
	{
		get
		{
			return base.Source;
		}
		set
		{
			SetSource(value);
		}
	}

	public bool SingleSelect
	{
		get
		{
			return _singleSelect;
		}
		set
		{
			if (_singleSelect == value)
			{
				return;
			}
			if (value)
			{
				using (BatchUpdate())
				{
					int selectedIndex = SelectedIndex;
					Clear();
					SelectedIndex = selectedIndex;
				}
			}
			_singleSelect = value;
			base.RangesEnabled = !value;
			if (base.RangesEnabled && _selectedIndex >= 0)
			{
				CommitSelect(_selectedIndex, _selectedIndex);
			}
			RaisePropertyChanged("SingleSelect");
		}
	}

	public int SelectedIndex
	{
		get
		{
			return _selectedIndex;
		}
		set
		{
			using (BatchUpdate())
			{
				Clear();
				Select(value);
			}
		}
	}

	public IReadOnlyList<int> SelectedIndexes => _selectedIndexes ?? (_selectedIndexes = new SelectedIndexes<T>(this));

	public T? SelectedItem
	{
		get
		{
			if (base.ItemsView != null)
			{
				return GetItemAt(_selectedIndex);
			}
			if (_initSelectedItems != null && _initSelectedItems.Count > 0)
			{
				return (T)_initSelectedItems[0];
			}
			return default(T);
		}
		set
		{
			if (base.ItemsView != null)
			{
				SelectedIndex = base.ItemsView.IndexOf(value);
				return;
			}
			Clear();
			SetInitSelectedItems(new T[1] { value });
		}
	}

	public IReadOnlyList<T?> SelectedItems
	{
		get
		{
			if (base.ItemsView == null && _initSelectedItems != null)
			{
				if (!(_initSelectedItems is IReadOnlyList<T> result))
				{
					return _initSelectedItems.Cast<T>().ToList();
				}
				return result;
			}
			return _selectedItems ?? (_selectedItems = new SelectedItems<T>(this));
		}
	}

	public int AnchorIndex
	{
		get
		{
			return _anchorIndex;
		}
		set
		{
			using BatchUpdateOperation batchUpdateOperation = BatchUpdate();
			int anchorIndex = CoerceIndex(value);
			batchUpdateOperation.Operation.AnchorIndex = anchorIndex;
		}
	}

	public int Count
	{
		get
		{
			if (SingleSelect)
			{
				if (_selectedIndex < 0)
				{
					return 0;
				}
				return 1;
			}
			return IndexRange.GetCount(base.Ranges);
		}
	}

	IEnumerable? ISelectionModel.Source
	{
		get
		{
			return Source;
		}
		set
		{
			SetSource(value);
		}
	}

	object? ISelectionModel.SelectedItem
	{
		get
		{
			return SelectedItem;
		}
		set
		{
			if (value is T selectedItem)
			{
				SelectedItem = selectedItem;
			}
			else
			{
				SelectedIndex = -1;
			}
		}
	}

	IReadOnlyList<object?> ISelectionModel.SelectedItems => _selectedItemsUntyped ?? (_selectedItemsUntyped = new SelectedItems<T>.Untyped(SelectedItems));

	public event EventHandler<SelectionModelIndexesChangedEventArgs>? IndexesChanged;

	public event EventHandler<SelectionModelSelectionChangedEventArgs<T>>? SelectionChanged;

	public event EventHandler? LostSelection;

	public event EventHandler? SourceReset;

	public event PropertyChangedEventHandler? PropertyChanged;

	event EventHandler<SelectionModelSelectionChangedEventArgs>? ISelectionModel.SelectionChanged
	{
		add
		{
			_untypedSelectionChanged = (EventHandler<SelectionModelSelectionChangedEventArgs>)Delegate.Combine(_untypedSelectionChanged, value);
		}
		remove
		{
			_untypedSelectionChanged = (EventHandler<SelectionModelSelectionChangedEventArgs>)Delegate.Remove(_untypedSelectionChanged, value);
		}
	}

	public SelectionModel()
	{
	}

	public SelectionModel(IEnumerable<T>? source)
	{
		Source = source;
	}

	public BatchUpdateOperation BatchUpdate()
	{
		return new BatchUpdateOperation(this);
	}

	public void BeginBatchUpdate()
	{
		if (_operation == null)
		{
			_operation = new Operation(this);
		}
		Operation? operation = _operation;
		int updateCount = operation.UpdateCount + 1;
		operation.UpdateCount = updateCount;
	}

	public void EndBatchUpdate()
	{
		if (_operation == null || _operation.UpdateCount == 0)
		{
			throw new InvalidOperationException("No batch update in progress.");
		}
		if (--_operation.UpdateCount == 0 && !_isSourceCollectionChanging)
		{
			CommitOperation(_operation);
		}
	}

	public bool IsSelected(int index)
	{
		if (index < 0)
		{
			return false;
		}
		if (SingleSelect)
		{
			return _selectedIndex == index;
		}
		return IndexRange.Contains(base.Ranges, index);
	}

	public void Select(int index)
	{
		SelectRange(index, index, forceSelectedIndex: false, forceAnchorIndex: true);
	}

	public void Deselect(int index)
	{
		DeselectRange(index, index);
	}

	public void SelectRange(int start, int end)
	{
		SelectRange(start, end, forceSelectedIndex: false, forceAnchorIndex: false);
	}

	public void DeselectRange(int start, int end)
	{
		using BatchUpdateOperation batchUpdateOperation = BatchUpdate();
		Operation operation = batchUpdateOperation.Operation;
		IndexRange range = new IndexRange(Math.Max(0, start), end);
		if (base.RangesEnabled)
		{
			List<IndexRange> ranges = base.Ranges.ToList();
			List<IndexRange> list = new List<IndexRange>();
			List<IndexRange> list2 = new List<IndexRange>();
			Operation operation2 = operation;
			if (operation2.DeselectedRanges == null)
			{
				List<IndexRange> list4 = (operation2.DeselectedRanges = new List<IndexRange>());
			}
			IndexRange.Remove(operation.SelectedRanges, range, list2);
			IndexRange.Remove(ranges, range, list);
			IndexRange.Add(operation.DeselectedRanges, list);
			if (IndexRange.Contains(list, operation.SelectedIndex) || IndexRange.Contains(list2, operation.SelectedIndex))
			{
				operation.SelectedIndex = GetFirstSelectedIndexFromRanges(list);
			}
		}
		else if (range.Contains(_selectedIndex))
		{
			operation.SelectedIndex = -1;
		}
		_initSelectedItems = null;
	}

	public void SelectAll()
	{
		SelectRange(0, int.MaxValue);
	}

	public void Clear()
	{
		DeselectRange(0, int.MaxValue);
	}

	protected void RaisePropertyChanged(string propertyName)
	{
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}

	private protected virtual void SetSource(IEnumerable? value)
	{
		if (base.Source == value)
		{
			return;
		}
		Operation? operation = _operation;
		if (operation != null && operation.UpdateCount > 0)
		{
			throw new InvalidOperationException("Cannot change source while update is in progress.");
		}
		if (base.Source != null && value != null)
		{
			using BatchUpdateOperation batchUpdateOperation = BatchUpdate();
			batchUpdateOperation.Operation.SkipLostSelection = true;
			Clear();
		}
		base.Source = value;
		using BatchUpdateOperation batchUpdateOperation2 = BatchUpdate();
		batchUpdateOperation2.Operation.IsSourceUpdate = true;
		if (_initSelectedItems != null && base.ItemsView != null)
		{
			foreach (T initSelectedItem in _initSelectedItems)
			{
				Select(base.ItemsView.IndexOf(initSelectedItem));
			}
			_initSelectedItems = null;
		}
		else
		{
			TrimInvalidSelections(batchUpdateOperation2.Operation);
		}
		RaisePropertyChanged("Source");
	}

	protected override void OnIndexesChanged(int shiftIndex, int shiftDelta)
	{
		this.IndexesChanged?.Invoke(this, new SelectionModelIndexesChangedEventArgs(shiftIndex, shiftDelta));
	}

	protected override void OnSourceCollectionChangeStarted()
	{
		base.OnSourceCollectionChangeStarted();
		_isSourceCollectionChanging = true;
	}

	protected override void OnSourceReset()
	{
		_selectedIndex = (_anchorIndex = -1);
		CommitDeselect(0, int.MaxValue);
		if (this.SourceReset != null)
		{
			this.SourceReset(this, EventArgs.Empty);
		}
	}

	protected override void OnSelectionRemoved(int index, int count, IReadOnlyList<T> deselectedItems)
	{
		BatchUpdateOperation batchUpdateOperation = BatchUpdate();
		batchUpdateOperation.Operation.DeselectedItems = deselectedItems;
		if (_selectedIndex == -1 && this.LostSelection != null)
		{
			this.LostSelection(this, EventArgs.Empty);
		}
		CommitOperation(batchUpdateOperation.Operation, raisePropertyChanged: false);
	}

	protected override CollectionChangeState OnItemsAdded(int index, IList items)
	{
		int count = items.Count;
		bool flag = SelectedIndex >= index;
		int num = (flag ? count : 0);
		_selectedIndex += num;
		_anchorIndex += num;
		SelectionNodeBase<T>.CollectionChangeState collectionChangeState = base.OnItemsAdded(index, items);
		flag |= collectionChangeState.ShiftDelta != 0;
		return new SelectionNodeBase<T>.CollectionChangeState
		{
			ShiftIndex = index,
			ShiftDelta = (flag ? count : 0)
		};
	}

	private protected override CollectionChangeState OnItemsRemoved(int index, IList items)
	{
		int count = items.Count;
		IndexRange indexRange = new IndexRange(index, index + count - 1);
		bool flag = false;
		SelectionNodeBase<T>.CollectionChangeState collectionChangeState = base.OnItemsRemoved(index, items);
		flag |= collectionChangeState.ShiftDelta != 0;
		List<T> removedItems = collectionChangeState.RemovedItems;
		if (indexRange.Contains(SelectedIndex))
		{
			if (SingleSelect)
			{
				removedItems = new List<T> { (T)items[SelectedIndex - index] };
			}
			_selectedIndex = GetFirstSelectedIndexFromRanges();
		}
		else if (SelectedIndex >= index)
		{
			_selectedIndex -= count;
			flag = true;
		}
		if (indexRange.Contains(AnchorIndex))
		{
			_anchorIndex = GetFirstSelectedIndexFromRanges();
		}
		else if (AnchorIndex >= index)
		{
			_anchorIndex -= count;
			flag = true;
		}
		return new SelectionNodeBase<T>.CollectionChangeState
		{
			ShiftIndex = index,
			ShiftDelta = (flag ? (-count) : 0),
			RemovedItems = removedItems
		};
	}

	protected override void OnSourceCollectionChanged(NotifyCollectionChangedEventArgs e)
	{
		Operation? operation = _operation;
		if (operation != null && operation.UpdateCount > 0)
		{
			throw new InvalidOperationException("Source collection was modified during selection update.");
		}
		int anchorIndex = _anchorIndex;
		int selectedIndex = _selectedIndex;
		base.OnSourceCollectionChanged(e);
		if (selectedIndex != _selectedIndex)
		{
			RaisePropertyChanged("SelectedIndex");
		}
		if ((e.Action == NotifyCollectionChangedAction.Remove && e.OldStartingIndex <= selectedIndex) || (e.Action == NotifyCollectionChangedAction.Replace && e.OldStartingIndex == selectedIndex) || (e.Action == NotifyCollectionChangedAction.Move && e.OldStartingIndex == selectedIndex) || e.Action == NotifyCollectionChangedAction.Reset)
		{
			RaisePropertyChanged("SelectedItem");
		}
		if (anchorIndex != _anchorIndex)
		{
			RaisePropertyChanged("AnchorIndex");
		}
	}

	private protected void SetInitSelectedItems(IList items)
	{
		if (Source != null)
		{
			throw new InvalidOperationException("Cannot set init selected items when Source is set.");
		}
		_initSelectedItems = items;
	}

	private protected override bool IsValidCollectionChange(NotifyCollectionChangedEventArgs e)
	{
		if (!base.IsValidCollectionChange(e))
		{
			return false;
		}
		if (base.ItemsView != null && e.Action == NotifyCollectionChangedAction.Add)
		{
			if (e.NewStartingIndex <= _selectedIndex)
			{
				return _selectedIndex + e.NewItems.Count < base.ItemsView.Count;
			}
			if (e.NewStartingIndex <= _anchorIndex)
			{
				return _anchorIndex + e.NewItems.Count < base.ItemsView.Count;
			}
		}
		return true;
	}

	protected override void OnSourceCollectionChangeFinished()
	{
		_isSourceCollectionChanging = false;
		if (_operation != null)
		{
			CommitOperation(_operation);
		}
	}

	private int GetFirstSelectedIndexFromRanges(List<IndexRange>? except = null)
	{
		if (base.RangesEnabled)
		{
			int count = IndexRange.GetCount(base.Ranges);
			int num = 0;
			while (num < count)
			{
				int at = IndexRange.GetAt(base.Ranges, num++);
				if (!IndexRange.Contains(except, at))
				{
					return at;
				}
			}
		}
		return -1;
	}

	private void SelectRange(int start, int end, bool forceSelectedIndex, bool forceAnchorIndex)
	{
		if (SingleSelect && start != end)
		{
			throw new InvalidOperationException("Cannot select range with single selection.");
		}
		IndexRange range = CoerceRange(start, end);
		if (range.Begin == -1)
		{
			return;
		}
		using BatchUpdateOperation batchUpdateOperation = BatchUpdate();
		Operation operation = batchUpdateOperation.Operation;
		new List<IndexRange>();
		if (base.RangesEnabled)
		{
			Operation operation2 = operation;
			if (operation2.SelectedRanges == null)
			{
				List<IndexRange> list2 = (operation2.SelectedRanges = new List<IndexRange>());
			}
			IndexRange.Remove(operation.DeselectedRanges, range);
			IndexRange.Add(operation.SelectedRanges, range);
			IndexRange.Remove(operation.SelectedRanges, base.Ranges);
			if (operation.SelectedIndex == -1 || forceSelectedIndex)
			{
				operation.SelectedIndex = range.Begin;
			}
			if (operation.AnchorIndex == -1 || forceAnchorIndex)
			{
				operation.AnchorIndex = range.Begin;
			}
		}
		else
		{
			int selectedIndex = (operation.AnchorIndex = start);
			operation.SelectedIndex = selectedIndex;
		}
		_initSelectedItems = null;
	}

	[return: MaybeNull]
	private T GetItemAt(int index)
	{
		if (base.ItemsView == null || index < 0 || index >= base.ItemsView.Count)
		{
			return default(T);
		}
		return base.ItemsView[index];
	}

	private int CoerceIndex(int index)
	{
		index = Math.Max(index, -1);
		if (base.ItemsView != null && index >= base.ItemsView.Count)
		{
			index = -1;
		}
		return index;
	}

	private IndexRange CoerceRange(int start, int end)
	{
		int num = ((base.ItemsView != null) ? (base.ItemsView.Count - 1) : int.MaxValue);
		if (start > num || (start < 0 && end < 0))
		{
			return new IndexRange(-1);
		}
		start = Math.Max(start, 0);
		end = Math.Min(end, num);
		return new IndexRange(start, end);
	}

	private void TrimInvalidSelections(Operation operation)
	{
		if (base.ItemsView == null)
		{
			return;
		}
		int num = base.ItemsView.Count - 1;
		if (operation.SelectedIndex > num)
		{
			operation.SelectedIndex = GetFirstSelectedIndexFromRanges();
		}
		if (operation.AnchorIndex > num)
		{
			operation.AnchorIndex = GetFirstSelectedIndexFromRanges();
		}
		if (base.RangesEnabled && base.Ranges.Count > 0)
		{
			List<IndexRange> list = base.Ranges.ToList();
			if (num < 0)
			{
				operation.DeselectedRanges = list;
				return;
			}
			IndexRange range = new IndexRange(0, num);
			List<IndexRange> list2 = new List<IndexRange>();
			IndexRange.Intersect(list, range, list2);
			operation.DeselectedRanges = list2;
		}
	}

	private void CommitOperation(Operation operation, bool raisePropertyChanged = true)
	{
		try
		{
			int anchorIndex = _anchorIndex;
			int selectedIndex = _selectedIndex;
			bool flag = false;
			if (operation.SelectedIndex == -1 && this.LostSelection != null && !operation.SkipLostSelection)
			{
				operation.UpdateCount++;
				this.LostSelection?.Invoke(this, EventArgs.Empty);
			}
			_selectedIndex = operation.SelectedIndex;
			_anchorIndex = operation.AnchorIndex;
			if (operation.SelectedRanges != null)
			{
				foreach (IndexRange selectedRange in operation.SelectedRanges)
				{
					flag |= CommitSelect(selectedRange.Begin, selectedRange.End) > 0;
				}
			}
			if (operation.DeselectedRanges != null)
			{
				foreach (IndexRange deselectedRange in operation.DeselectedRanges)
				{
					flag |= CommitDeselect(deselectedRange.Begin, deselectedRange.End) > 0;
				}
			}
			if (this.SelectionChanged != null || _untypedSelectionChanged != null)
			{
				IReadOnlyList<IndexRange> readOnlyList = operation.DeselectedRanges;
				IReadOnlyList<IndexRange> readOnlyList2 = operation.SelectedRanges;
				if (SingleSelect && selectedIndex != _selectedIndex)
				{
					if (selectedIndex != -1)
					{
						readOnlyList = new IndexRange[1]
						{
							new IndexRange(selectedIndex)
						};
					}
					if (_selectedIndex != -1)
					{
						readOnlyList2 = new IndexRange[1]
						{
							new IndexRange(_selectedIndex)
						};
					}
				}
				if ((readOnlyList != null && readOnlyList.Count > 0) || (readOnlyList2 != null && readOnlyList2.Count > 0) || operation.DeselectedItems != null)
				{
					ItemsSourceView<T> items = (operation.IsSourceUpdate ? null : base.ItemsView);
					IReadOnlyList<T> deselectedItems = operation.DeselectedItems;
					IReadOnlyList<T> deselectedItems2 = deselectedItems ?? SelectedItems<T>.Create(readOnlyList, items);
					SelectionModelSelectionChangedEventArgs<T> e = new SelectionModelSelectionChangedEventArgs<T>(SelectedIndexes<T>.Create(readOnlyList), SelectedIndexes<T>.Create(readOnlyList2), deselectedItems2, SelectedItems<T>.Create(readOnlyList2, (Source != null) ? base.ItemsView : null));
					this.SelectionChanged?.Invoke(this, e);
					_untypedSelectionChanged?.Invoke(this, e);
				}
			}
			if (raisePropertyChanged)
			{
				if (selectedIndex != _selectedIndex)
				{
					flag = true;
					RaisePropertyChanged("SelectedIndex");
				}
				if (selectedIndex != _selectedIndex || operation.IsSourceUpdate)
				{
					RaisePropertyChanged("SelectedItem");
				}
				if (anchorIndex != _anchorIndex)
				{
					flag = true;
					RaisePropertyChanged("AnchorIndex");
				}
				if (flag)
				{
					RaisePropertyChanged("SelectedIndexes");
				}
				if (flag || operation.IsSourceUpdate)
				{
					RaisePropertyChanged("SelectedItems");
				}
			}
		}
		finally
		{
			_operation = null;
		}
	}
}
