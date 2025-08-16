using System;
using System.Runtime.CompilerServices;

namespace AssetRipper.TextureDecoder.Astc;

public static class AstcDecoder
{
	private struct BlockData
	{
		public int bw;

		public int bh;

		public int width;

		public int height;

		public int part_num;

		public int dual_plane;

		public int plane_selector;

		public int weight_range;

		public int weight_num;

		public unsafe fixed int cem[4];

		public int cem_range;

		public int endpoint_value_num;

		public unsafe fixed int endpoints[32];

		public unsafe fixed int weights[288];

		public unsafe fixed int partition[144];
	}

	private struct IntSeqData
	{
		public int bits;

		public int nonbits;
	}

	private static readonly byte[] BitReverseTable = new byte[256]
	{
		0, 128, 64, 192, 32, 160, 96, 224, 16, 144,
		80, 208, 48, 176, 112, 240, 8, 136, 72, 200,
		40, 168, 104, 232, 24, 152, 88, 216, 56, 184,
		120, 248, 4, 132, 68, 196, 36, 164, 100, 228,
		20, 148, 84, 212, 52, 180, 116, 244, 12, 140,
		76, 204, 44, 172, 108, 236, 28, 156, 92, 220,
		60, 188, 124, 252, 2, 130, 66, 194, 34, 162,
		98, 226, 18, 146, 82, 210, 50, 178, 114, 242,
		10, 138, 74, 202, 42, 170, 106, 234, 26, 154,
		90, 218, 58, 186, 122, 250, 6, 134, 70, 198,
		38, 166, 102, 230, 22, 150, 86, 214, 54, 182,
		118, 246, 14, 142, 78, 206, 46, 174, 110, 238,
		30, 158, 94, 222, 62, 190, 126, 254, 1, 129,
		65, 193, 33, 161, 97, 225, 17, 145, 81, 209,
		49, 177, 113, 241, 9, 137, 73, 201, 41, 169,
		105, 233, 25, 153, 89, 217, 57, 185, 121, 249,
		5, 133, 69, 197, 37, 165, 101, 229, 21, 149,
		85, 213, 53, 181, 117, 245, 13, 141, 77, 205,
		45, 173, 109, 237, 29, 157, 93, 221, 61, 189,
		125, 253, 3, 131, 67, 195, 35, 163, 99, 227,
		19, 147, 83, 211, 51, 179, 115, 243, 11, 139,
		75, 203, 43, 171, 107, 235, 27, 155, 91, 219,
		59, 187, 123, 251, 7, 135, 71, 199, 39, 167,
		103, 231, 23, 151, 87, 215, 55, 183, 119, 247,
		15, 143, 79, 207, 47, 175, 111, 239, 31, 159,
		95, 223, 63, 191, 127, 255
	};

	private static readonly int[] WeightPrecTableA = new int[16]
	{
		0, 0, 0, 3, 0, 5, 3, 0, 0, 0,
		5, 3, 0, 5, 3, 0
	};

	private static readonly int[] WeightPrecTableB = new int[16]
	{
		0, 0, 1, 0, 2, 0, 1, 3, 0, 0,
		1, 2, 4, 2, 3, 5
	};

	private static readonly int[] CemTableA = new int[19]
	{
		0, 3, 5, 0, 3, 5, 0, 3, 5, 0,
		3, 5, 0, 3, 5, 0, 3, 0, 0
	};

	private static readonly int[] CemTableB = new int[19]
	{
		8, 6, 5, 7, 5, 4, 6, 4, 3, 5,
		3, 2, 4, 2, 1, 3, 1, 2, 1
	};

	private static readonly int[] DImt = new int[5] { 0, 2, 4, 5, 7 };

	private static readonly int[] DImq = new int[3] { 0, 3, 5 };

	private static readonly int[] DITritsTable = new int[1280]
	{
		0, 1, 2, 0, 0, 1, 2, 1, 0, 1,
		2, 2, 0, 1, 2, 2, 0, 1, 2, 0,
		0, 1, 2, 1, 0, 1, 2, 2, 0, 1,
		2, 0, 0, 1, 2, 0, 0, 1, 2, 1,
		0, 1, 2, 2, 0, 1, 2, 2, 0, 1,
		2, 0, 0, 1, 2, 1, 0, 1, 2, 2,
		0, 1, 2, 1, 0, 1, 2, 0, 0, 1,
		2, 1, 0, 1, 2, 2, 0, 1, 2, 2,
		0, 1, 2, 0, 0, 1, 2, 1, 0, 1,
		2, 2, 0, 1, 2, 2, 0, 1, 2, 0,
		0, 1, 2, 1, 0, 1, 2, 2, 0, 1,
		2, 2, 0, 1, 2, 0, 0, 1, 2, 1,
		0, 1, 2, 2, 0, 1, 2, 2, 0, 1,
		2, 0, 0, 1, 2, 1, 0, 1, 2, 2,
		0, 1, 2, 2, 0, 1, 2, 0, 0, 1,
		2, 1, 0, 1, 2, 2, 0, 1, 2, 0,
		0, 1, 2, 0, 0, 1, 2, 1, 0, 1,
		2, 2, 0, 1, 2, 2, 0, 1, 2, 0,
		0, 1, 2, 1, 0, 1, 2, 2, 0, 1,
		2, 1, 0, 1, 2, 0, 0, 1, 2, 1,
		0, 1, 2, 2, 0, 1, 2, 2, 0, 1,
		2, 0, 0, 1, 2, 1, 0, 1, 2, 2,
		0, 1, 2, 2, 0, 1, 2, 0, 0, 1,
		2, 1, 0, 1, 2, 2, 0, 1, 2, 2,
		0, 1, 2, 0, 0, 1, 2, 1, 0, 1,
		2, 2, 0, 1, 2, 2, 0, 0, 0, 0,
		1, 1, 1, 0, 2, 2, 2, 0, 2, 2,
		2, 0, 0, 0, 0, 1, 1, 1, 1, 1,
		2, 2, 2, 1, 0, 0, 0, 0, 0, 0,
		0, 0, 1, 1, 1, 0, 2, 2, 2, 0,
		2, 2, 2, 0, 0, 0, 0, 1, 1, 1,
		1, 1, 2, 2, 2, 1, 1, 1, 1, 0,
		0, 0, 0, 0, 1, 1, 1, 0, 2, 2,
		2, 0, 2, 2, 2, 0, 0, 0, 0, 1,
		1, 1, 1, 1, 2, 2, 2, 1, 2, 2,
		2, 0, 0, 0, 0, 0, 1, 1, 1, 0,
		2, 2, 2, 0, 2, 2, 2, 0, 0, 0,
		0, 1, 1, 1, 1, 1, 2, 2, 2, 1,
		2, 2, 2, 0, 0, 0, 0, 0, 1, 1,
		1, 0, 2, 2, 2, 0, 2, 2, 2, 0,
		0, 0, 0, 1, 1, 1, 1, 1, 2, 2,
		2, 1, 0, 0, 0, 1, 0, 0, 0, 0,
		1, 1, 1, 0, 2, 2, 2, 0, 2, 2,
		2, 0, 0, 0, 0, 1, 1, 1, 1, 1,
		2, 2, 2, 1, 1, 1, 1, 1, 0, 0,
		0, 0, 1, 1, 1, 0, 2, 2, 2, 0,
		2, 2, 2, 0, 0, 0, 0, 1, 1, 1,
		1, 1, 2, 2, 2, 1, 2, 2, 2, 1,
		0, 0, 0, 0, 1, 1, 1, 0, 2, 2,
		2, 0, 2, 2, 2, 0, 0, 0, 0, 1,
		1, 1, 1, 1, 2, 2, 2, 1, 2, 2,
		2, 1, 0, 0, 0, 2, 0, 0, 0, 2,
		0, 0, 0, 2, 2, 2, 2, 2, 1, 1,
		1, 2, 1, 1, 1, 2, 1, 1, 1, 2,
		0, 0, 0, 2, 0, 0, 0, 2, 0, 0,
		0, 2, 0, 0, 0, 2, 2, 2, 2, 2,
		1, 1, 1, 2, 1, 1, 1, 2, 1, 1,
		1, 2, 0, 0, 0, 2, 0, 0, 0, 2,
		0, 0, 0, 2, 0, 0, 0, 2, 2, 2,
		2, 2, 1, 1, 1, 2, 1, 1, 1, 2,
		1, 1, 1, 2, 0, 0, 0, 2, 0, 0,
		0, 2, 0, 0, 0, 2, 0, 0, 0, 2,
		2, 2, 2, 2, 1, 1, 1, 2, 1, 1,
		1, 2, 1, 1, 1, 2, 2, 2, 2, 2,
		0, 0, 0, 2, 0, 0, 0, 2, 0, 0,
		0, 2, 2, 2, 2, 2, 1, 1, 1, 2,
		1, 1, 1, 2, 1, 1, 1, 2, 1, 1,
		1, 2, 0, 0, 0, 2, 0, 0, 0, 2,
		0, 0, 0, 2, 2, 2, 2, 2, 1, 1,
		1, 2, 1, 1, 1, 2, 1, 1, 1, 2,
		1, 1, 1, 2, 0, 0, 0, 2, 0, 0,
		0, 2, 0, 0, 0, 2, 2, 2, 2, 2,
		1, 1, 1, 2, 1, 1, 1, 2, 1, 1,
		1, 2, 1, 1, 1, 2, 0, 0, 0, 2,
		0, 0, 0, 2, 0, 0, 0, 2, 2, 2,
		2, 2, 1, 1, 1, 2, 1, 1, 1, 2,
		1, 1, 1, 2, 2, 2, 2, 2, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 2, 2, 2, 2,
		1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
		1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
		1, 1, 1, 1, 1, 1, 1, 1, 2, 2,
		2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
		2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
		2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
		2, 2, 2, 2, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 2, 2, 2, 2, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 2, 2, 2, 2, 1, 1,
		1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
		1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
		1, 1, 1, 1, 1, 1, 2, 2, 2, 2,
		2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
		2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
		2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
		2, 2, 1, 1, 1, 1, 1, 1, 1, 1,
		1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
		1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
		2, 2, 2, 2, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 2, 2, 2, 2, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 2, 2, 2, 2, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 2, 2, 2, 2,
		2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
		2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
		2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
		2, 2, 1, 1, 1, 1, 1, 1, 1, 1,
		1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
		1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
		2, 2, 2, 2, 1, 1, 1, 1, 1, 1,
		1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
		1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
		1, 1, 2, 2, 2, 2, 1, 1, 1, 1,
		1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
		1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
		1, 1, 1, 1, 2, 2, 2, 2, 2, 2,
		2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
		2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
		2, 2, 2, 2, 2, 2, 2, 2, 2, 2
	};

	private static readonly int[] DIQuintsTable = new int[384]
	{
		0, 1, 2, 3, 4, 0, 4, 4, 0, 1,
		2, 3, 4, 1, 4, 4, 0, 1, 2, 3,
		4, 2, 4, 4, 0, 1, 2, 3, 4, 3,
		4, 4, 0, 1, 2, 3, 4, 0, 4, 0,
		0, 1, 2, 3, 4, 1, 4, 1, 0, 1,
		2, 3, 4, 2, 4, 2, 0, 1, 2, 3,
		4, 3, 4, 3, 0, 1, 2, 3, 4, 0,
		2, 3, 0, 1, 2, 3, 4, 1, 2, 3,
		0, 1, 2, 3, 4, 2, 2, 3, 0, 1,
		2, 3, 4, 3, 2, 3, 0, 1, 2, 3,
		4, 0, 0, 1, 0, 1, 2, 3, 4, 1,
		0, 1, 0, 1, 2, 3, 4, 2, 0, 1,
		0, 1, 2, 3, 4, 3, 0, 1, 0, 0,
		0, 0, 0, 4, 4, 4, 1, 1, 1, 1,
		1, 4, 4, 4, 2, 2, 2, 2, 2, 4,
		4, 4, 3, 3, 3, 3, 3, 4, 4, 4,
		0, 0, 0, 0, 0, 4, 0, 4, 1, 1,
		1, 1, 1, 4, 1, 4, 2, 2, 2, 2,
		2, 4, 2, 4, 3, 3, 3, 3, 3, 4,
		3, 4, 0, 0, 0, 0, 0, 4, 0, 0,
		1, 1, 1, 1, 1, 4, 1, 1, 2, 2,
		2, 2, 2, 4, 2, 2, 3, 3, 3, 3,
		3, 4, 3, 3, 0, 0, 0, 0, 0, 4,
		0, 0, 1, 1, 1, 1, 1, 4, 1, 1,
		2, 2, 2, 2, 2, 4, 2, 2, 3, 3,
		3, 3, 3, 4, 3, 3, 0, 0, 0, 0,
		0, 0, 0, 4, 0, 0, 0, 0, 0, 0,
		1, 4, 0, 0, 0, 0, 0, 0, 2, 4,
		0, 0, 0, 0, 0, 0, 3, 4, 1, 1,
		1, 1, 1, 1, 4, 4, 1, 1, 1, 1,
		1, 1, 4, 4, 1, 1, 1, 1, 1, 1,
		4, 4, 1, 1, 1, 1, 1, 1, 4, 4,
		2, 2, 2, 2, 2, 2, 4, 4, 2, 2,
		2, 2, 2, 2, 4, 4, 2, 2, 2, 2,
		2, 2, 4, 4, 2, 2, 2, 2, 2, 2,
		4, 4, 3, 3, 3, 3, 3, 3, 4, 4,
		3, 3, 3, 3, 3, 3, 4, 4, 3, 3,
		3, 3, 3, 3, 4, 4, 3, 3, 3, 3,
		3, 3, 4, 4
	};

	private static readonly int[] DETritsTable = new int[7] { 0, 204, 93, 44, 22, 11, 5 };

	private static readonly int[] DEQuintsTable = new int[6] { 0, 113, 54, 26, 13, 6 };

	public static int DecodeASTC(ReadOnlySpan<byte> input, int width, int height, int blockWidth, int blockHeight, out byte[] output)
	{
		output = new byte[width * height * 4];
		return DecodeASTC(input, width, height, blockWidth, blockHeight, output);
	}

	public unsafe static int DecodeASTC(ReadOnlySpan<byte> input, int width, int height, int blockWidth, int blockHeight, Span<byte> output)
	{
		fixed (byte* input2 = input)
		{
			fixed (byte* output2 = output)
			{
				return DecodeASTC(input2, width, height, blockWidth, blockHeight, output2);
			}
		}
	}

	private unsafe static int DecodeASTC(byte* input, int width, int height, int blockWidth, int blockHeight, byte* output)
	{
		int num = (width + blockWidth - 1) / blockWidth;
		int num2 = (height + blockHeight - 1) / blockHeight;
		int num3 = (width + blockWidth - 1) % blockWidth + 1;
		uint* ptr = stackalloc uint[blockWidth * blockHeight];
		int num4 = 0;
		for (int i = 0; i < num2; i++)
		{
			int num5 = 0;
			while (num5 < num)
			{
				DecodeBlock(input + num4, blockWidth, blockHeight, ptr);
				int num6 = ((num5 < num - 1) ? blockWidth : num3);
				uint* ptr2 = (uint*)(output + (i * blockHeight * 4 * width + num5 * 4 * blockWidth));
				uint* ptr3 = ptr;
				int num7 = 0;
				int num8 = i * blockHeight;
				while (num7 < blockHeight && num8 < height)
				{
					for (int j = 0; j < num6; j++)
					{
						ptr2[j] = ptr3[j];
					}
					ptr2 += width;
					ptr3 += blockWidth;
					num7++;
					num8++;
				}
				num5++;
				num4 += 16;
			}
		}
		return num4;
	}

	private unsafe static void DecodeBlock(byte* input, int blockWidth, int blockHeight, uint* output)
	{
		if (*input == 252 && (input[1] & 1) == 1)
		{
			uint num = Color(input[9], input[11], input[13], input[15]);
			for (int i = 0; i < blockWidth * blockHeight; i++)
			{
				output[i] = num;
			}
			return;
		}
		BlockData blockData = default(BlockData);
		blockData.bw = blockWidth;
		blockData.bh = blockHeight;
		BlockData* ptr = &blockData;
		DecodeBlockParameters(input, ptr);
		DecodeEndpoints(input, ptr);
		DecodeWeights(input, ptr);
		if (blockData.part_num > 1)
		{
			SelectPartition(input, ptr);
		}
		ApplicateColor(ptr, output);
	}

	private unsafe static void DecodeBlockParameters(byte* input, BlockData* pBlock)
	{
		pBlock->dual_plane = (input[1] & 4) >> 2;
		pBlock->weight_range = ((*input >> 4) & 1) | ((input[1] << 2) & 8);
		if ((*input & 3) != 0)
		{
			pBlock->weight_range |= (*input << 1) & 6;
			switch (*input & 0xC)
			{
			case 0:
				pBlock->width = ((*(int*)input >> 7) & 3) + 4;
				pBlock->height = ((*input >> 5) & 3) + 2;
				break;
			case 4:
				pBlock->width = ((*(int*)input >> 7) & 3) + 8;
				pBlock->height = ((*input >> 5) & 3) + 2;
				break;
			case 8:
				pBlock->width = ((*input >> 5) & 3) + 2;
				pBlock->height = ((*(int*)input >> 7) & 3) + 8;
				break;
			case 12:
				if ((input[1] & 1) != 0)
				{
					pBlock->width = ((*input >> 7) & 1) + 2;
					pBlock->height = ((*input >> 5) & 3) + 2;
				}
				else
				{
					pBlock->width = ((*input >> 5) & 3) + 2;
					pBlock->height = ((*input >> 7) & 1) + 6;
				}
				break;
			}
		}
		else
		{
			pBlock->weight_range |= (*input >> 1) & 6;
			switch (*(int*)input & 0x180)
			{
			case 0:
				pBlock->width = 12;
				pBlock->height = ((*input >> 5) & 3) + 2;
				break;
			case 128:
				pBlock->width = ((*input >> 5) & 3) + 2;
				pBlock->height = 12;
				break;
			case 256:
				pBlock->width = ((*input >> 5) & 3) + 6;
				pBlock->height = ((input[1] >> 1) & 3) + 6;
				pBlock->dual_plane = 0;
				pBlock->weight_range &= 7;
				break;
			case 384:
				pBlock->width = (((*input & 0x20) != 0) ? 10 : 6);
				pBlock->height = (((*input & 0x20) != 0) ? 6 : 10);
				break;
			}
		}
		pBlock->part_num = ((input[1] >> 3) & 3) + 1;
		pBlock->weight_num = pBlock->width * pBlock->height;
		if (pBlock->dual_plane != 0)
		{
			pBlock->weight_num *= 2;
		}
		int num = 0;
		int num2 = WeightPrecTableA[pBlock->weight_range] switch
		{
			3 => pBlock->weight_num * WeightPrecTableB[pBlock->weight_range] + (pBlock->weight_num * 8 + 4) / 5, 
			5 => pBlock->weight_num * WeightPrecTableB[pBlock->weight_range] + (pBlock->weight_num * 7 + 2) / 3, 
			_ => pBlock->weight_num * WeightPrecTableB[pBlock->weight_range], 
		};
		int num3;
		if (pBlock->part_num == 1)
		{
			int* cem = pBlock->cem;
			*cem = (*(int*)(input + 1) >> 5) & 0xF;
			num3 = 17;
		}
		else
		{
			num = (*(int*)(input + 2) >> 7) & 3;
			if (num == 0)
			{
				int num4 = (input[3] >> 1) & 0xF;
				for (int i = 0; i < pBlock->part_num; i++)
				{
					pBlock->cem[i] = num4;
				}
				num3 = 29;
			}
			else
			{
				for (int j = 0; j < pBlock->part_num; j++)
				{
					pBlock->cem[j] = ((input[3] >> j + 1) & 1) + num - 1 << 2;
				}
				switch (pBlock->part_num)
				{
				case 2:
					*pBlock->cem |= (input[3] >> 3) & 3;
					pBlock->cem[1] |= GetBits(input, 126 - num2, 2);
					break;
				case 3:
					*pBlock->cem |= (input[3] >> 4) & 1;
					*pBlock->cem |= GetBits(input, 122 - num2, 2) & 2;
					pBlock->cem[1] |= GetBits(input, 124 - num2, 2);
					pBlock->cem[2] |= GetBits(input, 126 - num2, 2);
					break;
				case 4:
				{
					for (int k = 0; k < 4; k++)
					{
						pBlock->cem[k] |= GetBits(input, 120 + k * 2 - num2, 2);
					}
					break;
				}
				}
				num3 = 25 + pBlock->part_num * 3;
			}
		}
		if (pBlock->dual_plane != 0)
		{
			num3 += 2;
			pBlock->plane_selector = GetBits(input, (num != 0) ? (130 - num2 - pBlock->part_num * 3) : (126 - num2), 2);
		}
		int num5 = 128 - num3 - num2;
		pBlock->endpoint_value_num = 0;
		for (int l = 0; l < pBlock->part_num; l++)
		{
			pBlock->endpoint_value_num += ((pBlock->cem[l] >> 1) & 6) + 2;
		}
		for (int m = 0; m < CemTableA.Length; m++)
		{
			if (CemTableA[m] switch
			{
				3 => pBlock->endpoint_value_num * CemTableB[m] + (pBlock->endpoint_value_num * 8 + 4) / 5, 
				5 => pBlock->endpoint_value_num * CemTableB[m] + (pBlock->endpoint_value_num * 7 + 2) / 3, 
				_ => pBlock->endpoint_value_num * CemTableB[m], 
			} <= num5)
			{
				pBlock->cem_range = m;
				break;
			}
		}
	}

	private unsafe static void DecodeEndpoints(byte* input, BlockData* pBlock)
	{
		IntSeqData* ptr = stackalloc IntSeqData[32];
		DecodeIntseq(input, (pBlock->part_num == 1) ? 17 : 29, CemTableA[pBlock->cem_range], CemTableB[pBlock->cem_range], pBlock->endpoint_value_num, reverse: false, ptr);
		int* ptr2 = stackalloc int[32];
		switch (CemTableA[pBlock->cem_range])
		{
		case 3:
		{
			int num8 = 0;
			int num9 = 0;
			int num10 = DETritsTable[CemTableB[pBlock->cem_range]];
			for (; num8 < pBlock->endpoint_value_num; num8++)
			{
				int num11 = (ptr[num8].bits & 1) * 511;
				int num12 = ptr[num8].bits >> 1;
				switch (CemTableB[pBlock->cem_range])
				{
				case 1:
					num9 = 0;
					break;
				case 2:
					num9 = 278 * num12;
					break;
				case 3:
					num9 = (num12 << 7) | (num12 << 2) | num12;
					break;
				case 4:
					num9 = (num12 << 6) | num12;
					break;
				case 5:
					num9 = (num12 << 5) | (num12 >> 2);
					break;
				case 6:
					num9 = (num12 << 4) | (num12 >> 4);
					break;
				}
				ptr2[num8] = (num11 & 0x80) | (((ptr[num8].nonbits * num10 + num9) ^ num11) >> 2);
			}
			break;
		}
		case 5:
		{
			int num3 = 0;
			int num4 = 0;
			int num5 = DEQuintsTable[CemTableB[pBlock->cem_range]];
			for (; num3 < pBlock->endpoint_value_num; num3++)
			{
				int num6 = (ptr[num3].bits & 1) * 511;
				int num7 = ptr[num3].bits >> 1;
				switch (CemTableB[pBlock->cem_range])
				{
				case 1:
					num4 = 0;
					break;
				case 2:
					num4 = 268 * num7;
					break;
				case 3:
					num4 = (num7 << 7) | (num7 << 1) | (num7 >> 1);
					break;
				case 4:
					num4 = (num7 << 6) | (num7 >> 1);
					break;
				case 5:
					num4 = (num7 << 5) | (num7 >> 3);
					break;
				}
				ptr2[num3] = (num6 & 0x80) | (((ptr[num3].nonbits * num5 + num4) ^ num6) >> 2);
			}
			break;
		}
		default:
			switch (CemTableB[pBlock->cem_range])
			{
			case 1:
			{
				for (int num2 = 0; num2 < pBlock->endpoint_value_num; num2++)
				{
					ptr2[num2] = ptr[num2].bits * 255;
				}
				break;
			}
			case 2:
			{
				for (int l = 0; l < pBlock->endpoint_value_num; l++)
				{
					ptr2[l] = ptr[l].bits * 85;
				}
				break;
			}
			case 3:
			{
				for (int n = 0; n < pBlock->endpoint_value_num; n++)
				{
					ptr2[n] = (ptr[n].bits << 5) | (ptr[n].bits << 2) | (ptr[n].bits >> 1);
				}
				break;
			}
			case 4:
			{
				for (int j = 0; j < pBlock->endpoint_value_num; j++)
				{
					ptr2[j] = (ptr[j].bits << 4) | ptr[j].bits;
				}
				break;
			}
			case 5:
			{
				for (int num = 0; num < pBlock->endpoint_value_num; num++)
				{
					ptr2[num] = (ptr[num].bits << 3) | (ptr[num].bits >> 2);
				}
				break;
			}
			case 6:
			{
				for (int m = 0; m < pBlock->endpoint_value_num; m++)
				{
					ptr2[m] = (ptr[m].bits << 2) | (ptr[m].bits >> 4);
				}
				break;
			}
			case 7:
			{
				for (int k = 0; k < pBlock->endpoint_value_num; k++)
				{
					ptr2[k] = (ptr[k].bits << 1) | (ptr[k].bits >> 6);
				}
				break;
			}
			case 8:
			{
				for (int i = 0; i < pBlock->endpoint_value_num; i++)
				{
					ptr2[i] = ptr[i].bits;
				}
				break;
			}
			}
			break;
		}
		int* ptr3 = ptr2;
		int num13 = 0;
		int num14 = 0;
		while (num13 < pBlock->part_num)
		{
			switch (pBlock->cem[num13])
			{
			case 0:
				SetEndpoint(pBlock->endpoints + num14, *ptr3, *ptr3, *ptr3, 255, ptr3[1], ptr3[1], ptr3[1], 255);
				break;
			case 1:
			{
				int num15 = (*ptr3 >> 2) | (ptr3[1] & 0xC0);
				int num16 = Clamp(num15 + (ptr3[1] & 0x3F));
				SetEndpoint(pBlock->endpoints + num14, num15, num15, num15, 255, num16, num16, num16, 255);
				break;
			}
			case 4:
				SetEndpoint(pBlock->endpoints + num14, *ptr3, *ptr3, *ptr3, ptr3[2], ptr3[1], ptr3[1], ptr3[1], ptr3[3]);
				break;
			case 5:
				BitTransferSigned(ptr3 + 1, ptr3);
				BitTransferSigned(ptr3 + 3, ptr3 + 2);
				ptr3[1] += *ptr3;
				SetEndpointClamp(pBlock->endpoints + num14, *ptr3, *ptr3, *ptr3, ptr3[2], ptr3[1], ptr3[1], ptr3[1], ptr3[2] + ptr3[3]);
				break;
			case 6:
				SetEndpoint(pBlock->endpoints + num14, *ptr3 * ptr3[3] >> 8, ptr3[1] * ptr3[3] >> 8, ptr3[2] * ptr3[3] >> 8, 255, *ptr3, ptr3[1], ptr3[2], 255);
				break;
			case 8:
				if (*ptr3 + ptr3[2] + ptr3[4] <= ptr3[1] + ptr3[3] + ptr3[5])
				{
					SetEndpoint(pBlock->endpoints + num14, *ptr3, ptr3[2], ptr3[4], 255, ptr3[1], ptr3[3], ptr3[5], 255);
				}
				else
				{
					SetEndpointBlue(pBlock->endpoints + num14, ptr3[1], ptr3[3], ptr3[5], 255, *ptr3, ptr3[2], ptr3[4], 255);
				}
				break;
			case 9:
				BitTransferSigned(ptr3 + 1, ptr3);
				BitTransferSigned(ptr3 + 3, ptr3 + 2);
				BitTransferSigned(ptr3 + 5, ptr3 + 4);
				if (ptr3[1] + ptr3[3] + ptr3[5] >= 0)
				{
					SetEndpointClamp(pBlock->endpoints + num14, *ptr3, ptr3[2], ptr3[4], 255, *ptr3 + ptr3[1], ptr3[2] + ptr3[3], ptr3[4] + ptr3[5], 255);
				}
				else
				{
					SetEndpointBlueClamp(pBlock->endpoints + num14, *ptr3 + ptr3[1], ptr3[2] + ptr3[3], ptr3[4] + ptr3[5], 255, *ptr3, ptr3[2], ptr3[4], 255);
				}
				break;
			case 10:
				SetEndpoint(pBlock->endpoints + num14, *ptr3 * ptr3[3] >> 8, ptr3[1] * ptr3[3] >> 8, ptr3[2] * ptr3[3] >> 8, ptr3[4], *ptr3, ptr3[1], ptr3[2], ptr3[5]);
				break;
			case 12:
				if (*ptr3 + ptr3[2] + ptr3[4] <= ptr3[1] + ptr3[3] + ptr3[5])
				{
					SetEndpoint(pBlock->endpoints + num14, *ptr3, ptr3[2], ptr3[4], ptr3[6], ptr3[1], ptr3[3], ptr3[5], ptr3[7]);
				}
				else
				{
					SetEndpointBlue(pBlock->endpoints + num14, ptr3[1], ptr3[3], ptr3[5], ptr3[7], *ptr3, ptr3[2], ptr3[4], ptr3[6]);
				}
				break;
			case 13:
				BitTransferSigned(ptr3 + 1, ptr3);
				BitTransferSigned(ptr3 + 3, ptr3 + 2);
				BitTransferSigned(ptr3 + 5, ptr3 + 4);
				BitTransferSigned(ptr3 + 7, ptr3 + 6);
				if (ptr3[1] + ptr3[3] + ptr3[5] >= 0)
				{
					SetEndpointClamp(pBlock->endpoints + num14, *ptr3, ptr3[2], ptr3[4], ptr3[6], *ptr3 + ptr3[1], ptr3[2] + ptr3[3], ptr3[4] + ptr3[5], ptr3[6] + ptr3[7]);
				}
				else
				{
					SetEndpointBlueClamp(pBlock->endpoints + num14, *ptr3 + ptr3[1], ptr3[2] + ptr3[3], ptr3[4] + ptr3[5], ptr3[6] + ptr3[7], *ptr3, ptr3[2], ptr3[4], ptr3[6]);
				}
				break;
			}
			ptr3 += (pBlock->cem[num13] / 4 + 1) * 2;
			num13++;
			num14 += 8;
		}
	}

	private unsafe static void DecodeWeights(byte* input, BlockData* block)
	{
		IntSeqData* ptr = stackalloc IntSeqData[128];
		DecodeIntseq(input, 128, WeightPrecTableA[block->weight_range], WeightPrecTableB[block->weight_range], block->weight_num, reverse: true, ptr);
		int* ptr2 = stackalloc int[128];
		if (WeightPrecTableA[block->weight_range] == 0)
		{
			switch (WeightPrecTableB[block->weight_range])
			{
			case 1:
			{
				for (int j = 0; j < block->weight_num; j++)
				{
					ptr2[j] = ((ptr[j].bits != 0) ? 63 : 0);
				}
				break;
			}
			case 2:
			{
				for (int l = 0; l < block->weight_num; l++)
				{
					ptr2[l] = (ptr[l].bits << 4) | (ptr[l].bits << 2) | ptr[l].bits;
				}
				break;
			}
			case 3:
			{
				for (int m = 0; m < block->weight_num; m++)
				{
					ptr2[m] = (ptr[m].bits << 3) | ptr[m].bits;
				}
				break;
			}
			case 4:
			{
				for (int k = 0; k < block->weight_num; k++)
				{
					ptr2[k] = (ptr[k].bits << 2) | (ptr[k].bits >> 2);
				}
				break;
			}
			case 5:
			{
				for (int i = 0; i < block->weight_num; i++)
				{
					ptr2[i] = (ptr[i].bits << 1) | (ptr[i].bits >> 4);
				}
				break;
			}
			}
			for (int n = 0; n < block->weight_num; n++)
			{
				if (ptr2[n] > 32)
				{
					ptr2[n]++;
				}
			}
		}
		else if (WeightPrecTableB[block->weight_range] == 0)
		{
			int num = ((WeightPrecTableA[block->weight_range] == 3) ? 32 : 16);
			for (int num2 = 0; num2 < block->weight_num; num2++)
			{
				ptr2[num2] = ptr[num2].nonbits * num;
			}
		}
		else
		{
			if (WeightPrecTableA[block->weight_range] == 3)
			{
				switch (WeightPrecTableB[block->weight_range])
				{
				case 1:
				{
					for (int num4 = 0; num4 < block->weight_num; num4++)
					{
						ptr2[num4] = ptr[num4].nonbits * 50;
					}
					break;
				}
				case 2:
				{
					for (int num5 = 0; num5 < block->weight_num; num5++)
					{
						ptr2[num5] = ptr[num5].nonbits * 23;
						if ((ptr[num5].bits & 2) != 0)
						{
							ptr2[num5] += 69;
						}
					}
					break;
				}
				case 3:
				{
					for (int num3 = 0; num3 < block->weight_num; num3++)
					{
						ptr2[num3] = ptr[num3].nonbits * 11 + (((ptr[num3].bits << 4) | (ptr[num3].bits >> 1)) & 0x63);
					}
					break;
				}
				}
			}
			else if (WeightPrecTableA[block->weight_range] == 5)
			{
				switch (WeightPrecTableB[block->weight_range])
				{
				case 1:
				{
					for (int num7 = 0; num7 < block->weight_num; num7++)
					{
						ptr2[num7] = ptr[num7].nonbits * 28;
					}
					break;
				}
				case 2:
				{
					for (int num6 = 0; num6 < block->weight_num; num6++)
					{
						ptr2[num6] = ptr[num6].nonbits * 13;
						if ((ptr[num6].bits & 2) != 0)
						{
							ptr2[num6] += 66;
						}
					}
					break;
				}
				}
			}
			for (int num8 = 0; num8 < block->weight_num; num8++)
			{
				int num9 = (ptr[num8].bits & 1) * 127;
				ptr2[num8] = (num9 & 0x20) | ((ptr2[num8] ^ num9) >> 2);
				if (ptr2[num8] > 32)
				{
					ptr2[num8]++;
				}
			}
		}
		int num10 = (1024 + block->bw / 2) / (block->bw - 1);
		int num11 = (1024 + block->bh / 2) / (block->bh - 1);
		int num12 = ((block->dual_plane == 0) ? 1 : 2);
		int num13 = 0;
		int num14 = 0;
		for (; num13 < block->bh; num13++)
		{
			int num15 = 0;
			while (num15 < block->bw)
			{
				int num16 = num10 * num15 * (block->width - 1) + 32 >> 6;
				int num17 = num11 * num13 * (block->height - 1) + 32 >> 6;
				int num18 = num16 & 0xF;
				int num19 = num17 & 0xF;
				int num20 = (num16 >> 4) + (num17 >> 4) * block->width;
				int num21 = num18 * num19 + 8 >> 4;
				int num22 = num19 - num21;
				int num23 = num18 - num21;
				int num24 = 16 - num18 - num19 + num21;
				for (int num25 = 0; num25 < num12; num25++)
				{
					int num26 = ptr2[num20 * num12 + num25];
					int num27 = ptr2[(num20 + 1) * num12 + num25];
					int num28 = ptr2[(num20 + block->width) * num12 + num25];
					int num29 = ptr2[(num20 + block->width + 1) * num12 + num25];
					block->weights[num14 * 2 + num25] = num26 * num24 + num27 * num23 + num28 * num22 + num29 * num21 + 8 >> 4;
				}
				num15++;
				num14++;
			}
		}
	}

	private unsafe static void SelectPartition(byte* input, BlockData* block)
	{
		bool flag = block->bw * block->bh < 31;
		int num = ((*(int*)input >> 13) & 0x3FF) | (block->part_num - 1 << 10);
		uint num2 = (uint)num;
		num2 ^= num2 >> 15;
		num2 -= num2 << 17;
		num2 += num2 << 7;
		num2 += num2 << 4;
		num2 ^= num2 >> 5;
		num2 += num2 << 16;
		num2 ^= num2 >> 7;
		num2 ^= num2 >> 3;
		num2 ^= num2 << 6;
		num2 ^= num2 >> 17;
		int* ptr = stackalloc int[8];
		for (int i = 0; i < 8; i++)
		{
			ptr[i] = (int)((num2 >> i * 4) & 0xF);
			ptr[i] *= ptr[i];
		}
		int* ptr2 = stackalloc int[2];
		*ptr2 = (((num & 2) != 0) ? 4 : 5);
		ptr2[1] = ((block->part_num == 3) ? 6 : 5);
		if ((num & 1) != 0)
		{
			for (int j = 0; j < 8; j++)
			{
				ptr[j] >>= ptr2[j % 2];
			}
		}
		else
		{
			for (int k = 0; k < 8; k++)
			{
				ptr[k] >>= ptr2[1 - k % 2];
			}
		}
		if (flag)
		{
			int l = 0;
			int num3 = 0;
			for (; l < block->bh; l++)
			{
				int num4 = 0;
				while (num4 < block->bw)
				{
					int num5 = num4 << 1;
					int num6 = l << 1;
					int num7 = (int)((*ptr * num5 + ptr[1] * num6 + (num2 >> 14)) & 0x3F);
					int num8 = (int)((ptr[2] * num5 + ptr[3] * num6 + (num2 >> 10)) & 0x3F);
					int num9 = (int)((block->part_num < 3) ? 0 : ((ptr[4] * num5 + ptr[5] * num6 + (num2 >> 6)) & 0x3F));
					int num10 = (int)((block->part_num < 4) ? 0 : ((ptr[6] * num5 + ptr[7] * num6 + (num2 >> 2)) & 0x3F));
					block->partition[num3] = ((num7 < num8 || num7 < num9 || num7 < num10) ? ((num8 >= num9 && num8 >= num10) ? 1 : ((num9 >= num10) ? 2 : 3)) : 0);
					num4++;
					num3++;
				}
			}
			return;
		}
		int m = 0;
		int num11 = 0;
		for (; m < block->bh; m++)
		{
			int num12 = 0;
			while (num12 < block->bw)
			{
				int num13 = (int)((*ptr * num12 + ptr[1] * m + (num2 >> 14)) & 0x3F);
				int num14 = (int)((ptr[2] * num12 + ptr[3] * m + (num2 >> 10)) & 0x3F);
				int num15 = (int)((block->part_num < 3) ? 0 : ((ptr[4] * num12 + ptr[5] * m + (num2 >> 6)) & 0x3F));
				int num16 = (int)((block->part_num < 4) ? 0 : ((ptr[6] * num12 + ptr[7] * m + (num2 >> 2)) & 0x3F));
				block->partition[num11] = ((num13 < num14 || num13 < num15 || num13 < num16) ? ((num14 >= num15 && num14 >= num16) ? 1 : ((num15 >= num16) ? 2 : 3)) : 0);
				num12++;
				num11++;
			}
		}
	}

	private unsafe static void ApplicateColor(BlockData* block, uint* output)
	{
		if (block->dual_plane != 0)
		{
			byte* intPtr = stackalloc byte[16];
			// IL initblk instruction
			Unsafe.InitBlock(intPtr, 0, 16);
			int* ptr = (int*)intPtr;
			ptr[block->plane_selector] = 1;
			if (block->part_num > 1)
			{
				for (int i = 0; i < block->bw * block->bh; i++)
				{
					int num = block->partition[i];
					byte r = SelectColor(block->endpoints[num * 8], block->endpoints[num * 8 + 4], block->weights[i * 2 + *ptr]);
					byte g = SelectColor(block->endpoints[num * 8 + 1], block->endpoints[num * 8 + 5], block->weights[i * 2 + ptr[1]]);
					byte b = SelectColor(block->endpoints[num * 8 + 2], block->endpoints[num * 8 + 6], block->weights[i * 2 + ptr[2]]);
					byte a = SelectColor(block->endpoints[num * 8 + 3], block->endpoints[num * 8 + 7], block->weights[i * 2 + ptr[3]]);
					output[i] = Color(r, g, b, a);
				}
			}
			else
			{
				for (int j = 0; j < block->bw * block->bh; j++)
				{
					byte r2 = SelectColor(*block->endpoints, block->endpoints[4], block->weights[j * 2 + *ptr]);
					byte g2 = SelectColor(block->endpoints[1], block->endpoints[5], block->weights[j * 2 + ptr[1]]);
					byte b2 = SelectColor(block->endpoints[2], block->endpoints[6], block->weights[j * 2 + ptr[2]]);
					byte a2 = SelectColor(block->endpoints[3], block->endpoints[7], block->weights[j * 2 + ptr[3]]);
					output[j] = Color(r2, g2, b2, a2);
				}
			}
		}
		else if (block->part_num > 1)
		{
			for (int k = 0; k < block->bw * block->bh; k++)
			{
				int num2 = block->partition[k];
				byte r3 = SelectColor(block->endpoints[num2 * 8], block->endpoints[num2 * 8 + 4], block->weights[k * 2]);
				byte g3 = SelectColor(block->endpoints[num2 * 8 + 1], block->endpoints[num2 * 8 + 5], block->weights[k * 2]);
				byte b3 = SelectColor(block->endpoints[num2 * 8 + 2], block->endpoints[num2 * 8 + 6], block->weights[k * 2]);
				byte a3 = SelectColor(block->endpoints[num2 * 8 + 3], block->endpoints[num2 * 8 + 7], block->weights[k * 2]);
				output[k] = Color(r3, g3, b3, a3);
			}
		}
		else
		{
			for (int l = 0; l < block->bw * block->bh; l++)
			{
				byte r4 = SelectColor(*block->endpoints, block->endpoints[4], block->weights[l * 2]);
				byte g4 = SelectColor(block->endpoints[1], block->endpoints[5], block->weights[l * 2]);
				byte b4 = SelectColor(block->endpoints[2], block->endpoints[6], block->weights[l * 2]);
				byte a4 = SelectColor(block->endpoints[3], block->endpoints[7], block->weights[l * 2]);
				output[l] = Color(r4, g4, b4, a4);
			}
		}
	}

	private unsafe static void DecodeIntseq(byte* input, int offset, int a, int b, int count, bool reverse, IntSeqData* _out)
	{
		if (count <= 0)
		{
			return;
		}
		int num = 0;
		switch (a)
		{
		case 3:
		{
			int num2 = (1 << b) - 1;
			int num3 = (count + 4) / 5;
			int num4 = (count + 4) % 5 + 1;
			int num5 = 8 + 5 * b;
			int num6 = (num5 * num4 + 4) / 5;
			if (reverse)
			{
				int num7 = 0;
				int num8 = offset;
				while (num7 < num3)
				{
					int num9 = ((num7 < num3 - 1) ? num5 : num6);
					ulong num10 = BitReverseU64(GetBits64(input, num8 - num9, num9), num9);
					int num11 = (int)(((num10 >> b) & 3) | ((num10 >> b * 2) & 0xC) | ((num10 >> b * 3) & 0x10) | ((num10 >> b * 4) & 0x60) | ((num10 >> b * 5) & 0x80));
					int num12 = 0;
					while (num12 < 5 && num < count)
					{
						_out[num] = new IntSeqData
						{
							bits = ((int)(num10 >> DImt[num12] + b * num12) & num2),
							nonbits = DITritsTable[num12 * 256 + num11]
						};
						num12++;
						num++;
					}
					num7++;
					num8 -= num5;
				}
				return;
			}
			int num13 = 0;
			int num14 = offset;
			while (num13 < num3)
			{
				ulong bits = GetBits64(input, num14, (num13 < num3 - 1) ? num5 : num6);
				int num15 = (int)(((bits >> b) & 3) | ((bits >> b * 2) & 0xC) | ((bits >> b * 3) & 0x10) | ((bits >> b * 4) & 0x60) | ((bits >> b * 5) & 0x80));
				int num16 = 0;
				while (num16 < 5 && num < count)
				{
					_out[num] = new IntSeqData
					{
						bits = ((int)(bits >> DImt[num16] + b * num16) & num2),
						nonbits = DITritsTable[num16 * 256 + num15]
					};
					num16++;
					num++;
				}
				num13++;
				num14 += num5;
			}
			return;
		}
		case 5:
		{
			int num17 = (1 << b) - 1;
			int num18 = (count + 2) / 3;
			int num19 = (count + 2) % 3 + 1;
			int num20 = 7 + 3 * b;
			int num21 = (num20 * num19 + 2) / 3;
			if (reverse)
			{
				int num22 = 0;
				int num23 = offset;
				while (num22 < num18)
				{
					int num24 = ((num22 < num18 - 1) ? num20 : num21);
					ulong num25 = BitReverseU64(GetBits64(input, num23 - num24, num24), num24);
					int num26 = (int)(((num25 >> b) & 7) | ((num25 >> b * 2) & 0x18) | ((num25 >> b * 3) & 0x60));
					int num27 = 0;
					while (num27 < 3 && num < count)
					{
						_out[num] = new IntSeqData
						{
							bits = (((int)num25 >> DImq[num27] + b * num27) & num17),
							nonbits = DIQuintsTable[num27 * 128 + num26]
						};
						num27++;
						num++;
					}
					num22++;
					num23 -= num20;
				}
				return;
			}
			int num28 = 0;
			int num29 = offset;
			while (num28 < num18)
			{
				ulong bits2 = GetBits64(input, num29, (num28 < num18 - 1) ? num20 : num21);
				int num30 = (int)(((bits2 >> b) & 7) | ((bits2 >> b * 2) & 0x18) | ((bits2 >> b * 3) & 0x60));
				int num31 = 0;
				while (num31 < 3 && num < count)
				{
					_out[num] = new IntSeqData
					{
						bits = (((int)bits2 >> DImq[num31] + b * num31) & num17),
						nonbits = DIQuintsTable[num31 * 128 + num30]
					};
					num31++;
					num++;
				}
				num28++;
				num29 += num20;
			}
			return;
		}
		}
		if (reverse)
		{
			int num32 = offset - b;
			while (num < count)
			{
				_out[num] = new IntSeqData
				{
					bits = BitReverseU8((byte)GetBits(input, num32, b), b),
					nonbits = 0
				};
				num++;
				num32 -= b;
			}
		}
		else
		{
			int num33 = offset;
			while (num < count)
			{
				_out[num] = new IntSeqData
				{
					bits = GetBits(input, num33, b),
					nonbits = 0
				};
				num++;
				num33 += b;
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static uint Color(uint r, uint g, uint b, uint a)
	{
		return (r << 16) | (g << 8) | b | (a << 24);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static byte BitReverseU8(byte c, int bits)
	{
		return (byte)(BitReverseTable[c] >> 8 - bits);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ulong BitReverseU64(ulong d, int bits)
	{
		return (((ulong)BitReverseTable[(uint)(d & 0xFF)] << 56) | ((ulong)BitReverseTable[(uint)((d >> 8) & 0xFF)] << 48) | ((ulong)BitReverseTable[(uint)((d >> 16) & 0xFF)] << 40) | ((ulong)BitReverseTable[(uint)((d >> 24) & 0xFF)] << 32) | ((ulong)BitReverseTable[(uint)((d >> 32) & 0xFF)] << 24) | ((ulong)BitReverseTable[(uint)((d >> 40) & 0xFF)] << 16) | ((ulong)BitReverseTable[(uint)((d >> 48) & 0xFF)] << 8) | BitReverseTable[(uint)((d >> 56) & 0xFF)]) >> 64 - bits;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static int GetBits(byte* input, int bit, int len)
	{
		return (*(int*)(input + bit / 8) >> bit % 8) & ((1 << len) - 1);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static ulong GetBits64(byte* input, int bit, int len)
	{
		ulong num = (ulong)((len == 64) ? (-1) : ((1L << len) - 1));
		if (len < 1)
		{
			return 0uL;
		}
		if (bit >= 64)
		{
			return (ulong)(*(long*)(input + 8) >>> bit - 64) & num;
		}
		if (bit <= 0)
		{
			return (ulong)(*(long*)input << -bit) & num;
		}
		if (bit + len <= 64)
		{
			return (ulong)(*(long*)input >>> bit) & num;
		}
		return (ulong)((*(long*)input >>> bit) | (*(long*)(input + 8) << 64 - bit)) & num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static byte Clamp(int n)
	{
		if (n >= 0)
		{
			if (n <= 255)
			{
				return (byte)n;
			}
			return byte.MaxValue;
		}
		return 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static void BitTransferSigned(int* a, int* b)
	{
		*b = (*b >> 1) | (*a & 0x80);
		*a = (*a >> 1) & 0x3F;
		if ((*a & 0x20) != 0)
		{
			*a -= 64;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static void SetEndpoint(int* endpoint, int r1, int g1, int b1, int a1, int r2, int g2, int b2, int a2)
	{
		*endpoint = r1;
		endpoint[1] = g1;
		endpoint[2] = b1;
		endpoint[3] = a1;
		endpoint[4] = r2;
		endpoint[5] = g2;
		endpoint[6] = b2;
		endpoint[7] = a2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static void SetEndpointClamp(int* endpoint, int r1, int g1, int b1, int a1, int r2, int g2, int b2, int a2)
	{
		*endpoint = Clamp(r1);
		endpoint[1] = Clamp(g1);
		endpoint[2] = Clamp(b1);
		endpoint[3] = Clamp(a1);
		endpoint[4] = Clamp(r2);
		endpoint[5] = Clamp(g2);
		endpoint[6] = Clamp(b2);
		endpoint[7] = Clamp(a2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static void SetEndpointBlue(int* endpoint, int r1, int g1, int b1, int a1, int r2, int g2, int b2, int a2)
	{
		*endpoint = r1 + b1 >> 1;
		endpoint[1] = g1 + b1 >> 1;
		endpoint[2] = b1;
		endpoint[3] = a1;
		endpoint[4] = r2 + b2 >> 1;
		endpoint[5] = g2 + b2 >> 1;
		endpoint[6] = b2;
		endpoint[7] = a2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static void SetEndpointBlueClamp(int* endpoint, int r1, int g1, int b1, int a1, int r2, int g2, int b2, int a2)
	{
		*endpoint = Clamp(r1 + b1 >> 1);
		endpoint[1] = Clamp(g1 + b1 >> 1);
		endpoint[2] = Clamp(b1);
		endpoint[3] = Clamp(a1);
		endpoint[4] = Clamp(r2 + b2 >> 1);
		endpoint[5] = Clamp(g2 + b2 >> 1);
		endpoint[6] = Clamp(b2);
		endpoint[7] = Clamp(a2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static byte SelectColor(int v0, int v1, int weight)
	{
		return (byte)(((((v0 << 8) | v0) * (64 - weight) + ((v1 << 8) | v1) * weight + 32 >> 6) * 255 + 32768) / 65536);
	}
}
