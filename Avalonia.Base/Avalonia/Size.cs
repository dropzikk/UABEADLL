using System;
using System.Globalization;
using System.Numerics;
using Avalonia.Utilities;

namespace Avalonia;

public readonly struct Size : IEquatable<Size>
{
	public static readonly Size Infinity = new Size(double.PositiveInfinity, double.PositiveInfinity);

	private readonly double _width;

	private readonly double _height;

	public double AspectRatio => _width / _height;

	public double Width => _width;

	public double Height => _height;

	public Size(double width, double height)
	{
		_width = width;
		_height = height;
	}

	public Size(Vector2 vector2)
		: this(vector2.X, vector2.Y)
	{
	}

	public static bool operator ==(Size left, Size right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(Size left, Size right)
	{
		return !(left == right);
	}

	public static Size operator *(Size size, Vector scale)
	{
		return new Size(size._width * scale.X, size._height * scale.Y);
	}

	public static Size operator /(Size size, Vector scale)
	{
		return new Size(size._width / scale.X, size._height / scale.Y);
	}

	public static Vector operator /(Size left, Size right)
	{
		return new Vector(left._width / right._width, left._height / right._height);
	}

	public static Size operator *(Size size, double scale)
	{
		return new Size(size._width * scale, size._height * scale);
	}

	public static Size operator /(Size size, double scale)
	{
		return new Size(size._width / scale, size._height / scale);
	}

	public static Size operator +(Size size, Size toAdd)
	{
		return new Size(size._width + toAdd._width, size._height + toAdd._height);
	}

	public static Size operator -(Size size, Size toSubtract)
	{
		return new Size(size._width - toSubtract._width, size._height - toSubtract._height);
	}

	public static Size Parse(string s)
	{
		using StringTokenizer stringTokenizer = new StringTokenizer(s, CultureInfo.InvariantCulture, "Invalid Size.");
		return new Size(stringTokenizer.ReadDouble(null), stringTokenizer.ReadDouble(null));
	}

	public Size Constrain(Size constraint)
	{
		return new Size(Math.Min(_width, constraint._width), Math.Min(_height, constraint._height));
	}

	public Size Deflate(Thickness thickness)
	{
		return new Size(Math.Max(0.0, _width - thickness.Left - thickness.Right), Math.Max(0.0, _height - thickness.Top - thickness.Bottom));
	}

	public bool Equals(Size other)
	{
		if (_width == other._width)
		{
			return _height == other._height;
		}
		return false;
	}

	public bool NearlyEquals(Size other)
	{
		if (MathUtilities.AreClose(_width, other._width))
		{
			return MathUtilities.AreClose(_height, other._height);
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is Size other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (17 * 23 + Width.GetHashCode()) * 23 + Height.GetHashCode();
	}

	public Size Inflate(Thickness thickness)
	{
		return new Size(_width + thickness.Left + thickness.Right, _height + thickness.Top + thickness.Bottom);
	}

	public Size WithWidth(double width)
	{
		return new Size(width, _height);
	}

	public Size WithHeight(double height)
	{
		return new Size(_width, height);
	}

	public override string ToString()
	{
		return string.Format(CultureInfo.InvariantCulture, "{0}, {1}", _width, _height);
	}

	public void Deconstruct(out double width, out double height)
	{
		width = _width;
		height = _height;
	}
}
