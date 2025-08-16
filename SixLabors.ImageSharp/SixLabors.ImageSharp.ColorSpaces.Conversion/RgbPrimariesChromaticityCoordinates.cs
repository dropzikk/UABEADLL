using System;

namespace SixLabors.ImageSharp.ColorSpaces.Conversion;

public readonly struct RgbPrimariesChromaticityCoordinates : IEquatable<RgbPrimariesChromaticityCoordinates>
{
	public CieXyChromaticityCoordinates R { get; }

	public CieXyChromaticityCoordinates G { get; }

	public CieXyChromaticityCoordinates B { get; }

	public RgbPrimariesChromaticityCoordinates(CieXyChromaticityCoordinates r, CieXyChromaticityCoordinates g, CieXyChromaticityCoordinates b)
	{
		R = r;
		G = g;
		B = b;
	}

	public static bool operator ==(RgbPrimariesChromaticityCoordinates left, RgbPrimariesChromaticityCoordinates right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(RgbPrimariesChromaticityCoordinates left, RgbPrimariesChromaticityCoordinates right)
	{
		return !left.Equals(right);
	}

	public override bool Equals(object? obj)
	{
		if (obj is RgbPrimariesChromaticityCoordinates other)
		{
			return Equals(other);
		}
		return false;
	}

	public bool Equals(RgbPrimariesChromaticityCoordinates other)
	{
		if (R.Equals(other.R) && G.Equals(other.G))
		{
			return B.Equals(other.B);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(R, G, B);
	}
}
