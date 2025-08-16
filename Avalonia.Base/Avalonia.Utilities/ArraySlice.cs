using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Avalonia.Utilities;

internal readonly struct ArraySlice<T> : IReadOnlyList<T>, IEnumerable<T>, IEnumerable, IReadOnlyCollection<T>
{
	private readonly T[] _data;

	public static ArraySlice<T> Empty => new ArraySlice<T>(Array.Empty<T>());

	public bool IsEmpty => Length == 0;

	public int Start { get; }

	public int Length { get; }

	public Span<T> Span => new Span<T>(_data, Start, Length);

	public ref T this[int index]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			int num = index + Start;
			return ref _data[num];
		}
	}

	T IReadOnlyList<T>.this[int index] => this[index];

	int IReadOnlyCollection<T>.Count => Length;

	public ArraySlice(T[] data)
		: this(data, 0, data.Length)
	{
	}

	public ArraySlice(T[] data, int start, int length)
	{
		_data = data;
		Start = start;
		Length = length;
	}

	public static implicit operator ArraySlice<T>(T[] array)
	{
		return new ArraySlice<T>(array, 0, array.Length);
	}

	public void Fill(T value)
	{
		Span.Fill(value);
	}

	public ArraySlice<T> Slice(int start, int length)
	{
		return new ArraySlice<T>(_data, start, length);
	}

	public ArraySlice<T> Take(int length)
	{
		if (IsEmpty)
		{
			return this;
		}
		if (length > Length)
		{
			throw new ArgumentOutOfRangeException("length");
		}
		return new ArraySlice<T>(_data, Start, length);
	}

	public ArraySlice<T> Skip(int length)
	{
		if (IsEmpty)
		{
			return this;
		}
		if (length > Length)
		{
			throw new ArgumentOutOfRangeException("length");
		}
		return new ArraySlice<T>(_data, Start + length, Length - length);
	}

	public ImmutableReadOnlyListStructEnumerator<T> GetEnumerator()
	{
		return new ImmutableReadOnlyListStructEnumerator<T>(this);
	}

	IEnumerator<T> IEnumerable<T>.GetEnumerator()
	{
		return GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
