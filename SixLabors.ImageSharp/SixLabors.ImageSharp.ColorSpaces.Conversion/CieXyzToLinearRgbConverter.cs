using System.Numerics;
using System.Runtime.CompilerServices;

namespace SixLabors.ImageSharp.ColorSpaces.Conversion;

internal sealed class CieXyzToLinearRgbConverter : LinearRgbAndCieXyzConverterBase
{
	private readonly Matrix4x4 conversionMatrix;

	public RgbWorkingSpace TargetWorkingSpace { get; }

	public CieXyzToLinearRgbConverter()
		: this(Rgb.DefaultWorkingSpace)
	{
	}

	public CieXyzToLinearRgbConverter(RgbWorkingSpace workingSpace)
	{
		TargetWorkingSpace = workingSpace;
		Matrix4x4.Invert(LinearRgbAndCieXyzConverterBase.GetRgbToCieXyzMatrix(workingSpace), out var result);
		conversionMatrix = result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public LinearRgb Convert(in CieXyz input)
	{
		return new LinearRgb(Vector3.Transform(input.ToVector3(), conversionMatrix), TargetWorkingSpace);
	}
}
