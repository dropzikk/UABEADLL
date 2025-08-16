using System;
using System.Runtime.CompilerServices;

namespace SixLabors.ImageSharp.ColorSpaces.Conversion;

internal sealed class CieXyzToCieLuvConverter
{
	public CieXyz LuvWhitePoint { get; }

	public CieXyzToCieLuvConverter()
		: this(CieLuv.DefaultWhitePoint)
	{
	}

	public CieXyzToCieLuvConverter(CieXyz luvWhitePoint)
	{
		LuvWhitePoint = luvWhitePoint;
	}

	public CieLuv Convert(in CieXyz input)
	{
		float num = input.Y / LuvWhitePoint.Y;
		float num2 = ComputeUp(in input);
		float num3 = ComputeVp(in input);
		float num4 = ComputeUp(LuvWhitePoint);
		float num5 = ComputeVp(LuvWhitePoint);
		float num6 = ((num > 0.008856452f) ? (116f * MathF.Pow(num, 0.3333333f) - 16f) : (903.2963f * num));
		if (float.IsNaN(num6) || num6 < 0f)
		{
			num6 = 0f;
		}
		float num7 = 13f * num6 * (num2 - num4);
		float num8 = 13f * num6 * (num3 - num5);
		if (float.IsNaN(num7))
		{
			num7 = 0f;
		}
		if (float.IsNaN(num8))
		{
			num8 = 0f;
		}
		return new CieLuv(num6, num7, num8, LuvWhitePoint);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static float ComputeUp(in CieXyz input)
	{
		return 4f * input.X / (input.X + 15f * input.Y + 3f * input.Z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static float ComputeVp(in CieXyz input)
	{
		return 9f * input.Y / (input.X + 15f * input.Y + 3f * input.Z);
	}
}
