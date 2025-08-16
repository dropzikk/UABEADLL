using System;
using System.Collections;
using System.Collections.Generic;

namespace Avalonia.Controls.Selection;

internal class SelectedIndexes<T> : IReadOnlyList<int>, IEnumerable<int>, IEnumerable, IReadOnlyCollection<int>
{
	private readonly SelectionModel<T>? _owner;

	private readonly IReadOnlyList<IndexRange>? _ranges;

	public int this[int index]
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
				return _owner.SelectedIndex;
			}
			return IndexRange.GetAt(Ranges, index);
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
			return IndexRange.GetCount(Ranges);
		}
	}

	private IReadOnlyList<IndexRange> Ranges => _ranges ?? _owner.Ranges;

	public SelectedIndexes(SelectionModel<T> owner)
	{
		_owner = owner;
	}

	public SelectedIndexes(IReadOnlyList<IndexRange> ranges)
	{
		_ranges = ranges;
	}

	public IEnumerator<int> GetEnumerator()
	{
		SelectionModel<T>? owner = _owner;
		if (owner != null && owner.SingleSelect)
		{
			return SingleSelect();
		}
		return IndexRange.EnumerateIndices(Ranges).GetEnumerator();
		IEnumerator<int> SingleSelect()
		{
			if (_owner.SelectedIndex >= 0)
			{
				yield return _owner.SelectedIndex;
			}
		}
	}

	public static SelectedIndexes<T>? Create(IReadOnlyList<IndexRange>? ranges)
	{
		if (ranges == null)
		{
			return null;
		}
		return new SelectedIndexes<T>(ranges);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
