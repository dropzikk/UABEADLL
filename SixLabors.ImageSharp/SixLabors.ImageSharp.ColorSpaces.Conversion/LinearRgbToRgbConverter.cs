using System.Runtime.CompilerServices;

namespace SixLabors.ImageSharp.ColorSpaces.Conversion;

internal static class LinearRgbToRgbConverter
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Rgb Convert(in LinearRgb input)
	{
		return new Rgb(input.WorkingSpace.Compress(input.R), input.WorkingSpace.Compress(input.G), input.WorkingSpace.Compress(input.B), input.WorkingSpace);
	}
}
