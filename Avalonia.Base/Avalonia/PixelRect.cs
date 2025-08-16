using System;
using System.Globalization;
using Avalonia.Utilities;

namespace Avalonia;

public readonly struct PixelRect : IEquatable<PixelRect>
{
	public int X { get; }

	public int Y { get; }

	public int Width { get; }

	public int Height { get; }

	public PixelPoint Position => new PixelPoint(X, Y);

	public PixelSize Size => new PixelSize(Width, Height);

	public int Right => X + Width;

	public int Bottom => Y + Height;

	public PixelPoint TopLeft => new PixelPoint(X, Y);

	public PixelPoint TopRight => new PixelPoint(Right, Y);

	public PixelPoint BottomLeft => new PixelPoint(X, Bottom);

	public PixelPoint BottomRight => new PixelPoint(Right, Bottom);

	public PixelPoint Center => new PixelPoint(X + Width / 2, Y + Height / 2);

	public PixelRect(int x, int y, int width, int height)
	{
		X = x;
		Y = y;
		Width = width;
		Height = height;
	}

	public PixelRect(PixelSize size)
	{
		X = 0;
		Y = 0;
		Width = size.Width;
		Height = size.Height;
	}

	public PixelRect(PixelPoint position, PixelSize size)
	{
		X = position.X;
		Y = position.Y;
		Width = size.Width;
		Height = size.Height;
	}

	public PixelRect(PixelPoint topLeft, PixelPoint bottomRight)
	{
		X = topLeft.X;
		Y = topLeft.Y;
		Width = bottomRight.X - topLeft.X;
		Height = bottomRight.Y - topLeft.Y;
	}

	public static bool operator ==(PixelRect left, PixelRect right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(PixelRect left, PixelRect right)
	{
		return !(left == right);
	}

	public bool Contains(PixelPoint p)
	{
		if (p.X >= X && p.X <= Right && p.Y >= Y)
		{
			return p.Y <= Bottom;
		}
		return false;
	}

	public bool ContainsExclusive(PixelPoint p)
	{
		if (p.X >= X && p.X < X + Width && p.Y >= Y)
		{
			return p.Y < Y + Height;
		}
		return false;
	}

	public bool Contains(PixelRect r)
	{
		if (Contains(r.TopLeft))
		{
			return Contains(r.BottomRight);
		}
		return false;
	}

	public PixelRect CenterRect(PixelRect rect)
	{
		return new PixelRect(X + (Width - rect.Width) / 2, Y + (Height - rect.Height) / 2, rect.Width, rect.Height);
	}

	public bool Equals(PixelRect other)
	{
		if (Position == other.Position)
		{
			return Size == other.Size;
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is PixelRect other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (((17 * 23 + X.GetHashCode()) * 23 + Y.GetHashCode()) * 23 + Width.GetHashCode()) * 23 + Height.GetHashCode();
	}

	public PixelRect Intersect(PixelRect rect)
	{
		int num = ((rect.X > X) ? rect.X : X);
		int num2 = ((rect.Y > Y) ? rect.Y : Y);
		int num3 = ((rect.Right < Right) ? rect.Right : Right);
		int num4 = ((rect.Bottom < Bottom) ? rect.Bottom : Bottom);
		if (num3 > num && num4 > num2)
		{
			return new PixelRect(num, num2, num3 - num, num4 - num2);
		}
		return default(PixelRect);
	}

	public bool Intersects(PixelRect rect)
	{
		if (rect.X < Right && X < rect.Right && rect.Y < Bottom)
		{
			return Y < rect.Bottom;
		}
		return false;
	}

	public PixelRect Translate(PixelVector offset)
	{
		return new PixelRect(Position + offset, Size);
	}

	public PixelRect Union(PixelRect rect)
	{
		if (Width == 0 && Height == 0)
		{
			return rect;
		}
		if (rect.Width == 0 && rect.Height == 0)
		{
			return this;
		}
		int x = Math.Min(X, rect.X);
		int x2 = Math.Max(Right, rect.Right);
		int y = Math.Min(Y, rect.Y);
		return new PixelRect(bottomRight: new PixelPoint(x2, Math.Max(Bottom, rect.Bottom)), topLeft: new PixelPoint(x, y));
	}

	public PixelRect WithX(int x)
	{
		return new PixelRect(x, Y, Width, Height);
	}

	public PixelRect WithY(int y)
	{
		return new PixelRect(X, y, Width, Height);
	}

	public PixelRect WithWidth(int width)
	{
		return new PixelRect(X, Y, width, Height);
	}

	public PixelRect WithHeight(int height)
	{
		return new PixelRect(X, Y, Width, height);
	}

	public Rect ToRect(double scale)
	{
		return new Rect(Position.ToPoint(scale), Size.ToSize(scale));
	}

	public Rect ToRect(Vector scale)
	{
		return new Rect(Position.ToPoint(scale), Size.ToSize(scale));
	}

	public Rect ToRectWithDpi(double dpi)
	{
		return new Rect(Position.ToPointWithDpi(dpi), Size.ToSizeWithDpi(dpi));
	}

	public Rect ToRectWithDpi(Vector dpi)
	{
		return new Rect(Position.ToPointWithDpi(dpi), Size.ToSizeWithDpi(dpi));
	}

	public static PixelRect FromRect(Rect rect, double scale)
	{
		return new PixelRect(PixelPoint.FromPoint(rect.Position, scale), FromPointCeiling(rect.BottomRight, new Vector(scale, scale)));
	}

	public static PixelRect FromRect(Rect rect, Vector scale)
	{
		return new PixelRect(PixelPoint.FromPoint(rect.Position, scale), FromPointCeiling(rect.BottomRight, scale));
	}

	public static PixelRect FromRectWithDpi(Rect rect, double dpi)
	{
		return new PixelRect(PixelPoint.FromPointWithDpi(rect.Position, dpi), FromPointCeiling(rect.BottomRight, new Vector(dpi / 96.0, dpi / 96.0)));
	}

	public static PixelRect FromRectWithDpi(Rect rect, Vector dpi)
	{
		return new PixelRect(PixelPoint.FromPointWithDpi(rect.Position, dpi), FromPointCeiling(rect.BottomRight, dpi / 96.0));
	}

	public override string ToString()
	{
		return string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}, {3}", X, Y, Width, Height);
	}

	public static PixelRect Parse(string s)
	{
		using StringTokenizer stringTokenizer = new StringTokenizer(s, CultureInfo.InvariantCulture, "Invalid PixelRect.");
		return new PixelRect(stringTokenizer.ReadInt32(null), stringTokenizer.ReadInt32(null), stringTokenizer.ReadInt32(null), stringTokenizer.ReadInt32(null));
	}

	private static PixelPoint FromPointCeiling(Point point, Vector scale)
	{
		return new PixelPoint((int)Math.Ceiling(point.X * scale.X), (int)Math.Ceiling(point.Y * scale.Y));
	}
}
