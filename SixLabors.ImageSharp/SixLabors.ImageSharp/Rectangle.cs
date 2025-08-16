using System;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SixLabors.ImageSharp;

public struct Rectangle : IEquatable<Rectangle>
{
	public static readonly Rectangle Empty;

	public int X { get; set; }

	public int Y { get; set; }

	public int Width { get; set; }

	public int Height { get; set; }

	[EditorBrowsable(EditorBrowsableState.Never)]
	public Point Location
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return new Point(X, Y);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			X = value.X;
			Y = value.Y;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public Size Size
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return new Size(Width, Height);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			Width = value.Width;
			Height = value.Height;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public bool IsEmpty => Equals(Empty);

	public int Top => Y;

	public int Right
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return X + Width;
		}
	}

	public int Bottom
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return Y + Height;
		}
	}

	public int Left => X;

	public Rectangle(int x, int y, int width, int height)
	{
		X = x;
		Y = y;
		Width = width;
		Height = height;
	}

	public Rectangle(Point point, Size size)
	{
		X = point.X;
		Y = point.Y;
		Width = size.Width;
		Height = size.Height;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator RectangleF(Rectangle rectangle)
	{
		return new RectangleF(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Vector4(Rectangle rectangle)
	{
		return new Vector4(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(Rectangle left, Rectangle right)
	{
		return left.Equals(right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(Rectangle left, Rectangle right)
	{
		return !left.Equals(right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Rectangle FromLTRB(int left, int top, int right, int bottom)
	{
		return new Rectangle(left, top, right - left, bottom - top);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Point Center(Rectangle rectangle)
	{
		return new Point(rectangle.Left + (rectangle.Width >> 1), rectangle.Top + (rectangle.Height >> 1));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Rectangle Intersect(Rectangle a, Rectangle b)
	{
		int num = Math.Max(a.X, b.X);
		int num2 = Math.Min(a.Right, b.Right);
		int num3 = Math.Max(a.Y, b.Y);
		int num4 = Math.Min(a.Bottom, b.Bottom);
		if (num2 >= num && num4 >= num3)
		{
			return new Rectangle(num, num3, num2 - num, num4 - num3);
		}
		return Empty;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Rectangle Inflate(Rectangle rectangle, int x, int y)
	{
		Rectangle result = rectangle;
		result.Inflate(x, y);
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Rectangle Ceiling(RectangleF rectangle)
	{
		return new Rectangle((int)MathF.Ceiling(rectangle.X), (int)MathF.Ceiling(rectangle.Y), (int)MathF.Ceiling(rectangle.Width), (int)MathF.Ceiling(rectangle.Height));
	}

	public static RectangleF Transform(Rectangle rectangle, Matrix3x2 matrix)
	{
		PointF pointF = Point.Transform(new Point(rectangle.Right, rectangle.Bottom), matrix);
		PointF pointF2 = Point.Transform(rectangle.Location, matrix);
		return new RectangleF(pointF2, new SizeF(pointF - pointF2));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Rectangle Truncate(RectangleF rectangle)
	{
		return new Rectangle((int)rectangle.X, (int)rectangle.Y, (int)rectangle.Width, (int)rectangle.Height);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Rectangle Round(RectangleF rectangle)
	{
		return new Rectangle((int)MathF.Round(rectangle.X), (int)MathF.Round(rectangle.Y), (int)MathF.Round(rectangle.Width), (int)MathF.Round(rectangle.Height));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Rectangle Union(Rectangle a, Rectangle b)
	{
		int num = Math.Min(a.X, b.X);
		int num2 = Math.Max(a.Right, b.Right);
		int num3 = Math.Min(a.Y, b.Y);
		int num4 = Math.Max(a.Bottom, b.Bottom);
		return new Rectangle(num, num3, num2 - num, num4 - num3);
	}

	public void Deconstruct(out int x, out int y, out int width, out int height)
	{
		x = X;
		y = Y;
		width = Width;
		height = Height;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Intersect(Rectangle rectangle)
	{
		Rectangle rectangle2 = Intersect(rectangle, this);
		X = rectangle2.X;
		Y = rectangle2.Y;
		Width = rectangle2.Width;
		Height = rectangle2.Height;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Inflate(int width, int height)
	{
		X -= width;
		Y -= height;
		Width += 2 * width;
		Height += 2 * height;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Inflate(Size size)
	{
		Inflate(size.Width, size.Height);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Contains(int x, int y)
	{
		if (X <= x && x < Right && Y <= y)
		{
			return y < Bottom;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Contains(Point point)
	{
		return Contains(point.X, point.Y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Contains(Rectangle rectangle)
	{
		if (X <= rectangle.X && rectangle.Right <= Right && Y <= rectangle.Y)
		{
			return rectangle.Bottom <= Bottom;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IntersectsWith(Rectangle rectangle)
	{
		if (rectangle.X < Right && X < rectangle.Right && rectangle.Y < Bottom)
		{
			return Y < rectangle.Bottom;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Offset(Point point)
	{
		Offset(point.X, point.Y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Offset(int dx, int dy)
	{
		X += dx;
		Y += dy;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(X, Y, Width, Height);
	}

	public override string ToString()
	{
		return $"Rectangle [ X={X}, Y={Y}, Width={Width}, Height={Height} ]";
	}

	public override bool Equals(object? obj)
	{
		if (obj is Rectangle other)
		{
			return Equals(other);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(Rectangle other)
	{
		if (X.Equals(other.X) && Y.Equals(other.Y) && Width.Equals(other.Width))
		{
			return Height.Equals(other.Height);
		}
		return false;
	}
}
