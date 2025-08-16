using System;
using System.Globalization;
using Avalonia.Utilities;

namespace Avalonia;

public readonly struct PixelSize : IEquatable<PixelSize>
{
	public static readonly PixelSize Empty = new PixelSize(0, 0);

	public double AspectRatio => (double)Width / (double)Height;

	public int Width { get; }

	public int Height { get; }

	public PixelSize(int width, int height)
	{
		Width = width;
		Height = height;
	}

	public static bool operator ==(PixelSize left, PixelSize right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(PixelSize left, PixelSize right)
	{
		return !(left == right);
	}

	public static PixelSize Parse(string s)
	{
		using StringTokenizer stringTokenizer = new StringTokenizer(s, CultureInfo.InvariantCulture, "Invalid PixelSize.");
		return new PixelSize(stringTokenizer.ReadInt32(null), stringTokenizer.ReadInt32(null));
	}

	public bool Equals(PixelSize other)
	{
		if (Width == other.Width)
		{
			return Height == other.Height;
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is PixelSize other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (17 * 23 + Width.GetHashCode()) * 23 + Height.GetHashCode();
	}

	public PixelSize WithWidth(int width)
	{
		return new PixelSize(width, Height);
	}

	public PixelSize WithHeight(int height)
	{
		return new PixelSize(Width, height);
	}

	public Size ToSize(double scale)
	{
		return new Size((double)Width / scale, (double)Height / scale);
	}

	public Size ToSize(Vector scale)
	{
		return new Size((double)Width / scale.X, (double)Height / scale.Y);
	}

	public Size ToSizeWithDpi(double dpi)
	{
		return ToSize(dpi / 96.0);
	}

	public Size ToSizeWithDpi(Vector dpi)
	{
		return ToSize(new Vector(dpi.X / 96.0, dpi.Y / 96.0));
	}

	public static PixelSize FromSize(Size size, double scale)
	{
		return new PixelSize((int)Math.Ceiling(size.Width * scale), (int)Math.Ceiling(size.Height * scale));
	}

	public static PixelSize FromSize(Size size, Vector scale)
	{
		return new PixelSize((int)Math.Ceiling(size.Width * scale.X), (int)Math.Ceiling(size.Height * scale.Y));
	}

	public static PixelSize FromSizeWithDpi(Size size, double dpi)
	{
		return FromSize(size, dpi / 96.0);
	}

	public static PixelSize FromSizeWithDpi(Size size, Vector dpi)
	{
		return FromSize(size, new Vector(dpi.X / 96.0, dpi.Y / 96.0));
	}

	public override string ToString()
	{
		return string.Format(CultureInfo.InvariantCulture, "{0}, {1}", Width, Height);
	}
}
