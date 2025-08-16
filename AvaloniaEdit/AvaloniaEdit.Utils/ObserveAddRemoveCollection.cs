using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AvaloniaEdit.Utils;

internal sealed class ObserveAddRemoveCollection<T> : Collection<T>
{
	private readonly Action<T> _onAdd;

	private readonly Action<T> _onRemove;

	public ObserveAddRemoveCollection(Action<T> onAdd, Action<T> onRemove)
	{
		_onAdd = onAdd ?? throw new ArgumentNullException("onAdd");
		_onRemove = onRemove ?? throw new ArgumentNullException("onRemove");
	}

	protected override void ClearItems()
	{
		if (_onRemove != null)
		{
			using IEnumerator<T> enumerator = GetEnumerator();
			while (enumerator.MoveNext())
			{
				T current = enumerator.Current;
				_onRemove(current);
			}
		}
		base.ClearItems();
	}

	protected override void InsertItem(int index, T item)
	{
		_onAdd?.Invoke(item);
		base.InsertItem(index, item);
	}

	protected override void RemoveItem(int index)
	{
		_onRemove?.Invoke(base[index]);
		base.RemoveItem(index);
	}

	protected override void SetItem(int index, T item)
	{
		_onRemove?.Invoke(base[index]);
		try
		{
			_onAdd?.Invoke(item);
		}
		catch
		{
			RemoveAt(index);
			throw;
		}
		base.SetItem(index, item);
	}
}
