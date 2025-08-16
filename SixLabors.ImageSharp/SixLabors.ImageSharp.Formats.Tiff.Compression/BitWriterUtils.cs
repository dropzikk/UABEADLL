using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SixLabors.ImageSharp.Formats.Tiff.Compression;

internal static class BitWriterUtils
{
	public static void WriteBits(Span<byte> buffer, nint pos, nint count, byte value)
	{
		nint num = Numerics.Modulo8(pos);
		nint num2 = pos / 8;
		nint num3 = num2 + num;
		nint num4 = num3 + count;
		if (value == 1)
		{
			for (nint num5 = num3; num5 < num4; num5++)
			{
				WriteBit(buffer, num2, num);
				num++;
				if (num >= 8)
				{
					num = 0;
					num2++;
				}
			}
			return;
		}
		for (nint num6 = num3; num6 < num4; num6++)
		{
			WriteZeroBit(buffer, num2, num);
			num++;
			if (num >= 8)
			{
				num = 0;
				num2++;
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteBit(Span<byte> buffer, nint bufferPos, nint bitPos)
	{
		Unsafe.Add(ref MemoryMarshal.GetReference(buffer), bufferPos) |= (byte)(1 << (int)(7 - bitPos));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteZeroBit(Span<byte> buffer, nint bufferPos, nint bitPos)
	{
		ref byte reference = ref Unsafe.Add(ref MemoryMarshal.GetReference(buffer), bufferPos);
		reference = (byte)(reference & ~(1 << (int)(7 - bitPos)));
	}
}
