using System;
using System.Collections;
using System.Collections.Generic;

namespace Avalonia.Utilities;

public struct ImmutableReadOnlyListStructEnumerator<T> : IEnumerator<T>, IEnumerator, IDisposable
{
	private readonly IReadOnlyList<T> _readOnlyList;

	private int _pos;

	private T? _current;

	public T Current => _current;

	object? IEnumerator.Current => Current;

	public ImmutableReadOnlyListStructEnumerator(IReadOnlyList<T> readOnlyList)
	{
		_readOnlyList = readOnlyList;
		_pos = -1;
		_current = default(T);
	}

	public void Dispose()
	{
	}

	public bool MoveNext()
	{
		if (_pos >= _readOnlyList.Count - 1)
		{
			return false;
		}
		_current = _readOnlyList[++_pos];
		return true;
	}

	public void Reset()
	{
		_pos = -1;
		_current = default(T);
	}
}
