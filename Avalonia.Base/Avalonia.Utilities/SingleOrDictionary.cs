using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Avalonia.Utilities;

internal class SingleOrDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable where TKey : notnull
{
	private class SingleEnumerator<T> : IEnumerator<T>, IEnumerator, IDisposable
	{
		private T value;

		private int index = -1;

		public T Current
		{
			get
			{
				if (index == 0)
				{
					return value;
				}
				throw new InvalidOperationException();
			}
		}

		object? IEnumerator.Current => Current;

		public SingleEnumerator(T value)
		{
			this.value = value;
		}

		public void Dispose()
		{
		}

		public bool MoveNext()
		{
			index++;
			return index < 1;
		}

		public void Reset()
		{
			index = -1;
		}
	}

	private KeyValuePair<TKey, TValue>? _singleValue;

	private Dictionary<TKey, TValue>? dictionary;

	public IEnumerable<TValue> Values
	{
		get
		{
			if (dictionary == null)
			{
				if (_singleValue.HasValue)
				{
					return new TValue[1] { _singleValue.Value.Value };
				}
				return Enumerable.Empty<TValue>();
			}
			return dictionary.Values;
		}
	}

	public void Add(TKey key, TValue value)
	{
		if (_singleValue.HasValue)
		{
			dictionary = new Dictionary<TKey, TValue>();
			((ICollection<KeyValuePair<TKey, TValue>>)dictionary).Add(_singleValue.Value);
			_singleValue = null;
		}
		if (dictionary != null)
		{
			dictionary.Add(key, value);
		}
		else
		{
			_singleValue = new KeyValuePair<TKey, TValue>(key, value);
		}
	}

	public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
	{
		if (dictionary == null)
		{
			if (!_singleValue.HasValue || !EqualityComparer<TKey>.Default.Equals(_singleValue.Value.Key, key))
			{
				value = default(TValue);
				return false;
			}
			value = _singleValue.Value.Value;
			return true;
		}
		return dictionary.TryGetValue(key, out value);
	}

	public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
	{
		if (dictionary == null)
		{
			if (_singleValue.HasValue)
			{
				return new SingleEnumerator<KeyValuePair<TKey, TValue>>(_singleValue.Value);
			}
			return Enumerable.Empty<KeyValuePair<TKey, TValue>>().GetEnumerator();
		}
		return dictionary.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
