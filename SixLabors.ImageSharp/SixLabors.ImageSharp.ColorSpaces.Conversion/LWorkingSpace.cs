using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.ColorSpaces.Companding;

namespace SixLabors.ImageSharp.ColorSpaces.Conversion;

public sealed class LWorkingSpace : RgbWorkingSpace
{
	public LWorkingSpace(CieXyz referenceWhite, RgbPrimariesChromaticityCoordinates chromaticityCoordinates)
		: base(referenceWhite, chromaticityCoordinates)
	{
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override float Compress(float channel)
	{
		return LCompanding.Compress(channel);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override float Expand(float channel)
	{
		return LCompanding.Expand(channel);
	}
}
