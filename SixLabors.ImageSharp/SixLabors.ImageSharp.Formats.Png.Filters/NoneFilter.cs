using System;
using System.Runtime.CompilerServices;

namespace SixLabors.ImageSharp.Formats.Png.Filters;

internal static class NoneFilter
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Encode(ReadOnlySpan<byte> scanline, Span<byte> result)
	{
		result[0] = 0;
		result = result.Slice(1, result.Length - 1);
		scanline.Slice(0, Math.Min(scanline.Length, result.Length)).CopyTo(result);
	}
}
