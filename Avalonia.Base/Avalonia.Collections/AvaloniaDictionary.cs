using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Avalonia.Collections;

public class AvaloniaDictionary<TKey, TValue> : IAvaloniaDictionary<TKey, TValue>, IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, IAvaloniaReadOnlyDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>, IReadOnlyCollection<KeyValuePair<TKey, TValue>>, INotifyCollectionChanged, INotifyPropertyChanged, IDictionary, ICollection where TKey : notnull
{
	private Dictionary<TKey, TValue> _inner;

	public int Count => _inner.Count;

	public bool IsReadOnly => false;

	public ICollection<TKey> Keys => _inner.Keys;

	public ICollection<TValue> Values => _inner.Values;

	bool IDictionary.IsFixedSize => ((IDictionary)_inner).IsFixedSize;

	ICollection IDictionary.Keys => ((IDictionary)_inner).Keys;

	ICollection IDictionary.Values => ((IDictionary)_inner).Values;

	bool ICollection.IsSynchronized => ((ICollection)_inner).IsSynchronized;

	object ICollection.SyncRoot => ((ICollection)_inner).SyncRoot;

	IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => _inner.Keys;

	IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => _inner.Values;

	public TValue this[TKey key]
	{
		get
		{
			return _inner[key];
		}
		set
		{
			TValue value2;
			bool num = _inner.TryGetValue(key, out value2);
			_inner[key] = value;
			if (num)
			{
				this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs($"{"Item"}[{key}]"));
				if (this.CollectionChanged != null)
				{
					NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, new KeyValuePair<TKey, TValue>(key, value), new KeyValuePair<TKey, TValue>(key, value2));
					this.CollectionChanged(this, e);
				}
			}
			else
			{
				NotifyAdd(key, value);
			}
		}
	}

	object? IDictionary.this[object key]
	{
		get
		{
			return ((IDictionary)_inner)[key];
		}
		set
		{
			((IDictionary)_inner)[key] = value;
		}
	}

	public event NotifyCollectionChangedEventHandler? CollectionChanged;

	public event PropertyChangedEventHandler? PropertyChanged;

	public AvaloniaDictionary()
	{
		_inner = new Dictionary<TKey, TValue>();
	}

	public AvaloniaDictionary(int capacity)
	{
		_inner = new Dictionary<TKey, TValue>(capacity);
	}

	public void Add(TKey key, TValue value)
	{
		_inner.Add(key, value);
		NotifyAdd(key, value);
	}

	public void Clear()
	{
		Dictionary<TKey, TValue> inner = _inner;
		_inner = new Dictionary<TKey, TValue>();
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item"));
		if (this.CollectionChanged != null)
		{
			NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, inner.ToArray(), -1);
			this.CollectionChanged(this, e);
		}
	}

	public bool ContainsKey(TKey key)
	{
		return _inner.ContainsKey(key);
	}

	public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
	{
		((ICollection<KeyValuePair<TKey, TValue>>)_inner).CopyTo(array, arrayIndex);
	}

	public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
	{
		return _inner.GetEnumerator();
	}

	public bool Remove(TKey key)
	{
		if (_inner.TryGetValue(key, out var value))
		{
			_inner.Remove(key);
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs($"{"Item"}[{key}]"));
			if (this.CollectionChanged != null)
			{
				NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new KeyValuePair<TKey, TValue>[1]
				{
					new KeyValuePair<TKey, TValue>(key, value)
				}, -1);
				this.CollectionChanged(this, e);
			}
			return true;
		}
		return false;
	}

	public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
	{
		return _inner.TryGetValue(key, out value);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return _inner.GetEnumerator();
	}

	void ICollection.CopyTo(Array array, int index)
	{
		((ICollection)_inner).CopyTo(array, index);
	}

	void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
	{
		Add(item.Key, item.Value);
	}

	bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
	{
		return _inner.Contains(item);
	}

	bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
	{
		return Remove(item.Key);
	}

	void IDictionary.Add(object key, object? value)
	{
		Add((TKey)key, (TValue)value);
	}

	bool IDictionary.Contains(object key)
	{
		return ((IDictionary)_inner).Contains(key);
	}

	IDictionaryEnumerator IDictionary.GetEnumerator()
	{
		return ((IDictionary)_inner).GetEnumerator();
	}

	void IDictionary.Remove(object key)
	{
		Remove((TKey)key);
	}

	private void NotifyAdd(TKey key, TValue value)
	{
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs($"{"Item"}[{key}]"));
		if (this.CollectionChanged != null)
		{
			NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>[1]
			{
				new KeyValuePair<TKey, TValue>(key, value)
			}, -1);
			this.CollectionChanged(this, e);
		}
	}
}
