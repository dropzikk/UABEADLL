using System;
using System.Runtime.CompilerServices;

namespace SixLabors.ImageSharp.PixelFormats;

internal static class HalfTypeHelper
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static ushort Pack(float value)
	{
		return BitConverter.HalfToUInt16Bits((Half)value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static float Unpack(ushort value)
	{
		return (float)BitConverter.UInt16BitsToHalf(value);
	}
}
