using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Avalonia.Diagnostics;

namespace Avalonia.Collections;

public class AvaloniaList<T> : IAvaloniaList<T>, IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IAvaloniaReadOnlyList<T>, IReadOnlyList<T>, IReadOnlyCollection<T>, INotifyCollectionChanged, INotifyPropertyChanged, IList, ICollection, INotifyCollectionChangedDebug
{
	public struct Enumerator : IEnumerator<T>, IEnumerator, IDisposable
	{
		private List<T>.Enumerator _innerEnumerator;

		public T Current => _innerEnumerator.Current;

		object? IEnumerator.Current => Current;

		public Enumerator(List<T> inner)
		{
			_innerEnumerator = inner.GetEnumerator();
		}

		public bool MoveNext()
		{
			return _innerEnumerator.MoveNext();
		}

		void IEnumerator.Reset()
		{
			((IEnumerator)_innerEnumerator).Reset();
		}

		public void Dispose()
		{
			_innerEnumerator.Dispose();
		}
	}

	private readonly List<T> _inner;

	private NotifyCollectionChangedEventHandler? _collectionChanged;

	public int Count => _inner.Count;

	public ResetBehavior ResetBehavior { get; set; }

	public Action<T>? Validate { get; set; }

	bool IList.IsFixedSize => false;

	bool IList.IsReadOnly => false;

	int ICollection.Count => _inner.Count;

	bool ICollection.IsSynchronized => false;

	object ICollection.SyncRoot => this;

	bool ICollection<T>.IsReadOnly => false;

	public T this[int index]
	{
		get
		{
			return _inner[index];
		}
		set
		{
			Validate?.Invoke(value);
			T val = _inner[index];
			if (!EqualityComparer<T>.Default.Equals(val, value))
			{
				_inner[index] = value;
				if (_collectionChanged != null)
				{
					NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, val, index);
					_collectionChanged(this, e);
				}
			}
		}
	}

	object? IList.this[int index]
	{
		get
		{
			return this[index];
		}
		set
		{
			this[index] = (T)value;
		}
	}

	public int Capacity
	{
		get
		{
			return _inner.Capacity;
		}
		set
		{
			_inner.Capacity = value;
		}
	}

	public event NotifyCollectionChangedEventHandler? CollectionChanged
	{
		add
		{
			_collectionChanged = (NotifyCollectionChangedEventHandler)Delegate.Combine(_collectionChanged, value);
		}
		remove
		{
			_collectionChanged = (NotifyCollectionChangedEventHandler)Delegate.Remove(_collectionChanged, value);
		}
	}

	public event PropertyChangedEventHandler? PropertyChanged;

	public AvaloniaList()
	{
		_inner = new List<T>();
	}

	public AvaloniaList(int capacity)
	{
		_inner = new List<T>(capacity);
	}

	public AvaloniaList(IEnumerable<T> items)
	{
		_inner = new List<T>(items);
	}

	public AvaloniaList(params T[] items)
	{
		_inner = new List<T>(items);
	}

	public virtual void Add(T item)
	{
		Validate?.Invoke(item);
		int count = _inner.Count;
		_inner.Add(item);
		NotifyAdd(item, count);
	}

	public virtual void AddRange(IEnumerable<T> items)
	{
		InsertRange(_inner.Count, items);
	}

	public virtual void Clear()
	{
		if (Count > 0)
		{
			if (_collectionChanged != null)
			{
				NotifyCollectionChangedEventArgs e = ((ResetBehavior == ResetBehavior.Reset) ? EventArgsCache.ResetCollectionChanged : new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, _inner.ToArray(), 0));
				_inner.Clear();
				_collectionChanged(this, e);
			}
			else
			{
				_inner.Clear();
			}
			NotifyCountChanged();
		}
	}

	public bool Contains(T item)
	{
		return _inner.Contains(item);
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		_inner.CopyTo(array, arrayIndex);
	}

	IEnumerator<T> IEnumerable<T>.GetEnumerator()
	{
		return new Enumerator(_inner);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return new Enumerator(_inner);
	}

	public Enumerator GetEnumerator()
	{
		return new Enumerator(_inner);
	}

	public IEnumerable<T> GetRange(int index, int count)
	{
		return _inner.GetRange(index, count);
	}

	public int IndexOf(T item)
	{
		return _inner.IndexOf(item);
	}

	public virtual void Insert(int index, T item)
	{
		Validate?.Invoke(item);
		_inner.Insert(index, item);
		NotifyAdd(item, index);
	}

	public virtual void InsertRange(int index, IEnumerable<T> items)
	{
		if (items == null)
		{
			throw new ArgumentNullException("items");
		}
		bool flag = _collectionChanged != null;
		bool flag2 = Validate != null;
		if (items is IList list)
		{
			if (list.Count <= 0)
			{
				return;
			}
			if (list is ICollection<T> collection)
			{
				if (flag2)
				{
					foreach (T item in collection)
					{
						Validate(item);
					}
				}
				_inner.InsertRange(index, collection);
				NotifyAdd(list, index);
				return;
			}
			EnsureCapacity(_inner.Count + list.Count);
			using (IEnumerator<T> enumerator2 = items.GetEnumerator())
			{
				int num = index;
				while (enumerator2.MoveNext())
				{
					T current2 = enumerator2.Current;
					if (flag2)
					{
						Validate(current2);
					}
					_inner.Insert(num++, current2);
				}
			}
			NotifyAdd(list, index);
			return;
		}
		using IEnumerator<T> enumerator3 = items.GetEnumerator();
		if (!enumerator3.MoveNext())
		{
			return;
		}
		List<T> list2 = (flag ? new List<T>() : null);
		int num2 = index;
		do
		{
			T current3 = enumerator3.Current;
			if (flag2)
			{
				Validate(current3);
			}
			_inner.Insert(num2++, current3);
			list2?.Add(current3);
		}
		while (enumerator3.MoveNext());
		if (list2 != null)
		{
			NotifyAdd(list2, index);
		}
		else
		{
			NotifyCountChanged();
		}
	}

	public void Move(int oldIndex, int newIndex)
	{
		T val = this[oldIndex];
		_inner.RemoveAt(oldIndex);
		_inner.Insert(newIndex, val);
		if (_collectionChanged != null)
		{
			NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, val, newIndex, oldIndex);
			_collectionChanged(this, e);
		}
	}

	public void MoveRange(int oldIndex, int count, int newIndex)
	{
		List<T> range = _inner.GetRange(oldIndex, count);
		int num = newIndex;
		_inner.RemoveRange(oldIndex, count);
		if (newIndex > oldIndex)
		{
			num -= count;
		}
		_inner.InsertRange(num, range);
		if (_collectionChanged != null)
		{
			NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, range, newIndex, oldIndex);
			_collectionChanged(this, e);
		}
	}

	public void EnsureCapacity(int capacity)
	{
		int capacity2 = _inner.Capacity;
		if (capacity2 < capacity)
		{
			int num = ((capacity2 == 0) ? 4 : (capacity2 * 2));
			if (num < capacity)
			{
				num = capacity;
			}
			_inner.Capacity = num;
		}
	}

	public virtual bool Remove(T item)
	{
		int num = _inner.IndexOf(item);
		if (num != -1)
		{
			_inner.RemoveAt(num);
			NotifyRemove(item, num);
			return true;
		}
		return false;
	}

	public virtual void RemoveAll(IEnumerable<T> items)
	{
		if (items == null)
		{
			throw new ArgumentNullException("items");
		}
		HashSet<T> hashSet = new HashSet<T>(items);
		int num = 0;
		for (int num2 = _inner.Count - 1; num2 >= 0; num2--)
		{
			if (hashSet.Contains(_inner[num2]))
			{
				num++;
			}
			else if (num > 0)
			{
				RemoveRange(num2 + 1, num);
				num = 0;
			}
		}
		if (num > 0)
		{
			RemoveRange(0, num);
		}
	}

	public virtual void RemoveAt(int index)
	{
		T item = _inner[index];
		_inner.RemoveAt(index);
		NotifyRemove(item, index);
	}

	public virtual void RemoveRange(int index, int count)
	{
		if (count > 0)
		{
			List<T> range = _inner.GetRange(index, count);
			_inner.RemoveRange(index, count);
			NotifyRemove(range, index);
		}
	}

	int IList.Add(object? value)
	{
		int count = Count;
		Add((T)value);
		return count;
	}

	bool IList.Contains(object? value)
	{
		return Contains((T)value);
	}

	void IList.Clear()
	{
		Clear();
	}

	int IList.IndexOf(object? value)
	{
		return IndexOf((T)value);
	}

	void IList.Insert(int index, object? value)
	{
		Insert(index, (T)value);
	}

	void IList.Remove(object? value)
	{
		Remove((T)value);
	}

	void IList.RemoveAt(int index)
	{
		RemoveAt(index);
	}

	void ICollection.CopyTo(Array array, int index)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (array.Rank != 1)
		{
			throw new ArgumentException("Multi-dimensional arrays are not supported.");
		}
		if (array.GetLowerBound(0) != 0)
		{
			throw new ArgumentException("Non-zero lower bounds are not supported.");
		}
		if (index < 0)
		{
			throw new ArgumentException("Invalid index.");
		}
		if (array.Length - index < Count)
		{
			throw new ArgumentException("The target array is too small.");
		}
		if (array is T[] array2)
		{
			_inner.CopyTo(array2, index);
			return;
		}
		Type elementType = array.GetType().GetElementType();
		Type typeFromHandle = typeof(T);
		if (!elementType.IsAssignableFrom(typeFromHandle) && !typeFromHandle.IsAssignableFrom(elementType))
		{
			throw new ArgumentException("Invalid array type");
		}
		if (!(array is object[] array3))
		{
			throw new ArgumentException("Invalid array type");
		}
		int count = _inner.Count;
		try
		{
			for (int i = 0; i < count; i++)
			{
				array3[index++] = _inner[i];
			}
		}
		catch (ArrayTypeMismatchException)
		{
			throw new ArgumentException("Invalid array type");
		}
	}

	Delegate[]? INotifyCollectionChangedDebug.GetCollectionChangedSubscribers()
	{
		return _collectionChanged?.GetInvocationList();
	}

	private void NotifyAdd(IList t, int index)
	{
		if (_collectionChanged != null)
		{
			NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, t, index);
			_collectionChanged(this, e);
		}
		NotifyCountChanged();
	}

	private void NotifyAdd(T item, int index)
	{
		if (_collectionChanged != null)
		{
			NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new T[1] { item }, index);
			_collectionChanged(this, e);
		}
		NotifyCountChanged();
	}

	private void NotifyCountChanged()
	{
		this.PropertyChanged?.Invoke(this, EventArgsCache.CountPropertyChanged);
	}

	private void NotifyRemove(IList t, int index)
	{
		if (_collectionChanged != null)
		{
			NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, t, index);
			_collectionChanged(this, e);
		}
		NotifyCountChanged();
	}

	private void NotifyRemove(T item, int index)
	{
		if (_collectionChanged != null)
		{
			NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new T[1] { item }, index);
			_collectionChanged(this, e);
		}
		NotifyCountChanged();
	}
}
