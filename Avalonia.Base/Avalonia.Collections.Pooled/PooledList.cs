using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading;

namespace Avalonia.Collections.Pooled;

[Serializable]
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(ICollectionDebugView<>))]
internal class PooledList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IReadOnlyPooledList<T>, IReadOnlyList<T>, IReadOnlyCollection<T>, IList, ICollection, IDisposable, IDeserializationCallback
{
	public struct Enumerator : IEnumerator<T>, IEnumerator, IDisposable
	{
		private readonly PooledList<T> _list;

		private int _index;

		private readonly int _version;

		private T? _current;

		public T Current => _current;

		object? IEnumerator.Current
		{
			get
			{
				if (_index == 0 || _index == _list._size + 1)
				{
					ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumOpCantHappen();
				}
				return Current;
			}
		}

		internal Enumerator(PooledList<T> list)
		{
			_list = list;
			_index = 0;
			_version = list._version;
			_current = default(T);
		}

		public void Dispose()
		{
		}

		public bool MoveNext()
		{
			PooledList<T> list = _list;
			if (_version == list._version && (uint)_index < (uint)list._size)
			{
				_current = list._items[_index];
				_index++;
				return true;
			}
			return MoveNextRare();
		}

		private bool MoveNextRare()
		{
			if (_version != _list._version)
			{
				ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion();
			}
			_index = _list._size + 1;
			_current = default(T);
			return false;
		}

		void IEnumerator.Reset()
		{
			if (_version != _list._version)
			{
				ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion();
			}
			_index = 0;
			_current = default(T);
		}
	}

	private readonly struct Comparer : IComparer<T>
	{
		private readonly Func<T?, T?, int> _comparison;

		public Comparer(Func<T?, T?, int> comparison)
		{
			_comparison = comparison;
		}

		public int Compare(T? x, T? y)
		{
			return _comparison(x, y);
		}
	}

	private const int MaxArrayLength = 2146435071;

	private const int DefaultCapacity = 4;

	private static readonly T[] s_emptyArray = Array.Empty<T>();

	[NonSerialized]
	private ArrayPool<T> _pool;

	[NonSerialized]
	private object? _syncRoot;

	private T[] _items;

	private int _size;

	private int _version;

	private readonly bool _clearOnFree;

	public Span<T> Span => _items.AsSpan(0, _size);

	ReadOnlySpan<T> IReadOnlyPooledList<T>.Span => Span;

	public int Capacity
	{
		get
		{
			return _items.Length;
		}
		set
		{
			if (value < _size)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.value, ExceptionResource.ArgumentOutOfRange_SmallCapacity);
			}
			if (value == _items.Length)
			{
				return;
			}
			if (value > 0)
			{
				T[] array = _pool.Rent(value);
				if (_size > 0)
				{
					Array.Copy(_items, array, _size);
				}
				ReturnArray();
				_items = array;
			}
			else
			{
				ReturnArray();
				_size = 0;
			}
		}
	}

	public int Count => _size;

	public ClearMode ClearMode
	{
		get
		{
			if (!_clearOnFree)
			{
				return ClearMode.Never;
			}
			return ClearMode.Always;
		}
	}

	bool IList.IsFixedSize => false;

	bool ICollection<T>.IsReadOnly => false;

	bool IList.IsReadOnly => false;

	int ICollection.Count => _size;

	bool ICollection.IsSynchronized => false;

	object ICollection.SyncRoot
	{
		get
		{
			if (_syncRoot == null)
			{
				Interlocked.CompareExchange<object>(ref _syncRoot, new object(), (object)null);
			}
			return _syncRoot;
		}
	}

	public T this[int index]
	{
		get
		{
			if ((uint)index >= (uint)_size)
			{
				ThrowHelper.ThrowArgumentOutOfRange_IndexException();
			}
			return _items[index];
		}
		set
		{
			if ((uint)index >= (uint)_size)
			{
				ThrowHelper.ThrowArgumentOutOfRange_IndexException();
			}
			_items[index] = value;
			_version++;
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
			ThrowHelper.IfNullAndNullsAreIllegalThenThrow<T>(value, ExceptionArgument.value);
			try
			{
				this[index] = (T)value;
			}
			catch (InvalidCastException)
			{
				ThrowHelper.ThrowWrongValueTypeArgumentException(value, typeof(T));
			}
		}
	}

	public PooledList()
		: this(ClearMode.Auto, ArrayPool<T>.Shared)
	{
	}

	public PooledList(ClearMode clearMode)
		: this(clearMode, ArrayPool<T>.Shared)
	{
	}

	public PooledList(ArrayPool<T> customPool)
		: this(ClearMode.Auto, customPool)
	{
	}

	public PooledList(ClearMode clearMode, ArrayPool<T> customPool)
	{
		_items = s_emptyArray;
		_pool = customPool ?? ArrayPool<T>.Shared;
		_clearOnFree = ShouldClear(clearMode);
	}

	public PooledList(int capacity)
		: this(capacity, ClearMode.Auto, ArrayPool<T>.Shared)
	{
	}

	public PooledList(int capacity, bool sizeToCapacity)
		: this(capacity, ClearMode.Auto, ArrayPool<T>.Shared, sizeToCapacity)
	{
	}

	public PooledList(int capacity, ClearMode clearMode)
		: this(capacity, clearMode, ArrayPool<T>.Shared)
	{
	}

	public PooledList(int capacity, ClearMode clearMode, bool sizeToCapacity)
		: this(capacity, clearMode, ArrayPool<T>.Shared, sizeToCapacity)
	{
	}

	public PooledList(int capacity, ArrayPool<T> customPool)
		: this(capacity, ClearMode.Auto, customPool)
	{
	}

	public PooledList(int capacity, ArrayPool<T> customPool, bool sizeToCapacity)
		: this(capacity, ClearMode.Auto, customPool, sizeToCapacity)
	{
	}

	public PooledList(int capacity, ClearMode clearMode, ArrayPool<T> customPool)
		: this(capacity, clearMode, customPool, sizeToCapacity: false)
	{
	}

	public PooledList(int capacity, ClearMode clearMode, ArrayPool<T> customPool, bool sizeToCapacity)
	{
		if (capacity < 0)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.capacity, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
		}
		_pool = customPool ?? ArrayPool<T>.Shared;
		_clearOnFree = ShouldClear(clearMode);
		if (capacity == 0)
		{
			_items = s_emptyArray;
		}
		else
		{
			_items = _pool.Rent(capacity);
		}
		if (sizeToCapacity)
		{
			_size = capacity;
			if (clearMode != ClearMode.Never)
			{
				Array.Clear(_items, 0, _size);
			}
		}
	}

	public PooledList(T[] array)
		: this((ReadOnlySpan<T>)array.AsSpan(), ClearMode.Auto, ArrayPool<T>.Shared)
	{
	}

	public PooledList(T[] array, ClearMode clearMode)
		: this((ReadOnlySpan<T>)array.AsSpan(), clearMode, ArrayPool<T>.Shared)
	{
	}

	public PooledList(T[] array, ArrayPool<T> customPool)
		: this((ReadOnlySpan<T>)array.AsSpan(), ClearMode.Auto, customPool)
	{
	}

	public PooledList(T[] array, ClearMode clearMode, ArrayPool<T> customPool)
		: this((ReadOnlySpan<T>)array.AsSpan(), clearMode, customPool)
	{
	}

	public PooledList(ReadOnlySpan<T> span)
		: this(span, ClearMode.Auto, ArrayPool<T>.Shared)
	{
	}

	public PooledList(ReadOnlySpan<T> span, ClearMode clearMode)
		: this(span, clearMode, ArrayPool<T>.Shared)
	{
	}

	public PooledList(ReadOnlySpan<T> span, ArrayPool<T> customPool)
		: this(span, ClearMode.Auto, customPool)
	{
	}

	public PooledList(ReadOnlySpan<T> span, ClearMode clearMode, ArrayPool<T> customPool)
	{
		_pool = customPool ?? ArrayPool<T>.Shared;
		_clearOnFree = ShouldClear(clearMode);
		int length = span.Length;
		if (length == 0)
		{
			_items = s_emptyArray;
			return;
		}
		_items = _pool.Rent(length);
		span.CopyTo(_items);
		_size = length;
	}

	public PooledList(IEnumerable<T> collection)
		: this(collection, ClearMode.Auto, ArrayPool<T>.Shared)
	{
	}

	public PooledList(IEnumerable<T> collection, ClearMode clearMode)
		: this(collection, clearMode, ArrayPool<T>.Shared)
	{
	}

	public PooledList(IEnumerable<T> collection, ArrayPool<T> customPool)
		: this(collection, ClearMode.Auto, customPool)
	{
	}

	public PooledList(IEnumerable<T> collection, ClearMode clearMode, ArrayPool<T> customPool)
	{
		_pool = customPool ?? ArrayPool<T>.Shared;
		_clearOnFree = ShouldClear(clearMode);
		if (collection != null)
		{
			if (!(collection is ICollection<T> { Count: var count } collection2))
			{
				_size = 0;
				_items = s_emptyArray;
				{
					foreach (T item in collection)
					{
						Add(item);
					}
					return;
				}
			}
			if (count == 0)
			{
				_items = s_emptyArray;
				return;
			}
			_items = _pool.Rent(count);
			collection2.CopyTo(_items, 0);
			_size = count;
		}
		else
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.collection);
		}
	}

	private static bool IsCompatibleObject(object? value)
	{
		if (!(value is T))
		{
			if (value == null)
			{
				return default(T) == null;
			}
			return false;
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Add(T item)
	{
		_version++;
		int size = _size;
		if ((uint)size < (uint)_items.Length)
		{
			_size = size + 1;
			_items[size] = item;
		}
		else
		{
			AddWithResize(item);
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private void AddWithResize(T item)
	{
		int size = _size;
		EnsureCapacity(size + 1);
		_size = size + 1;
		_items[size] = item;
	}

	int IList.Add(object? item)
	{
		ThrowHelper.IfNullAndNullsAreIllegalThenThrow<T>(item, ExceptionArgument.item);
		try
		{
			Add((T)item);
		}
		catch (InvalidCastException)
		{
			ThrowHelper.ThrowWrongValueTypeArgumentException(item, typeof(T));
		}
		return Count - 1;
	}

	public void AddRange(IEnumerable<T> collection)
	{
		InsertRange(_size, collection);
	}

	public void AddRange(T[] array)
	{
		AddRange(array.AsSpan());
	}

	public void AddRange(ReadOnlySpan<T> span)
	{
		Span<T> destination = InsertSpan(_size, span.Length, clearOutput: false);
		span.CopyTo(destination);
	}

	public Span<T> AddSpan(int count)
	{
		return InsertSpan(_size, count);
	}

	public ReadOnlyCollection<T> AsReadOnly()
	{
		return new ReadOnlyCollection<T>(this);
	}

	public int BinarySearch(int index, int count, T item, IComparer<T>? comparer)
	{
		if (index < 0)
		{
			ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException();
		}
		if (count < 0)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
		}
		if (_size - index < count)
		{
			ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);
		}
		return Array.BinarySearch(_items, index, count, item, comparer);
	}

	public int BinarySearch(T item)
	{
		return BinarySearch(0, Count, item, null);
	}

	public int BinarySearch(T item, IComparer<T> comparer)
	{
		return BinarySearch(0, Count, item, comparer);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Clear()
	{
		_version++;
		int size = _size;
		_size = 0;
		if (size > 0 && _clearOnFree)
		{
			Array.Clear(_items, 0, size);
		}
	}

	public bool Contains(T item)
	{
		if (_size != 0)
		{
			return IndexOf(item) != -1;
		}
		return false;
	}

	bool IList.Contains(object? item)
	{
		if (IsCompatibleObject(item))
		{
			return Contains((T)item);
		}
		return false;
	}

	public PooledList<TOutput> ConvertAll<TOutput>(Func<T, TOutput> converter)
	{
		if (converter == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.converter);
		}
		PooledList<TOutput> pooledList = new PooledList<TOutput>(_size);
		for (int i = 0; i < _size; i++)
		{
			pooledList._items[i] = converter(_items[i]);
		}
		pooledList._size = _size;
		return pooledList;
	}

	public void CopyTo(Span<T> span)
	{
		if (span.Length < Count)
		{
			throw new ArgumentException("Destination span is shorter than the list to be copied.");
		}
		Span.CopyTo(span);
	}

	void ICollection<T>.CopyTo(T[] array, int arrayIndex)
	{
		Array.Copy(_items, 0, array, arrayIndex, _size);
	}

	void ICollection.CopyTo(Array array, int arrayIndex)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (array.Rank != 1)
		{
			ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RankMultiDimNotSupported);
		}
		try
		{
			Array.Copy(_items, 0, array, arrayIndex, _size);
		}
		catch (ArrayTypeMismatchException)
		{
			ThrowHelper.ThrowArgumentException_Argument_InvalidArrayType();
		}
	}

	private void EnsureCapacity(int min)
	{
		if (_items.Length < min)
		{
			int num = ((_items.Length == 0) ? 4 : (_items.Length * 2));
			if ((uint)num > 2146435071u)
			{
				num = 2146435071;
			}
			if (num < min)
			{
				num = min;
			}
			Capacity = num;
		}
	}

	public bool Exists(Func<T, bool> match)
	{
		return FindIndex(match) != -1;
	}

	public bool TryFind(Func<T, bool> match, [MaybeNullWhen(false)] out T result)
	{
		if (match == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);
		}
		for (int i = 0; i < _size; i++)
		{
			if (match(_items[i]))
			{
				result = _items[i];
				return true;
			}
		}
		result = default(T);
		return false;
	}

	public PooledList<T> FindAll(Func<T, bool> match)
	{
		if (match == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);
		}
		PooledList<T> pooledList = new PooledList<T>();
		for (int i = 0; i < _size; i++)
		{
			if (match(_items[i]))
			{
				pooledList.Add(_items[i]);
			}
		}
		return pooledList;
	}

	public int FindIndex(Func<T, bool> match)
	{
		return FindIndex(0, _size, match);
	}

	public int FindIndex(int startIndex, Func<T, bool> match)
	{
		return FindIndex(startIndex, _size - startIndex, match);
	}

	public int FindIndex(int startIndex, int count, Func<T, bool> match)
	{
		if ((uint)startIndex > (uint)_size)
		{
			ThrowHelper.ThrowStartIndexArgumentOutOfRange_ArgumentOutOfRange_Index();
		}
		if (count < 0 || startIndex > _size - count)
		{
			ThrowHelper.ThrowCountArgumentOutOfRange_ArgumentOutOfRange_Count();
		}
		if (match == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);
		}
		int num = startIndex + count;
		for (int i = startIndex; i < num; i++)
		{
			if (match(_items[i]))
			{
				return i;
			}
		}
		return -1;
	}

	public bool TryFindLast(Func<T, bool> match, [MaybeNullWhen(false)] out T result)
	{
		if (match == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);
		}
		for (int num = _size - 1; num >= 0; num--)
		{
			if (match(_items[num]))
			{
				result = _items[num];
				return true;
			}
		}
		result = default(T);
		return false;
	}

	public int FindLastIndex(Func<T, bool> match)
	{
		return FindLastIndex(_size - 1, _size, match);
	}

	public int FindLastIndex(int startIndex, Func<T, bool> match)
	{
		return FindLastIndex(startIndex, startIndex + 1, match);
	}

	public int FindLastIndex(int startIndex, int count, Func<T, bool> match)
	{
		if (match == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);
		}
		if (_size == 0)
		{
			if (startIndex != -1)
			{
				ThrowHelper.ThrowStartIndexArgumentOutOfRange_ArgumentOutOfRange_Index();
			}
		}
		else if ((uint)startIndex >= (uint)_size)
		{
			ThrowHelper.ThrowStartIndexArgumentOutOfRange_ArgumentOutOfRange_Index();
		}
		if (count < 0 || startIndex - count + 1 < 0)
		{
			ThrowHelper.ThrowCountArgumentOutOfRange_ArgumentOutOfRange_Count();
		}
		int num = startIndex - count;
		for (int num2 = startIndex; num2 > num; num2--)
		{
			if (match(_items[num2]))
			{
				return num2;
			}
		}
		return -1;
	}

	public void ForEach(Action<T> action)
	{
		if (action == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.action);
		}
		int version = _version;
		for (int i = 0; i < _size; i++)
		{
			if (version != _version)
			{
				break;
			}
			action(_items[i]);
		}
		if (version != _version)
		{
			ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion();
		}
	}

	public Enumerator GetEnumerator()
	{
		return new Enumerator(this);
	}

	IEnumerator<T> IEnumerable<T>.GetEnumerator()
	{
		return new Enumerator(this);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return new Enumerator(this);
	}

	public Span<T> GetRange(int index, int count)
	{
		if (index < 0)
		{
			ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException();
		}
		if (count < 0)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
		}
		if (_size - index < count)
		{
			ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);
		}
		return Span.Slice(index, count);
	}

	public int IndexOf(T item)
	{
		return Array.IndexOf(_items, item, 0, _size);
	}

	int IList.IndexOf(object? item)
	{
		if (IsCompatibleObject(item))
		{
			return IndexOf((T)item);
		}
		return -1;
	}

	public int IndexOf(T item, int index)
	{
		if (index > _size)
		{
			ThrowHelper.ThrowArgumentOutOfRange_IndexException();
		}
		return Array.IndexOf(_items, item, index, _size - index);
	}

	public int IndexOf(T item, int index, int count)
	{
		if (index > _size)
		{
			ThrowHelper.ThrowArgumentOutOfRange_IndexException();
		}
		if (count < 0 || index > _size - count)
		{
			ThrowHelper.ThrowCountArgumentOutOfRange_ArgumentOutOfRange_Count();
		}
		return Array.IndexOf(_items, item, index, count);
	}

	public void Insert(int index, T item)
	{
		if ((uint)index > (uint)_size)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_ListInsert);
		}
		if (_size == _items.Length)
		{
			EnsureCapacity(_size + 1);
		}
		if (index < _size)
		{
			Array.Copy(_items, index, _items, index + 1, _size - index);
		}
		_items[index] = item;
		_size++;
		_version++;
	}

	void IList.Insert(int index, object? item)
	{
		ThrowHelper.IfNullAndNullsAreIllegalThenThrow<T>(item, ExceptionArgument.item);
		try
		{
			Insert(index, (T)item);
		}
		catch (InvalidCastException)
		{
			ThrowHelper.ThrowWrongValueTypeArgumentException(item, typeof(T));
		}
	}

	public void InsertRange(int index, IEnumerable<T> collection)
	{
		if ((uint)index > (uint)_size)
		{
			ThrowHelper.ThrowArgumentOutOfRange_IndexException();
		}
		if (collection != null)
		{
			if (collection is ICollection<T> { Count: var count } collection2)
			{
				if (count > 0)
				{
					EnsureCapacity(_size + count);
					if (index < _size)
					{
						Array.Copy(_items, index, _items, index + count, _size - index);
					}
					if (this == collection2)
					{
						Array.Copy(_items, 0, _items, index, index);
						Array.Copy(_items, index + count, _items, index * 2, _size - index);
					}
					else
					{
						collection2.CopyTo(_items, index);
					}
					_size += count;
				}
			}
			else
			{
				using IEnumerator<T> enumerator = collection.GetEnumerator();
				while (enumerator.MoveNext())
				{
					Insert(index++, enumerator.Current);
				}
			}
		}
		else
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.collection);
		}
		_version++;
	}

	public void InsertRange(int index, ReadOnlySpan<T> span)
	{
		Span<T> destination = InsertSpan(index, span.Length, clearOutput: false);
		span.CopyTo(destination);
	}

	public void InsertRange(int index, T[] array)
	{
		if (array == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
		}
		InsertRange(index, array.AsSpan());
	}

	public Span<T> InsertSpan(int index, int count)
	{
		return InsertSpan(index, count, clearOutput: true);
	}

	private Span<T> InsertSpan(int index, int count, bool clearOutput)
	{
		EnsureCapacity(_size + count);
		if (index < _size)
		{
			Array.Copy(_items, index, _items, index + count, _size - index);
		}
		_size += count;
		_version++;
		Span<T> result = _items.AsSpan(index, count);
		if (clearOutput && _clearOnFree)
		{
			result.Clear();
		}
		return result;
	}

	public int LastIndexOf(T item)
	{
		if (_size == 0)
		{
			return -1;
		}
		return LastIndexOf(item, _size - 1, _size);
	}

	public int LastIndexOf(T item, int index)
	{
		if (index >= _size)
		{
			ThrowHelper.ThrowArgumentOutOfRange_IndexException();
		}
		return LastIndexOf(item, index, index + 1);
	}

	public int LastIndexOf(T item, int index, int count)
	{
		if (Count != 0 && index < 0)
		{
			ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException();
		}
		if (Count != 0 && count < 0)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
		}
		if (_size == 0)
		{
			return -1;
		}
		if (index >= _size)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_BiggerThanCollection);
		}
		if (count > index + 1)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_BiggerThanCollection);
		}
		return Array.LastIndexOf(_items, item, index, count);
	}

	public bool Remove(T item)
	{
		int num = IndexOf(item);
		if (num >= 0)
		{
			RemoveAt(num);
			return true;
		}
		return false;
	}

	void IList.Remove(object? item)
	{
		if (IsCompatibleObject(item))
		{
			Remove((T)item);
		}
	}

	public int RemoveAll(Func<T, bool> match)
	{
		if (match == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);
		}
		int i;
		for (i = 0; i < _size && !match(_items[i]); i++)
		{
		}
		if (i >= _size)
		{
			return 0;
		}
		int j = i + 1;
		while (j < _size)
		{
			for (; j < _size && match(_items[j]); j++)
			{
			}
			if (j < _size)
			{
				_items[i++] = _items[j++];
			}
		}
		if (_clearOnFree)
		{
			Array.Clear(_items, i, _size - i);
		}
		int result = _size - i;
		_size = i;
		_version++;
		return result;
	}

	public void RemoveAt(int index)
	{
		if ((uint)index >= (uint)_size)
		{
			ThrowHelper.ThrowArgumentOutOfRange_IndexException();
		}
		_size--;
		if (index < _size)
		{
			Array.Copy(_items, index + 1, _items, index, _size - index);
		}
		_version++;
		if (_clearOnFree)
		{
			_items[_size] = default(T);
		}
	}

	public void RemoveRange(int index, int count)
	{
		if (index < 0)
		{
			ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException();
		}
		if (count < 0)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
		}
		if (_size - index < count)
		{
			ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);
		}
		if (count > 0)
		{
			_size -= count;
			if (index < _size)
			{
				Array.Copy(_items, index + count, _items, index, _size - index);
			}
			_version++;
			if (_clearOnFree)
			{
				Array.Clear(_items, _size, count);
			}
		}
	}

	public void Reverse()
	{
		Reverse(0, _size);
	}

	public void Reverse(int index, int count)
	{
		if (index < 0)
		{
			ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException();
		}
		if (count < 0)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
		}
		if (_size - index < count)
		{
			ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);
		}
		if (count > 1)
		{
			Array.Reverse(_items, index, count);
		}
		_version++;
	}

	public void Sort()
	{
		Sort(0, Count, null);
	}

	public void Sort(IComparer<T> comparer)
	{
		Sort(0, Count, comparer);
	}

	public void Sort(int index, int count, IComparer<T>? comparer)
	{
		if (index < 0)
		{
			ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException();
		}
		if (count < 0)
		{
			ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
		}
		if (_size - index < count)
		{
			ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);
		}
		if (count > 1)
		{
			Array.Sort(_items, index, count, comparer);
		}
		_version++;
	}

	public void Sort(Func<T?, T?, int> comparison)
	{
		if (comparison == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.comparison);
		}
		if (_size > 1)
		{
			Array.Sort(_items, 0, _size, new Comparer(comparison));
		}
		_version++;
	}

	public T[] ToArray()
	{
		if (_size == 0)
		{
			return s_emptyArray;
		}
		return Span.ToArray();
	}

	public void TrimExcess()
	{
		int num = (int)((double)_items.Length * 0.9);
		if (_size < num)
		{
			Capacity = _size;
		}
	}

	public bool TrueForAll(Func<T, bool> match)
	{
		if (match == null)
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);
		}
		for (int i = 0; i < _size; i++)
		{
			if (!match(_items[i]))
			{
				return false;
			}
		}
		return true;
	}

	private void ReturnArray()
	{
		if (_items.Length != 0)
		{
			try
			{
				_pool.Return(_items, _clearOnFree);
			}
			catch (ArgumentException)
			{
			}
			_items = s_emptyArray;
		}
	}

	private static bool ShouldClear(ClearMode mode)
	{
		return mode switch
		{
			ClearMode.Auto => RuntimeHelpers.IsReferenceOrContainsReferences<T>(), 
			ClearMode.Always => true, 
			_ => false, 
		};
	}

	public virtual void Dispose()
	{
		ReturnArray();
		_size = 0;
		_version++;
	}

	void IDeserializationCallback.OnDeserialization(object? sender)
	{
		_pool = ArrayPool<T>.Shared;
	}
}
