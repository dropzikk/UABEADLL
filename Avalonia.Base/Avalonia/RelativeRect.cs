using System;
using System.Globalization;
using Avalonia.Utilities;

namespace Avalonia;

public readonly struct RelativeRect : IEquatable<RelativeRect>
{
	private static readonly char[] PercentChar = new char[1] { '%' };

	public static readonly RelativeRect Fill = new RelativeRect(0.0, 0.0, 1.0, 1.0, RelativeUnit.Relative);

	public RelativeUnit Unit { get; }

	public Rect Rect { get; }

	public RelativeRect(double x, double y, double width, double height, RelativeUnit unit)
	{
		Rect = new Rect(x, y, width, height);
		Unit = unit;
	}

	public RelativeRect(Rect rect, RelativeUnit unit)
	{
		Rect = rect;
		Unit = unit;
	}

	public RelativeRect(Size size, RelativeUnit unit)
	{
		Rect = new Rect(size);
		Unit = unit;
	}

	public RelativeRect(Point position, Size size, RelativeUnit unit)
	{
		Rect = new Rect(position, size);
		Unit = unit;
	}

	public RelativeRect(Point topLeft, Point bottomRight, RelativeUnit unit)
	{
		Rect = new Rect(topLeft, bottomRight);
		Unit = unit;
	}

	public static bool operator ==(RelativeRect left, RelativeRect right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(RelativeRect left, RelativeRect right)
	{
		return !left.Equals(right);
	}

	public override bool Equals(object? obj)
	{
		if (obj is RelativeRect p)
		{
			return Equals(p);
		}
		return false;
	}

	public bool Equals(RelativeRect p)
	{
		if (Unit == p.Unit)
		{
			return Rect == p.Rect;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return ((int)Unit * 397) ^ Rect.GetHashCode();
	}

	public Rect ToPixels(Size size)
	{
		if (Unit != RelativeUnit.Absolute)
		{
			return new Rect(Rect.X * size.Width, Rect.Y * size.Height, Rect.Width * size.Width, Rect.Height * size.Height);
		}
		return Rect;
	}

	public static RelativeRect Parse(string s)
	{
		using StringTokenizer stringTokenizer = new StringTokenizer(s, ',', "Invalid RelativeRect.");
		string text = stringTokenizer.ReadString(null);
		string text2 = stringTokenizer.ReadString(null);
		string text3 = stringTokenizer.ReadString(null);
		string text4 = stringTokenizer.ReadString(null);
		RelativeUnit unit = RelativeUnit.Absolute;
		double num = 1.0;
		bool flag = text.EndsWith("%", StringComparison.Ordinal);
		bool flag2 = text2.EndsWith("%", StringComparison.Ordinal);
		bool flag3 = text3.EndsWith("%", StringComparison.Ordinal);
		bool flag4 = text4.EndsWith("%", StringComparison.Ordinal);
		if (flag && flag2 && flag3 && flag4)
		{
			text = text.TrimEnd(PercentChar);
			text2 = text2.TrimEnd(PercentChar);
			text3 = text3.TrimEnd(PercentChar);
			text4 = text4.TrimEnd(PercentChar);
			unit = RelativeUnit.Relative;
			num = 0.01;
		}
		else if (flag || flag2 || flag3 || flag4)
		{
			throw new FormatException("If one coordinate is relative, all must be.");
		}
		return new RelativeRect(double.Parse(text, CultureInfo.InvariantCulture) * num, double.Parse(text2, CultureInfo.InvariantCulture) * num, double.Parse(text3, CultureInfo.InvariantCulture) * num, double.Parse(text4, CultureInfo.InvariantCulture) * num, unit);
	}
}
