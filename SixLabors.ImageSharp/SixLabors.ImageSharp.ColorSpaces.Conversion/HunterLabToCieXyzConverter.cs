using System;
using System.Runtime.CompilerServices;

namespace SixLabors.ImageSharp.ColorSpaces.Conversion;

internal sealed class HunterLabToCieXyzConverter : CieXyzAndHunterLabConverterBase
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CieXyz Convert(in HunterLab input)
	{
		float l = input.L;
		float a = input.A;
		float b = input.B;
		float x = input.WhitePoint.X;
		float y = input.WhitePoint.Y;
		float z = input.WhitePoint.Z;
		float num = CieXyzAndHunterLabConverterBase.ComputeKa(input.WhitePoint);
		float num2 = CieXyzAndHunterLabConverterBase.ComputeKb(input.WhitePoint);
		float num3 = Numerics.Pow2(l / 100f);
		float num4 = MathF.Sqrt(num3);
		float y2 = num3 * y;
		float x2 = (a / num * num4 + num3) * x;
		float z2 = (b / num2 * num4 - num3) * (0f - z);
		return new CieXyz(x2, y2, z2);
	}
}
