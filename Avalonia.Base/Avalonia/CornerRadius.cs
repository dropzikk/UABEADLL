using System;
using System.Globalization;
using Avalonia.Utilities;

namespace Avalonia;

public readonly struct CornerRadius : IEquatable<CornerRadius>
{
	public double TopLeft { get; }

	public double TopRight { get; }

	public double BottomRight { get; }

	public double BottomLeft { get; }

	public bool IsUniform
	{
		get
		{
			if (TopLeft.Equals(TopRight) && BottomLeft.Equals(BottomRight))
			{
				return TopRight.Equals(BottomRight);
			}
			return false;
		}
	}

	public CornerRadius(double uniformRadius)
	{
		TopLeft = (TopRight = (BottomLeft = (BottomRight = uniformRadius)));
	}

	public CornerRadius(double top, double bottom)
	{
		TopLeft = (TopRight = top);
		BottomLeft = (BottomRight = bottom);
	}

	public CornerRadius(double topLeft, double topRight, double bottomRight, double bottomLeft)
	{
		TopLeft = topLeft;
		TopRight = topRight;
		BottomRight = bottomRight;
		BottomLeft = bottomLeft;
	}

	public bool Equals(CornerRadius other)
	{
		if (TopLeft == other.TopLeft && TopRight == other.TopRight && BottomRight == other.BottomRight)
		{
			return BottomLeft == other.BottomLeft;
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is CornerRadius other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return TopLeft.GetHashCode() ^ TopRight.GetHashCode() ^ BottomLeft.GetHashCode() ^ BottomRight.GetHashCode();
	}

	public override string ToString()
	{
		return FormattableString.Invariant($"{TopLeft},{TopRight},{BottomRight},{BottomLeft}");
	}

	public static CornerRadius Parse(string s)
	{
		using StringTokenizer stringTokenizer = new StringTokenizer(s, CultureInfo.InvariantCulture, "Invalid CornerRadius.");
		if (stringTokenizer.TryReadDouble(out var result, null))
		{
			if (stringTokenizer.TryReadDouble(out var result2, null))
			{
				if (stringTokenizer.TryReadDouble(out var result3, null))
				{
					return new CornerRadius(result, result2, result3, stringTokenizer.ReadDouble(null));
				}
				return new CornerRadius(result, result2);
			}
			return new CornerRadius(result);
		}
		throw new FormatException("Invalid CornerRadius.");
	}

	public static bool operator ==(CornerRadius left, CornerRadius right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(CornerRadius left, CornerRadius right)
	{
		return !(left == right);
	}
}
