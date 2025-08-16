using System;
using System.Collections.Generic;

namespace Avalonia.Data;

public readonly struct Optional<T> : IEquatable<Optional<T>>
{
	private readonly T _value;

	public bool HasValue { get; }

	public T Value
	{
		get
		{
			if (!HasValue)
			{
				throw new InvalidOperationException("Optional has no value.");
			}
			return _value;
		}
	}

	public static Optional<T> Empty => default(Optional<T>);

	public Optional(T value)
	{
		_value = value;
		HasValue = true;
	}

	public override bool Equals(object? obj)
	{
		if (obj is Optional<T> optional)
		{
			return this == optional;
		}
		return false;
	}

	public bool Equals(Optional<T> other)
	{
		return this == other;
	}

	public override int GetHashCode()
	{
		if (!HasValue)
		{
			return 0;
		}
		T value = _value;
		if (value == null)
		{
			return 0;
		}
		return value.GetHashCode();
	}

	public Optional<object?> ToObject()
	{
		if (!HasValue)
		{
			return default(Optional<object>);
		}
		return new Optional<object>(_value);
	}

	public override string ToString()
	{
		if (!HasValue)
		{
			return "(empty)";
		}
		T value = _value;
		return ((value != null) ? value.ToString() : null) ?? "(null)";
	}

	public T? GetValueOrDefault()
	{
		if (!HasValue)
		{
			return default(T);
		}
		return _value;
	}

	public T? GetValueOrDefault(T defaultValue)
	{
		if (!HasValue)
		{
			return defaultValue;
		}
		return _value;
	}

	public TResult? GetValueOrDefault<TResult>()
	{
		if (!HasValue)
		{
			return default(TResult);
		}
		T value = _value;
		if (value is TResult)
		{
			object obj = value;
			return (TResult)((obj is TResult) ? obj : null);
		}
		return default(TResult);
	}

	public TResult? GetValueOrDefault<TResult>(TResult defaultValue)
	{
		if (!HasValue)
		{
			return defaultValue;
		}
		T value = _value;
		if (value is TResult)
		{
			object obj = value;
			return (TResult)((obj is TResult) ? obj : null);
		}
		return default(TResult);
	}

	public static implicit operator Optional<T>(T value)
	{
		return new Optional<T>(value);
	}

	public static bool operator !=(Optional<T> x, Optional<T> y)
	{
		return !(x == y);
	}

	public static bool operator ==(Optional<T> x, Optional<T> y)
	{
		if (!x.HasValue && !y.HasValue)
		{
			return true;
		}
		if (x.HasValue && y.HasValue)
		{
			return EqualityComparer<T>.Default.Equals(x.Value, y.Value);
		}
		return false;
	}
}
