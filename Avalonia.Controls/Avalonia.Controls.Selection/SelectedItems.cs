using System;
using System.Collections;
using System.Collections.Generic;

namespace Avalonia.Controls.Selection;

internal class SelectedItems<T> : IReadOnlyList<T?>, IEnumerable<T?>, IEnumerable, IReadOnlyCollection<T?>
{
	public class Untyped : IReadOnlyList<object?>, IEnumerable<object?>, IEnumerable, IReadOnlyCollection<object?>
	{
		private readonly IReadOnlyList<T?> _source;

		public object? this[int index] => _source[index];

		public int Count => _source.Count;

		public Untyped(IReadOnlyList<T?> source)
		{
			_source = source;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerator<object?> GetEnumerator()
		{
			foreach (T item in _source)
			{
				yield return item;
			}
		}
	}

	private readonly SelectionModel<T>? _owner;

	private readonly ItemsSourceView<T>? _items;

	private readonly IReadOnlyList<IndexRange>? _ranges;

	public T? this[int index]
	{
		get
		{
			if (index >= Count)
			{
				throw new IndexOutOfRangeException("The index was out of range.");
			}
			SelectionModel<T>? owner = _owner;
			if (owner != null && owner.SingleSelect)
			{
				return _owner.SelectedItem;
			}
			if (Items != null && Ranges != null)
			{
				return Items[IndexRange.GetAt(Ranges, index)];
			}
			return default(T);
		}
	}

	public int Count
	{
		get
		{
			SelectionModel<T>? owner = _owner;
			if (owner != null && owner.SingleSelect)
			{
				if (_owner.SelectedIndex != -1)
				{
					return 1;
				}
				return 0;
			}
			if (Ranges == null)
			{
				return 0;
			}
			return IndexRange.GetCount(Ranges);
		}
	}

	private ItemsSourceView<T>? Items
	{
		get
		{
			ItemsSourceView<T>? itemsSourceView = _items;
			if (itemsSourceView == null)
			{
				SelectionModel<T>? owner = _owner;
				if (owner == null)
				{
					return null;
				}
				itemsSourceView = owner.ItemsView;
			}
			return itemsSourceView;
		}
	}

	private IReadOnlyList<IndexRange>? Ranges => _ranges ?? _owner.Ranges;

	public SelectedItems(SelectionModel<T> owner)
	{
		_owner = owner;
	}

	public SelectedItems(IReadOnlyList<IndexRange> ranges, ItemsSourceView<T>? items)
	{
		_ranges = ranges ?? throw new ArgumentNullException("ranges");
		_items = items;
	}

	public IEnumerator<T?> GetEnumerator()
	{
		if (_owner?.SingleSelect ?? false)
		{
			if (_owner.SelectedIndex >= 0)
			{
				yield return _owner.SelectedItem;
			}
			yield break;
		}
		ItemsSourceView<T> items = Items;
		foreach (IndexRange range in Ranges)
		{
			int i = range.Begin;
			while (i <= range.End)
			{
				yield return (items != null) ? items[i] : default(T);
				int num = i + 1;
				i = num;
			}
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public static SelectedItems<T>? Create(IReadOnlyList<IndexRange>? ranges, ItemsSourceView<T>? items)
	{
		if (ranges == null)
		{
			return null;
		}
		return new SelectedItems<T>(ranges, items);
	}
}
