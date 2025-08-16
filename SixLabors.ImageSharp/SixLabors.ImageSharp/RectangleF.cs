using System;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SixLabors.ImageSharp;

public struct RectangleF : IEquatable<RectangleF>
{
	public static readonly RectangleF Empty;

	public float X { get; set; }

	public float Y { get; set; }

	public float Width { get; set; }

	public float Height { get; set; }

	[EditorBrowsable(EditorBrowsableState.Never)]
	public PointF Location
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return new PointF(X, Y);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			X = value.X;
			Y = value.Y;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public SizeF Size
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return new SizeF(Width, Height);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			Width = value.Width;
			Height = value.Height;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public bool IsEmpty
	{
		get
		{
			if (!(Width <= 0f))
			{
				return Height <= 0f;
			}
			return true;
		}
	}

	public float Top => Y;

	public float Right
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return X + Width;
		}
	}

	public float Bottom
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return Y + Height;
		}
	}

	public float Left => X;

	public RectangleF(float x, float y, float width, float height)
	{
		X = x;
		Y = y;
		Width = width;
		Height = height;
	}

	public RectangleF(PointF point, SizeF size)
	{
		X = point.X;
		Y = point.Y;
		Width = size.Width;
		Height = size.Height;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator Rectangle(RectangleF rectangle)
	{
		return Rectangle.Truncate(rectangle);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(RectangleF left, RectangleF right)
	{
		return left.Equals(right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(RectangleF left, RectangleF right)
	{
		return !left.Equals(right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static RectangleF FromLTRB(float left, float top, float right, float bottom)
	{
		return new RectangleF(left, top, right - left, bottom - top);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static PointF Center(RectangleF rectangle)
	{
		return new PointF(rectangle.Left + rectangle.Width / 2f, rectangle.Top + rectangle.Height / 2f);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static RectangleF Intersect(RectangleF a, RectangleF b)
	{
		float num = MathF.Max(a.X, b.X);
		float num2 = MathF.Min(a.Right, b.Right);
		float num3 = MathF.Max(a.Y, b.Y);
		float num4 = MathF.Min(a.Bottom, b.Bottom);
		if (num2 >= num && num4 >= num3)
		{
			return new RectangleF(num, num3, num2 - num, num4 - num3);
		}
		return Empty;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static RectangleF Inflate(RectangleF rectangle, float x, float y)
	{
		RectangleF result = rectangle;
		result.Inflate(x, y);
		return result;
	}

	public static RectangleF Transform(RectangleF rectangle, Matrix3x2 matrix)
	{
		PointF pointF = PointF.Transform(new PointF(rectangle.Right, rectangle.Bottom), matrix);
		PointF pointF2 = PointF.Transform(rectangle.Location, matrix);
		return new RectangleF(pointF2, new SizeF(pointF - pointF2));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static RectangleF Union(RectangleF a, RectangleF b)
	{
		float num = MathF.Min(a.X, b.X);
		float num2 = MathF.Max(a.Right, b.Right);
		float num3 = MathF.Min(a.Y, b.Y);
		float num4 = MathF.Max(a.Bottom, b.Bottom);
		return new RectangleF(num, num3, num2 - num, num4 - num3);
	}

	public void Deconstruct(out float x, out float y, out float width, out float height)
	{
		x = X;
		y = Y;
		width = Width;
		height = Height;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Intersect(RectangleF rectangle)
	{
		RectangleF rectangleF = Intersect(rectangle, this);
		X = rectangleF.X;
		Y = rectangleF.Y;
		Width = rectangleF.Width;
		Height = rectangleF.Height;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Inflate(float width, float height)
	{
		X -= width;
		Y -= height;
		Width += 2f * width;
		Height += 2f * height;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Inflate(SizeF size)
	{
		Inflate(size.Width, size.Height);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Contains(float x, float y)
	{
		if (X <= x && x < Right && Y <= y)
		{
			return y < Bottom;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Contains(PointF point)
	{
		return Contains(point.X, point.Y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Contains(RectangleF rectangle)
	{
		if (X <= rectangle.X && rectangle.Right <= Right && Y <= rectangle.Y)
		{
			return rectangle.Bottom <= Bottom;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IntersectsWith(RectangleF rectangle)
	{
		if (rectangle.X < Right && X < rectangle.Right && rectangle.Y < Bottom)
		{
			return Y < rectangle.Bottom;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Offset(PointF point)
	{
		Offset(point.X, point.Y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Offset(float dx, float dy)
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
		return $"RectangleF [ X={X}, Y={Y}, Width={Width}, Height={Height} ]";
	}

	public override bool Equals(object? obj)
	{
		if (obj is RectangleF other)
		{
			return Equals(other);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(RectangleF other)
	{
		if (X.Equals(other.X) && Y.Equals(other.Y) && Width.Equals(other.Width))
		{
			return Height.Equals(other.Height);
		}
		return false;
	}
}
