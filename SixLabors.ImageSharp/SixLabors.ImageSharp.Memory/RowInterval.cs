using System;

namespace SixLabors.ImageSharp.Memory;

public readonly struct RowInterval : IEquatable<RowInterval>
{
	public int Min { get; }

	public int Max { get; }

	public int Height => Max - Min;

	public RowInterval(int min, int max)
	{
		Guard.MustBeLessThan(min, max, "min");
		Min = min;
		Max = max;
	}

	public static bool operator ==(RowInterval left, RowInterval right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(RowInterval left, RowInterval right)
	{
		return !left.Equals(right);
	}

	public bool Equals(RowInterval other)
	{
		if (Min == other.Min)
		{
			return Max == other.Max;
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj != null && obj is RowInterval other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Min, Max);
	}

	public override string ToString()
	{
		return $"RowInterval [{Min}->{Max}]";
	}

	internal RowInterval Slice(int start)
	{
		return new RowInterval(Min + start, Max);
	}

	internal RowInterval Slice(int start, int length)
	{
		return new RowInterval(Min + start, Min + start + length);
	}
}
