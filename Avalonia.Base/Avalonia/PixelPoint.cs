using System;
using System.Globalization;
using Avalonia.Utilities;

namespace Avalonia;

public readonly struct PixelPoint : IEquatable<PixelPoint>
{
	public static readonly PixelPoint Origin = new PixelPoint(0, 0);

	public int X { get; }

	public int Y { get; }

	public PixelPoint(int x, int y)
	{
		X = x;
		Y = y;
	}

	public static bool operator ==(PixelPoint left, PixelPoint right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(PixelPoint left, PixelPoint right)
	{
		return !(left == right);
	}

	public static implicit operator PixelVector(PixelPoint p)
	{
		return new PixelVector(p.X, p.Y);
	}

	public static PixelPoint operator +(PixelPoint a, PixelPoint b)
	{
		return new PixelPoint(a.X + b.X, a.Y + b.Y);
	}

	public static PixelPoint operator +(PixelPoint a, PixelVector b)
	{
		return new PixelPoint(a.X + b.X, a.Y + b.Y);
	}

	public static PixelPoint operator -(PixelPoint a, PixelPoint b)
	{
		return new PixelPoint(a.X - b.X, a.Y - b.Y);
	}

	public static PixelPoint operator -(PixelPoint a, PixelVector b)
	{
		return new PixelPoint(a.X - b.X, a.Y - b.Y);
	}

	public static PixelPoint Parse(string s)
	{
		using StringTokenizer stringTokenizer = new StringTokenizer(s, CultureInfo.InvariantCulture, "Invalid PixelPoint.");
		return new PixelPoint(stringTokenizer.ReadInt32(null), stringTokenizer.ReadInt32(null));
	}

	public bool Equals(PixelPoint other)
	{
		if (X == other.X)
		{
			return Y == other.Y;
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is PixelPoint other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (17 * 23 + X.GetHashCode()) * 23 + Y.GetHashCode();
	}

	public PixelPoint WithX(int x)
	{
		return new PixelPoint(x, Y);
	}

	public PixelPoint WithY(int y)
	{
		return new PixelPoint(X, y);
	}

	public Point ToPoint(double scale)
	{
		return new Point((double)X / scale, (double)Y / scale);
	}

	public Point ToPoint(Vector scale)
	{
		return new Point((double)X / scale.X, (double)Y / scale.Y);
	}

	public Point ToPointWithDpi(double dpi)
	{
		return ToPoint(dpi / 96.0);
	}

	public Point ToPointWithDpi(Vector dpi)
	{
		return ToPoint(new Vector(dpi.X / 96.0, dpi.Y / 96.0));
	}

	public static PixelPoint FromPoint(Point point, double scale)
	{
		return new PixelPoint((int)(point.X * scale), (int)(point.Y * scale));
	}

	public static PixelPoint FromPoint(Point point, Vector scale)
	{
		return new PixelPoint((int)(point.X * scale.X), (int)(point.Y * scale.Y));
	}

	public static PixelPoint FromPointWithDpi(Point point, double dpi)
	{
		return FromPoint(point, dpi / 96.0);
	}

	public static PixelPoint FromPointWithDpi(Point point, Vector dpi)
	{
		return FromPoint(point, new Vector(dpi.X / 96.0, dpi.Y / 96.0));
	}

	public override string ToString()
	{
		return string.Format(CultureInfo.InvariantCulture, "{0}, {1}", X, Y);
	}
}
