using System;
using System.Globalization;
using System.Numerics;
using Avalonia.Utilities;

namespace Avalonia;

public readonly struct Point : IEquatable<Point>
{
	private readonly double _x;

	private readonly double _y;

	public double X => _x;

	public double Y => _y;

	public Point(double x, double y)
	{
		_x = x;
		_y = y;
	}

	public static implicit operator Vector(Point p)
	{
		return new Vector(p._x, p._y);
	}

	public static Point operator -(Point a)
	{
		return new Point(0.0 - a._x, 0.0 - a._y);
	}

	public static bool operator ==(Point left, Point right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(Point left, Point right)
	{
		return !(left == right);
	}

	public static Point operator +(Point a, Point b)
	{
		return new Point(a._x + b._x, a._y + b._y);
	}

	public static Point operator +(Point a, Vector b)
	{
		return new Point(a._x + b.X, a._y + b.Y);
	}

	public static Point operator -(Point a, Point b)
	{
		return new Point(a._x - b._x, a._y - b._y);
	}

	public static Point operator -(Point a, Vector b)
	{
		return new Point(a._x - b.X, a._y - b.Y);
	}

	public static Point operator *(Point p, double k)
	{
		return new Point(p.X * k, p.Y * k);
	}

	public static Point operator *(double k, Point p)
	{
		return new Point(p.X * k, p.Y * k);
	}

	public static Point operator /(Point p, double k)
	{
		return new Point(p.X / k, p.Y / k);
	}

	public static Point operator *(Point point, Matrix matrix)
	{
		return matrix.Transform(point);
	}

	public static double Distance(Point value1, Point value2)
	{
		return Math.Sqrt((value2.X - value1.X) * (value2.X - value1.X) + (value2.Y - value1.Y) * (value2.Y - value1.Y));
	}

	public static Point Parse(string s)
	{
		using StringTokenizer stringTokenizer = new StringTokenizer(s, CultureInfo.InvariantCulture, "Invalid Point.");
		return new Point(stringTokenizer.ReadDouble(null), stringTokenizer.ReadDouble(null));
	}

	public bool Equals(Point other)
	{
		if (_x == other._x)
		{
			return _y == other._y;
		}
		return false;
	}

	public bool NearlyEquals(Point other)
	{
		if (MathUtilities.AreClose(_x, other._x))
		{
			return MathUtilities.AreClose(_y, other._y);
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is Point other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (17 * 23 + _x.GetHashCode()) * 23 + _y.GetHashCode();
	}

	public override string ToString()
	{
		return string.Format(CultureInfo.InvariantCulture, "{0}, {1}", _x, _y);
	}

	public Point Transform(Matrix transform)
	{
		return transform.Transform(this);
	}

	internal Point Transform(Matrix4x4 matrix)
	{
		Vector2 vector = Vector2.Transform(new Vector2((float)X, (float)Y), matrix);
		return new Point(vector.X, vector.Y);
	}

	public Point WithX(double x)
	{
		return new Point(x, _y);
	}

	public Point WithY(double y)
	{
		return new Point(_x, y);
	}

	public void Deconstruct(out double x, out double y)
	{
		x = _x;
		y = _y;
	}
}
