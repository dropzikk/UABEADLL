using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SixLabors.ImageSharp.Formats.Png;

internal static class PngEncoderHelpers
{
	public static void ScaleDownFrom8BitArray(ReadOnlySpan<byte> source, Span<byte> result, int bits, float scale = 1f)
	{
		ref byte reference = ref MemoryMarshal.GetReference(source);
		ref byte reference2 = ref MemoryMarshal.GetReference(result);
		int num = 8 - bits;
		byte b = (byte)(255 >> num);
		byte b2 = (byte)num;
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < source.Length; i++)
		{
			int num4 = (int)MathF.Round((float)(int)Unsafe.Add(ref reference, (uint)i) / scale) & b;
			num2 |= num4 << num;
			if (num == 0)
			{
				num = b2;
				Unsafe.Add(ref reference2, (uint)num3) = (byte)num2;
				num3++;
				num2 = 0;
			}
			else
			{
				num -= bits;
			}
		}
		if (num != b2)
		{
			Unsafe.Add(ref reference2, (uint)num3) = (byte)num2;
		}
	}
}
