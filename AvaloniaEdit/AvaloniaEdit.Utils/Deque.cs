using System;
using System.Collections;
using System.Collections.Generic;

namespace AvaloniaEdit.Utils;

internal sealed class Deque<T> : ICollection<T>, IEnumerable<T>, IEnumerable
{
	private T[] _arr = Empty<T>.Array;

	private int _head;

	private int _tail;

	public int Count { get; private set; }

	public T this[int index]
	{
		get
		{
			ThrowUtil.CheckInRangeInclusive(index, "index", 0, Count - 1);
			return _arr[(_head + index) % _arr.Length];
		}
		set
		{
			ThrowUtil.CheckInRangeInclusive(index, "index", 0, Count - 1);
			_arr[(_head + index) % _arr.Length] = value;
		}
	}

	bool ICollection<T>.IsReadOnly => false;

	public void Clear()
	{
		_arr = Empty<T>.Array;
		Count = 0;
		_head = 0;
		_tail = 0;
	}

	public void PushBack(T item)
	{
		if (Count == _arr.Length)
		{
			SetCapacity(Math.Max(4, _arr.Length * 2));
		}
		_arr[_tail++] = item;
		if (_tail == _arr.Length)
		{
			_tail = 0;
		}
		Count++;
	}

	public T PopBack()
	{
		if (Count == 0)
		{
			throw new InvalidOperationException();
		}
		if (_tail == 0)
		{
			_tail = _arr.Length - 1;
		}
		else
		{
			_tail--;
		}
		T result = _arr[_tail];
		_arr[_tail] = default(T);
		Count--;
		return result;
	}

	public void PushFront(T item)
	{
		if (Count == _arr.Length)
		{
			SetCapacity(Math.Max(4, _arr.Length * 2));
		}
		if (_head == 0)
		{
			_head = _arr.Length - 1;
		}
		else
		{
			_head--;
		}
		_arr[_head] = item;
		Count++;
	}

	public T PopFront()
	{
		if (Count == 0)
		{
			throw new InvalidOperationException();
		}
		T result = _arr[_head];
		_arr[_head] = default(T);
		_head++;
		if (_head == _arr.Length)
		{
			_head = 0;
		}
		Count--;
		return result;
	}

	private void SetCapacity(int capacity)
	{
		T[] array = new T[capacity];
		CopyTo(array, 0);
		_head = 0;
		_tail = ((Count != capacity) ? Count : 0);
		_arr = array;
	}

	public IEnumerator<T> GetEnumerator()
	{
		if (_head < _tail)
		{
			for (int i = _head; i < _tail; i++)
			{
				yield return _arr[i];
			}
			yield break;
		}
		for (int i = _head; i < _arr.Length; i++)
		{
			yield return _arr[i];
		}
		for (int i = 0; i < _tail; i++)
		{
			yield return _arr[i];
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	void ICollection<T>.Add(T item)
	{
		PushBack(item);
	}

	public bool Contains(T item)
	{
		EqualityComparer<T> @default = EqualityComparer<T>.Default;
		using (IEnumerator<T> enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				T current = enumerator.Current;
				if (@default.Equals(item, current))
				{
					return true;
				}
			}
		}
		return false;
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (_head < _tail)
		{
			Array.Copy(_arr, _head, array, arrayIndex, _tail - _head);
			return;
		}
		int num = _arr.Length - _head;
		Array.Copy(_arr, _head, array, arrayIndex, num);
		Array.Copy(_arr, 0, array, arrayIndex + num, _tail);
	}

	bool ICollection<T>.Remove(T item)
	{
		throw new NotSupportedException();
	}
}
