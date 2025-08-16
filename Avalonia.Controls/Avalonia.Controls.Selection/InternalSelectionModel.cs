using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Avalonia.Collections;
using Avalonia.Data;

namespace Avalonia.Controls.Selection;

internal class InternalSelectionModel : SelectionModel<object?>
{
	private IList? _writableSelectedItems;

	private int _ignoreModelChanges;

	private bool _ignoreSelectedItemsChanges;

	private bool _skipSyncFromSelectedItems;

	private bool _isResetting;

	public IList WritableSelectedItems
	{
		get
		{
			if (_writableSelectedItems == null)
			{
				_writableSelectedItems = new AvaloniaList<object>();
				SubscribeToSelectedItems();
			}
			return _writableSelectedItems;
		}
		[param: AllowNull]
		set
		{
			if (value == null)
			{
				value = new AvaloniaList<object>();
			}
			if (value.IsFixedSize)
			{
				throw new NotSupportedException("Cannot assign fixed size selection to SelectedItems.");
			}
			if (_writableSelectedItems != value)
			{
				UnsubscribeFromSelectedItems();
				_writableSelectedItems = value;
				SyncFromSelectedItems();
				SubscribeToSelectedItems();
				if (base.ItemsView == null)
				{
					SetInitSelectedItems(value);
				}
				RaisePropertyChanged("WritableSelectedItems");
			}
		}
	}

	public InternalSelectionModel()
	{
		base.SelectionChanged += OnSelectionChanged;
		base.SourceReset += OnSourceReset;
	}

	internal void Update(IEnumerable? source, Optional<IList?> selectedItems)
	{
		IEnumerable source2 = base.Source;
		IList writableSelectedItems = _writableSelectedItems;
		OnSourceCollectionChangeStarted();
		try
		{
			_skipSyncFromSelectedItems = true;
			SetSource(source);
			if (selectedItems.HasValue)
			{
				WritableSelectedItems = selectedItems.Value;
			}
		}
		finally
		{
			_skipSyncFromSelectedItems = false;
		}
		if (writableSelectedItems != _writableSelectedItems)
		{
			base.OnSourceCollectionChangeFinished();
			SyncFromSelectedItems();
		}
		else if (source2 != base.Source)
		{
			SyncFromSelectedItems();
			base.OnSourceCollectionChangeFinished();
		}
		else
		{
			base.OnSourceCollectionChangeFinished();
		}
	}

	private protected override void SetSource(IEnumerable? value)
	{
		if (base.Source != value)
		{
			object[] array = null;
			if (base.Source != null && value != null)
			{
				array = new object[WritableSelectedItems.Count];
				WritableSelectedItems.CopyTo(array, 0);
			}
			try
			{
				_ignoreSelectedItemsChanges = true;
				_ignoreModelChanges++;
				base.SetSource(value);
			}
			finally
			{
				_ignoreModelChanges--;
				_ignoreSelectedItemsChanges = false;
			}
			if (array == null)
			{
				SyncToSelectedItems();
			}
			else
			{
				SyncFromSelectedItems();
			}
		}
	}

	private void SyncToSelectedItems()
	{
		if (_writableSelectedItems == null || SequenceEqual(_writableSelectedItems, base.SelectedItems))
		{
			return;
		}
		try
		{
			_ignoreSelectedItemsChanges = true;
			_writableSelectedItems.Clear();
			foreach (object selectedItem in base.SelectedItems)
			{
				_writableSelectedItems.Add(selectedItem);
			}
		}
		finally
		{
			_ignoreSelectedItemsChanges = false;
		}
	}

	private void SyncFromSelectedItems()
	{
		if (_skipSyncFromSelectedItems || base.Source == null || _writableSelectedItems == null)
		{
			return;
		}
		try
		{
			_ignoreModelChanges++;
			using (BatchUpdate())
			{
				Clear();
				for (int i = 0; i < _writableSelectedItems.Count; i++)
				{
					int num = IndexOf(base.Source, _writableSelectedItems[i]);
					if (num != -1)
					{
						Select(num);
						continue;
					}
					_writableSelectedItems.RemoveAt(i);
					i--;
				}
			}
		}
		finally
		{
			_ignoreModelChanges--;
		}
	}

	private void SubscribeToSelectedItems()
	{
		if (_writableSelectedItems is INotifyCollectionChanged notifyCollectionChanged)
		{
			notifyCollectionChanged.CollectionChanged += OnSelectedItemsCollectionChanged;
		}
	}

	private void UnsubscribeFromSelectedItems()
	{
		if (_writableSelectedItems is INotifyCollectionChanged notifyCollectionChanged)
		{
			notifyCollectionChanged.CollectionChanged -= OnSelectedItemsCollectionChanged;
		}
	}

	private void OnSelectionChanged(object? sender, SelectionModelSelectionChangedEventArgs e)
	{
		if (_ignoreModelChanges > 0)
		{
			return;
		}
		try
		{
			IList writableSelectedItems = WritableSelectedItems;
			object?[] array = e.DeselectedItems.ToArray();
			object[] array2 = e.SelectedItems.ToArray();
			_ignoreSelectedItemsChanges = true;
			object[] array3 = array;
			foreach (object value in array3)
			{
				writableSelectedItems.Remove(value);
			}
			array3 = array2;
			foreach (object value2 in array3)
			{
				writableSelectedItems.Add(value2);
			}
		}
		finally
		{
			_ignoreSelectedItemsChanges = false;
		}
	}

	protected override void OnSourceCollectionChanged(NotifyCollectionChangedEventArgs e)
	{
		if (e.Action == NotifyCollectionChangedAction.Reset)
		{
			_ignoreModelChanges++;
			_isResetting = true;
		}
		base.OnSourceCollectionChanged(e);
	}

	protected override void OnSourceCollectionChangeFinished()
	{
		base.OnSourceCollectionChangeFinished();
		if (_isResetting)
		{
			_ignoreModelChanges--;
			_isResetting = false;
		}
	}

	private void OnSourceReset(object? sender, EventArgs e)
	{
		SyncFromSelectedItems();
	}

	private void OnSelectedItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		if (_ignoreSelectedItemsChanges)
		{
			return;
		}
		if (_writableSelectedItems == null)
		{
			throw new AvaloniaInternalException("CollectionChanged raised but we don't have items.");
		}
		try
		{
			using (BatchUpdate())
			{
				_ignoreModelChanges++;
				switch (e.Action)
				{
				case NotifyCollectionChangedAction.Add:
					Add(e.NewItems);
					break;
				case NotifyCollectionChangedAction.Remove:
					Remove();
					break;
				case NotifyCollectionChangedAction.Replace:
					Remove();
					Add(e.NewItems);
					break;
				case NotifyCollectionChangedAction.Reset:
					Clear();
					Add(_writableSelectedItems);
					break;
				case NotifyCollectionChangedAction.Move:
					break;
				}
			}
		}
		finally
		{
			_ignoreModelChanges--;
		}
		void Remove()
		{
			foreach (object oldItem in e.OldItems)
			{
				int num = IndexOf(base.Source, oldItem);
				if (num != -1)
				{
					Deselect(num);
				}
			}
		}
	}

	private void Add(IList newItems)
	{
		foreach (object newItem in newItems)
		{
			int num = IndexOf(base.Source, newItem);
			if (num != -1)
			{
				Select(num);
			}
		}
	}

	private static int IndexOf(object? source, object? item)
	{
		if (source is IList list)
		{
			return list.IndexOf(item);
		}
		if (source is ItemsSourceView itemsSourceView)
		{
			return itemsSourceView.IndexOf(item);
		}
		return -1;
	}

	private static bool SequenceEqual(IList first, IReadOnlyList<object?> second)
	{
		if (first is IEnumerable<object> first2)
		{
			return first2.SequenceEqual<object>(second);
		}
		EqualityComparer<object> @default = EqualityComparer<object>.Default;
		IEnumerator enumerator = first.GetEnumerator();
		using IEnumerator<object> enumerator2 = second.GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (!enumerator2.MoveNext() || !@default.Equals(enumerator.Current, enumerator2.Current))
			{
				return false;
			}
		}
		return !enumerator2.MoveNext();
	}
}
