using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AssetRipper.TextureDecoder.Atc;

public static class AtcDecoder
{
	public static int DecompressAtcRgb4(ReadOnlySpan<byte> input, int width, int height, out byte[] output)
	{
		output = new byte[width * height * 4];
		return DecompressAtcRgb4(input, width, height, output);
	}

	public unsafe static int DecompressAtcRgb4(ReadOnlySpan<byte> input, int width, int height, Span<byte> output)
	{
		fixed (byte* input2 = input)
		{
			fixed (byte* output2 = output)
			{
				return DecompressAtcRgb4(input2, width, height, output2);
			}
		}
	}

	private unsafe static int DecompressAtcRgb4(byte* input, int width, int height, byte* output)
	{
		int num = (width + 3) / 4;
		int num2 = (height + 3) / 4;
		int num3 = (width + 3) % 4 + 1;
		uint* ptr = stackalloc uint[16];
		int num4 = 0;
		for (int i = 0; i < num2; i++)
		{
			int num5 = 0;
			while (num5 < num)
			{
				DecodeAtcRgb4Block(new ReadOnlySpan<byte>(input + num4, 8), new Span<uint>(ptr, 16));
				int num6 = ((num5 < num - 1) ? 4 : num3);
				uint* ptr2 = (uint*)(output + (i * 16 * width + num5 * 16));
				uint* ptr3 = ptr;
				int num7 = 0;
				int num8 = i * 4;
				while (num7 < 4 && num8 < height)
				{
					for (int j = 0; j < num6; j++)
					{
						ptr2[j] = ptr3[j];
					}
					ptr2 += width;
					ptr3 += 4;
					num7++;
					num8++;
				}
				num5++;
				num4 += 8;
			}
		}
		return num4;
	}

	public static int DecompressAtcRgba8(ReadOnlySpan<byte> input, int width, int height, out byte[] output)
	{
		output = new byte[width * height * 4];
		return DecompressAtcRgba8(input, width, height, output);
	}

	public unsafe static int DecompressAtcRgba8(ReadOnlySpan<byte> input, int width, int height, Span<byte> output)
	{
		fixed (byte* input2 = input)
		{
			fixed (byte* output2 = output)
			{
				return DecompressAtcRgba8(input2, width, height, output2);
			}
		}
	}

	private unsafe static int DecompressAtcRgba8(byte* input, int width, int height, byte* output)
	{
		int num = (width + 3) / 4;
		int num2 = (height + 3) / 4;
		int num3 = (width + 3) % 4 + 1;
		uint* ptr = stackalloc uint[16];
		int num4 = 0;
		for (int i = 0; i < num2; i++)
		{
			int num5 = 0;
			while (num5 < num)
			{
				DecodeAtcRgba8Block(new ReadOnlySpan<byte>(input + num4, 16), new Span<uint>(ptr, 16));
				int num6 = ((num5 < num - 1) ? 4 : num3);
				uint* ptr2 = (uint*)(output + (i * 16 * width + num5 * 16));
				uint* ptr3 = ptr;
				int num7 = 0;
				int num8 = i * 4;
				while (num7 < 4 && num8 < height)
				{
					for (int j = 0; j < num6; j++)
					{
						ptr2[j] = ptr3[j];
					}
					ptr2 += width;
					ptr3 += 4;
					num7++;
					num8++;
				}
				num5++;
				num4 += 16;
			}
		}
		return num4;
	}

	private static void DecodeAtcRgb4Block(ReadOnlySpan<byte> input, Span<uint> output)
	{
		Span<int> colors = stackalloc int[16];
		int c = input.ReadAtOffset<ushort>(0);
		int c2 = input.ReadAtOffset<ushort>(2);
		uint num = input.ReadAtOffset<uint>(4);
		DecodeColors(colors, c, c2);
		for (int i = 0; i < 16; i++)
		{
			int num2 = (int)(num & 3);
			int b = colors[num2 * 4];
			int g = colors[num2 * 4 + 1];
			int r = colors[num2 * 4 + 2];
			output[i] = Color(r, g, b, 255);
			num >>= 2;
		}
	}

	private static void DecodeAtcRgba8Block(ReadOnlySpan<byte> input, Span<uint> output)
	{
		Span<int> alphas = stackalloc int[16];
		ulong num = input.ReadAtOffset<ulong>(0);
		int a = (int)num & 0xFF;
		int a2 = (int)(num >> 8) & 0xFF;
		ulong num2 = num >> 16;
		Span<int> colors = stackalloc int[16];
		int c = input.ReadAtOffset<ushort>(8);
		int c2 = input.ReadAtOffset<ushort>(10);
		uint num3 = input.ReadAtOffset<uint>(12);
		DecodeColors(colors, c, c2);
		DecodeAlphas(alphas, a, a2);
		for (int i = 0; i < 16; i++)
		{
			int num4 = (int)(num3 & 3);
			int b = colors[num4 * 4];
			int g = colors[num4 * 4 + 1];
			int r = colors[num4 * 4 + 2];
			int index = (int)num2 & 7;
			int a3 = alphas[index];
			output[i] = Color(r, g, b, a3);
			num3 >>= 2;
			num2 >>= 3;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static T ReadAtOffset<T>(this ReadOnlySpan<byte> input, int offset) where T : unmanaged
	{
		return MemoryMarshal.Read<T>(input.Slice(offset));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void DecodeColors(Span<int> colors, int c0, int c1)
	{
		if ((c0 & 0x8000) == 0)
		{
			colors[0] = Extend(c0 & 0x1F, 5, 8);
			colors[1] = Extend((c0 >> 5) & 0x1F, 5, 8);
			colors[2] = Extend((c0 >> 10) & 0x1F, 5, 8);
			colors[12] = Extend(c1 & 0x1F, 5, 8);
			colors[13] = Extend((c1 >> 5) & 0x3F, 6, 8);
			colors[14] = Extend((c1 >> 11) & 0x1F, 5, 8);
			colors[4] = (5 * colors[0] + 3 * colors[12]) / 8;
			colors[5] = (5 * colors[1] + 3 * colors[13]) / 8;
			colors[6] = (5 * colors[2] + 3 * colors[14]) / 8;
			colors[8] = (3 * colors[0] + 5 * colors[12]) / 8;
			colors[9] = (3 * colors[1] + 5 * colors[13]) / 8;
			colors[10] = (3 * colors[2] + 5 * colors[14]) / 8;
		}
		else
		{
			colors[0] = 0;
			colors[1] = 0;
			colors[2] = 0;
			colors[8] = Extend(c0 & 0x1F, 5, 8);
			colors[9] = Extend((c0 >> 5) & 0x1F, 5, 8);
			colors[10] = Extend((c0 >> 10) & 0x1F, 5, 8);
			colors[12] = Extend(c1 & 0x1F, 5, 8);
			colors[13] = Extend((c1 >> 5) & 0x3F, 6, 8);
			colors[14] = Extend((c1 >> 11) & 0x1F, 5, 8);
			colors[4] = Math.Max(0, colors[8] - colors[12] / 4);
			colors[5] = Math.Max(0, colors[9] - colors[13] / 4);
			colors[6] = Math.Max(0, colors[10] - colors[14] / 4);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void DecodeAlphas(Span<int> alphas, int a0, int a1)
	{
		alphas[0] = a0;
		alphas[1] = a1;
		if (a0 > a1)
		{
			alphas[2] = (a0 * 6 + a1) / 7;
			alphas[3] = (a0 * 5 + a1 * 2) / 7;
			alphas[4] = (a0 * 4 + a1 * 3) / 7;
			alphas[5] = (a0 * 3 + a1 * 4) / 7;
			alphas[6] = (a0 * 2 + a1 * 5) / 7;
			alphas[7] = (a0 + a1 * 6) / 7;
		}
		else
		{
			alphas[2] = (a0 * 4 + a1) / 5;
			alphas[3] = (a0 * 3 + a1 * 2) / 5;
			alphas[4] = (a0 * 2 + a1 * 3) / 5;
			alphas[5] = (a0 + a1 * 4) / 5;
			alphas[6] = 0;
			alphas[7] = 255;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int Extend(int value, int from, int to)
	{
		return (value << to - from) | (value >> from * 2 - to);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static uint Color(int r, int g, int b, int a)
	{
		return (uint)((r << 16) | (g << 8) | b | (a << 24));
	}
}
