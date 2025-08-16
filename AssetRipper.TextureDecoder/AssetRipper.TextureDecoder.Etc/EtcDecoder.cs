using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AssetRipper.TextureDecoder.Etc;

public static class EtcDecoder
{
	private static readonly byte[] WriteOrderTable = new byte[16]
	{
		0, 4, 8, 12, 1, 5, 9, 13, 2, 6,
		10, 14, 3, 7, 11, 15
	};

	private static readonly byte[] WriteOrderTableRev = new byte[16]
	{
		15, 11, 7, 3, 14, 10, 6, 2, 13, 9,
		5, 1, 12, 8, 4, 0
	};

	private static readonly int[] Etc1SubblockTable = new int[32]
	{
		0, 0, 0, 0, 0, 0, 0, 0, 1, 1,
		1, 1, 1, 1, 1, 1, 0, 0, 1, 1,
		0, 0, 1, 1, 0, 0, 1, 1, 0, 0,
		1, 1
	};

	private static readonly int[] Etc1ModifierTable = new int[32]
	{
		2, 8, -2, -8, 5, 17, -5, -17, 9, 29,
		-9, -29, 13, 42, -13, -42, 18, 60, -18, -60,
		24, 80, -24, -80, 33, 106, -33, -106, 47, 183,
		-47, -183
	};

	private static readonly int[] PunchthroughModifierTable = new int[32]
	{
		0, 8, 0, -8, 0, 17, 0, -17, 0, 29,
		0, -29, 0, 42, 0, -42, 0, 60, 0, -60,
		0, 80, 0, -80, 0, 106, 0, -106, 0, 183,
		0, -183
	};

	private static readonly byte[] Etc2DistanceTable = new byte[8] { 3, 6, 11, 16, 23, 32, 41, 64 };

	private static readonly sbyte[] Etc2AlphaModTable = new sbyte[128]
	{
		-3, -6, -9, -15, 2, 5, 8, 14, -3, -7,
		-10, -13, 2, 6, 9, 12, -2, -5, -8, -13,
		1, 4, 7, 12, -2, -4, -6, -13, 1, 3,
		5, 12, -3, -6, -8, -12, 2, 5, 7, 11,
		-3, -7, -9, -11, 2, 6, 8, 10, -4, -7,
		-8, -11, 3, 6, 7, 10, -3, -5, -8, -11,
		2, 4, 7, 10, -2, -6, -8, -10, 1, 5,
		7, 9, -2, -5, -8, -10, 1, 4, 7, 9,
		-2, -4, -8, -10, 1, 3, 7, 9, -2, -5,
		-7, -10, 1, 4, 6, 9, -3, -4, -7, -10,
		2, 3, 6, 9, -1, -2, -3, -10, 0, 1,
		2, 9, -4, -6, -8, -9, 3, 5, 7, 8,
		-3, -5, -7, -9, 2, 4, 6, 8
	};

	private static readonly uint[] PunchthroughMaskTable = new uint[4] { 4294967295u, 4294967295u, 0u, 4294967295u };

	public static int DecompressETC(ReadOnlySpan<byte> input, int width, int height, out byte[] output)
	{
		output = new byte[width * height * 4];
		return DecompressETC(input, width, height, output);
	}

	public unsafe static int DecompressETC(ReadOnlySpan<byte> input, int width, int height, Span<byte> output)
	{
		int num = (width + 3) / 4 * ((height + 3) / 4) * 8;
		if (input.Length < num)
		{
			throw new ArgumentException($"{"input"} has length {input.Length} which is less than the required length {num}", "input");
		}
		fixed (byte* output2 = output)
		{
			return DecompressETC(input, width, height, output2);
		}
	}

	private unsafe static int DecompressETC(ReadOnlySpan<byte> input, int width, int height, byte* output)
	{
		int num = (width + 3) / 4;
		int num2 = (height + 3) / 4;
		int num3 = (width + 3) % 4 + 1;
		Span<uint> output2 = stackalloc uint[16];
		int num4 = 0;
		for (int i = 0; i < num2; i++)
		{
			int num5 = 0;
			while (num5 < num)
			{
				DecodeEtc1Block(input.Slice(num4, 8), output2);
				int num6 = ((num5 < num - 1) ? 4 : num3);
				uint* ptr = (uint*)(output + (i * 16 * width + num5 * 16));
				int num7 = 0;
				int num8 = i * 4;
				while (num7 < 4 && num8 < height)
				{
					for (int j = 0; j < num6; j++)
					{
						ptr[j] = output2[j + 4 * num7];
					}
					ptr += width;
					num7++;
					num8++;
				}
				num5++;
				num4 += 8;
			}
		}
		return num4;
	}

	public static int DecompressETC2(ReadOnlySpan<byte> input, int width, int height, out byte[] output)
	{
		output = new byte[width * height * 4];
		return DecompressETC2(input, width, height, output);
	}

	public unsafe static int DecompressETC2(ReadOnlySpan<byte> input, int width, int height, Span<byte> output)
	{
		int num = (width + 3) / 4 * ((height + 3) / 4) * 8;
		if (input.Length < num)
		{
			throw new ArgumentException($"{"input"} has length {input.Length} which is less than the required length {num}", "input");
		}
		fixed (byte* output2 = output)
		{
			return DecompressETC2(input, width, height, output2);
		}
	}

	private unsafe static int DecompressETC2(ReadOnlySpan<byte> input, int width, int height, byte* output)
	{
		int num = (width + 3) / 4;
		int num2 = (height + 3) / 4;
		int num3 = (width + 3) % 4 + 1;
		Span<uint> output2 = stackalloc uint[16];
		int num4 = 0;
		for (int i = 0; i < num2; i++)
		{
			int num5 = 0;
			while (num5 < num)
			{
				DecodeEtc2Block(input.Slice(num4, 8), output2);
				int num6 = ((num5 < num - 1) ? 4 : num3);
				uint* ptr = (uint*)(output + (i * 16 * width + num5 * 16));
				int num7 = 0;
				int num8 = i * 4;
				while (num7 < 4 && num8 < height)
				{
					for (int j = 0; j < num6; j++)
					{
						ptr[j] = output2[j + 4 * num7];
					}
					ptr += width;
					num7++;
					num8++;
				}
				num5++;
				num4 += 8;
			}
		}
		return num4;
	}

	public static int DecompressETC2A1(ReadOnlySpan<byte> input, int width, int height, out byte[] output)
	{
		output = new byte[width * height * 4];
		return DecompressETC2A1(input, width, height, output);
	}

	public unsafe static int DecompressETC2A1(ReadOnlySpan<byte> input, int width, int height, Span<byte> output)
	{
		int num = (width + 3) / 4 * ((height + 3) / 4) * 8;
		if (input.Length < num)
		{
			throw new ArgumentException($"{"input"} has length {input.Length} which is less than the required length {num}", "input");
		}
		fixed (byte* output2 = output)
		{
			return DecompressETC2A1(input, width, height, output2);
		}
	}

	private unsafe static int DecompressETC2A1(ReadOnlySpan<byte> input, int width, int height, byte* output)
	{
		int num = (width + 3) / 4;
		int num2 = (height + 3) / 4;
		int num3 = (width + 3) % 4 + 1;
		Span<uint> output2 = stackalloc uint[16];
		int num4 = 0;
		for (int i = 0; i < num2; i++)
		{
			int num5 = 0;
			while (num5 < num)
			{
				DecodeEtc2a1Block(input.Slice(num4, 8), output2);
				int num6 = ((num5 < num - 1) ? 4 : num3);
				uint* ptr = (uint*)(output + (i * 16 * width + num5 * 16));
				int num7 = 0;
				int num8 = i * 4;
				while (num7 < 4 && num8 < height)
				{
					for (int j = 0; j < num6; j++)
					{
						ptr[j] = output2[j + 4 * num7];
					}
					ptr += width;
					num7++;
					num8++;
				}
				num5++;
				num4 += 8;
			}
		}
		return num4;
	}

	public static int DecompressETC2A8(ReadOnlySpan<byte> input, int width, int height, out byte[] output)
	{
		output = new byte[width * height * 4];
		return DecompressETC2A8(input, width, height, output);
	}

	public unsafe static int DecompressETC2A8(ReadOnlySpan<byte> input, int width, int height, Span<byte> output)
	{
		int num = (width + 3) / 4 * ((height + 3) / 4) * 16;
		if (input.Length < num)
		{
			throw new ArgumentException($"{"input"} has length {input.Length} which is less than the required length {num}", "input");
		}
		fixed (byte* output2 = output)
		{
			return DecompressETC2A8(input, width, height, output2);
		}
	}

	private unsafe static int DecompressETC2A8(ReadOnlySpan<byte> input, int width, int height, byte* output)
	{
		int num = (width + 3) / 4;
		int num2 = (height + 3) / 4;
		int num3 = (width + 3) % 4 + 1;
		Span<uint> output2 = stackalloc uint[16];
		int num4 = 0;
		for (int i = 0; i < num2; i++)
		{
			int num5 = 0;
			while (num5 < num)
			{
				DecodeEtc2Block(input.Slice(num4 + 8, 8), output2);
				DecodeEtc2a8Block(input.Slice(num4, 8), output2);
				int num6 = ((num5 < num - 1) ? 4 : num3);
				uint* ptr = (uint*)(output + (i * 16 * width + num5 * 16));
				int num7 = 0;
				int num8 = i * 4;
				while (num7 < 4 && num8 < height)
				{
					for (int j = 0; j < num6; j++)
					{
						ptr[j] = output2[j + 4 * num7];
					}
					ptr += width;
					num7++;
					num8++;
				}
				num5++;
				num4 += 16;
			}
		}
		return num4;
	}

	public static int DecompressEACRUnsigned(ReadOnlySpan<byte> input, int width, int height, out byte[] output)
	{
		output = new byte[width * height * 4];
		return DecompressEACRUnsigned(input, width, height, output);
	}

	public unsafe static int DecompressEACRUnsigned(ReadOnlySpan<byte> input, int width, int height, Span<byte> output)
	{
		int num = (width + 3) / 4 * ((height + 3) / 4) * 8;
		if (input.Length < num)
		{
			throw new ArgumentException($"{"input"} has length {input.Length} which is less than the required length {num}", "input");
		}
		fixed (byte* output2 = output)
		{
			return DecompressEACRUnsigned(input, width, height, output2);
		}
	}

	private unsafe static int DecompressEACRUnsigned(ReadOnlySpan<byte> input, int width, int height, byte* output)
	{
		int num = (width + 3) / 4;
		int num2 = (height + 3) / 4;
		int num3 = (width + 3) % 4 + 1;
		Span<uint> output2 = stackalloc uint[16];
		for (int i = 0; i < 16; i++)
		{
			output2[i] = 4278190080u;
		}
		int num4 = 0;
		for (int j = 0; j < num2; j++)
		{
			int num5 = 0;
			while (num5 < num)
			{
				DecodeEacUnsignedBlock(input.Slice(num4, 8), output2, 2);
				int num6 = ((num5 < num - 1) ? 4 : num3);
				uint* ptr = (uint*)(output + (j * 16 * width + num5 * 16));
				int num7 = 0;
				int num8 = j * 4;
				while (num7 < 4 && num8 < height)
				{
					for (int k = 0; k < num6; k++)
					{
						ptr[k] = output2[k + 4 * num7];
					}
					ptr += width;
					num7++;
					num8++;
				}
				num5++;
				num4 += 8;
			}
		}
		return num4;
	}

	public static int DecompressEACRSigned(ReadOnlySpan<byte> input, int width, int height, out byte[] output)
	{
		output = new byte[width * height * 4];
		return DecompressEACRSigned(input, width, height, output);
	}

	public unsafe static int DecompressEACRSigned(ReadOnlySpan<byte> input, int width, int height, Span<byte> output)
	{
		int num = (width + 3) / 4 * ((height + 3) / 4) * 8;
		if (input.Length < num)
		{
			throw new ArgumentException($"{"input"} has length {input.Length} which is less than the required length {num}", "input");
		}
		fixed (byte* output2 = output)
		{
			return DecompressEACRSigned(input, width, height, output2);
		}
	}

	private unsafe static int DecompressEACRSigned(ReadOnlySpan<byte> input, int width, int height, byte* output)
	{
		int num = (width + 3) / 4;
		int num2 = (height + 3) / 4;
		int num3 = (width + 3) % 4 + 1;
		Span<uint> output2 = stackalloc uint[16];
		for (int i = 0; i < 16; i++)
		{
			output2[i] = 4278190080u;
		}
		int num4 = 0;
		for (int j = 0; j < num2; j++)
		{
			int num5 = 0;
			while (num5 < num)
			{
				DecodeEacSignedBlock(input.Slice(num4, 8), output2, 2);
				int num6 = ((num5 < num - 1) ? 4 : num3);
				uint* ptr = (uint*)(output + (j * 16 * width + num5 * 16));
				int num7 = 0;
				int num8 = j * 4;
				while (num7 < 4 && num8 < height)
				{
					for (int k = 0; k < num6; k++)
					{
						ptr[k] = output2[k + 4 * num7];
					}
					ptr += width;
					num7++;
					num8++;
				}
				num5++;
				num4 += 8;
			}
		}
		return num4;
	}

	public static int DecompressEACRGUnsigned(ReadOnlySpan<byte> input, int width, int height, out byte[] output)
	{
		output = new byte[width * height * 4];
		return DecompressEACRGUnsigned(input, width, height, output);
	}

	public static int DecompressEACRGUnsigned(ReadOnlySpan<byte> input, int width, int height, Span<byte> output)
	{
		int num = (width + 3) / 4;
		int num2 = (height + 3) / 4;
		if (input.Length < num * num2 * 16)
		{
			throw new ArgumentException($"{"input"} has length {input.Length} which is less than the required length {num * num2 * 16}", "input");
		}
		int num3 = (width + 3) % 4 + 1;
		Span<uint> output2 = stackalloc uint[16];
		for (int i = 0; i < 16; i++)
		{
			output2[i] = 4278190080u;
		}
		int num4 = 0;
		for (int j = 0; j < num2; j++)
		{
			int num5 = 0;
			while (num5 < num)
			{
				DecodeEacUnsignedBlock(input.Slice(num4, 8), output2, 2);
				DecodeEacUnsignedBlock(input.Slice(num4 + 8, 8), output2, 1);
				int num6 = ((num5 < num - 1) ? 4 : num3);
				int start = j * 16 * width + num5 * 16;
				Span<uint> span = MemoryMarshal.Cast<byte, uint>(output.Slice(start));
				int num7 = 0;
				int num8 = j * 4;
				while (num7 < 4 && num8 < height)
				{
					for (int k = 0; k < num6; k++)
					{
						span[k + num7 * width] = output2[k + 4 * num7];
					}
					num7++;
					num8++;
				}
				num5++;
				num4 += 16;
			}
		}
		return num4;
	}

	public static int DecompressEACRGSigned(ReadOnlySpan<byte> input, int width, int height, out byte[] output)
	{
		output = new byte[width * height * 4];
		return DecompressEACRGSigned(input, width, height, output);
	}

	public unsafe static int DecompressEACRGSigned(ReadOnlySpan<byte> input, int width, int height, Span<byte> output)
	{
		int num = (width + 3) / 4 * ((height + 3) / 4) * 16;
		if (input.Length < num)
		{
			throw new ArgumentException($"{"input"} has length {input.Length} which is less than the required length {num}", "input");
		}
		fixed (byte* output2 = output)
		{
			return DecompressEACRGSigned(input, width, height, output2);
		}
	}

	private unsafe static int DecompressEACRGSigned(ReadOnlySpan<byte> input, int width, int height, byte* output)
	{
		int num = (width + 3) / 4;
		int num2 = (height + 3) / 4;
		int num3 = (width + 3) % 4 + 1;
		Span<uint> output2 = stackalloc uint[16];
		for (int i = 0; i < 16; i++)
		{
			output2[i] = 4278190080u;
		}
		int num4 = 0;
		for (int j = 0; j < num2; j++)
		{
			int num5 = 0;
			while (num5 < num)
			{
				DecodeEacSignedBlock(input.Slice(num4, 8), output2, 2);
				DecodeEacSignedBlock(input.Slice(num4 + 8, 8), output2, 1);
				int num6 = ((num5 < num - 1) ? 4 : num3);
				uint* ptr = (uint*)(output + (j * 16 * width + num5 * 16));
				int num7 = 0;
				int num8 = j * 4;
				while (num7 < 4 && num8 < height)
				{
					for (int k = 0; k < num6; k++)
					{
						ptr[k] = output2[k + 4 * num7];
					}
					ptr += width;
					num7++;
					num8++;
				}
				num5++;
				num4 += 16;
			}
		}
		return num4;
	}

	private static void DecodeEtc1Block(ReadOnlySpan<byte> input, Span<uint> output)
	{
		byte b = input[3];
		Span<int> span = stackalloc int[2];
		span[0] = (b >> 5) * 4;
		span[1] = ((b >> 2) & 7) * 4;
		Span<byte> span2 = stackalloc byte[6];
		int num = (b & 1) * 16;
		if ((b & 2) != 0)
		{
			span2[0] = (byte)(input[0] & 0xF8);
			span2[1] = (byte)(input[1] & 0xF8);
			span2[2] = (byte)(input[2] & 0xF8);
			span2[3] = (byte)(span2[0] + ((input[0] << 3) & 0x18) - ((input[0] << 3) & 0x20));
			span2[4] = (byte)(span2[1] + ((input[1] << 3) & 0x18) - ((input[1] << 3) & 0x20));
			span2[5] = (byte)(span2[2] + ((input[2] << 3) & 0x18) - ((input[2] << 3) & 0x20));
			span2[0] |= (byte)(span2[0] >> 5);
			span2[1] |= (byte)(span2[1] >> 5);
			span2[2] |= (byte)(span2[2] >> 5);
			span2[3] |= (byte)(span2[3] >> 5);
			span2[4] |= (byte)(span2[4] >> 5);
			span2[5] |= (byte)(span2[5] >> 5);
		}
		else
		{
			span2[0] = (byte)((input[0] & 0xF0) | (input[0] >> 4));
			span2[3] = (byte)((input[0] & 0xF) | (input[0] << 4));
			span2[1] = (byte)((input[1] & 0xF0) | (input[1] >> 4));
			span2[4] = (byte)((input[1] & 0xF) | (input[1] << 4));
			span2[2] = (byte)((input[2] & 0xF0) | (input[2] >> 4));
			span2[5] = (byte)((input[2] & 0xF) | (input[2] << 4));
		}
		int num2 = (input[6] << 8) | input[7];
		int num3 = (input[4] << 8) | input[5];
		int num4 = 0;
		while (num4 < 16)
		{
			int num5 = Etc1SubblockTable[num + num4];
			int num6 = ((num3 << 1) & 2) | (num2 & 1);
			int num7 = span[num5];
			int m = Etc1ModifierTable[num7 + num6];
			uint num8 = ApplicateColor(span2, num5, m);
			int index = WriteOrderTable[num4];
			output[index] = num8;
			num4++;
			num2 >>= 1;
			num3 >>= 1;
		}
	}

	private static void DecodeEtc2Block(ReadOnlySpan<byte> input, Span<uint> output)
	{
		int num = (input[6] << 8) | input[7];
		int num2 = (input[4] << 8) | input[5];
		Span<byte> span = stackalloc byte[9];
		if ((input[3] & 2) != 0)
		{
			int num3 = input[0] & 0xF8;
			int num4 = ((input[0] << 3) & 0x18) - ((input[0] << 3) & 0x20);
			if (num3 + num4 < 0 || num3 + num4 > 255)
			{
				span[0] = (byte)(((input[0] << 3) & 0xC0) | ((input[0] << 4) & 0x30) | ((input[0] >> 1) & 0xC) | (input[0] & 3));
				span[1] = (byte)((input[1] & 0xF0) | (input[1] >> 4));
				span[2] = (byte)((input[1] & 0xF) | (input[1] << 4));
				span[3] = (byte)((input[2] & 0xF0) | (input[2] >> 4));
				span[4] = (byte)((input[2] & 0xF) | (input[2] << 4));
				span[5] = (byte)((input[3] & 0xF0) | (input[3] >> 4));
				int num5 = ((input[3] >> 1) & 6) | (input[3] & 1);
				byte b = Etc2DistanceTable[num5];
				ReadOnlySpan<uint> readOnlySpan = stackalloc uint[4]
				{
					ApplicateColorRaw(span, 0),
					ApplicateColor(span, 1, b),
					ApplicateColorRaw(span, 1),
					ApplicateColor(span, 1, -b)
				};
				int num6 = 0;
				while (num6 < 16)
				{
					int index = ((num2 << 1) & 2) | (num & 1);
					uint num7 = readOnlySpan[index];
					int index2 = WriteOrderTable[num6];
					output[index2] = num7;
					num6++;
					num >>= 1;
					num2 >>= 1;
				}
				return;
			}
			int num8 = input[1] & 0xF8;
			int num9 = ((input[1] << 3) & 0x18) - ((input[1] << 3) & 0x20);
			if (num8 + num9 < 0 || num8 + num9 > 255)
			{
				span[0] = (byte)(((input[0] << 1) & 0xF0) | ((input[0] >> 3) & 0xF));
				span[1] = (byte)(((input[0] << 5) & 0xE0) | (input[1] & 0x10));
				span[1] |= (byte)(span[1] >> 4);
				span[2] = (byte)((input[1] & 8) | ((input[1] << 1) & 6) | (input[2] >> 7));
				span[2] |= (byte)(span[2] << 4);
				span[3] = (byte)(((input[2] << 1) & 0xF0) | ((input[2] >> 3) & 0xF));
				span[4] = (byte)(((input[2] << 5) & 0xE0) | ((input[3] >> 3) & 0x10));
				span[4] |= (byte)(span[4] >> 4);
				span[5] = (byte)(((input[3] << 1) & 0xF0) | ((input[3] >> 3) & 0xF));
				int num10 = (input[3] & 4) | ((input[3] << 1) & 2);
				if (span[0] > span[3] || (span[0] == span[3] && (span[1] > span[4] || (span[1] == span[4] && span[2] >= span[5]))))
				{
					num10++;
				}
				byte b2 = Etc2DistanceTable[num10];
				ReadOnlySpan<uint> readOnlySpan2 = stackalloc uint[4]
				{
					ApplicateColor(span, 0, b2),
					ApplicateColor(span, 0, -b2),
					ApplicateColor(span, 1, b2),
					ApplicateColor(span, 1, -b2)
				};
				int num11 = 0;
				while (num11 < 16)
				{
					int index3 = ((num2 << 1) & 2) | (num & 1);
					uint num12 = readOnlySpan2[index3];
					int index4 = WriteOrderTable[num11];
					output[index4] = num12;
					num11++;
					num >>= 1;
					num2 >>= 1;
				}
				return;
			}
			int num13 = input[2] & 0xF8;
			int num14 = ((input[2] << 3) & 0x18) - ((input[2] << 3) & 0x20);
			if (num13 + num14 < 0 || num13 + num14 > 255)
			{
				span[0] = (byte)(((input[0] << 1) & 0xFC) | ((input[0] >> 5) & 3));
				span[1] = (byte)(((input[0] << 7) & 0x80) | (input[1] & 0x7E) | (input[0] & 1));
				span[2] = (byte)(((input[1] << 7) & 0x80) | ((input[2] << 2) & 0x60) | ((input[2] << 3) & 0x18) | ((input[3] >> 5) & 4));
				span[2] |= (byte)(span[2] >> 6);
				span[3] = (byte)(((input[3] << 1) & 0xF8) | ((input[3] << 2) & 4) | ((input[3] >> 5) & 3));
				span[4] = (byte)((input[4] & 0xFE) | (input[4] >> 7));
				span[5] = (byte)(((input[4] << 7) & 0x80) | ((input[5] >> 1) & 0x7C));
				span[5] |= (byte)(span[5] >> 6);
				span[6] = (byte)(((input[5] << 5) & 0xE0) | ((input[6] >> 3) & 0x1C) | ((input[5] >> 1) & 3));
				span[7] = (byte)(((input[6] << 3) & 0xF8) | ((input[7] >> 5) & 6) | ((input[6] >> 4) & 1));
				span[8] = (byte)((input[7] << 2) | ((input[7] >> 4) & 3));
				int i = 0;
				int num15 = 0;
				for (; i < 4; i++)
				{
					int num16 = 0;
					while (num16 < 4)
					{
						int r = Clamp255(num16 * (span[3] - span[0]) + i * (span[6] - span[0]) + 4 * span[0] + 2 >> 2);
						int g = Clamp255(num16 * (span[4] - span[1]) + i * (span[7] - span[1]) + 4 * span[1] + 2 >> 2);
						int b3 = Clamp255(num16 * (span[5] - span[2]) + i * (span[8] - span[2]) + 4 * span[2] + 2 >> 2);
						output[num15] = Color(r, g, b3, 255);
						num16++;
						num15++;
					}
				}
				return;
			}
			Span<int> span2 = stackalloc int[2];
			span2[0] = (input[3] >> 5) * 4;
			span2[1] = ((input[3] >> 2) & 7) * 4;
			int num17 = (input[3] & 1) * 16;
			span[0] = (byte)(num3 | (num3 >> 5));
			span[1] = (byte)(num8 | (num8 >> 5));
			span[2] = (byte)(num13 | (num13 >> 5));
			span[3] = (byte)(num3 + num4);
			span[4] = (byte)(num8 + num9);
			span[5] = (byte)(num13 + num14);
			span[3] |= (byte)(span[3] >> 5);
			span[4] |= (byte)(span[4] >> 5);
			span[5] |= (byte)(span[5] >> 5);
			int num18 = 0;
			while (num18 < 16)
			{
				int num19 = Etc1SubblockTable[num17 + num18];
				int num20 = ((num2 << 1) & 2) | (num & 1);
				int num21 = span2[num19];
				int m = Etc1ModifierTable[num21 + num20];
				uint num22 = ApplicateColor(span, num19, m);
				int index5 = WriteOrderTable[num18];
				output[index5] = num22;
				num18++;
				num >>= 1;
				num2 >>= 1;
			}
		}
		else
		{
			Span<int> span3 = stackalloc int[2];
			span3[0] = (input[3] >> 5) * 4;
			span3[1] = ((input[3] >> 2) & 7) * 4;
			int num23 = (input[3] & 1) * 16;
			span[0] = (byte)((input[0] & 0xF0) | (input[0] >> 4));
			span[3] = (byte)((input[0] & 0xF) | (input[0] << 4));
			span[1] = (byte)((input[1] & 0xF0) | (input[1] >> 4));
			span[4] = (byte)((input[1] & 0xF) | (input[1] << 4));
			span[2] = (byte)((input[2] & 0xF0) | (input[2] >> 4));
			span[5] = (byte)((input[2] & 0xF) | (input[2] << 4));
			int num24 = 0;
			while (num24 < 16)
			{
				int num25 = Etc1SubblockTable[num23 + num24];
				int num26 = ((num2 << 1) & 2) | (num & 1);
				int num27 = span3[num25];
				int m2 = Etc1ModifierTable[num27 + num26];
				uint num28 = ApplicateColor(span, num25, m2);
				int index6 = WriteOrderTable[num24];
				output[index6] = num28;
				num24++;
				num >>= 1;
				num2 >>= 1;
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void DecodeEtc2a1Block(ReadOnlySpan<byte> input, Span<uint> output)
	{
		if ((input[3] & 2) != 0)
		{
			DecodeEtc2Block(input, output);
		}
		else
		{
			DecodeEtc2PunchThrowBlock(input, output);
		}
	}

	private unsafe static void DecodeEtc2PunchThrowBlock(ReadOnlySpan<byte> input, Span<uint> output)
	{
		int num = (input[6] << 8) | input[7];
		int num2 = (input[4] << 8) | input[5];
		Span<byte> span = stackalloc byte[9];
		int num3 = input[0] & 0xF8;
		int num4 = ((input[0] << 3) & 0x18) - ((input[0] << 3) & 0x20);
		if (num3 + num4 < 0 || num3 + num4 > 255)
		{
			span[0] = (byte)(((input[0] << 3) & 0xC0) | ((input[0] << 4) & 0x30) | ((input[0] >> 1) & 0xC) | (input[0] & 3));
			span[1] = (byte)((input[1] & 0xF0) | (input[1] >> 4));
			span[2] = (byte)((input[1] & 0xF) | (input[1] << 4));
			span[3] = (byte)((input[2] & 0xF0) | (input[2] >> 4));
			span[4] = (byte)((input[2] & 0xF) | (input[2] << 4));
			span[5] = (byte)((input[3] & 0xF0) | (input[3] >> 4));
			int num5 = ((input[3] >> 1) & 6) | (input[3] & 1);
			byte b = Etc2DistanceTable[num5];
			uint* ptr = stackalloc uint[4]
			{
				ApplicateColorRaw(span, 0),
				ApplicateColor(span, 1, b),
				ApplicateColorRaw(span, 1),
				ApplicateColor(span, 1, -b)
			};
			int num6 = 0;
			while (num6 < 16)
			{
				int num7 = ((num2 << 1) & 2) | (num & 1);
				uint num8 = ptr[num7];
				uint num9 = PunchthroughMaskTable[num7];
				int index = WriteOrderTable[num6];
				output[index] = num8 & num9;
				num6++;
				num >>= 1;
				num2 >>= 1;
			}
			return;
		}
		int num10 = input[1] & 0xF8;
		int num11 = ((input[1] << 3) & 0x18) - ((input[1] << 3) & 0x20);
		if (num10 + num11 < 0 || num10 + num11 > 255)
		{
			span[0] = (byte)(((input[0] << 1) & 0xF0) | ((input[0] >> 3) & 0xF));
			span[1] = (byte)(((input[0] << 5) & 0xE0) | (input[1] & 0x10));
			span[1] |= (byte)(span[1] >> 4);
			span[2] = (byte)((input[1] & 8) | ((input[1] << 1) & 6) | (input[2] >> 7));
			span[2] |= (byte)(span[2] << 4);
			span[3] = (byte)(((input[2] << 1) & 0xF0) | ((input[2] >> 3) & 0xF));
			span[4] = (byte)(((input[2] << 5) & 0xE0) | ((input[3] >> 3) & 0x10));
			span[4] |= (byte)(span[4] >> 4);
			span[5] = (byte)(((input[3] << 1) & 0xF0) | ((input[3] >> 3) & 0xF));
			int num12 = (input[3] & 4) | ((input[3] << 1) & 2);
			if (span[0] > span[3] || (span[0] == span[3] && (span[1] > span[4] || (span[1] == span[4] && span[2] >= span[5]))))
			{
				num12++;
			}
			byte b2 = Etc2DistanceTable[num12];
			uint* ptr2 = stackalloc uint[4]
			{
				ApplicateColor(span, 0, b2),
				ApplicateColor(span, 0, -b2),
				ApplicateColor(span, 1, b2),
				ApplicateColor(span, 1, -b2)
			};
			int num13 = 0;
			while (num13 < 16)
			{
				int num14 = ((num2 << 1) & 2) | (num & 1);
				uint num15 = ptr2[num14];
				uint num16 = PunchthroughMaskTable[num14];
				int index2 = WriteOrderTable[num13];
				output[index2] = num15 & num16;
				num13++;
				num >>= 1;
				num2 >>= 1;
			}
			return;
		}
		int num17 = input[2] & 0xF8;
		int num18 = ((input[2] << 3) & 0x18) - ((input[2] << 3) & 0x20);
		if (num17 + num18 < 0 || num17 + num18 > 255)
		{
			span[0] = (byte)(((input[0] << 1) & 0xFC) | ((input[0] >> 5) & 3));
			span[1] = (byte)(((input[0] << 7) & 0x80) | (input[1] & 0x7E) | (input[0] & 1));
			span[2] = (byte)(((input[1] << 7) & 0x80) | ((input[2] << 2) & 0x60) | ((input[2] << 3) & 0x18) | ((input[3] >> 5) & 4));
			span[2] |= (byte)(span[2] >> 6);
			span[3] = (byte)(((input[3] << 1) & 0xF8) | ((input[3] << 2) & 4) | ((input[3] >> 5) & 3));
			span[4] = (byte)((input[4] & 0xFE) | (input[4] >> 7));
			span[5] = (byte)(((input[4] << 7) & 0x80) | ((input[5] >> 1) & 0x7C));
			span[5] |= (byte)(span[5] >> 6);
			span[6] = (byte)(((input[5] << 5) & 0xE0) | ((input[6] >> 3) & 0x1C) | ((input[5] >> 1) & 3));
			span[7] = (byte)(((input[6] << 3) & 0xF8) | ((input[7] >> 5) & 6) | ((input[6] >> 4) & 1));
			span[8] = (byte)((input[7] << 2) | ((input[7] >> 4) & 3));
			int i = 0;
			int num19 = 0;
			for (; i < 4; i++)
			{
				int num20 = 0;
				while (num20 < 4)
				{
					int r = Clamp255(num20 * (span[3] - span[0]) + i * (span[6] - span[0]) + 4 * span[0] + 2 >> 2);
					int g = Clamp255(num20 * (span[4] - span[1]) + i * (span[7] - span[1]) + 4 * span[1] + 2 >> 2);
					int b3 = Clamp255(num20 * (span[5] - span[2]) + i * (span[8] - span[2]) + 4 * span[2] + 2 >> 2);
					output[num19] = Color(r, g, b3, 255);
					num20++;
					num19++;
				}
			}
			return;
		}
		int* ptr3 = stackalloc int[2];
		*ptr3 = (input[3] >> 5) * 4;
		ptr3[1] = ((input[3] >> 2) & 7) * 4;
		int num21 = (input[3] & 1) * 16;
		span[0] = (byte)(num3 | (num3 >> 5));
		span[1] = (byte)(num10 | (num10 >> 5));
		span[2] = (byte)(num17 | (num17 >> 5));
		span[3] = (byte)(num3 + num4);
		span[4] = (byte)(num10 + num11);
		span[5] = (byte)(num17 + num18);
		span[3] |= (byte)(span[3] >> 5);
		span[4] |= (byte)(span[4] >> 5);
		span[5] |= (byte)(span[5] >> 5);
		int num22 = 0;
		while (num22 < 16)
		{
			int num23 = Etc1SubblockTable[num21 + num22];
			int num24 = ((num2 << 1) & 2) | (num & 1);
			int num25 = ptr3[num23];
			int m = PunchthroughModifierTable[num25 + num24];
			uint num26 = ApplicateColor(span, num23, m);
			uint num27 = PunchthroughMaskTable[num24];
			int index3 = WriteOrderTable[num22];
			output[index3] = num26 & num27;
			num22++;
			num >>= 1;
			num2 >>= 1;
		}
	}

	private static void DecodeEtc2a8Block(ReadOnlySpan<byte> input, Span<uint> output)
	{
		int @base = input[0];
		int num = input[1];
		int num2 = num >> 4;
		if (num2 == 0)
		{
			for (int i = 0; i < 16; i++)
			{
				DecodeEac11Block(MemoryMarshal.Cast<uint, byte>(output).Slice(3), @base);
			}
		}
		else
		{
			int ti = num & 0xF;
			ulong l = Get6SwapedBytes(input);
			DecodeEac11Block(MemoryMarshal.Cast<uint, byte>(output).Slice(3), @base, ti, num2, l);
		}
	}

	private static void DecodeEacUnsignedBlock(ReadOnlySpan<byte> input, Span<uint> output, int channel)
	{
		int @base = input[0];
		int num = input[1];
		int num2 = num >> 4;
		if (num2 == 0)
		{
			DecodeEac11Block(MemoryMarshal.Cast<uint, byte>(output).Slice(channel), @base);
			return;
		}
		int ti = num & 0xF;
		ulong l = Get6SwapedBytes(input);
		DecodeEac11Block(MemoryMarshal.Cast<uint, byte>(output).Slice(channel), @base, ti, num2, l);
	}

	private static void DecodeEacSignedBlock(ReadOnlySpan<byte> input, Span<uint> output, int channel)
	{
		int @base = 127 + (sbyte)input[0];
		int num = input[1];
		int num2 = num >> 4;
		if (num2 == 0)
		{
			DecodeEac11Block(MemoryMarshal.Cast<uint, byte>(output).Slice(channel), @base);
			return;
		}
		int ti = num & 0xF;
		ulong l = Get6SwapedBytes(input);
		DecodeEac11Block(MemoryMarshal.Cast<uint, byte>(output).Slice(channel), @base, ti, num2, l);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void DecodeEac11Block(Span<byte> output, int @base)
	{
		for (int i = 0; i < 16; i++)
		{
			output[WriteOrderTableRev[i] * 4] = (byte)@base;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static void DecodeEac11Block(Span<byte> output, int @base, int ti, int mul, ulong l)
	{
		fixed (sbyte* ptr = &Etc2AlphaModTable[ti * 8])
		{
			int num = 0;
			while (num < 16)
			{
				int n = @base + mul * ptr[l & 7];
				output[WriteOrderTableRev[num] * 4] = (byte)Clamp255(n);
				num++;
				l >>= 3;
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static uint Color(int r, int g, int b, int a)
	{
		return (uint)((r << 16) | (g << 8) | b | (a << 24));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int Clamp255(int n)
	{
		if (n >= 0)
		{
			if (n <= 255)
			{
				return n;
			}
			return 255;
		}
		return 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static uint ApplicateColor(ReadOnlySpan<byte> c, int o, int m)
	{
		return Color(Clamp255(c[o * 3] + m), Clamp255(c[o * 3 + 1] + m), Clamp255(c[o * 3 + 2] + m), 255);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static uint ApplicateColor(ReadOnlySpan<int> c, int o, int m)
	{
		return Color(Clamp255(c[o * 3] + m), Clamp255(c[o * 3 + 1] + m), Clamp255(c[o * 3 + 2] + m), 255);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static uint ApplicateColorRaw(ReadOnlySpan<byte> c, int o)
	{
		return Color(c[o * 3], c[o * 3 + 1], c[o * 3 + 2], 255);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ulong Get6SwapedBytes(ReadOnlySpan<byte> data)
	{
		return (uint)(data[7] | (data[6] << 8) | (data[5] << 16) | (data[4] << 24)) | ((ulong)data[3] << 32) | ((ulong)data[2] << 40);
	}
}
