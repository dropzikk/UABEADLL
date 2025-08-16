using System;
using System.IO;

namespace SixLabors.ImageSharp.Formats.Tiff.Compression.Decompressors;

internal sealed class TiffLzwDecoder
{
	private readonly Stream stream;

	private const int ClearCode = 256;

	private const int EoiCode = 257;

	private const int MinBits = 9;

	private const int MaxBits = 12;

	private const int TableSize = 4096;

	private readonly LzwString[] table;

	private int tableLength;

	private int bitsPerCode;

	private int oldCode = 256;

	private int maxCode;

	private int bitMask;

	private int maxString;

	private bool eofReached;

	private int nextData;

	private int nextBits;

	public TiffLzwDecoder(Stream stream)
	{
		Guard.NotNull(stream, "stream");
		this.stream = stream;
		table = new LzwString[4096];
		for (int i = 0; i < 256; i++)
		{
			table[i] = new LzwString((byte)i);
		}
		Init();
	}

	private void Init()
	{
		tableLength = 258;
		bitsPerCode = 9;
		bitMask = BitmaskFor(bitsPerCode);
		maxCode = MaxCode();
		maxString = 1;
	}

	public void DecodePixels(Span<byte> pixels)
	{
		int num = 0;
		int nextCode;
		while ((nextCode = GetNextCode()) != 257)
		{
			if (nextCode == 256)
			{
				Init();
				nextCode = GetNextCode();
				if (nextCode == 257)
				{
					break;
				}
				if (table[nextCode] == null)
				{
					TiffThrowHelper.ThrowImageFormatException($"Corrupted TIFF LZW: code {nextCode} (table size: {tableLength})");
				}
				num += table[nextCode].WriteTo(pixels, num);
			}
			else
			{
				if (table[oldCode] == null)
				{
					TiffThrowHelper.ThrowImageFormatException($"Corrupted TIFF LZW: code {oldCode} (table size: {tableLength})");
				}
				if (IsInTable(nextCode))
				{
					num += table[nextCode].WriteTo(pixels, num);
					AddStringToTable(table[oldCode].Concatenate(table[nextCode].FirstChar));
				}
				else
				{
					LzwString lzwString = table[oldCode].Concatenate(table[oldCode].FirstChar);
					num += lzwString.WriteTo(pixels, num);
					AddStringToTable(lzwString);
				}
			}
			oldCode = nextCode;
			if (num >= pixels.Length)
			{
				break;
			}
		}
	}

	private void AddStringToTable(LzwString lzwString)
	{
		if (tableLength > table.Length)
		{
			TiffThrowHelper.ThrowImageFormatException($"TIFF LZW with more than {12} bits per code encountered (table overflow)");
		}
		table[tableLength++] = lzwString;
		if (tableLength > maxCode)
		{
			bitsPerCode++;
			if (bitsPerCode > 12)
			{
				bitsPerCode = 12;
			}
			bitMask = BitmaskFor(bitsPerCode);
			maxCode = MaxCode();
		}
		if (lzwString.Length > maxString)
		{
			maxString = lzwString.Length;
		}
	}

	private int GetNextCode()
	{
		if (eofReached)
		{
			return 257;
		}
		int num = stream.ReadByte();
		if (num < 0)
		{
			eofReached = true;
			return 257;
		}
		nextData = (nextData << 8) | num;
		nextBits += 8;
		if (nextBits < bitsPerCode)
		{
			num = stream.ReadByte();
			if (num < 0)
			{
				eofReached = true;
				return 257;
			}
			nextData = (nextData << 8) | num;
			nextBits += 8;
		}
		int result = (nextData >> nextBits - bitsPerCode) & bitMask;
		nextBits -= bitsPerCode;
		return result;
	}

	private bool IsInTable(int code)
	{
		return code < tableLength;
	}

	private int MaxCode()
	{
		return bitMask - 1;
	}

	private static int BitmaskFor(int bits)
	{
		return (1 << bits) - 1;
	}
}
