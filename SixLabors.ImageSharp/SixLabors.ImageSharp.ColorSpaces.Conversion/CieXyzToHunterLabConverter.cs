using System;
using System.Runtime.CompilerServices;

namespace SixLabors.ImageSharp.ColorSpaces.Conversion;

internal sealed class CieXyzToHunterLabConverter : CieXyzAndHunterLabConverterBase
{
	public CieXyz HunterLabWhitePoint { get; }

	public CieXyzToHunterLabConverter()
		: this(HunterLab.DefaultWhitePoint)
	{
	}

	public CieXyzToHunterLabConverter(CieXyz labWhitePoint)
	{
		HunterLabWhitePoint = labWhitePoint;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public HunterLab Convert(in CieXyz input)
	{
		float x = input.X;
		float y = input.Y;
		float z = input.Z;
		float x2 = HunterLabWhitePoint.X;
		float y2 = HunterLabWhitePoint.Y;
		float z2 = HunterLabWhitePoint.Z;
		float num = CieXyzAndHunterLabConverterBase.ComputeKa(HunterLabWhitePoint);
		float num2 = CieXyzAndHunterLabConverterBase.ComputeKb(HunterLabWhitePoint);
		float num3 = y / y2;
		float num4 = MathF.Sqrt(num3);
		float l = 100f * num4;
		float num5 = num * ((x / x2 - num3) / num4);
		float num6 = num2 * ((num3 - z / z2) / num4);
		if (float.IsNaN(num5))
		{
			num5 = 0f;
		}
		if (float.IsNaN(num6))
		{
			num6 = 0f;
		}
		return new HunterLab(l, num5, num6, HunterLabWhitePoint);
	}
}
