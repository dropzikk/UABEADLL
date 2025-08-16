using System;

namespace SixLabors.ImageSharp.Formats.Tiff.Compression.Compressors;

internal static class PackBitsWriter
{
	public static int PackBits(ReadOnlySpan<byte> rowSpan, Span<byte> compressedRowSpan)
	{
		int num = 127;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		while (num2 < rowSpan.Length)
		{
			if (IsReplicateRun(rowSpan, num2))
			{
				if (num4 > 0)
				{
					WriteLiteralRun(rowSpan, num2, num4, compressedRowSpan, num3);
					num3 += num4 + 1;
				}
				int num5 = FindRunLength(rowSpan, num2, num);
				WriteRun(rowSpan, num2, num5, compressedRowSpan, num3);
				num3 += 2;
				num4 = 0;
				num2 += num5;
			}
			else
			{
				num4++;
				num2++;
				if (num4 >= num)
				{
					WriteLiteralRun(rowSpan, num2, num4, compressedRowSpan, num3);
					num3 += num4 + 1;
					num4 = 0;
				}
			}
		}
		if (num4 > 0)
		{
			WriteLiteralRun(rowSpan, num2, num4, compressedRowSpan, num3);
			num3 += num4 + 1;
		}
		return num3;
	}

	private static void WriteLiteralRun(ReadOnlySpan<byte> rowSpan, int end, int literalRunLength, Span<byte> compressedRowSpan, int compressedRowPos)
	{
		int start = end - literalRunLength;
		sbyte b = (sbyte)(literalRunLength - 1);
		compressedRowSpan[compressedRowPos] = (byte)b;
		ReadOnlySpan<byte> readOnlySpan = rowSpan.Slice(start, literalRunLength);
		int num = compressedRowPos + 1;
		readOnlySpan.CopyTo(compressedRowSpan.Slice(num, compressedRowSpan.Length - num));
	}

	private static void WriteRun(ReadOnlySpan<byte> rowSpan, int start, int runLength, Span<byte> compressedRowSpan, int compressedRowPos)
	{
		sbyte b = (sbyte)(-runLength + 1);
		compressedRowSpan[compressedRowPos] = (byte)b;
		compressedRowSpan[compressedRowPos + 1] = rowSpan[start];
	}

	private static bool IsReplicateRun(ReadOnlySpan<byte> rowSpan, int startPos)
	{
		byte b = rowSpan[startPos];
		int num = 0;
		for (int i = startPos + 1; i < rowSpan.Length && rowSpan[i] == b; i++)
		{
			num++;
			if (num >= 2)
			{
				return true;
			}
		}
		return false;
	}

	private static int FindRunLength(ReadOnlySpan<byte> rowSpan, int startPos, int maxRunLength)
	{
		byte b = rowSpan[startPos];
		int num = 1;
		for (int i = startPos + 1; i < rowSpan.Length && rowSpan[i] == b; i++)
		{
			num++;
			if (num == maxRunLength)
			{
				break;
			}
		}
		return num;
	}
}
