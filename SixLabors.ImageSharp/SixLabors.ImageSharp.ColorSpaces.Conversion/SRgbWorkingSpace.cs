using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.ColorSpaces.Companding;

namespace SixLabors.ImageSharp.ColorSpaces.Conversion;

public sealed class SRgbWorkingSpace : RgbWorkingSpace
{
	public SRgbWorkingSpace(CieXyz referenceWhite, RgbPrimariesChromaticityCoordinates chromaticityCoordinates)
		: base(referenceWhite, chromaticityCoordinates)
	{
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override float Compress(float channel)
	{
		return SRgbCompanding.Compress(channel);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override float Expand(float channel)
	{
		return SRgbCompanding.Expand(channel);
	}
}
