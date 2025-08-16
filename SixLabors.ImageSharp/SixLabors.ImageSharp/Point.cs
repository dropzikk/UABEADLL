using System;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SixLabors.ImageSharp;

public struct Point : IEquatable<Point>
{
	public static readonly Point Empty;

	public int X { get; set; }

	public int Y { get; set; }

	[EditorBrowsable(EditorBrowsableState.Never)]
	public bool IsEmpty => Equals(Empty);

	public Point(int value)
	{
		this = default(Point);
		X = LowInt16(value);
		Y = HighInt16(value);
	}

	public Point(int x, int y)
	{
		this = default(Point);
		X = x;
		Y = y;
	}

	public Point(Size size)
	{
		X = size.Width;
		Y = size.Height;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator PointF(Point point)
	{
		return new PointF(point.X, point.Y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Vector2(Point point)
	{
		return new Vector2(point.X, point.Y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator Size(Point point)
	{
		return new Size(point.X, point.Y);
	}

	public static Point operator -(Point value)
	{
		return new Point(-value.X, -value.Y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Point operator +(Point point, Size size)
	{
		return Add(point, size);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Point operator -(Point point, Size size)
	{
		return Subtract(point, size);
	}

	public static Point operator *(int left, Point right)
	{
		return Multiply(right, left);
	}

	public static Point operator *(Point left, int right)
	{
		return Multiply(left, right);
	}

	public static Point operator /(Point left, int right)
	{
		return new Point(left.X / right, left.Y / right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(Point left, Point right)
	{
		return left.Equals(right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(Point left, Point right)
	{
		return !left.Equals(right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Point Add(Point point, Size size)
	{
		return new Point(point.X + size.Width, point.Y + size.Height);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Point Multiply(Point point, int value)
	{
		return new Point(point.X * value, point.Y * value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Point Subtract(Point point, Size size)
	{
		return new Point(point.X - size.Width, point.Y - size.Height);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Point Ceiling(PointF point)
	{
		return new Point((int)MathF.Ceiling(point.X), (int)MathF.Ceiling(point.Y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Point Round(PointF point)
	{
		return new Point((int)MathF.Round(point.X), (int)MathF.Round(point.Y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Point Round(Vector2 vector)
	{
		return new Point((int)MathF.Round(vector.X), (int)MathF.Round(vector.Y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Point Truncate(PointF point)
	{
		return new Point((int)point.X, (int)point.Y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Point Transform(Point point, Matrix3x2 matrix)
	{
		return Round(Vector2.Transform(new Vector2(point.X, point.Y), matrix));
	}

	public void Deconstruct(out int x, out int y)
	{
		x = X;
		y = Y;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Offset(int dx, int dy)
	{
		X += dx;
		Y += dy;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Offset(Point point)
	{
		Offset(point.X, point.Y);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(X, Y);
	}

	public override string ToString()
	{
		return $"Point [ X={X}, Y={Y} ]";
	}

	public override bool Equals(object? obj)
	{
		if (obj is Point other)
		{
			return Equals(other);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(Point other)
	{
		if (X.Equals(other.X))
		{
			return Y.Equals(other.Y);
		}
		return false;
	}

	private static short HighInt16(int n)
	{
		return (short)((n >> 16) & 0xFFFF);
	}

	private static short LowInt16(int n)
	{
		return (short)(n & 0xFFFF);
	}
}
