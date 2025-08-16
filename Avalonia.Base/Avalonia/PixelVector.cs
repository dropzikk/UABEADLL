using System;
using System.Globalization;

namespace Avalonia;

public readonly struct PixelVector
{
	private readonly int _x;

	private readonly int _y;

	public int X => _x;

	public int Y => _y;

	public double Length => Math.Sqrt(X * X + Y * Y);

	public PixelVector(int x, int y)
	{
		_x = x;
		_y = y;
	}

	public static explicit operator PixelPoint(PixelVector a)
	{
		return new PixelPoint(a._x, a._y);
	}

	public static int operator *(PixelVector a, PixelVector b)
	{
		return a.X * b.X + a.Y * b.Y;
	}

	public static PixelVector operator *(PixelVector vector, int scale)
	{
		return new PixelVector(vector._x * scale, vector._y * scale);
	}

	public static PixelVector operator /(PixelVector vector, int scale)
	{
		return new PixelVector(vector._x / scale, vector._y / scale);
	}

	public static PixelVector operator -(PixelVector a)
	{
		return new PixelVector(-a._x, -a._y);
	}

	public static PixelVector operator +(PixelVector a, PixelVector b)
	{
		return new PixelVector(a._x + b._x, a._y + b._y);
	}

	public static PixelVector operator -(PixelVector a, PixelVector b)
	{
		return new PixelVector(a._x - b._x, a._y - b._y);
	}

	public bool Equals(PixelVector other)
	{
		if (_x == other._x)
		{
			return _y == other._y;
		}
		return false;
	}

	public bool NearlyEquals(PixelVector other)
	{
		if ((float)Math.Abs(_x - other._x) < float.Epsilon)
		{
			return (float)Math.Abs(_y - other._y) < float.Epsilon;
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (obj is PixelVector other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (_x.GetHashCode() * 397) ^ _y.GetHashCode();
	}

	public static bool operator ==(PixelVector left, PixelVector right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(PixelVector left, PixelVector right)
	{
		return !left.Equals(right);
	}

	public override string ToString()
	{
		return string.Format(CultureInfo.InvariantCulture, "{0}, {1}", _x, _y);
	}

	public PixelVector WithX(int x)
	{
		return new PixelVector(x, _y);
	}

	public PixelVector WithY(int y)
	{
		return new PixelVector(_x, y);
	}
}
