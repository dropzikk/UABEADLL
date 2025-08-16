using System;
using System.Buffers;
using System.IO;
using SixLabors.ImageSharp.Formats.Tiff.Constants;
using SixLabors.ImageSharp.Memory;

namespace SixLabors.ImageSharp.Formats.Tiff.Compression.Compressors;

internal sealed class T6BitCompressor : TiffCcittCompressor
{
	private static readonly (uint Length, uint Code)[] VerticalCodes = new(uint, uint)[7]
	{
		(7u, 3u),
		(6u, 3u),
		(3u, 3u),
		(1u, 1u),
		(3u, 2u),
		(6u, 2u),
		(7u, 2u)
	};

	private IMemoryOwner<byte> referenceLineBuffer;

	public override TiffCompression Method => TiffCompression.CcittGroup4Fax;

	public T6BitCompressor(Stream output, MemoryAllocator allocator, int width, int bitsPerPixel)
		: base(output, allocator, width, bitsPerPixel)
	{
	}

	protected override void CompressStrip(Span<byte> pixelsAsGray, int height, Span<byte> compressedData)
	{
		Span<byte> span = referenceLineBuffer.GetSpan();
		span.Fill(byte.MaxValue);
		for (int i = 0; i < height; i++)
		{
			Span<byte> row = pixelsAsGray.Slice(i * base.Width, base.Width);
			uint num = 0u;
			uint num2 = ((row[0] != 0) ? FindRunEnd(row, 0u, null) : 0u);
			uint num3 = ((span[0] != 0) ? FindRunEnd(span, 0u, null) : 0u);
			while (true)
			{
				uint num4 = FindRunEnd(span, num3, null);
				if (num4 < num2)
				{
					WriteCode(4u, 1u, compressedData);
					num = num4;
				}
				else
				{
					int num5 = int.MaxValue;
					if (num3 >= num2 && num3 - num2 <= 3)
					{
						num5 = (int)(num3 - num2);
					}
					else if (num3 < num2 && num2 - num3 <= 3)
					{
						num5 = (int)(0 - (num2 - num3));
					}
					if (num5 >= -3 && num5 <= 3)
					{
						var (codeLength, code) = VerticalCodes[num5 + 3];
						WriteCode(codeLength, code, compressedData);
						num = num2;
					}
					else
					{
						WriteCode(3u, 1u, compressedData);
						uint num6 = FindRunEnd(row, num2, null);
						if (num + num2 == 0 || row[(int)num] != 0)
						{
							WriteRun(num2 - num, isWhiteRun: true, compressedData);
							WriteRun(num6 - num2, isWhiteRun: false, compressedData);
						}
						else
						{
							WriteRun(num2 - num, isWhiteRun: false, compressedData);
							WriteRun(num6 - num2, isWhiteRun: true, compressedData);
						}
						num = num6;
					}
				}
				if (num >= row.Length)
				{
					break;
				}
				byte b = row[(int)num];
				num2 = FindRunEnd(row, num, b);
				num3 = FindRunEnd(span, num, (byte)(~b));
				num3 = FindRunEnd(span, num3, b);
			}
			row.CopyTo(span);
		}
		WriteCode(12u, 1u, compressedData);
		WriteCode(12u, 1u, compressedData);
	}

	protected override void Dispose(bool disposing)
	{
		referenceLineBuffer?.Dispose();
		base.Dispose(disposing);
	}

	private static uint FindRunEnd(Span<byte> row, uint startIndex, byte? color = null)
	{
		if (startIndex >= row.Length)
		{
			return (uint)row.Length;
		}
		byte b = color ?? row[(int)startIndex];
		for (int i = (int)startIndex; i < row.Length; i++)
		{
			if (row[i] != b)
			{
				return (uint)i;
			}
		}
		return (uint)row.Length;
	}

	public override void Initialize(int rowsPerStrip)
	{
		base.Initialize(rowsPerStrip);
		referenceLineBuffer = base.Allocator.Allocate<byte>(base.Width);
	}

	private void WriteRun(uint runLength, bool isWhiteRun, Span<byte> compressedData)
	{
		uint codeLength;
		uint makeupCode;
		while (runLength > 63)
		{
			uint bestFittingMakeupRunLength = TiffCcittCompressor.GetBestFittingMakeupRunLength(runLength);
			makeupCode = TiffCcittCompressor.GetMakeupCode(bestFittingMakeupRunLength, out codeLength, isWhiteRun);
			WriteCode(codeLength, makeupCode, compressedData);
			runLength -= bestFittingMakeupRunLength;
		}
		makeupCode = TiffCcittCompressor.GetTermCode(runLength, out codeLength, isWhiteRun);
		WriteCode(codeLength, makeupCode, compressedData);
	}
}
