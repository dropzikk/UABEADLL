using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Avalonia.Controls.Utils;

namespace Avalonia.Controls;

public class ItemsSourceView : IReadOnlyList<object?>, IEnumerable<object?>, IEnumerable, IReadOnlyCollection<object?>, IList, ICollection, INotifyCollectionChanged, ICollectionChangedListener
{
	private IList _source;

	private NotifyCollectionChangedEventHandler? _collectionChanged;

	private NotifyCollectionChangedEventHandler? _preCollectionChanged;

	private NotifyCollectionChangedEventHandler? _postCollectionChanged;

	private bool _listening;

	public static ItemsSourceView Empty { get; } = new ItemsSourceView(Array.Empty<object>());

	public int Count => Source.Count;

	public IList Source => _source;

	public object? this[int index] => GetAt(index);

	bool IList.IsFixedSize => false;

	bool IList.IsReadOnly => true;

	bool ICollection.IsSynchronized => false;

	object ICollection.SyncRoot => this;

	object? IList.this[int index]
	{
		get
		{
			return GetAt(index);
		}
		set
		{
			ThrowReadOnly();
		}
	}

	internal bool HasKeyIndexMapping => false;

	public event NotifyCollectionChangedEventHandler? CollectionChanged
	{
		add
		{
			AddListenerIfNecessary();
			_collectionChanged = (NotifyCollectionChangedEventHandler)Delegate.Combine(_collectionChanged, value);
		}
		remove
		{
			_collectionChanged = (NotifyCollectionChangedEventHandler)Delegate.Remove(_collectionChanged, value);
			RemoveListenerIfNecessary();
		}
	}

	internal event NotifyCollectionChangedEventHandler? PreCollectionChanged
	{
		add
		{
			AddListenerIfNecessary();
			_preCollectionChanged = (NotifyCollectionChangedEventHandler)Delegate.Combine(_preCollectionChanged, value);
		}
		remove
		{
			_preCollectionChanged = (NotifyCollectionChangedEventHandler)Delegate.Remove(_preCollectionChanged, value);
			RemoveListenerIfNecessary();
		}
	}

	internal event NotifyCollectionChangedEventHandler? PostCollectionChanged
	{
		add
		{
			AddListenerIfNecessary();
			_postCollectionChanged = (NotifyCollectionChangedEventHandler)Delegate.Combine(_postCollectionChanged, value);
		}
		remove
		{
			_postCollectionChanged = (NotifyCollectionChangedEventHandler)Delegate.Remove(_postCollectionChanged, value);
			RemoveListenerIfNecessary();
		}
	}

	private protected ItemsSourceView(IEnumerable source)
	{
		SetSource(source);
	}

	public object? GetAt(int index)
	{
		return Source[index];
	}

	public bool Contains(object? item)
	{
		return Source.Contains(item);
	}

	public int IndexOf(object? item)
	{
		return Source.IndexOf(item);
	}

	public static ItemsSourceView GetOrCreate(IEnumerable? items)
	{
		if (!(items is ItemsSourceView result))
		{
			if (items == null)
			{
				return Empty;
			}
			return new ItemsSourceView(items);
		}
		return result;
	}

	public static ItemsSourceView<T> GetOrCreate<T>(IEnumerable? items)
	{
		if (!(items is ItemsSourceView<T> result))
		{
			if (!(items is ItemsSourceView itemsSourceView))
			{
				if (items == null)
				{
					return ItemsSourceView<T>.Empty;
				}
				return new ItemsSourceView<T>(items);
			}
			return new ItemsSourceView<T>(itemsSourceView.Source);
		}
		return result;
	}

	public static ItemsSourceView<T> GetOrCreate<T>(IEnumerable<T>? items)
	{
		if (!(items is ItemsSourceView<T> result))
		{
			if (items == null)
			{
				return ItemsSourceView<T>.Empty;
			}
			return new ItemsSourceView<T>(items);
		}
		return result;
	}

	public IEnumerator<object?> GetEnumerator()
	{
		IList source = Source;
		if (source is IEnumerable<object> enumerable)
		{
			return enumerable.GetEnumerator();
		}
		return EnumerateItems(source);
		static IEnumerator<object> EnumerateItems(IList list)
		{
			foreach (object item in list)
			{
				yield return item;
			}
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return Source.GetEnumerator();
	}

	void ICollectionChangedListener.PreChanged(INotifyCollectionChanged sender, NotifyCollectionChangedEventArgs e)
	{
		_preCollectionChanged?.Invoke(this, e);
	}

	void ICollectionChangedListener.Changed(INotifyCollectionChanged sender, NotifyCollectionChangedEventArgs e)
	{
		_collectionChanged?.Invoke(this, e);
	}

	void ICollectionChangedListener.PostChanged(INotifyCollectionChanged sender, NotifyCollectionChangedEventArgs e)
	{
		_postCollectionChanged?.Invoke(this, e);
	}

	int IList.Add(object? value)
	{
		return ThrowReadOnly();
	}

	void IList.Clear()
	{
		ThrowReadOnly();
	}

	void IList.Insert(int index, object? value)
	{
		ThrowReadOnly();
	}

	void IList.Remove(object? value)
	{
		ThrowReadOnly();
	}

	void IList.RemoveAt(int index)
	{
		ThrowReadOnly();
	}

	void ICollection.CopyTo(Array array, int index)
	{
		Source.CopyTo(array, index);
	}

	internal string KeyFromIndex(int index)
	{
		throw new NotImplementedException();
	}

	private protected void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e)
	{
		_preCollectionChanged?.Invoke(this, e);
		_collectionChanged?.Invoke(this, e);
		_postCollectionChanged?.Invoke(this, e);
	}

	[MemberNotNull("_source")]
	private protected void SetSource(IEnumerable source)
	{
		if (_listening && _source is INotifyCollectionChanged collection)
		{
			CollectionChangedEventManager.Instance.RemoveListener(collection, this);
		}
		IList source2;
		if (!(source is ItemsSourceView itemsSourceView))
		{
			if (!(source is IList list))
			{
				if (source is INotifyCollectionChanged)
				{
					throw new ArgumentException("Collection implements INotifyCollectionChanged but not IList.", "source");
				}
				if (!(source is IEnumerable<object> collection2))
				{
					if (source == null)
					{
						throw new ArgumentNullException("source");
					}
					source2 = new List<object>(source.Cast<object>());
				}
				else
				{
					source2 = new List<object>(collection2);
				}
			}
			else
			{
				source2 = list;
			}
		}
		else
		{
			source2 = itemsSourceView.Source;
		}
		_source = source2;
		if (_listening && _source is INotifyCollectionChanged collection3)
		{
			CollectionChangedEventManager.Instance.AddListener(collection3, this);
		}
	}

	private void AddListenerIfNecessary()
	{
		if (!_listening)
		{
			if (_source is INotifyCollectionChanged collection)
			{
				CollectionChangedEventManager.Instance.AddListener(collection, this);
			}
			_listening = true;
		}
	}

	private void RemoveListenerIfNecessary()
	{
		if (_listening && _collectionChanged == null && _postCollectionChanged == null)
		{
			if (_source is INotifyCollectionChanged collection)
			{
				CollectionChangedEventManager.Instance.RemoveListener(collection, this);
			}
			_listening = false;
		}
	}

	[DoesNotReturn]
	private static int ThrowReadOnly()
	{
		throw new NotSupportedException("Collection is read-only.");
	}
}
public sealed class ItemsSourceView<T> : ItemsSourceView, IReadOnlyList<T>, IEnumerable<T>, IEnumerable, IReadOnlyCollection<T>
{
	public new static ItemsSourceView<T> Empty { get; } = new ItemsSourceView<T>(Array.Empty<T>());

	public new T this[int index] => GetAt(index);

	internal ItemsSourceView(IEnumerable<T> source)
		: base(source)
	{
	}

	internal ItemsSourceView(IEnumerable source)
		: base(source)
	{
	}

	public new T GetAt(int index)
	{
		return (T)base.Source[index];
	}

	public new IEnumerator<T> GetEnumerator()
	{
		IList source = base.Source;
		if (source is IEnumerable<T> enumerable)
		{
			return enumerable.GetEnumerator();
		}
		return EnumerateItems(source);
		static IEnumerator<T> EnumerateItems(IList list)
		{
			foreach (object item in list)
			{
				yield return (T)item;
			}
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return base.Source.GetEnumerator();
	}
}
