using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Avalonia.Reactive;

internal sealed class CompositeDisposable : ICollection<IDisposable>, IEnumerable<IDisposable>, IEnumerable, IDisposable
{
	private sealed class CompositeEnumerator : IEnumerator<IDisposable>, IEnumerator, IDisposable
	{
		private readonly IDisposable?[] _disposables;

		private int _index;

		public IDisposable Current => _disposables[_index];

		object IEnumerator.Current => _disposables[_index];

		public CompositeEnumerator(IDisposable?[] disposables)
		{
			_disposables = disposables;
			_index = -1;
		}

		public void Dispose()
		{
			IDisposable[] disposables = _disposables;
			Array.Clear(disposables, 0, disposables.Length);
		}

		public bool MoveNext()
		{
			IDisposable[] disposables = _disposables;
			int num;
			do
			{
				num = ++_index;
				if (num >= disposables.Length)
				{
					return false;
				}
			}
			while (disposables[num] == null);
			return true;
		}

		public void Reset()
		{
			_index = -1;
		}
	}

	private readonly object _gate = new object();

	private bool _disposed;

	private List<IDisposable?> _disposables;

	private int _count;

	private const int ShrinkThreshold = 64;

	private static readonly CompositeEnumerator EmptyEnumerator = new CompositeEnumerator(Array.Empty<IDisposable>());

	public int Count => Volatile.Read(ref _count);

	public bool IsReadOnly => false;

	public bool IsDisposed => Volatile.Read(ref _disposed);

	public CompositeDisposable(int capacity)
	{
		if (capacity < 0)
		{
			throw new ArgumentOutOfRangeException("capacity");
		}
		_disposables = new List<IDisposable>(capacity);
	}

	public CompositeDisposable(params IDisposable[] disposables)
	{
		if (disposables == null)
		{
			throw new ArgumentNullException("disposables");
		}
		_disposables = ToList(disposables);
		Volatile.Write(ref _count, _disposables.Count);
	}

	public CompositeDisposable(IList<IDisposable> disposables)
	{
		if (disposables == null)
		{
			throw new ArgumentNullException("disposables");
		}
		_disposables = ToList(disposables);
		Volatile.Write(ref _count, _disposables.Count);
	}

	private static List<IDisposable?> ToList(IEnumerable<IDisposable> disposables)
	{
		int capacity = ((disposables is IDisposable[] array) ? array.Length : ((!(disposables is ICollection<IDisposable> collection)) ? 12 : collection.Count));
		List<IDisposable> list = new List<IDisposable>(capacity);
		foreach (IDisposable disposable in disposables)
		{
			if (disposable == null)
			{
				throw new ArgumentException("Disposables can't contain null", "disposables");
			}
			list.Add(disposable);
		}
		return list;
	}

	public void Add(IDisposable item)
	{
		if (item == null)
		{
			throw new ArgumentNullException("item");
		}
		lock (_gate)
		{
			if (!_disposed)
			{
				_disposables.Add(item);
				Volatile.Write(ref _count, _count + 1);
				return;
			}
		}
		item.Dispose();
	}

	public bool Remove(IDisposable item)
	{
		if (item == null)
		{
			throw new ArgumentNullException("item");
		}
		lock (_gate)
		{
			if (_disposed)
			{
				return false;
			}
			List<IDisposable> disposables = _disposables;
			int num = disposables.IndexOf(item);
			if (num < 0)
			{
				return false;
			}
			disposables[num] = null;
			if (disposables.Capacity > 64 && _count < disposables.Capacity / 2)
			{
				List<IDisposable> list = new List<IDisposable>(disposables.Capacity / 2);
				foreach (IDisposable item2 in disposables)
				{
					if (item2 != null)
					{
						list.Add(item2);
					}
				}
				_disposables = list;
			}
			Volatile.Write(ref _count, _count - 1);
		}
		item.Dispose();
		return true;
	}

	public void Dispose()
	{
		List<IDisposable> list = null;
		lock (_gate)
		{
			if (!_disposed)
			{
				list = _disposables;
				_disposables = null;
				Volatile.Write(ref _count, 0);
				Volatile.Write(ref _disposed, value: true);
			}
		}
		if (list == null)
		{
			return;
		}
		foreach (IDisposable item in list)
		{
			item?.Dispose();
		}
	}

	public void Clear()
	{
		IDisposable[] array;
		lock (_gate)
		{
			if (_disposed)
			{
				return;
			}
			List<IDisposable?> disposables = _disposables;
			array = disposables.ToArray();
			disposables.Clear();
			Volatile.Write(ref _count, 0);
		}
		IDisposable[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i]?.Dispose();
		}
	}

	public bool Contains(IDisposable item)
	{
		if (item == null)
		{
			throw new ArgumentNullException("item");
		}
		lock (_gate)
		{
			if (_disposed)
			{
				return false;
			}
			return _disposables.Contains(item);
		}
	}

	public void CopyTo(IDisposable[] array, int arrayIndex)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (arrayIndex < 0 || arrayIndex >= array.Length)
		{
			throw new ArgumentOutOfRangeException("arrayIndex");
		}
		lock (_gate)
		{
			if (_disposed)
			{
				return;
			}
			if (arrayIndex + _count > array.Length)
			{
				throw new ArgumentOutOfRangeException("arrayIndex");
			}
			int num = arrayIndex;
			foreach (IDisposable disposable in _disposables)
			{
				if (disposable != null)
				{
					array[num++] = disposable;
				}
			}
		}
	}

	public IEnumerator<IDisposable> GetEnumerator()
	{
		lock (_gate)
		{
			if (_disposed || _count == 0)
			{
				return EmptyEnumerator;
			}
			return new CompositeEnumerator(_disposables.ToArray());
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
