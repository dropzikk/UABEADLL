using System;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SixLabors.ImageSharp;

public struct SizeF : IEquatable<SizeF>
{
	public static readonly SizeF Empty;

	public float Width { get; set; }

	public float Height { get; set; }

	[EditorBrowsable(EditorBrowsableState.Never)]
	public bool IsEmpty => Equals(Empty);

	public SizeF(float width, float height)
	{
		Width = width;
		Height = height;
	}

	public SizeF(SizeF size)
	{
		this = default(SizeF);
		Width = size.Width;
		Height = size.Height;
	}

	public SizeF(PointF point)
	{
		Width = point.X;
		Height = point.Y;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Vector2(SizeF point)
	{
		return new Vector2(point.Width, point.Height);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator Size(SizeF size)
	{
		return new Size((int)size.Width, (int)size.Height);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator PointF(SizeF size)
	{
		return new PointF(size.Width, size.Height);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static SizeF operator +(SizeF left, SizeF right)
	{
		return Add(left, right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static SizeF operator -(SizeF left, SizeF right)
	{
		return Subtract(left, right);
	}

	public static SizeF operator *(float left, SizeF right)
	{
		return Multiply(right, left);
	}

	public static SizeF operator *(SizeF left, float right)
	{
		return Multiply(left, right);
	}

	public static SizeF operator /(SizeF left, float right)
	{
		return new SizeF(left.Width / right, left.Height / right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(SizeF left, SizeF right)
	{
		return left.Equals(right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(SizeF left, SizeF right)
	{
		return !left.Equals(right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static SizeF Add(SizeF left, SizeF right)
	{
		return new SizeF(left.Width + right.Width, left.Height + right.Height);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static SizeF Subtract(SizeF left, SizeF right)
	{
		return new SizeF(left.Width - right.Width, left.Height - right.Height);
	}

	public static SizeF Transform(SizeF size, Matrix3x2 matrix)
	{
		Vector2 vector = Vector2.Transform(new Vector2(size.Width, size.Height), matrix);
		return new SizeF(vector.X, vector.Y);
	}

	public void Deconstruct(out float width, out float height)
	{
		width = Width;
		height = Height;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Width, Height);
	}

	public override string ToString()
	{
		return $"SizeF [ Width={Width}, Height={Height} ]";
	}

	public override bool Equals(object? obj)
	{
		if (obj is SizeF)
		{
			return Equals((SizeF)obj);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(SizeF other)
	{
		if (Width.Equals(other.Width))
		{
			return Height.Equals(other.Height);
		}
		return false;
	}

	private static SizeF Multiply(SizeF size, float multiplier)
	{
		return new SizeF(size.Width * multiplier, size.Height * multiplier);
	}
}
