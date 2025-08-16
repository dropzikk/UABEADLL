using System;
using System.Collections.Generic;
using System.Linq;

namespace Avalonia.Media.Immutable;

public class ImmutableDashStyle : IDashStyle, IEquatable<IDashStyle>
{
	private readonly double[] _dashes;

	public IReadOnlyList<double> Dashes => _dashes;

	public double Offset { get; }

	public ImmutableDashStyle(IEnumerable<double>? dashes, double offset)
	{
		_dashes = dashes?.ToArray() ?? Array.Empty<double>();
		Offset = offset;
	}

	public override bool Equals(object? obj)
	{
		return Equals(obj as IDashStyle);
	}

	public bool Equals(IDashStyle? other)
	{
		if (this == other)
		{
			return true;
		}
		if (other != null && Offset == other.Offset)
		{
			return SequenceEqual(_dashes, other.Dashes);
		}
		return false;
	}

	public override int GetHashCode()
	{
		int num = 717868523;
		num = num * -1521134295 + Offset.GetHashCode();
		double[] dashes = _dashes;
		foreach (double num2 in dashes)
		{
			num = num * -1521134295 + num2.GetHashCode();
		}
		return num;
	}

	private static bool SequenceEqual(double[] left, IReadOnlyList<double>? right)
	{
		if (left == right)
		{
			return true;
		}
		if (right == null || left.Length != right.Count)
		{
			return false;
		}
		for (int i = 0; i < left.Length; i++)
		{
			if (left[i] != right[i])
			{
				return false;
			}
		}
		return true;
	}
}
