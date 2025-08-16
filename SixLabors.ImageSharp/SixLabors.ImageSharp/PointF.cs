using System;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SixLabors.ImageSharp;

public struct PointF : IEquatable<PointF>
{
	public static readonly PointF Empty;

	public float X { get; set; }

	public float Y { get; set; }

	[EditorBrowsable(EditorBrowsableState.Never)]
	public bool IsEmpty => Equals(Empty);

	public PointF(float x, float y)
	{
		this = default(PointF);
		X = x;
		Y = y;
	}

	public PointF(SizeF size)
	{
		X = size.Width;
		Y = size.Height;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator PointF(Vector2 vector)
	{
		return new PointF(vector.X, vector.Y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Vector2(PointF point)
	{
		return new Vector2(point.X, point.Y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator Point(PointF point)
	{
		return Point.Truncate(point);
	}

	public static PointF operator -(PointF value)
	{
		return new PointF(0f - value.X, 0f - value.Y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static PointF operator +(PointF point, SizeF size)
	{
		return Add(point, size);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static PointF operator -(PointF point, PointF size)
	{
		return Subtract(point, size);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static PointF operator +(PointF point, PointF size)
	{
		return Add(point, size);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static PointF operator -(PointF point, SizeF size)
	{
		return Subtract(point, size);
	}

	public static PointF operator *(float left, PointF right)
	{
		return Multiply(right, left);
	}

	public static PointF operator *(PointF left, float right)
	{
		return Multiply(left, right);
	}

	public static PointF operator /(PointF left, float right)
	{
		return new PointF(left.X / right, left.Y / right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(PointF left, PointF right)
	{
		return left.Equals(right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(PointF left, PointF right)
	{
		return !left.Equals(right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static PointF Add(PointF point, SizeF size)
	{
		return new PointF(point.X + size.Width, point.Y + size.Height);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static PointF Add(PointF point, PointF pointb)
	{
		return new PointF(point.X + pointb.X, point.Y + pointb.Y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static PointF Subtract(PointF point, SizeF size)
	{
		return new PointF(point.X - size.Width, point.Y - size.Height);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static PointF Subtract(PointF point, PointF pointb)
	{
		return new PointF(point.X - pointb.X, point.Y - pointb.Y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static PointF Multiply(PointF point, float right)
	{
		return new PointF(point.X * right, point.Y * right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static PointF Transform(PointF point, Matrix3x2 matrix)
	{
		return Vector2.Transform(point, matrix);
	}

	public void Deconstruct(out float x, out float y)
	{
		x = X;
		y = Y;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Offset(float dx, float dy)
	{
		X += dx;
		Y += dy;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Offset(PointF point)
	{
		Offset(point.X, point.Y);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(X, Y);
	}

	public override string ToString()
	{
		return $"PointF [ X={X}, Y={Y} ]";
	}

	public override bool Equals(object? obj)
	{
		if (obj is PointF other)
		{
			return Equals(other);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(PointF other)
	{
		if (X.Equals(other.X))
		{
			return Y.Equals(other.Y);
		}
		return false;
	}
}
