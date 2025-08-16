using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Collections;

namespace Avalonia.Controls;

public class ItemCollection : ItemsSourceView, IList, ICollection, IEnumerable
{
	private enum Mode
	{
		Items,
		ItemsSource
	}

	private static readonly object?[] s_uninitialized = new object[0];

	private Mode _mode;

	public new object? this[int index]
	{
		get
		{
			return base[index];
		}
		set
		{
			WritableSource[index] = value;
		}
	}

	public bool IsReadOnly => _mode == Mode.ItemsSource;

	private IList WritableSource
	{
		get
		{
			if (IsReadOnly)
			{
				ThrowIsItemsSource();
			}
			if (base.Source == s_uninitialized)
			{
				SetSource(CreateDefaultCollection());
			}
			return base.Source;
		}
	}

	internal event EventHandler? SourceChanged;

	internal ItemCollection()
		: base(s_uninitialized)
	{
	}

	public int Add(object? value)
	{
		return WritableSource.Add(value);
	}

	public void Clear()
	{
		WritableSource.Clear();
	}

	public void Insert(int index, object? value)
	{
		WritableSource.Insert(index, value);
	}

	public void RemoveAt(int index)
	{
		WritableSource.RemoveAt(index);
	}

	public bool Remove(object? value)
	{
		int count = base.Count;
		WritableSource.Remove(value);
		return base.Count < count;
	}

	int IList.Add(object? value)
	{
		return Add(value);
	}

	void IList.Clear()
	{
		Clear();
	}

	void IList.Insert(int index, object? value)
	{
		Insert(index, value);
	}

	void IList.RemoveAt(int index)
	{
		RemoveAt(index);
	}

	internal void SetItemsSource(IEnumerable? value)
	{
		if (_mode != Mode.ItemsSource && base.Count > 0)
		{
			throw new InvalidOperationException("Items collection must be empty before using ItemsSource.");
		}
		_mode = ((value != null) ? Mode.ItemsSource : Mode.Items);
		SetSource(value ?? CreateDefaultCollection());
	}

	private new void SetSource(IEnumerable source)
	{
		IList source2 = base.Source;
		base.SetSource(source);
		if (source2.Count > 0)
		{
			RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, source2, 0));
		}
		if (base.Source.Count > 0)
		{
			RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, base.Source, 0));
		}
		this.SourceChanged?.Invoke(this, EventArgs.Empty);
	}

	private static AvaloniaList<object?> CreateDefaultCollection()
	{
		return new AvaloniaList<object>
		{
			ResetBehavior = ResetBehavior.Remove
		};
	}

	[DoesNotReturn]
	private static void ThrowIsItemsSource()
	{
		throw new InvalidOperationException("Operation is not valid while ItemsSource is in use.Access and modify elements with ItemsControl.ItemsSource instead.");
	}
}
