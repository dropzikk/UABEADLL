using System;
using System.ComponentModel;
using System.Globalization;

namespace Avalonia.Animation;

[TypeConverter(typeof(IterationCountTypeConverter))]
public struct IterationCount : IEquatable<IterationCount>
{
	private readonly IterationType _type;

	private readonly ulong _value;

	public static IterationCount Infinite => new IterationCount(0uL, IterationType.Infinite);

	public IterationType RepeatType => _type;

	public bool IsInfinite => _type == IterationType.Infinite;

	public ulong Value => _value;

	public IterationCount(ulong value)
		: this(value, IterationType.Many)
	{
	}

	public IterationCount(ulong value, IterationType type)
	{
		if (type > IterationType.Infinite)
		{
			throw new ArgumentException("Invalid value", "type");
		}
		_type = type;
		_value = value;
	}

	public static bool operator ==(IterationCount a, IterationCount b)
	{
		if (!a.IsInfinite || !b.IsInfinite)
		{
			if (a._value == b._value)
			{
				return a._type == b._type;
			}
			return false;
		}
		return true;
	}

	public static bool operator !=(IterationCount rc1, IterationCount rc2)
	{
		return !(rc1 == rc2);
	}

	public override bool Equals(object? o)
	{
		if (o == null)
		{
			return false;
		}
		if (!(o is IterationCount))
		{
			return false;
		}
		return this == (IterationCount)o;
	}

	public bool Equals(IterationCount IterationCount)
	{
		return this == IterationCount;
	}

	public override int GetHashCode()
	{
		return _value.GetHashCode() ^ _type.GetHashCode();
	}

	public override string ToString()
	{
		if (IsInfinite)
		{
			return "Infinite";
		}
		return _value.ToString();
	}

	public static IterationCount Parse(string s)
	{
		s = s.ToUpperInvariant().Trim();
		if (s.EndsWith("INFINITE"))
		{
			return Infinite;
		}
		if (s.StartsWith("-"))
		{
			throw new InvalidCastException("IterationCount can't be a negative number.");
		}
		return new IterationCount(ulong.Parse(s, CultureInfo.InvariantCulture));
	}
}
