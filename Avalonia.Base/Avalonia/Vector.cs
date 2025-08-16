using System;
using System.Globalization;
using System.Numerics;
using Avalonia.Utilities;

namespace Avalonia;

public readonly struct Vector : IEquatable<Vector>
{
	private readonly double _x;

	private readonly double _y;

	public double X => _x;

	public double Y => _y;

	public double Length => Math.Sqrt(SquaredLength);

	public double SquaredLength => _x * _x + _y * _y;

	public static Vector Zero => new Vector(0.0, 0.0);

	public static Vector One => new Vector(1.0, 1.0);

	public static Vector UnitX => new Vector(1.0, 0.0);

	public static Vector UnitY => new Vector(0.0, 1.0);

	public Vector(double x, double y)
	{
		_x = x;
		_y = y;
	}

	public static explicit operator Point(Vector a)
	{
		return new Point(a._x, a._y);
	}

	public static double operator *(Vector a, Vector b)
	{
		return Dot(a, b);
	}

	public static Vector operator *(Vector vector, double scale)
	{
		return Multiply(vector, scale);
	}

	public static Vector operator *(double scale, Vector vector)
	{
		return Multiply(vector, scale);
	}

	public static Vector operator /(Vector vector, double scale)
	{
		return Divide(vector, scale);
	}

	public static Vector Parse(string s)
	{
		using StringTokenizer stringTokenizer = new StringTokenizer(s, CultureInfo.InvariantCulture, "Invalid Vector.");
		return new Vector(stringTokenizer.ReadDouble(null), stringTokenizer.ReadDouble(null));
	}

	public static Vector operator -(Vector a)
	{
		return Negate(a);
	}

	public static Vector operator +(Vector a, Vector b)
	{
		return Add(a, b);
	}

	public static Vector operator -(Vector a, Vector b)
	{
		return Subtract(a, b);
	}

	public bool Equals(Vector other)
	{
		if (_x == other._x)
		{
			return _y == other._y;
		}
		return false;
	}

	public bool NearlyEquals(Vector other)
	{
		if (MathUtilities.AreClose(_x, other._x))
		{
			return MathUtilities.AreClose(_y, other._y);
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is Vector other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (_x.GetHashCode() * 397) ^ _y.GetHashCode();
	}

	public static bool operator ==(Vector left, Vector right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(Vector left, Vector right)
	{
		return !left.Equals(right);
	}

	public override string ToString()
	{
		return string.Format(CultureInfo.InvariantCulture, "{0}, {1}", _x, _y);
	}

	public Vector WithX(double x)
	{
		return new Vector(x, _y);
	}

	public Vector WithY(double y)
	{
		return new Vector(_x, y);
	}

	public Vector Normalize()
	{
		return Normalize(this);
	}

	public Vector Negate()
	{
		return Negate(this);
	}

	public static double Dot(Vector a, Vector b)
	{
		return a._x * b._x + a._y * b._y;
	}

	public static double Cross(Vector a, Vector b)
	{
		return a._x * b._y - a._y * b._x;
	}

	public static Vector Normalize(Vector vector)
	{
		return Divide(vector, vector.Length);
	}

	public static Vector Divide(Vector a, Vector b)
	{
		return new Vector(a._x / b._x, a._y / b._y);
	}

	public static Vector Divide(Vector vector, double scalar)
	{
		return new Vector(vector._x / scalar, vector._y / scalar);
	}

	public static Vector Multiply(Vector a, Vector b)
	{
		return new Vector(a._x * b._x, a._y * b._y);
	}

	public static Vector Multiply(Vector vector, double scalar)
	{
		return new Vector(vector._x * scalar, vector._y * scalar);
	}

	public static Vector Add(Vector a, Vector b)
	{
		return new Vector(a._x + b._x, a._y + b._y);
	}

	public static Vector Subtract(Vector a, Vector b)
	{
		return new Vector(a._x - b._x, a._y - b._y);
	}

	public static Vector Negate(Vector vector)
	{
		return new Vector(0.0 - vector._x, 0.0 - vector._y);
	}

	public void Deconstruct(out double x, out double y)
	{
		x = _x;
		y = _y;
	}

	internal Vector2 ToVector2()
	{
		return new Vector2((float)X, (float)Y);
	}

	internal Vector(Vector2 v)
		: this(v.X, v.Y)
	{
	}

	public Vector Abs()
	{
		return new Vector(Math.Abs(X), Math.Abs(Y));
	}

	public static Vector Clamp(Vector value, Vector min, Vector max)
	{
		return Min(Max(value, min), max);
	}

	public static Vector Max(Vector left, Vector right)
	{
		return new Vector(Math.Max(left.X, right.X), Math.Max(left.Y, right.Y));
	}

	public static Vector Min(Vector left, Vector right)
	{
		return new Vector(Math.Min(left.X, right.X), Math.Min(left.Y, right.Y));
	}

	public static double Distance(Vector value1, Vector value2)
	{
		return Math.Sqrt(DistanceSquared(value1, value2));
	}

	public static double DistanceSquared(Vector value1, Vector value2)
	{
		Vector vector = value1 - value2;
		return Dot(vector, vector);
	}

	public static implicit operator Vector(Vector2 v)
	{
		return new Vector(v);
	}
}
