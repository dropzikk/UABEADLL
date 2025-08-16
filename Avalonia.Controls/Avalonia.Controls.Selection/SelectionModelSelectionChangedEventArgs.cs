using System;
using System.Collections.Generic;

namespace Avalonia.Controls.Selection;

public abstract class SelectionModelSelectionChangedEventArgs : EventArgs
{
	public abstract IReadOnlyList<int> DeselectedIndexes { get; }

	public abstract IReadOnlyList<int> SelectedIndexes { get; }

	public IReadOnlyList<object?> DeselectedItems => GetUntypedDeselectedItems();

	public IReadOnlyList<object?> SelectedItems => GetUntypedSelectedItems();

	protected abstract IReadOnlyList<object?> GetUntypedDeselectedItems();

	protected abstract IReadOnlyList<object?> GetUntypedSelectedItems();
}
public class SelectionModelSelectionChangedEventArgs<T> : SelectionModelSelectionChangedEventArgs
{
	private IReadOnlyList<object?>? _deselectedItems;

	private IReadOnlyList<object?>? _selectedItems;

	public override IReadOnlyList<int> DeselectedIndexes { get; }

	public override IReadOnlyList<int> SelectedIndexes { get; }

	public new IReadOnlyList<T?> DeselectedItems { get; }

	public new IReadOnlyList<T?> SelectedItems { get; }

	public SelectionModelSelectionChangedEventArgs(IReadOnlyList<int>? deselectedIndices = null, IReadOnlyList<int>? selectedIndices = null, IReadOnlyList<T?>? deselectedItems = null, IReadOnlyList<T?>? selectedItems = null)
	{
		DeselectedIndexes = deselectedIndices ?? Array.Empty<int>();
		SelectedIndexes = selectedIndices ?? Array.Empty<int>();
		DeselectedItems = deselectedItems ?? Array.Empty<T>();
		SelectedItems = selectedItems ?? Array.Empty<T>();
	}

	protected override IReadOnlyList<object?> GetUntypedDeselectedItems()
	{
		IReadOnlyList<object> readOnlyList = _deselectedItems;
		if (readOnlyList == null)
		{
			IReadOnlyList<object> obj = (DeselectedItems as IReadOnlyList<object>) ?? new SelectedItems<T>.Untyped(DeselectedItems);
			IReadOnlyList<object> readOnlyList2 = obj;
			_deselectedItems = obj;
			readOnlyList = readOnlyList2;
		}
		return readOnlyList;
	}

	protected override IReadOnlyList<object?> GetUntypedSelectedItems()
	{
		IReadOnlyList<object> readOnlyList = _selectedItems;
		if (readOnlyList == null)
		{
			IReadOnlyList<object> obj = (SelectedItems as IReadOnlyList<object>) ?? new SelectedItems<T>.Untyped(SelectedItems);
			IReadOnlyList<object> readOnlyList2 = obj;
			_selectedItems = obj;
			readOnlyList = readOnlyList2;
		}
		return readOnlyList;
	}
}
