using System.Runtime.CompilerServices;

namespace SixLabors.ImageSharp.ColorSpaces.Conversion;

internal static class RgbToLinearRgbConverter
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static LinearRgb Convert(in Rgb input)
	{
		return new LinearRgb(input.WorkingSpace.Expand(input.R), input.WorkingSpace.Expand(input.G), input.WorkingSpace.Expand(input.B), input.WorkingSpace);
	}
}
