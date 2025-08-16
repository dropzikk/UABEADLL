using System;
using System.Globalization;
using Avalonia.Utilities;

namespace Avalonia;

public readonly struct RelativePoint : IEquatable<RelativePoint>
{
	public static readonly RelativePoint TopLeft = new RelativePoint(0.0, 0.0, RelativeUnit.Relative);

	public static readonly RelativePoint Center = new RelativePoint(0.5, 0.5, RelativeUnit.Relative);

	public static readonly RelativePoint BottomRight = new RelativePoint(1.0, 1.0, RelativeUnit.Relative);

	private readonly Point _point;

	private readonly RelativeUnit _unit;

	public Point Point => _point;

	public RelativeUnit Unit => _unit;

	public RelativePoint(double x, double y, RelativeUnit unit)
		: this(new Point(x, y), unit)
	{
	}

	public RelativePoint(Point point, RelativeUnit unit)
	{
		_point = point;
		_unit = unit;
	}

	public static bool operator ==(RelativePoint left, RelativePoint right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(RelativePoint left, RelativePoint right)
	{
		return !left.Equals(right);
	}

	public override bool Equals(object? obj)
	{
		if (obj is RelativePoint p)
		{
			return Equals(p);
		}
		return false;
	}

	public bool Equals(RelativePoint p)
	{
		if (Unit == p.Unit)
		{
			return Point == p.Point;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (_point.GetHashCode() * 397) ^ (int)_unit;
	}

	public Point ToPixels(Size size)
	{
		if (_unit != RelativeUnit.Absolute)
		{
			return new Point(_point.X * size.Width, _point.Y * size.Height);
		}
		return _point;
	}

	public static RelativePoint Parse(string s)
	{
		using StringTokenizer stringTokenizer = new StringTokenizer(s, CultureInfo.InvariantCulture, "Invalid RelativePoint.");
		string text = stringTokenizer.ReadString(null);
		string text2 = stringTokenizer.ReadString(null);
		RelativeUnit unit = RelativeUnit.Absolute;
		double num = 1.0;
		if (text.EndsWith("%"))
		{
			if (!text2.EndsWith("%"))
			{
				throw new FormatException("If one coordinate is relative, both must be.");
			}
			text = text.TrimEnd('%');
			text2 = text2.TrimEnd('%');
			unit = RelativeUnit.Relative;
			num = 0.01;
		}
		return new RelativePoint(double.Parse(text, CultureInfo.InvariantCulture) * num, double.Parse(text2, CultureInfo.InvariantCulture) * num, unit);
	}

	public override string ToString()
	{
		if (_unit != RelativeUnit.Absolute)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}%, {1}%", _point.X * 100.0, _point.Y * 100.0);
		}
		return _point.ToString();
	}
}
