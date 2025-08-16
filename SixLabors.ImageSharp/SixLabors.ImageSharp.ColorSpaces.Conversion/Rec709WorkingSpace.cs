using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.ColorSpaces.Companding;

namespace SixLabors.ImageSharp.ColorSpaces.Conversion;

public sealed class Rec709WorkingSpace : RgbWorkingSpace
{
	public Rec709WorkingSpace(CieXyz referenceWhite, RgbPrimariesChromaticityCoordinates chromaticityCoordinates)
		: base(referenceWhite, chromaticityCoordinates)
	{
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override float Compress(float channel)
	{
		return Rec709Companding.Compress(channel);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override float Expand(float channel)
	{
		return Rec709Companding.Expand(channel);
	}
}
