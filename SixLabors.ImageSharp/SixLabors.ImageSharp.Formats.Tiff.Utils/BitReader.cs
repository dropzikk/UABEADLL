using System;

namespace SixLabors.ImageSharp.Formats.Tiff.Utils;

internal ref struct BitReader
{
	private readonly ReadOnlySpan<byte> array;

	private int offset;

	private int bitOffset;

	public BitReader(ReadOnlySpan<byte> array)
	{
		this.array = array;
		offset = 0;
		bitOffset = 0;
	}

	public int ReadBits(uint bits)
	{
		int num = 0;
		for (uint num2 = 0u; num2 < bits; num2++)
		{
			int num3 = (array[offset] >> 7 - bitOffset) & 1;
			num = (num << 1) | num3;
			bitOffset++;
			if (bitOffset == 8)
			{
				bitOffset = 0;
				offset++;
			}
		}
		return num;
	}

	public void NextRow()
	{
		if (bitOffset > 0)
		{
			bitOffset = 0;
			offset++;
		}
	}
}
