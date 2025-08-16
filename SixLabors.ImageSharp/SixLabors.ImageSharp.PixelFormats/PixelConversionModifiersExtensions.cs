using System.Runtime.CompilerServices;

namespace SixLabors.ImageSharp.PixelFormats;

internal static class PixelConversionModifiersExtensions
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsDefined(this PixelConversionModifiers modifiers, PixelConversionModifiers expected)
	{
		return (modifiers & expected) == expected;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static PixelConversionModifiers Remove(this PixelConversionModifiers modifiers, PixelConversionModifiers removeThis)
	{
		return modifiers & ~removeThis;
	}

	public static PixelConversionModifiers ApplyCompanding(this PixelConversionModifiers originalModifiers, bool compand)
	{
		if (!compand)
		{
			return originalModifiers;
		}
		return originalModifiers | PixelConversionModifiers.Scale | PixelConversionModifiers.SRgbCompand;
	}
}
