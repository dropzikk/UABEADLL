using System.Numerics;
using System.Runtime.CompilerServices;

namespace SixLabors.ImageSharp.ColorSpaces.Conversion;

internal sealed class LinearRgbToCieXyzConverter : LinearRgbAndCieXyzConverterBase
{
	private readonly Matrix4x4 conversionMatrix;

	public RgbWorkingSpace SourceWorkingSpace { get; }

	public LinearRgbToCieXyzConverter()
		: this(Rgb.DefaultWorkingSpace)
	{
	}

	public LinearRgbToCieXyzConverter(RgbWorkingSpace workingSpace)
	{
		SourceWorkingSpace = workingSpace;
		conversionMatrix = LinearRgbAndCieXyzConverterBase.GetRgbToCieXyzMatrix(workingSpace);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CieXyz Convert(in LinearRgb input)
	{
		return new CieXyz(Vector3.Transform(input.ToVector3(), conversionMatrix));
	}
}
