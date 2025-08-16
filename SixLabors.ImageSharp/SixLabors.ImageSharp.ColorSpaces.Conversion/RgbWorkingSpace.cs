using System;

namespace SixLabors.ImageSharp.ColorSpaces.Conversion;

public abstract class RgbWorkingSpace
{
	public CieXyz WhitePoint { get; }

	public RgbPrimariesChromaticityCoordinates ChromaticityCoordinates { get; }

	protected RgbWorkingSpace(CieXyz referenceWhite, RgbPrimariesChromaticityCoordinates chromaticityCoordinates)
	{
		WhitePoint = referenceWhite;
		ChromaticityCoordinates = chromaticityCoordinates;
	}

	public abstract float Expand(float channel);

	public abstract float Compress(float channel);

	public override bool Equals(object? obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (this == obj)
		{
			return true;
		}
		if (obj is RgbWorkingSpace rgbWorkingSpace)
		{
			if (WhitePoint.Equals(rgbWorkingSpace.WhitePoint))
			{
				return ChromaticityCoordinates.Equals(rgbWorkingSpace.ChromaticityCoordinates);
			}
			return false;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(WhitePoint, ChromaticityCoordinates);
	}
}
