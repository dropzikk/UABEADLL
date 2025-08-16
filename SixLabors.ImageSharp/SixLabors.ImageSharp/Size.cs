using System;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SixLabors.ImageSharp;

public struct Size : IEquatable<Size>
{
	public static readonly Size Empty;

	public int Width { get; set; }

	public int Height { get; set; }

	[EditorBrowsable(EditorBrowsableState.Never)]
	public bool IsEmpty => Equals(Empty);

	public Size(int value)
	{
		this = default(Size);
		Width = value;
		Height = value;
	}

	public Size(int width, int height)
	{
		Width = width;
		Height = height;
	}

	public Size(Size size)
	{
		this = default(Size);
		Width = size.Width;
		Height = size.Height;
	}

	public Size(Point point)
	{
		Width = point.X;
		Height = point.Y;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator SizeF(Size size)
	{
		return new SizeF(size.Width, size.Height);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator Point(Size size)
	{
		return new Point(size.Width, size.Height);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Size operator +(Size left, Size right)
	{
		return Add(left, right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Size operator -(Size left, Size right)
	{
		return Subtract(left, right);
	}

	public static Size operator *(int left, Size right)
	{
		return Multiply(right, left);
	}

	public static Size operator *(Size left, int right)
	{
		return Multiply(left, right);
	}

	public static Size operator /(Size left, int right)
	{
		return new Size(left.Width / right, left.Height / right);
	}

	public static SizeF operator *(float left, Size right)
	{
		return Multiply(right, left);
	}

	public static SizeF operator *(Size left, float right)
	{
		return Multiply(left, right);
	}

	public static SizeF operator /(Size left, float right)
	{
		return new SizeF((float)left.Width / right, (float)left.Height / right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(Size left, Size right)
	{
		return left.Equals(right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(Size left, Size right)
	{
		return !left.Equals(right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Size Add(Size left, Size right)
	{
		return new Size(left.Width + right.Width, left.Height + right.Height);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Size Subtract(Size left, Size right)
	{
		return new Size(left.Width - right.Width, left.Height - right.Height);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Size Ceiling(SizeF size)
	{
		return new Size((int)MathF.Ceiling(size.Width), (int)MathF.Ceiling(size.Height));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Size Round(SizeF size)
	{
		return new Size((int)MathF.Round(size.Width), (int)MathF.Round(size.Height));
	}

	public static SizeF Transform(Size size, Matrix3x2 matrix)
	{
		Vector2 vector = Vector2.Transform(new Vector2(size.Width, size.Height), matrix);
		return new SizeF(vector.X, vector.Y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Size Truncate(SizeF size)
	{
		return new Size((int)size.Width, (int)size.Height);
	}

	public void Deconstruct(out int width, out int height)
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
		return $"Size [ Width={Width}, Height={Height} ]";
	}

	public override bool Equals(object? obj)
	{
		if (obj is Size other)
		{
			return Equals(other);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(Size other)
	{
		if (Width.Equals(other.Width))
		{
			return Height.Equals(other.Height);
		}
		return false;
	}

	private static Size Multiply(Size size, int multiplier)
	{
		return new Size(size.Width * multiplier, size.Height * multiplier);
	}

	private static SizeF Multiply(Size size, float multiplier)
	{
		return new SizeF((float)size.Width * multiplier, (float)size.Height * multiplier);
	}
}
