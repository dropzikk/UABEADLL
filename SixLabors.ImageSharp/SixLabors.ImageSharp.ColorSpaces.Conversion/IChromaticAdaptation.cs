using System;

namespace SixLabors.ImageSharp.ColorSpaces.Conversion;

public interface IChromaticAdaptation
{
	CieXyz Transform(in CieXyz source, in CieXyz sourceWhitePoint, in CieXyz destinationWhitePoint);

	void Transform(ReadOnlySpan<CieXyz> source, Span<CieXyz> destination, CieXyz sourceWhitePoint, in CieXyz destinationWhitePoint);
}
