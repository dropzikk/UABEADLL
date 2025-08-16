using System;
using System.Collections;
using System.Collections.Generic;

namespace Avalonia.Utilities;

internal class SafeEnumerableList<T> : IEnumerable<T>, IEnumerable
{
	public struct Enumerator : IEnumerator<T>, IEnumerator, IDisposable
	{
		private readonly SafeEnumerableList<T> _owner;

		private readonly List<T> _list;

		private readonly int _generation;

		private int _index;

		private T? _current;

		public T Current => _current;

		object? IEnumerator.Current => _current;

		internal Enumerator(SafeEnumerableList<T> owner, List<T> list)
		{
			_owner = owner;
			_list = list;
			_generation = owner._generation;
			_index = 0;
			_current = default(T);
			_owner._enumCount++;
		}

		public void Dispose()
		{
			if (_owner._generation == _generation)
			{
				_owner._enumCount--;
			}
		}

		public bool MoveNext()
		{
			if (_index < _list.Count)
			{
				_current = _list[_index++];
				return true;
			}
			_current = default(T);
			return false;
		}

		void IEnumerator.Reset()
		{
			_index = 0;
			_current = default(T);
		}
	}

	private List<T> _list = new List<T>();

	private int _generation;

	private int _enumCount;

	public int Count => _list.Count;

	internal List<T> Inner => _list;

	public void Add(T item)
	{
		GetList().Add(item);
	}

	public bool Remove(T item)
	{
		return GetList().Remove(item);
	}

	public Enumerator GetEnumerator()
	{
		return new Enumerator(this, _list);
	}

	IEnumerator<T> IEnumerable<T>.GetEnumerator()
	{
		return GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	private List<T> GetList()
	{
		if (_enumCount > 0)
		{
			_list = new List<T>(_list);
			_generation++;
			_enumCount = 0;
		}
		return _list;
	}
}
