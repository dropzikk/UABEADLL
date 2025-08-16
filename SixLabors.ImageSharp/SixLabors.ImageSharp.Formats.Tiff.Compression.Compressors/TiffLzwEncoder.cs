using System;
using System.Buffers;
using System.IO;
using SixLabors.ImageSharp.Memory;

namespace SixLabors.ImageSharp.Formats.Tiff.Compression.Compressors;

internal sealed class TiffLzwEncoder : IDisposable
{
	private static readonly int ClearCode = 256;

	private static readonly int EoiCode = 257;

	private static readonly int MinBits = 9;

	private static readonly int MaxBits = 12;

	private static readonly int TableSize = 1 << MaxBits;

	private readonly IMemoryOwner<int> children;

	private readonly IMemoryOwner<int> siblings;

	private readonly IMemoryOwner<int> suffixes;

	private int parent;

	private int bitsPerCode;

	private int nextValidCode;

	private int maxCode;

	private int bits;

	private int bitPos;

	private int bufferPosition;

	public TiffLzwEncoder(MemoryAllocator memoryAllocator)
	{
		children = memoryAllocator.Allocate<int>(TableSize);
		siblings = memoryAllocator.Allocate<int>(TableSize);
		suffixes = memoryAllocator.Allocate<int>(TableSize);
	}

	public void Encode(Span<byte> data, Stream stream)
	{
		Reset();
		Span<int> span = children.GetSpan();
		Span<int> span2 = suffixes.GetSpan();
		Span<int> span3 = siblings.GetSpan();
		if (data.Length == 0)
		{
			return;
		}
		if (parent == -1)
		{
			WriteCode(stream, ClearCode);
			parent = ReadNextByte(data);
		}
		while (bufferPosition < data.Length)
		{
			int num = ReadNextByte(data);
			int num2 = span[parent];
			if (num2 > 0)
			{
				if (span2[num2] == num)
				{
					parent = num2;
					continue;
				}
				int index = num2;
				while (true)
				{
					if (span3[index] > 0)
					{
						index = span3[index];
						if (span2[index] == num)
						{
							parent = index;
							break;
						}
						continue;
					}
					span3[index] = (short)nextValidCode;
					span2[nextValidCode] = (short)num;
					WriteCode(stream, parent);
					parent = num;
					nextValidCode++;
					IncreaseCodeSizeOrResetIfNeeded(stream);
					break;
				}
			}
			else
			{
				span[parent] = (short)nextValidCode;
				span2[nextValidCode] = (short)num;
				WriteCode(stream, parent);
				parent = num;
				nextValidCode++;
				IncreaseCodeSizeOrResetIfNeeded(stream);
			}
		}
		WriteCode(stream, parent);
		WriteCode(stream, EoiCode);
		if (bitPos > 0)
		{
			WriteCode(stream, 0);
		}
	}

	public void Dispose()
	{
		children.Dispose();
		siblings.Dispose();
		suffixes.Dispose();
	}

	private void Reset()
	{
		children.Clear();
		siblings.Clear();
		suffixes.Clear();
		parent = -1;
		bitsPerCode = MinBits;
		nextValidCode = EoiCode + 1;
		maxCode = (1 << bitsPerCode) - 1;
		bits = 0;
		bitPos = 0;
		bufferPosition = 0;
	}

	private byte ReadNextByte(Span<byte> data)
	{
		return data[bufferPosition++];
	}

	private void IncreaseCodeSizeOrResetIfNeeded(Stream stream)
	{
		if (nextValidCode > maxCode)
		{
			if (bitsPerCode == MaxBits)
			{
				WriteCode(stream, ClearCode);
				ResetTables();
			}
			else
			{
				bitsPerCode++;
				maxCode = MaxValue(bitsPerCode);
			}
		}
	}

	private void WriteCode(Stream stream, int code)
	{
		bits = (bits << bitsPerCode) | (code & maxCode);
		bitPos += bitsPerCode;
		while (bitPos >= 8)
		{
			int num = (bits >> bitPos - 8) & 0xFF;
			stream.WriteByte((byte)num);
			bitPos -= 8;
		}
		bits &= BitmaskFor(bitPos);
	}

	private void ResetTables()
	{
		children.GetSpan().Clear();
		siblings.GetSpan().Clear();
		bitsPerCode = MinBits;
		maxCode = MaxValue(bitsPerCode);
		nextValidCode = EoiCode + 1;
	}

	private static int MaxValue(int codeLen)
	{
		return (1 << codeLen) - 1;
	}

	private static int BitmaskFor(int bits)
	{
		return MaxValue(bits);
	}
}
