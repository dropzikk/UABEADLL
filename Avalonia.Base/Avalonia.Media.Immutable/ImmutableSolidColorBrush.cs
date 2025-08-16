using System;

namespace Avalonia.Media.Immutable;

public class ImmutableSolidColorBrush : IImmutableSolidColorBrush, ISolidColorBrush, IBrush, IImmutableBrush, IEquatable<ImmutableSolidColorBrush>
{
	public Color Color { get; }

	public double Opacity { get; }

	public ITransform? Transform { get; }

	public RelativePoint TransformOrigin { get; }

	public ImmutableSolidColorBrush(Color color, double opacity = 1.0, ImmutableTransform? transform = null)
	{
		Color = color;
		Opacity = opacity;
		Transform = null;
	}

	public ImmutableSolidColorBrush(uint color)
		: this(Color.FromUInt32(color))
	{
	}

	public ImmutableSolidColorBrush(ISolidColorBrush source)
		: this(source.Color, source.Opacity, source.Transform?.ToImmutable())
	{
	}

	public bool Equals(ImmutableSolidColorBrush? other)
	{
		if ((object)other == null)
		{
			return false;
		}
		if ((object)this == other)
		{
			return true;
		}
		if (Color.Equals(other.Color) && Opacity.Equals(other.Opacity))
		{
			if (Transform != null || other.Transform != null)
			{
				if (Transform != null)
				{
					return Transform.Equals(other.Transform);
				}
				return false;
			}
			return true;
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is ImmutableSolidColorBrush other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (Color.GetHashCode() * 397) ^ Opacity.GetHashCode() ^ ((Transform != null) ? Transform.GetHashCode() : 0);
	}

	public static bool operator ==(ImmutableSolidColorBrush left, ImmutableSolidColorBrush right)
	{
		return object.Equals(left, right);
	}

	public static bool operator !=(ImmutableSolidColorBrush left, ImmutableSolidColorBrush right)
	{
		return !object.Equals(left, right);
	}

	public override string ToString()
	{
		return Color.ToString();
	}
}
