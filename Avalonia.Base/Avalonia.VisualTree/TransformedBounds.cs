using System;

namespace Avalonia.VisualTree;

public readonly struct TransformedBounds : IEquatable<TransformedBounds>
{
	public Rect Bounds { get; }

	public Rect Clip { get; }

	public Matrix Transform { get; }

	public TransformedBounds(Rect bounds, Rect clip, Matrix transform)
	{
		Bounds = bounds;
		Clip = clip;
		Transform = transform;
	}

	public bool Contains(Point point)
	{
		if (Transform.HasInverse)
		{
			Point p = point * Transform.Invert();
			return Bounds.Contains(p);
		}
		return Bounds.Contains(point);
	}

	public bool Equals(TransformedBounds other)
	{
		if (Bounds == other.Bounds && Clip == other.Clip)
		{
			return Transform == other.Transform;
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is TransformedBounds other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (((Bounds.GetHashCode() * 397) ^ Clip.GetHashCode()) * 397) ^ Transform.GetHashCode();
	}

	public static bool operator ==(TransformedBounds left, TransformedBounds right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(TransformedBounds left, TransformedBounds right)
	{
		return !left.Equals(right);
	}

	public override string ToString()
	{
		return FormattableString.Invariant($"Bounds: {Bounds} Clip: {Clip} Transform {Transform}");
	}
}
