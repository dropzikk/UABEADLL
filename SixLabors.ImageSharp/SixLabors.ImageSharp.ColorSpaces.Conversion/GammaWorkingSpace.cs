using System;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.ColorSpaces.Companding;

namespace SixLabors.ImageSharp.ColorSpaces.Conversion;

public sealed class GammaWorkingSpace : RgbWorkingSpace
{
	public float Gamma { get; }

	public GammaWorkingSpace(float gamma, CieXyz referenceWhite, RgbPrimariesChromaticityCoordinates chromaticityCoordinates)
		: base(referenceWhite, chromaticityCoordinates)
	{
		Gamma = gamma;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override float Compress(float channel)
	{
		return GammaCompanding.Compress(channel, Gamma);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override float Expand(float channel)
	{
		return GammaCompanding.Expand(channel, Gamma);
	}

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
		if (obj is GammaWorkingSpace gammaWorkingSpace)
		{
			if (Gamma.Equals(gammaWorkingSpace.Gamma) && base.WhitePoint.Equals(gammaWorkingSpace.WhitePoint))
			{
				return base.ChromaticityCoordinates.Equals(gammaWorkingSpace.ChromaticityCoordinates);
			}
			return false;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(base.WhitePoint, base.ChromaticityCoordinates, Gamma);
	}
}
