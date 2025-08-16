using System;

namespace SixLabors.ImageSharp.Formats.Webp.Lossless;

internal static class NearLosslessEnc
{
	private const int MinDimForNearLossless = 64;

	public static void ApplyNearLossless(int xSize, int ySize, int quality, Span<uint> argbSrc, Span<uint> argbDst, int stride)
	{
		uint[] array = new uint[xSize * 3];
		int num = LosslessUtils.NearLosslessBits(quality);
		if ((xSize < 64 && ySize < 64) || ySize < 3)
		{
			for (int i = 0; i < ySize; i++)
			{
				argbSrc.Slice(i * stride, xSize).CopyTo(argbDst.Slice(i * xSize, xSize));
			}
			return;
		}
		NearLossless(xSize, ySize, argbSrc, stride, num, array, argbDst);
		for (int num2 = num - 1; num2 != 0; num2--)
		{
			NearLossless(xSize, ySize, argbDst, xSize, num2, array, argbDst);
		}
	}

	private static void NearLossless(int xSize, int ySize, Span<uint> argbSrc, int stride, int limitBits, Span<uint> copyBuffer, Span<uint> argbDst)
	{
		int limit = 1 << limitBits;
		Span<uint> span = copyBuffer;
		Span<uint> span2 = copyBuffer.Slice(xSize, xSize);
		Span<uint> span3 = copyBuffer.Slice(xSize * 2, xSize);
		argbSrc.Slice(0, xSize).CopyTo(span2);
		argbSrc.Slice(xSize, xSize).CopyTo(span3);
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < ySize; i++)
		{
			if (i == 0 || i == ySize - 1)
			{
				argbSrc.Slice(num, xSize).CopyTo(argbDst.Slice(num2, xSize));
			}
			else
			{
				argbSrc.Slice(num + stride, xSize).CopyTo(span3);
				argbDst[num2] = argbSrc[num];
				argbDst[num2 + xSize - 1] = argbSrc[num + xSize - 1];
				for (int j = 1; j < xSize - 1; j++)
				{
					if (IsSmooth(span, span2, span3, j, limit))
					{
						argbDst[num2 + j] = span2[j];
					}
					else
					{
						argbDst[num2 + j] = ClosestDiscretizedArgb(span2[j], limitBits);
					}
				}
			}
			Span<uint> span4 = span;
			span = span2;
			span2 = span3;
			span3 = span4;
			num += stride;
			num2 += xSize;
		}
	}

	private static uint ClosestDiscretizedArgb(uint a, int bits)
	{
		return (FindClosestDiscretized(a >> 24, bits) << 24) | (FindClosestDiscretized((a >> 16) & 0xFF, bits) << 16) | (FindClosestDiscretized((a >> 8) & 0xFF, bits) << 8) | FindClosestDiscretized(a & 0xFF, bits);
	}

	private static uint FindClosestDiscretized(uint a, int bits)
	{
		uint num = (uint)((1 << bits) - 1);
		uint num2 = a + (num >> 1) + ((a >> bits) & 1);
		if (num2 > 255)
		{
			return 255u;
		}
		return num2 & ~num;
	}

	private static bool IsSmooth(Span<uint> prevRow, Span<uint> currRow, Span<uint> nextRow, int ix, int limit)
	{
		if (IsNear(currRow[ix], currRow[ix - 1], limit) && IsNear(currRow[ix], currRow[ix + 1], limit) && IsNear(currRow[ix], prevRow[ix], limit))
		{
			return IsNear(currRow[ix], nextRow[ix], limit);
		}
		return false;
	}

	private static bool IsNear(uint a, uint b, int limit)
	{
		for (int i = 0; i < 4; i++)
		{
			int num = (int)(((a >> i * 8) & 0xFF) - ((b >> i * 8) & 0xFF));
			if (num >= limit || num <= -limit)
			{
				return false;
			}
		}
		return true;
	}
}
