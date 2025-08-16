using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AssetRipper.TextureDecoder.Dxt;

public static class DxtDecoder
{
	public static int DecompressDXT1(ReadOnlySpan<byte> input, int width, int height, out byte[] output)
	{
		output = new byte[width * height * 4];
		return DecompressDXT1(input, width, height, output);
	}

	public static int DecompressDXT1(ReadOnlySpan<byte> input, int width, int height, Span<byte> output)
	{
		ThrowHelper.ThrowIfNotEnoughSpace(output, width, height);
		int num = 0;
		int num2 = (width + 3) / 4;
		int num3 = (height + 3) / 4;
		int num4 = (width + 3) % 4 + 1;
		uint[] array = new uint[16];
		int[] array2 = new int[4];
		for (int i = 0; i < num3; i++)
		{
			int num5 = 0;
			while (num5 < num2)
			{
				int num6 = input[num] | (input[num + 1] << 8);
				int num7 = input[num + 2] | (input[num + 3] << 8);
				Rgb565(num6, out var r, out var g, out var b);
				Rgb565(num7, out var r2, out var g2, out var b2);
				array2[0] = Color(r, g, b, 255);
				array2[1] = Color(r2, g2, b2, 255);
				if (num6 > num7)
				{
					array2[2] = Color((r * 2 + r2) / 3, (g * 2 + g2) / 3, (b * 2 + b2) / 3, 255);
					array2[3] = Color((r + r2 * 2) / 3, (g + g2 * 2) / 3, (b + b2 * 2) / 3, 255);
				}
				else
				{
					array2[2] = Color((r + r2) / 2, (g + g2) / 2, (b + b2) / 2, 255);
				}
				uint num8 = ToUInt32(input, num + 4);
				int num9 = 0;
				while (num9 < 16)
				{
					array[num9] = (uint)array2[num8 & 3];
					num9++;
					num8 >>= 2;
				}
				int count = ((num5 < num2 - 1) ? 4 : num4) * 4;
				int num10 = 0;
				int num11 = i * 4;
				while (num10 < 4 && num11 < height)
				{
					BlockCopy(MemoryMarshal.Cast<uint, byte>(new ReadOnlySpan<uint>(array)), num10 * 4 * 4, output, (num11 * width + num5 * 4) * 4, count);
					num10++;
					num11++;
				}
				num5++;
				num += 8;
			}
		}
		return num;
	}

	public static int DecompressDXT3(ReadOnlySpan<byte> input, int width, int height, out byte[] output)
	{
		output = new byte[width * height * 4];
		return DecompressDXT3(input, width, height, output);
	}

	public static int DecompressDXT3(ReadOnlySpan<byte> input, int width, int height, Span<byte> output)
	{
		ThrowHelper.ThrowIfNotEnoughSpace(output, width, height);
		int num = 0;
		int num2 = (width + 3) / 4;
		int num3 = (height + 3) / 4;
		int num4 = (width + 3) % 4 + 1;
		uint[] array = new uint[16];
		int[] array2 = new int[4];
		int[] array3 = new int[16];
		for (int i = 0; i < num3; i++)
		{
			int num5 = 0;
			while (num5 < num2)
			{
				for (int j = 0; j < 4; j++)
				{
					int num6 = input[num + j * 2] | (input[num + j * 2 + 1] << 8);
					array3[j * 4] = (num6 & 0xF) * 17 << 24;
					array3[j * 4 + 1] = ((num6 >> 4) & 0xF) * 17 << 24;
					array3[j * 4 + 2] = ((num6 >> 8) & 0xF) * 17 << 24;
					array3[j * 4 + 3] = ((num6 >> 12) & 0xF) * 17 << 24;
				}
				int num7 = input[num + 8] | (input[num + 9] << 8);
				int num8 = input[num + 10] | (input[num + 11] << 8);
				Rgb565(num7, out var r, out var g, out var b);
				Rgb565(num8, out var r2, out var g2, out var b2);
				array2[0] = Color(r, g, b, 0);
				array2[1] = Color(r2, g2, b2, 0);
				if (num7 > num8)
				{
					array2[2] = Color((r * 2 + r2) / 3, (g * 2 + g2) / 3, (b * 2 + b2) / 3, 0);
					array2[3] = Color((r + r2 * 2) / 3, (g + g2 * 2) / 3, (b + b2 * 2) / 3, 0);
				}
				else
				{
					array2[2] = Color((r + r2) / 2, (g + g2) / 2, (b + b2) / 2, 0);
				}
				uint num9 = ToUInt32(input, num + 12);
				int num10 = 0;
				while (num10 < 16)
				{
					array[num10] = (uint)(array2[num9 & 3] | array3[num10]);
					num10++;
					num9 >>= 2;
				}
				int count = ((num5 < num2 - 1) ? 4 : num4) * 4;
				int num11 = 0;
				int num12 = i * 4;
				while (num11 < 4 && num12 < height)
				{
					BlockCopy(MemoryMarshal.Cast<uint, byte>(new ReadOnlySpan<uint>(array)), num11 * 4 * 4, output, (num12 * width + num5 * 4) * 4, count);
					num11++;
					num12++;
				}
				num5++;
				num += 16;
			}
		}
		return num;
	}

	public static int DecompressDXT5(ReadOnlySpan<byte> input, int width, int height, out byte[] output)
	{
		output = new byte[width * height * 4];
		return DecompressDXT5(input, width, height, output);
	}

	public static int DecompressDXT5(ReadOnlySpan<byte> input, int width, int height, Span<byte> output)
	{
		ThrowHelper.ThrowIfNotEnoughSpace(output, width, height);
		int num = 0;
		int num2 = (width + 3) / 4;
		int num3 = (height + 3) / 4;
		int num4 = (width + 3) % 4 + 1;
		uint[] array = new uint[16];
		int[] array2 = new int[4];
		int[] array3 = new int[8];
		for (int i = 0; i < num3; i++)
		{
			int num5 = 0;
			while (num5 < num2)
			{
				array3[0] = input[num];
				array3[1] = input[num + 1];
				if (array3[0] > array3[1])
				{
					array3[2] = (array3[0] * 6 + array3[1]) / 7;
					array3[3] = (array3[0] * 5 + array3[1] * 2) / 7;
					array3[4] = (array3[0] * 4 + array3[1] * 3) / 7;
					array3[5] = (array3[0] * 3 + array3[1] * 4) / 7;
					array3[6] = (array3[0] * 2 + array3[1] * 5) / 7;
					array3[7] = (array3[0] + array3[1] * 6) / 7;
				}
				else
				{
					array3[2] = (array3[0] * 4 + array3[1]) / 5;
					array3[3] = (array3[0] * 3 + array3[1] * 2) / 5;
					array3[4] = (array3[0] * 2 + array3[1] * 3) / 5;
					array3[5] = (array3[0] + array3[1] * 4) / 5;
					array3[7] = 255;
				}
				for (int j = 0; j < 8; j++)
				{
					array3[j] <<= 24;
				}
				int num6 = input[num + 8] | (input[num + 9] << 8);
				int num7 = input[num + 10] | (input[num + 11] << 8);
				Rgb565(num6, out var r, out var g, out var b);
				Rgb565(num7, out var r2, out var g2, out var b2);
				array2[0] = Color(r, g, b, 0);
				array2[1] = Color(r2, g2, b2, 0);
				if (num6 > num7)
				{
					array2[2] = Color((r * 2 + r2) / 3, (g * 2 + g2) / 3, (b * 2 + b2) / 3, 0);
					array2[3] = Color((r + r2 * 2) / 3, (g + g2 * 2) / 3, (b + b2 * 2) / 3, 0);
				}
				else
				{
					array2[2] = Color((r + r2) / 2, (g + g2) / 2, (b + b2) / 2, 0);
				}
				ulong num8 = ToUInt64(input, num) >> 16;
				uint num9 = ToUInt32(input, num + 12);
				int num10 = 0;
				while (num10 < 16)
				{
					array[num10] = (uint)(array3[num8 & 7] | array2[num9 & 3]);
					num10++;
					num8 >>= 3;
					num9 >>= 2;
				}
				int count = ((num5 < num2 - 1) ? 4 : num4) * 4;
				int num11 = 0;
				int num12 = i * 4;
				while (num11 < 4 && num12 < height)
				{
					BlockCopy(MemoryMarshal.Cast<uint, byte>(new ReadOnlySpan<uint>(array)), num11 * 4 * 4, output, (num12 * width + num5 * 4) * 4, count);
					num11++;
					num12++;
				}
				num5++;
				num += 16;
			}
		}
		return num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void Rgb565(int c, out int r, out int g, out int b)
	{
		r = (c & 0xF800) >> 8;
		g = (c & 0x7E0) >> 3;
		b = (c & 0x1F) << 3;
		r |= r >> 5;
		g |= g >> 6;
		b |= b >> 5;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int Color(int r, int g, int b, int a)
	{
		return (r << 16) | (g << 8) | b | (a << 24);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void BlockCopy(ReadOnlySpan<byte> source, int sourceOffset, Span<byte> destination, int destinationOffset, int count)
	{
		source.Slice(sourceOffset, count).CopyTo(destination.Slice(destinationOffset));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static uint ToUInt32(ReadOnlySpan<byte> input, int offset)
	{
		return BinaryPrimitives.ReadUInt32LittleEndian(input.Slice(offset));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ulong ToUInt64(ReadOnlySpan<byte> input, int offset)
	{
		return BinaryPrimitives.ReadUInt64LittleEndian(input.Slice(offset));
	}
}
