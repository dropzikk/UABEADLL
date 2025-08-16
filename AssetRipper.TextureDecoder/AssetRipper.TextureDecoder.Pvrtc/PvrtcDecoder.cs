using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AssetRipper.TextureDecoder.Pvrtc;

public static class PvrtcDecoder
{
	private struct AmtcBlock
	{
		public readonly uint PackedData0;

		public readonly uint PackedData1;

		public AmtcBlock(uint v0, uint v1)
		{
			PackedData0 = v0;
			PackedData1 = v1;
		}
	}

	private struct Colours5554
	{
		public unsafe fixed int Reps[8];
	}

	private const int PTIndex = 2;

	private const int BlockYSize = 4;

	private const int BlockXMax = 8;

	private const int BlockX2bpp = 8;

	private const int BlockX4bpp = 4;

	private static readonly int[] m_repVals0 = new int[4] { 0, 3, 5, 8 };

	private static readonly int[] m_repVals1 = new int[4] { 0, 4, 4, 8 };

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int DecompressPVRTC(ReadOnlySpan<byte> input, int xDim, int yDim, bool do2bitMode, out byte[] output)
	{
		output = new byte[xDim * yDim * 4];
		return DecompressPVRTC(input, xDim, yDim, do2bitMode, output);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public static int DecompressPVRTC(ReadOnlySpan<byte> input, int xDim, int yDim, bool do2bitMode, Span<byte> output)
	{
		int num = (do2bitMode ? 8 : 4);
		int num2 = Math.Max(2, xDim / num);
		int num3 = Math.Max(2, yDim / 4);
		uint num4 = uint.MaxValue;
		uint num5 = uint.MaxValue;
		uint num6 = uint.MaxValue;
		uint num7 = uint.MaxValue;
		uint num8 = 0u;
		Span<Colours5554> colorSpan = stackalloc Colours5554[4];
		Span<int> result = stackalloc int[4];
		Span<int> result2 = stackalloc int[4];
		Span<int> span = stackalloc int[128];
		Span<int> span2 = stackalloc int[128];
		ReadOnlySpan<AmtcBlock> readOnlySpan = MemoryMarshal.Cast<byte, AmtcBlock>(input);
		for (int i = 0; i < yDim; i++)
		{
			for (int j = 0; j < xDim; j++)
			{
				int value = j - num / 2;
				int value2 = i - 2;
				value = LimitCoord(value, xDim) / num;
				value2 = LimitCoord(value2, yDim) / 4;
				int xPos = LimitCoord(value + 1, num2);
				int yPos = LimitCoord(value2 + 1, num3);
				uint num9 = TwiddleUV((uint)num3, (uint)num2, (uint)value2, (uint)value);
				uint num10 = TwiddleUV((uint)num3, (uint)num2, (uint)value2, (uint)xPos);
				uint num11 = TwiddleUV((uint)num3, (uint)num2, (uint)yPos, (uint)value);
				uint num12 = TwiddleUV((uint)num3, (uint)num2, (uint)yPos, (uint)xPos);
				if (num9 != num4 || num10 != num5 || num11 != num6 || num12 != num7)
				{
					AmtcBlock block = readOnlySpan[(int)num9];
					Unpack5554Colour(block, colorSpan.GetIntSpanForColor(0));
					UnpackModulations(block, do2bitMode, span, span2, 0, 0);
					AmtcBlock block2 = readOnlySpan[(int)num10];
					Unpack5554Colour(block2, colorSpan.GetIntSpanForColor(1));
					UnpackModulations(block2, do2bitMode, span, span2, num, 0);
					AmtcBlock block3 = readOnlySpan[(int)num11];
					Unpack5554Colour(block3, colorSpan.GetIntSpanForColor(2));
					UnpackModulations(block3, do2bitMode, span, span2, 0, 4);
					AmtcBlock block4 = readOnlySpan[(int)num12];
					Unpack5554Colour(block4, colorSpan.GetIntSpanForColor(3));
					UnpackModulations(block4, do2bitMode, span, span2, num, 4);
					num4 = num9;
					num5 = num10;
					num6 = num11;
					num7 = num12;
					num8 = Math.Max(num8, Math.Max(Math.Max(num9, num10), Math.Max(num11, num12)));
				}
				InterpolateColours(colorSpan.GetIntSpanForColor(0), colorSpan.GetIntSpanForColor(1), colorSpan.GetIntSpanForColor(2), colorSpan.GetIntSpanForColor(3), 0, do2bitMode, j, i, result);
				InterpolateColours(colorSpan.GetIntSpanForColor(0), colorSpan.GetIntSpanForColor(1), colorSpan.GetIntSpanForColor(2), colorSpan.GetIntSpanForColor(3), 1, do2bitMode, j, i, result2);
				GetModulationValue(j, i, do2bitMode, span, span2, out var mod, out var doPT);
				int num13 = j + i * xDim << 2;
				output[num13] = (byte)(result[2] * 8 + mod * (result2[2] - result[2]) >> 3);
				output[num13 + 1] = (byte)(result[1] * 8 + mod * (result2[1] - result[1]) >> 3);
				output[num13 + 2] = (byte)(result[0] * 8 + mod * (result2[0] - result[0]) >> 3);
				output[num13 + 3] = (byte)((!doPT) ? ((byte)(result[3] * 8 + mod * (result2[3] - result[3]) >> 3)) : 0);
			}
		}
		return (int)(num8 + 1) * Unsafe.SizeOf<AmtcBlock>();
	}

	private static uint TwiddleUV(uint ySize, uint xSize, uint yPos, uint xPos)
	{
		uint num;
		uint num2;
		if (ySize < xSize)
		{
			num = ySize;
			num2 = xPos;
		}
		else
		{
			num = xSize;
			num2 = yPos;
		}
		uint num3 = 1u;
		uint num4 = 1u;
		uint num5 = 0u;
		int num6 = 0;
		while (num3 < num)
		{
			if ((yPos & num3) != 0)
			{
				num5 |= num4;
			}
			if ((xPos & num3) != 0)
			{
				num5 |= num4 << 1;
			}
			num3 <<= 1;
			num4 <<= 2;
			num6++;
		}
		num2 >>= num6;
		return num5 | (num2 << 2 * num6);
	}

	private static void GetModulationValue(int x, int y, bool do2bitMode, ReadOnlySpan<int> modulationVals, ReadOnlySpan<int> modulationModes, out int mod, out bool doPT)
	{
		y = (y & 3) | ((~y & 2) << 1);
		x = (do2bitMode ? ((x & 7) | ((~x & 4) << 1)) : ((x & 3) | ((~x & 2) << 1)));
		doPT = false;
		int num;
		if (modulationModes[y * 16 + x] == 0)
		{
			num = m_repVals0[modulationVals[y * 16 + x]];
		}
		else if (do2bitMode)
		{
			num = ((((x ^ y) & 1) == 0) ? m_repVals0[modulationVals[y * 16 + x]] : ((modulationModes[y * 16 + x] == 1) ? ((m_repVals0[modulationVals[(y - 1) * 16 + x]] + m_repVals0[modulationVals[(y + 1) * 16 + x]] + m_repVals0[modulationVals[y * 16 + x - 1]] + m_repVals0[modulationVals[y * 16 + x + 1]] + 2) / 4) : ((modulationModes[y * 16 + x] != 2) ? ((m_repVals0[modulationVals[(y - 1) * 16 + x]] + m_repVals0[modulationVals[(y + 1) * 16 + x]] + 1) / 2) : ((m_repVals0[modulationVals[y * 16 + x - 1]] + m_repVals0[modulationVals[y * 16 + x + 1]] + 1) / 2))));
		}
		else
		{
			num = m_repVals1[modulationVals[y * 16 + x]];
			doPT = modulationVals[y * 16 + x] == 2;
		}
		mod = num;
	}

	private static void InterpolateColours(ReadOnlySpan<int> colorP, ReadOnlySpan<int> colorQ, ReadOnlySpan<int> colorR, ReadOnlySpan<int> colorS, int ci, bool do2bitMode, int x, int y, Span<int> result)
	{
		int num = ((y & 3) | ((~y & 2) << 1)) - 2;
		int num2 = (do2bitMode ? (((x & 7) | ((~x & 4) << 1)) - 4) : (((x & 3) | ((~x & 2) << 1)) - 2));
		int num3 = (do2bitMode ? 8 : 4);
		for (int i = 0; i < 4; i++)
		{
			int num4 = colorP[ci * 4 + i] * num3 + num2 * (colorQ[ci * 4 + i] - colorP[ci * 4 + i]);
			int num5 = colorR[ci * 4 + i] * num3 + num2 * (colorS[ci * 4 + i] - colorR[ci * 4 + i]);
			result[i] = num4 * 4 + num * (num5 - num4);
		}
		if (do2bitMode)
		{
			for (int j = 0; j < 3; j++)
			{
				result[j] >>= 2;
			}
			result[3] >>= 1;
		}
		else
		{
			for (int k = 0; k < 3; k++)
			{
				result[k] >>= 1;
			}
		}
		for (int l = 0; l < 3; l++)
		{
			result[l] += result[l] >> 5;
		}
		result[3] += result[3] >> 4;
	}

	private static void UnpackModulations(AmtcBlock block, bool do2bitMode, Span<int> modulationVals, Span<int> modulationModes, int startX, int startY)
	{
		int num = (int)(block.PackedData1 & 1);
		uint num2 = block.PackedData0;
		if (do2bitMode && num != 0)
		{
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					modulationModes[(i + startY) * 16 + j + startX] = num;
					if (((j ^ i) & 1) == 0)
					{
						modulationVals[(i + startY) * 16 + j + startX] = (int)(num2 & 3);
						num2 >>= 2;
					}
				}
			}
		}
		else if (do2bitMode)
		{
			for (int k = 0; k < 4; k++)
			{
				for (int l = 0; l < 8; l++)
				{
					modulationModes[(k + startY) * 16 + l + startX] = num;
					if ((num2 & 1) != 0)
					{
						modulationVals[(k + startY) * 16 + l + startX] = 3;
					}
					else
					{
						modulationVals[(k + startY) * 16 + l + startX] = 0;
					}
					num2 >>= 1;
				}
			}
		}
		else
		{
			for (int m = 0; m < 4; m++)
			{
				for (int n = 0; n < 4; n++)
				{
					modulationModes[(m + startY) * 16 + n + startX] = num;
					modulationVals[(m + startY) * 16 + n + startX] = (int)(num2 & 3);
					num2 >>= 2;
				}
			}
		}
		if (num2 != 0)
		{
			throw new Exception("Something is left over");
		}
	}

	private static void Unpack5554Colour(AmtcBlock block, Span<int> abColors)
	{
		Span<uint> span = stackalloc uint[2];
		span[0] = block.PackedData1 & 0xFFFE;
		span[1] = block.PackedData1 >> 16;
		for (int i = 0; i < 2; i++)
		{
			if ((span[i] & 0x8000) != 0)
			{
				abColors[i * 4] = (int)((span[i] >> 10) & 0x1F);
				abColors[i * 4 + 1] = (int)((span[i] >> 5) & 0x1F);
				abColors[i * 4 + 2] = (int)(span[i] & 0x1F);
				if (i == 0)
				{
					abColors[2] |= abColors[2] >> 4;
				}
				abColors[i * 4 + 3] = 15;
				continue;
			}
			abColors[i * 4] = (int)((span[i] >> 7) & 0x1E);
			abColors[i * 4 + 1] = (int)((span[i] >> 3) & 0x1E);
			abColors[i * 4] |= abColors[i * 4] >> 4;
			abColors[i * 4 + 1] |= abColors[i * 4 + 1] >> 4;
			abColors[i * 4 + 2] = (int)((span[i] & 0xF) << 1);
			if (i == 0)
			{
				abColors[2] |= abColors[2] >> 3;
			}
			else
			{
				abColors[2] |= abColors[2] >> 4;
			}
			abColors[i * 4 + 3] = (int)((span[i] >> 11) & 0xE);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	private static Span<int> GetIntSpanForColor(this Span<Colours5554> colorSpan, int index)
	{
		return MemoryMarshal.Cast<Colours5554, int>(colorSpan.Slice(index, 1));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	private static bool IsPowerOf2(uint input)
	{
		if (input == 0)
		{
			return false;
		}
		uint num = input - 1;
		return (input | num) == (input ^ num);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	private static int LimitCoord(int value, int size)
	{
		return value & (size - 1);
	}
}
