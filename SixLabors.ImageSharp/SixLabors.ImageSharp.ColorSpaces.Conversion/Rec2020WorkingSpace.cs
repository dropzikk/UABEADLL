using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.ColorSpaces.Companding;

namespace SixLabors.ImageSharp.ColorSpaces.Conversion;

public sealed class Rec2020WorkingSpace : RgbWorkingSpace
{
	public Rec2020WorkingSpace(CieXyz referenceWhite, RgbPrimariesChromaticityCoordinates chromaticityCoordinates)
		: base(referenceWhite, chromaticityCoordinates)
	{
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override float Compress(float channel)
	{
		return Rec2020Companding.Compress(channel);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override float Expand(float channel)
	{
		return Rec2020Companding.Expand(channel);
	}
}
