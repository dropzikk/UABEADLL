using System;
using System.Globalization;
using Avalonia.Utilities;

namespace Avalonia;

public readonly struct Thickness : IEquatable<Thickness>
{
	private readonly double _left;

	private readonly double _top;

	private readonly double _right;

	private readonly double _bottom;

	public double Left => _left;

	public double Top => _top;

	public double Right => _right;

	public double Bottom => _bottom;

	public bool IsUniform
	{
		get
		{
			if (Left.Equals(Right) && Top.Equals(Bottom))
			{
				return Right.Equals(Bottom);
			}
			return false;
		}
	}

	public Thickness(double uniformLength)
	{
		_left = (_top = (_right = (_bottom = uniformLength)));
	}

	public Thickness(double horizontal, double vertical)
	{
		_left = (_right = horizontal);
		_top = (_bottom = vertical);
	}

	public Thickness(double left, double top, double right, double bottom)
	{
		_left = left;
		_top = top;
		_right = right;
		_bottom = bottom;
	}

	public static bool operator ==(Thickness a, Thickness b)
	{
		return a.Equals(b);
	}

	public static bool operator !=(Thickness a, Thickness b)
	{
		return !a.Equals(b);
	}

	public static Thickness operator +(Thickness a, Thickness b)
	{
		return new Thickness(a.Left + b.Left, a.Top + b.Top, a.Right + b.Right, a.Bottom + b.Bottom);
	}

	public static Thickness operator -(Thickness a, Thickness b)
	{
		return new Thickness(a.Left - b.Left, a.Top - b.Top, a.Right - b.Right, a.Bottom - b.Bottom);
	}

	public static Thickness operator *(Thickness a, double b)
	{
		return new Thickness(a.Left * b, a.Top * b, a.Right * b, a.Bottom * b);
	}

	public static Size operator +(Size size, Thickness thickness)
	{
		return new Size(size.Width + thickness.Left + thickness.Right, size.Height + thickness.Top + thickness.Bottom);
	}

	public static Size operator -(Size size, Thickness thickness)
	{
		return new Size(size.Width - (thickness.Left + thickness.Right), size.Height - (thickness.Top + thickness.Bottom));
	}

	public static Thickness Parse(string s)
	{
		using StringTokenizer stringTokenizer = new StringTokenizer(s, CultureInfo.InvariantCulture, "Invalid Thickness.");
		if (stringTokenizer.TryReadDouble(out var result, null))
		{
			if (stringTokenizer.TryReadDouble(out var result2, null))
			{
				if (stringTokenizer.TryReadDouble(out var result3, null))
				{
					return new Thickness(result, result2, result3, stringTokenizer.ReadDouble(null));
				}
				return new Thickness(result, result2);
			}
			return new Thickness(result);
		}
		throw new FormatException("Invalid Thickness.");
	}

	public bool Equals(Thickness other)
	{
		if (_left == other._left && _top == other._top && _right == other._right)
		{
			return _bottom == other._bottom;
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is Thickness other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (((17 * 23 + Left.GetHashCode()) * 23 + Top.GetHashCode()) * 23 + Right.GetHashCode()) * 23 + Bottom.GetHashCode();
	}

	public override string ToString()
	{
		return FormattableString.Invariant($"{_left},{_top},{_right},{_bottom}");
	}

	public void Deconstruct(out double left, out double top, out double right, out double bottom)
	{
		left = _left;
		top = _top;
		right = _right;
		bottom = _bottom;
	}
}
