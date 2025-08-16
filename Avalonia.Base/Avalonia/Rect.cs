using System;
using System.Globalization;
using System.Numerics;
using Avalonia.Utilities;

namespace Avalonia;

public readonly struct Rect : IEquatable<Rect>
{
	private readonly double _x;

	private readonly double _y;

	private readonly double _width;

	private readonly double _height;

	public double X => _x;

	public double Y => _y;

	public double Width => _width;

	public double Height => _height;

	public Point Position => new Point(_x, _y);

	public Size Size => new Size(_width, _height);

	public double Right => _x + _width;

	public double Bottom => _y + _height;

	public double Left => _x;

	public double Top => _y;

	public Point TopLeft => new Point(_x, _y);

	public Point TopRight => new Point(Right, _y);

	public Point BottomLeft => new Point(_x, Bottom);

	public Point BottomRight => new Point(Right, Bottom);

	public Point Center => new Point(_x + _width / 2.0, _y + _height / 2.0);

	public Rect(double x, double y, double width, double height)
	{
		_x = x;
		_y = y;
		_width = width;
		_height = height;
	}

	public Rect(Size size)
	{
		_x = 0.0;
		_y = 0.0;
		_width = size.Width;
		_height = size.Height;
	}

	public Rect(Point position, Size size)
	{
		_x = position.X;
		_y = position.Y;
		_width = size.Width;
		_height = size.Height;
	}

	public Rect(Point topLeft, Point bottomRight)
	{
		_x = topLeft.X;
		_y = topLeft.Y;
		_width = bottomRight.X - topLeft.X;
		_height = bottomRight.Y - topLeft.Y;
	}

	public static bool operator ==(Rect left, Rect right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(Rect left, Rect right)
	{
		return !(left == right);
	}

	public static Rect operator *(Rect rect, Vector scale)
	{
		return new Rect(rect.X * scale.X, rect.Y * scale.Y, rect.Width * scale.X, rect.Height * scale.Y);
	}

	public static Rect operator *(Rect rect, double scale)
	{
		return new Rect(rect.X * scale, rect.Y * scale, rect.Width * scale, rect.Height * scale);
	}

	public static Rect operator /(Rect rect, Vector scale)
	{
		return new Rect(rect.X / scale.X, rect.Y / scale.Y, rect.Width / scale.X, rect.Height / scale.Y);
	}

	public bool Contains(Point p)
	{
		if (p.X >= _x && p.X <= _x + _width && p.Y >= _y)
		{
			return p.Y <= _y + _height;
		}
		return false;
	}

	public bool ContainsExclusive(Point p)
	{
		if (p.X >= _x && p.X < _x + _width && p.Y >= _y)
		{
			return p.Y < _y + _height;
		}
		return false;
	}

	public bool Contains(Rect r)
	{
		if (Contains(r.TopLeft))
		{
			return Contains(r.BottomRight);
		}
		return false;
	}

	public Rect CenterRect(Rect rect)
	{
		return new Rect(_x + (_width - rect._width) / 2.0, _y + (_height - rect._height) / 2.0, rect._width, rect._height);
	}

	public Rect Inflate(double thickness)
	{
		return Inflate(new Thickness(thickness));
	}

	public Rect Inflate(Thickness thickness)
	{
		return new Rect(new Point(_x - thickness.Left, _y - thickness.Top), Size.Inflate(thickness));
	}

	public Rect Deflate(double thickness)
	{
		return Deflate(new Thickness(thickness));
	}

	public Rect Deflate(Thickness thickness)
	{
		return new Rect(new Point(_x + thickness.Left, _y + thickness.Top), Size.Deflate(thickness));
	}

	public bool Equals(Rect other)
	{
		if (_x == other._x && _y == other._y && _width == other._width)
		{
			return _height == other._height;
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is Rect other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (((17 * 23 + X.GetHashCode()) * 23 + Y.GetHashCode()) * 23 + Width.GetHashCode()) * 23 + Height.GetHashCode();
	}

	public Rect Intersect(Rect rect)
	{
		double num = ((rect.X > X) ? rect.X : X);
		double num2 = ((rect.Y > Y) ? rect.Y : Y);
		double num3 = ((rect.Right < Right) ? rect.Right : Right);
		double num4 = ((rect.Bottom < Bottom) ? rect.Bottom : Bottom);
		if (num3 > num && num4 > num2)
		{
			return new Rect(num, num2, num3 - num, num4 - num2);
		}
		return default(Rect);
	}

	public bool Intersects(Rect rect)
	{
		if (rect.X < Right && X < rect.Right && rect.Y < Bottom)
		{
			return Y < rect.Bottom;
		}
		return false;
	}

	public Rect TransformToAABB(Matrix matrix)
	{
		ReadOnlySpan<Point> readOnlySpan = stackalloc Point[4]
		{
			TopLeft.Transform(matrix),
			TopRight.Transform(matrix),
			BottomRight.Transform(matrix),
			BottomLeft.Transform(matrix)
		};
		double num = double.MaxValue;
		double num2 = double.MinValue;
		double num3 = double.MaxValue;
		double num4 = double.MinValue;
		ReadOnlySpan<Point> readOnlySpan2 = readOnlySpan;
		for (int i = 0; i < readOnlySpan2.Length; i++)
		{
			Point point = readOnlySpan2[i];
			if (point.X < num)
			{
				num = point.X;
			}
			if (point.X > num2)
			{
				num2 = point.X;
			}
			if (point.Y < num3)
			{
				num3 = point.Y;
			}
			if (point.Y > num4)
			{
				num4 = point.Y;
			}
		}
		return new Rect(new Point(num, num3), new Point(num2, num4));
	}

	internal Rect TransformToAABB(Matrix4x4 matrix)
	{
		ReadOnlySpan<Point> readOnlySpan = stackalloc Point[4]
		{
			TopLeft.Transform(matrix),
			TopRight.Transform(matrix),
			BottomRight.Transform(matrix),
			BottomLeft.Transform(matrix)
		};
		double num = double.MaxValue;
		double num2 = double.MinValue;
		double num3 = double.MaxValue;
		double num4 = double.MinValue;
		ReadOnlySpan<Point> readOnlySpan2 = readOnlySpan;
		for (int i = 0; i < readOnlySpan2.Length; i++)
		{
			Point point = readOnlySpan2[i];
			if (point.X < num)
			{
				num = point.X;
			}
			if (point.X > num2)
			{
				num2 = point.X;
			}
			if (point.Y < num3)
			{
				num3 = point.Y;
			}
			if (point.Y > num4)
			{
				num4 = point.Y;
			}
		}
		return new Rect(new Point(num, num3), new Point(num2, num4));
	}

	public Rect Translate(Vector offset)
	{
		return new Rect(Position + offset, Size);
	}

	public Rect Normalize()
	{
		Rect result = this;
		if (double.IsNaN(result.Right) || double.IsNaN(result.Bottom) || double.IsNaN(result.X) || double.IsNaN(result.Y) || double.IsNaN(Height) || double.IsNaN(Width))
		{
			return default(Rect);
		}
		if (result.Width < 0.0)
		{
			double num = X + Width;
			double width = X - num;
			result = result.WithX(num).WithWidth(width);
		}
		if (result.Height < 0.0)
		{
			double num2 = Y + Height;
			double height = Y - num2;
			result = result.WithY(num2).WithHeight(height);
		}
		return result;
	}

	public Rect Union(Rect rect)
	{
		if (Width == 0.0 && Height == 0.0)
		{
			return rect;
		}
		if (rect.Width == 0.0 && rect.Height == 0.0)
		{
			return this;
		}
		double x = Math.Min(X, rect.X);
		double x2 = Math.Max(Right, rect.Right);
		double y = Math.Min(Y, rect.Y);
		return new Rect(bottomRight: new Point(x2, Math.Max(Bottom, rect.Bottom)), topLeft: new Point(x, y));
	}

	internal static Rect? Union(Rect? left, Rect? right)
	{
		if (!left.HasValue)
		{
			return right;
		}
		if (!right.HasValue)
		{
			return left;
		}
		return left.Value.Union(right.Value);
	}

	public Rect WithX(double x)
	{
		return new Rect(x, _y, _width, _height);
	}

	public Rect WithY(double y)
	{
		return new Rect(_x, y, _width, _height);
	}

	public Rect WithWidth(double width)
	{
		return new Rect(_x, _y, width, _height);
	}

	public Rect WithHeight(double height)
	{
		return new Rect(_x, _y, _width, height);
	}

	public override string ToString()
	{
		return string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}, {3}", _x, _y, _width, _height);
	}

	public static Rect Parse(string s)
	{
		using StringTokenizer stringTokenizer = new StringTokenizer(s, CultureInfo.InvariantCulture, "Invalid Rect.");
		return new Rect(stringTokenizer.ReadDouble(null), stringTokenizer.ReadDouble(null), stringTokenizer.ReadDouble(null), stringTokenizer.ReadDouble(null));
	}

	internal bool IsEmpty()
	{
		return this == default(Rect);
	}
}
