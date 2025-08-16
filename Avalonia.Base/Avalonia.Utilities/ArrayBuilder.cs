using System;
using System.Runtime.CompilerServices;

namespace Avalonia.Utilities;

internal struct ArrayBuilder<T>
{
	private const int DefaultCapacity = 4;

	private const int MaxCoreClrArrayLength = 2146435071;

	private T[]? _data;

	private int _size;

	public int Length
	{
		get
		{
			return _size;
		}
		set
		{
			if (value != _size)
			{
				if (value > 0)
				{
					EnsureCapacity(value);
					_size = value;
				}
				else
				{
					_size = 0;
				}
			}
		}
	}

	public int Capacity
	{
		get
		{
			T[]? data = _data;
			if (data == null)
			{
				return 0;
			}
			return data.Length;
		}
	}

	public ref T this[int index]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return ref _data[index];
		}
	}

	public ArraySlice<T> Add(int length, bool clear = true)
	{
		int size = _size;
		Length += length;
		ArraySlice<T> result = AsSlice(size, Length - size);
		if (clear)
		{
			result.Span.Clear();
		}
		return result;
	}

	public ArraySlice<T> Add(in ArraySlice<T> value)
	{
		int size = _size;
		Length += value.Length;
		ArraySlice<T> result = AsSlice(size, Length - size);
		value.Span.CopyTo(result.Span);
		return result;
	}

	public void AddItem(T value)
	{
		int num = Length++;
		_data[num] = value;
	}

	public void Clear()
	{
		if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
		{
			ClearArray();
		}
		else
		{
			_size = 0;
		}
	}

	private void ClearArray()
	{
		int size = _size;
		_size = 0;
		if (size > 0)
		{
			Array.Clear(_data, 0, size);
		}
	}

	private void EnsureCapacity(int min)
	{
		T[]? data = _data;
		int num = ((data != null) ? data.Length : 0);
		if (num < min)
		{
			uint num2 = ((num == 0) ? 4u : ((uint)(num * 2)));
			if (num2 > 2146435071)
			{
				num2 = 2146435071u;
			}
			if (num2 < min)
			{
				num2 = (uint)min;
			}
			T[] array = new T[num2];
			if (_size > 0)
			{
				Array.Copy(_data, array, _size);
			}
			_data = array;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ArraySlice<T> AsSlice()
	{
		return AsSlice(Length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ArraySlice<T> AsSlice(int length)
	{
		return new ArraySlice<T>(_data, 0, length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ArraySlice<T> AsSlice(int start, int length)
	{
		return new ArraySlice<T>(_data, start, length);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Span<T> AsSpan()
	{
		return _data.AsSpan(0, _size);
	}
}
