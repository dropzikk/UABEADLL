using System;

namespace Avalonia;

public struct RoundedRect
{
	public Rect Rect { get; }

	public Vector RadiiTopLeft { get; }

	public Vector RadiiTopRight { get; }

	public Vector RadiiBottomLeft { get; }

	public Vector RadiiBottomRight { get; }

	public bool IsRounded
	{
		get
		{
			if (!(RadiiTopLeft != default(Vector)) && !(RadiiTopRight != default(Vector)) && !(RadiiBottomRight != default(Vector)))
			{
				return RadiiBottomLeft != default(Vector);
			}
			return true;
		}
	}

	public bool IsUniform
	{
		get
		{
			if (RadiiTopLeft.Equals(RadiiTopRight) && RadiiTopLeft.Equals(RadiiBottomRight))
			{
				return RadiiTopLeft.Equals(RadiiBottomLeft);
			}
			return false;
		}
	}

	public bool Equals(RoundedRect other)
	{
		if (Rect.Equals(other.Rect) && RadiiTopLeft.Equals(other.RadiiTopLeft) && RadiiTopRight.Equals(other.RadiiTopRight) && RadiiBottomLeft.Equals(other.RadiiBottomLeft))
		{
			return RadiiBottomRight.Equals(other.RadiiBottomRight);
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is RoundedRect other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (((((((Rect.GetHashCode() * 397) ^ RadiiTopLeft.GetHashCode()) * 397) ^ RadiiTopRight.GetHashCode()) * 397) ^ RadiiBottomLeft.GetHashCode()) * 397) ^ RadiiBottomRight.GetHashCode();
	}

	public static bool operator ==(RoundedRect left, RoundedRect right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(RoundedRect left, RoundedRect right)
	{
		return !left.Equals(right);
	}

	public RoundedRect(Rect rect, Vector radiiTopLeft, Vector radiiTopRight, Vector radiiBottomRight, Vector radiiBottomLeft)
	{
		Rect = rect;
		RadiiTopLeft = radiiTopLeft;
		RadiiTopRight = radiiTopRight;
		RadiiBottomRight = radiiBottomRight;
		RadiiBottomLeft = radiiBottomLeft;
	}

	public RoundedRect(Rect rect, double radiusTopLeft, double radiusTopRight, double radiusBottomRight, double radiusBottomLeft)
		: this(rect, new Vector(radiusTopLeft, radiusTopLeft), new Vector(radiusTopRight, radiusTopRight), new Vector(radiusBottomRight, radiusBottomRight), new Vector(radiusBottomLeft, radiusBottomLeft))
	{
	}

	public RoundedRect(Rect rect, Vector radii)
		: this(rect, radii, radii, radii, radii)
	{
	}

	public RoundedRect(Rect rect, double radiusX, double radiusY)
		: this(rect, new Vector(radiusX, radiusY))
	{
	}

	public RoundedRect(Rect rect, double radius)
		: this(rect, radius, radius)
	{
	}

	public RoundedRect(Rect rect)
		: this(rect, 0.0)
	{
	}

	public RoundedRect(in Rect bounds, in CornerRadius radius)
		: this(bounds, radius.TopLeft, radius.TopRight, radius.BottomRight, radius.BottomLeft)
	{
	}

	public static implicit operator RoundedRect(Rect r)
	{
		return new RoundedRect(r);
	}

	public RoundedRect Inflate(double dx, double dy)
	{
		return Deflate(0.0 - dx, 0.0 - dy);
	}

	public unsafe RoundedRect Deflate(double dx, double dy)
	{
		if (!IsRounded)
		{
			return new RoundedRect(Rect.Deflate(new Thickness(dx, dy)));
		}
		double num = Rect.X + dx;
		double num2 = Rect.Y + dy;
		double num3 = num + Rect.Width - dx * 2.0;
		double num4 = num2 + Rect.Height - dy * 2.0;
		Vector* ptr = stackalloc Vector[4];
		*ptr = RadiiTopLeft;
		ptr[1] = RadiiTopRight;
		ptr[2] = RadiiBottomRight;
		ptr[3] = RadiiBottomLeft;
		bool flag = false;
		if (num3 <= num)
		{
			flag = true;
			num = (num3 = (num + num3) * 0.5);
		}
		if (num4 <= num2)
		{
			flag = true;
			num2 = (num4 = (num2 + num4) * 0.5);
		}
		if (flag)
		{
			return new RoundedRect(new Rect(num, num2, num3 - num, num4 - num2));
		}
		for (int i = 0; i < 4; i++)
		{
			double num5 = Math.Max(0.0, ptr[i].X - dx);
			double num6 = Math.Max(0.0, ptr[i].Y - dy);
			if (num5 == 0.0 || num6 == 0.0)
			{
				ptr[i] = default(Vector);
			}
			else
			{
				ptr[i] = new Vector(num5, num6);
			}
		}
		return new RoundedRect(new Rect(num, num2, num3 - num, num4 - num2), *ptr, ptr[1], ptr[2], ptr[3]);
	}

	internal bool IsEmpty()
	{
		return this == default(RoundedRect);
	}
}
