using System;
using System.Runtime.InteropServices;

namespace AssetRipper.TextureDecoder.Bc;

internal static class BcHelpers
{
	public static int Bc1CompressedSize(int w, int h)
	{
		return (w >> 2) * (h >> 2) * 8;
	}

	public static int Bc2CompressedSize(int w, int h)
	{
		return (w >> 2) * (h >> 2) * 16;
	}

	public static int Bc3CompressedSize(int w, int h)
	{
		return (w >> 2) * (h >> 2) * 16;
	}

	public static int Bc4CompressedSize(int w, int h)
	{
		return (w >> 2) * (h >> 2) * 8;
	}

	public static int Bc5CompressedSize(int w, int h)
	{
		return (w >> 2) * (h >> 2) * 16;
	}

	public static int Bc6hCompressedSize(int w, int h)
	{
		return (w >> 2) * (h >> 2) * 16;
	}

	public static int Bc7CompressedSize(int w, int h)
	{
		return (w >> 2) * (h >> 2) * 16;
	}

	public static void DecompressBc1(ReadOnlySpan<byte> compressedBlock, Span<byte> decompressedBlock, int destinationPitch)
	{
		ColorBlock(compressedBlock, decompressedBlock, destinationPitch, 0);
	}

	public static void DecompressBc2(ReadOnlySpan<byte> compressedBlock, Span<byte> decompressedBlock, int destinationPitch)
	{
		ColorBlock(compressedBlock.Slice(8), decompressedBlock, destinationPitch, 1);
		SharpAlphaBlock(compressedBlock, decompressedBlock.Slice(3), destinationPitch);
	}

	public static void DecompressBc3(ReadOnlySpan<byte> compressedBlock, Span<byte> decompressedBlock, int destinationPitch)
	{
		ColorBlock(compressedBlock.Slice(8), decompressedBlock, destinationPitch, 1);
		SmoothAlphaBlock(compressedBlock, decompressedBlock.Slice(3), destinationPitch, 4);
	}

	public static void DecompressBc4(ReadOnlySpan<byte> compressedBlock, Span<byte> decompressedBlock, int destinationPitch)
	{
		SmoothAlphaBlock(compressedBlock, decompressedBlock, destinationPitch, 1);
	}

	public static void DecompressBc5(ReadOnlySpan<byte> compressedBlock, Span<byte> decompressedBlock, int destinationPitch)
	{
		SmoothAlphaBlock(compressedBlock, decompressedBlock, destinationPitch, 2);
		SmoothAlphaBlock(compressedBlock.Slice(8), decompressedBlock.Slice(1), destinationPitch, 2);
	}

	public unsafe static void DecompressBc6h_Float(byte* compressedBlock, byte* decompressedBlock, int destinationPitch, int isSigned)
	{
		ushort* ptr = stackalloc ushort[48];
		DecompressBc6h_Half(compressedBlock, (byte*)ptr, 12, isSigned);
		ushort* ptr2 = ptr;
		float* ptr3 = (float*)decompressedBlock;
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				ptr3[j * 3] = HalfToFloatQuick(*(ptr2++));
				ptr3[j * 3 + 1] = HalfToFloatQuick(*(ptr2++));
				ptr3[j * 3 + 2] = HalfToFloatQuick(*(ptr2++));
			}
			ptr3 += destinationPitch;
		}
	}

	public unsafe static void DecompressBc6h_Half(byte* compressedBlock, byte* decompressedBlock, int destinationPitch, int isSigned)
	{
		BitStream bitStream = default(BitStream);
		int num = 0;
		int[] array = new int[4];
		int[] array2 = new int[4];
		int[] array3 = new int[4];
		int[] array4 = new int[2];
		int[] array5 = new int[2];
		int[] array6 = new int[2];
		ushort* ptr = (ushort*)decompressedBlock;
		bitStream.low = *(ulong*)compressedBlock;
		bitStream.high = *(ulong*)(compressedBlock + 8);
		array[0] = (array[1] = (array[2] = (array[3] = 0)));
		array2[0] = (array2[1] = (array2[2] = (array2[3] = 0)));
		array3[0] = (array3[1] = (array3[2] = (array3[3] = 0)));
		int num2 = bitStream.ReadBits(2);
		if (num2 > 1)
		{
			num2 |= bitStream.ReadBits(3) << 2;
		}
		switch (num2)
		{
		case 0:
			array2[2] |= bitStream.ReadBit() << 4;
			array3[2] |= bitStream.ReadBit() << 4;
			array3[3] |= bitStream.ReadBit() << 4;
			array[0] |= bitStream.ReadBits(10);
			array2[0] |= bitStream.ReadBits(10);
			array3[0] |= bitStream.ReadBits(10);
			array[1] |= bitStream.ReadBits(5);
			array2[3] |= bitStream.ReadBit() << 4;
			array2[2] |= bitStream.ReadBits(4);
			array2[1] |= bitStream.ReadBits(5);
			array3[3] |= bitStream.ReadBit();
			array2[3] |= bitStream.ReadBits(4);
			array3[1] |= bitStream.ReadBits(5);
			array3[3] |= bitStream.ReadBit() << 1;
			array3[2] |= bitStream.ReadBits(4);
			array[2] |= bitStream.ReadBits(5);
			array3[3] |= bitStream.ReadBit() << 2;
			array[3] |= bitStream.ReadBits(5);
			array3[3] |= bitStream.ReadBit() << 3;
			num = bitStream.ReadBits(5);
			num2 = 0;
			break;
		case 1:
			array2[2] |= bitStream.ReadBit() << 5;
			array2[3] |= bitStream.ReadBit() << 4;
			array2[3] |= bitStream.ReadBit() << 5;
			array[0] |= bitStream.ReadBits(7);
			array3[3] |= bitStream.ReadBit();
			array3[3] |= bitStream.ReadBit() << 1;
			array3[2] |= bitStream.ReadBit() << 4;
			array2[0] |= bitStream.ReadBits(7);
			array3[2] |= bitStream.ReadBit() << 5;
			array3[3] |= bitStream.ReadBit() << 2;
			array2[2] |= bitStream.ReadBit() << 4;
			array3[0] |= bitStream.ReadBits(7);
			array3[3] |= bitStream.ReadBit() << 3;
			array3[3] |= bitStream.ReadBit() << 5;
			array3[3] |= bitStream.ReadBit() << 4;
			array[1] |= bitStream.ReadBits(6);
			array2[2] |= bitStream.ReadBits(4);
			array2[1] |= bitStream.ReadBits(6);
			array2[3] |= bitStream.ReadBits(4);
			array3[1] |= bitStream.ReadBits(6);
			array3[2] |= bitStream.ReadBits(4);
			array[2] |= bitStream.ReadBits(6);
			array[3] |= bitStream.ReadBits(6);
			num = bitStream.ReadBits(5);
			num2 = 1;
			break;
		case 2:
			array[0] |= bitStream.ReadBits(10);
			array2[0] |= bitStream.ReadBits(10);
			array3[0] |= bitStream.ReadBits(10);
			array[1] |= bitStream.ReadBits(5);
			array[0] |= bitStream.ReadBit() << 10;
			array2[2] |= bitStream.ReadBits(4);
			array2[1] |= bitStream.ReadBits(4);
			array2[0] |= bitStream.ReadBit() << 10;
			array3[3] |= bitStream.ReadBit();
			array2[3] |= bitStream.ReadBits(4);
			array3[1] |= bitStream.ReadBits(4);
			array3[0] |= bitStream.ReadBit() << 10;
			array3[3] |= bitStream.ReadBit() << 1;
			array3[2] |= bitStream.ReadBits(4);
			array[2] |= bitStream.ReadBits(5);
			array3[3] |= bitStream.ReadBit() << 2;
			array[3] |= bitStream.ReadBits(5);
			array3[3] |= bitStream.ReadBit() << 3;
			num = bitStream.ReadBits(5);
			num2 = 2;
			break;
		case 6:
			array[0] |= bitStream.ReadBits(10);
			array2[0] |= bitStream.ReadBits(10);
			array3[0] |= bitStream.ReadBits(10);
			array[1] |= bitStream.ReadBits(4);
			array[0] |= bitStream.ReadBit() << 10;
			array2[3] |= bitStream.ReadBit() << 4;
			array2[2] |= bitStream.ReadBits(4);
			array2[1] |= bitStream.ReadBits(5);
			array2[0] |= bitStream.ReadBit() << 10;
			array2[3] |= bitStream.ReadBits(4);
			array3[1] |= bitStream.ReadBits(4);
			array3[0] |= bitStream.ReadBit() << 10;
			array3[3] |= bitStream.ReadBit() << 1;
			array3[2] |= bitStream.ReadBits(4);
			array[2] |= bitStream.ReadBits(4);
			array3[3] |= bitStream.ReadBit();
			array3[3] |= bitStream.ReadBit() << 2;
			array[3] |= bitStream.ReadBits(4);
			array2[2] |= bitStream.ReadBit() << 4;
			array3[3] |= bitStream.ReadBit() << 3;
			num = bitStream.ReadBits(5);
			num2 = 3;
			break;
		case 10:
			array[0] |= bitStream.ReadBits(10);
			array2[0] |= bitStream.ReadBits(10);
			array3[0] |= bitStream.ReadBits(10);
			array[1] |= bitStream.ReadBits(4);
			array[0] |= bitStream.ReadBit() << 10;
			array3[2] |= bitStream.ReadBit() << 4;
			array2[2] |= bitStream.ReadBits(4);
			array2[1] |= bitStream.ReadBits(4);
			array2[0] |= bitStream.ReadBit() << 10;
			array3[3] |= bitStream.ReadBit();
			array2[3] |= bitStream.ReadBits(4);
			array3[1] |= bitStream.ReadBits(5);
			array3[0] |= bitStream.ReadBit() << 10;
			array3[2] |= bitStream.ReadBits(4);
			array[2] |= bitStream.ReadBits(4);
			array3[3] |= bitStream.ReadBit() << 1;
			array3[3] |= bitStream.ReadBit() << 2;
			array[3] |= bitStream.ReadBits(4);
			array3[3] |= bitStream.ReadBit() << 4;
			array3[3] |= bitStream.ReadBit() << 3;
			num = bitStream.ReadBits(5);
			num2 = 4;
			break;
		case 14:
			array[0] |= bitStream.ReadBits(9);
			array3[2] |= bitStream.ReadBit() << 4;
			array2[0] |= bitStream.ReadBits(9);
			array2[2] |= bitStream.ReadBit() << 4;
			array3[0] |= bitStream.ReadBits(9);
			array3[3] |= bitStream.ReadBit() << 4;
			array[1] |= bitStream.ReadBits(5);
			array2[3] |= bitStream.ReadBit() << 4;
			array2[2] |= bitStream.ReadBits(4);
			array2[1] |= bitStream.ReadBits(5);
			array3[3] |= bitStream.ReadBit();
			array2[3] |= bitStream.ReadBits(4);
			array3[1] |= bitStream.ReadBits(5);
			array3[3] |= bitStream.ReadBit() << 1;
			array3[2] |= bitStream.ReadBits(4);
			array[2] |= bitStream.ReadBits(5);
			array3[3] |= bitStream.ReadBit() << 2;
			array[3] |= bitStream.ReadBits(5);
			array3[3] |= bitStream.ReadBit() << 3;
			num = bitStream.ReadBits(5);
			num2 = 5;
			break;
		case 18:
			array[0] |= bitStream.ReadBits(8);
			array2[3] |= bitStream.ReadBit() << 4;
			array3[2] |= bitStream.ReadBit() << 4;
			array2[0] |= bitStream.ReadBits(8);
			array3[3] |= bitStream.ReadBit() << 2;
			array2[2] |= bitStream.ReadBit() << 4;
			array3[0] |= bitStream.ReadBits(8);
			array3[3] |= bitStream.ReadBit() << 3;
			array3[3] |= bitStream.ReadBit() << 4;
			array[1] |= bitStream.ReadBits(6);
			array2[2] |= bitStream.ReadBits(4);
			array2[1] |= bitStream.ReadBits(5);
			array3[3] |= bitStream.ReadBit();
			array2[3] |= bitStream.ReadBits(4);
			array3[1] |= bitStream.ReadBits(5);
			array3[3] |= bitStream.ReadBit() << 1;
			array3[2] |= bitStream.ReadBits(4);
			array[2] |= bitStream.ReadBits(6);
			array[3] |= bitStream.ReadBits(6);
			num = bitStream.ReadBits(5);
			num2 = 6;
			break;
		case 22:
			array[0] |= bitStream.ReadBits(8);
			array3[3] |= bitStream.ReadBit();
			array3[2] |= bitStream.ReadBit() << 4;
			array2[0] |= bitStream.ReadBits(8);
			array2[2] |= bitStream.ReadBit() << 5;
			array2[2] |= bitStream.ReadBit() << 4;
			array3[0] |= bitStream.ReadBits(8);
			array2[3] |= bitStream.ReadBit() << 5;
			array3[3] |= bitStream.ReadBit() << 4;
			array[1] |= bitStream.ReadBits(5);
			array2[3] |= bitStream.ReadBit() << 4;
			array2[2] |= bitStream.ReadBits(4);
			array2[1] |= bitStream.ReadBits(6);
			array2[3] |= bitStream.ReadBits(4);
			array3[1] |= bitStream.ReadBits(5);
			array3[3] |= bitStream.ReadBit() << 1;
			array3[2] |= bitStream.ReadBits(4);
			array[2] |= bitStream.ReadBits(5);
			array3[3] |= bitStream.ReadBit() << 2;
			array[3] |= bitStream.ReadBits(5);
			array3[3] |= bitStream.ReadBit() << 3;
			num = bitStream.ReadBits(5);
			num2 = 7;
			break;
		case 26:
			array[0] |= bitStream.ReadBits(8);
			array3[3] |= bitStream.ReadBit() << 1;
			array3[2] |= bitStream.ReadBit() << 4;
			array2[0] |= bitStream.ReadBits(8);
			array3[2] |= bitStream.ReadBit() << 5;
			array2[2] |= bitStream.ReadBit() << 4;
			array3[0] |= bitStream.ReadBits(8);
			array3[3] |= bitStream.ReadBit() << 5;
			array3[3] |= bitStream.ReadBit() << 4;
			array[1] |= bitStream.ReadBits(5);
			array2[3] |= bitStream.ReadBit() << 4;
			array2[2] |= bitStream.ReadBits(4);
			array2[1] |= bitStream.ReadBits(5);
			array3[3] |= bitStream.ReadBit();
			array2[3] |= bitStream.ReadBits(4);
			array3[1] |= bitStream.ReadBits(6);
			array3[2] |= bitStream.ReadBits(4);
			array[2] |= bitStream.ReadBits(5);
			array3[3] |= bitStream.ReadBit() << 2;
			array[3] |= bitStream.ReadBits(5);
			array3[3] |= bitStream.ReadBit() << 3;
			num = bitStream.ReadBits(5);
			num2 = 8;
			break;
		case 30:
			array[0] |= bitStream.ReadBits(6);
			array2[3] |= bitStream.ReadBit() << 4;
			array3[3] |= bitStream.ReadBit();
			array3[3] |= bitStream.ReadBit() << 1;
			array3[2] |= bitStream.ReadBit() << 4;
			array2[0] |= bitStream.ReadBits(6);
			array2[2] |= bitStream.ReadBit() << 5;
			array3[2] |= bitStream.ReadBit() << 5;
			array3[3] |= bitStream.ReadBit() << 2;
			array2[2] |= bitStream.ReadBit() << 4;
			array3[0] |= bitStream.ReadBits(6);
			array2[3] |= bitStream.ReadBit() << 5;
			array3[3] |= bitStream.ReadBit() << 3;
			array3[3] |= bitStream.ReadBit() << 5;
			array3[3] |= bitStream.ReadBit() << 4;
			array[1] |= bitStream.ReadBits(6);
			array2[2] |= bitStream.ReadBits(4);
			array2[1] |= bitStream.ReadBits(6);
			array2[3] |= bitStream.ReadBits(4);
			array3[1] |= bitStream.ReadBits(6);
			array3[2] |= bitStream.ReadBits(4);
			array[2] |= bitStream.ReadBits(6);
			array[3] |= bitStream.ReadBits(6);
			num = bitStream.ReadBits(5);
			num2 = 9;
			break;
		case 3:
			array[0] |= bitStream.ReadBits(10);
			array2[0] |= bitStream.ReadBits(10);
			array3[0] |= bitStream.ReadBits(10);
			array[1] |= bitStream.ReadBits(10);
			array2[1] |= bitStream.ReadBits(10);
			array3[1] |= bitStream.ReadBits(10);
			num2 = 10;
			break;
		case 7:
			array[0] |= bitStream.ReadBits(10);
			array2[0] |= bitStream.ReadBits(10);
			array3[0] |= bitStream.ReadBits(10);
			array[1] |= bitStream.ReadBits(9);
			array[0] |= bitStream.ReadBit() << 10;
			array2[1] |= bitStream.ReadBits(9);
			array2[0] |= bitStream.ReadBit() << 10;
			array3[1] |= bitStream.ReadBits(9);
			array3[0] |= bitStream.ReadBit() << 10;
			num2 = 11;
			break;
		case 11:
			array[0] |= bitStream.ReadBits(10);
			array2[0] |= bitStream.ReadBits(10);
			array3[0] |= bitStream.ReadBits(10);
			array[1] |= bitStream.ReadBits(8);
			array[0] |= bitStream.ReadBitsReversed(2) << 10;
			array2[1] |= bitStream.ReadBits(8);
			array2[0] |= bitStream.ReadBitsReversed(2) << 10;
			array3[1] |= bitStream.ReadBits(8);
			array3[0] |= bitStream.ReadBitsReversed(2) << 10;
			num2 = 12;
			break;
		case 15:
			array[0] |= bitStream.ReadBits(10);
			array2[0] |= bitStream.ReadBits(10);
			array3[0] |= bitStream.ReadBits(10);
			array[1] |= bitStream.ReadBits(4);
			array[0] |= bitStream.ReadBitsReversed(6) << 10;
			array2[1] |= bitStream.ReadBits(4);
			array2[0] |= bitStream.ReadBitsReversed(6) << 10;
			array3[1] |= bitStream.ReadBits(4);
			array3[0] |= bitStream.ReadBitsReversed(6) << 10;
			num2 = 13;
			break;
		default:
		{
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					ptr[j * 3] = 0;
					ptr[j * 3 + 1] = 0;
					ptr[j * 3 + 2] = 0;
				}
				ptr += destinationPitch;
			}
			return;
		}
		}
		int num3;
		if (num2 >= 10)
		{
			num = 0;
			num3 = 0;
		}
		else
		{
			num3 = 1;
		}
		if (isSigned != 0)
		{
			array[0] = ExtendSign(array[0], Bc6hTables.ActualBitsCount[0][num2]);
			array2[0] = ExtendSign(array2[0], Bc6hTables.ActualBitsCount[0][num2]);
			array3[0] = ExtendSign(array3[0], Bc6hTables.ActualBitsCount[0][num2]);
		}
		if ((num2 != 9 && num2 != 10) || isSigned != 0)
		{
			for (int i = 1; i < (num3 + 1) * 2; i++)
			{
				array[i] = ExtendSign(array[i], Bc6hTables.ActualBitsCount[1][num2]);
				array2[i] = ExtendSign(array2[i], Bc6hTables.ActualBitsCount[2][num2]);
				array3[i] = ExtendSign(array3[i], Bc6hTables.ActualBitsCount[3][num2]);
			}
		}
		if (num2 != 9 && num2 != 10)
		{
			for (int i = 1; i < (num3 + 1) * 2; i++)
			{
				array[i] = TransformInverse(array[i], array[0], Bc6hTables.ActualBitsCount[0][num2], isSigned);
				array2[i] = TransformInverse(array2[i], array2[0], Bc6hTables.ActualBitsCount[0][num2], isSigned);
				array3[i] = TransformInverse(array3[i], array3[0], Bc6hTables.ActualBitsCount[0][num2], isSigned);
			}
		}
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				int num4 = ((num2 < 10) ? Bc6hTables.PartitionSets[num][i][j] : (((i | j) == 0) ? 128 : 0));
				int num5 = ((num2 >= 10) ? 4 : 3);
				if ((num4 & 0x80) != 0)
				{
					num5--;
				}
				num4 &= 1;
				int index = bitStream.ReadBits(num5);
				array4[0] = Unquantize(array[num4 * 2], Bc6hTables.ActualBitsCount[0][num2], isSigned);
				array5[0] = Unquantize(array2[num4 * 2], Bc6hTables.ActualBitsCount[0][num2], isSigned);
				array6[0] = Unquantize(array3[num4 * 2], Bc6hTables.ActualBitsCount[0][num2], isSigned);
				array4[1] = Unquantize(array[num4 * 2 + 1], Bc6hTables.ActualBitsCount[0][num2], isSigned);
				array5[1] = Unquantize(array2[num4 * 2 + 1], Bc6hTables.ActualBitsCount[0][num2], isSigned);
				array6[1] = Unquantize(array3[num4 * 2 + 1], Bc6hTables.ActualBitsCount[0][num2], isSigned);
				ptr[j * 3] = FinishUnquantize(Interpolate(array4[0], array4[1], (num2 >= 10) ? Bc6hTables.AWeight4 : Bc6hTables.AWeight3, index), isSigned);
				ptr[j * 3 + 1] = FinishUnquantize(Interpolate(array5[0], array5[1], (num2 >= 10) ? Bc6hTables.AWeight4 : Bc6hTables.AWeight3, index), isSigned);
				ptr[j * 3 + 2] = FinishUnquantize(Interpolate(array6[0], array6[1], (num2 >= 10) ? Bc6hTables.AWeight4 : Bc6hTables.AWeight3, index), isSigned);
			}
			ptr += destinationPitch;
		}
	}

	public unsafe static void DecompressBc7(byte* compressedBlock, byte* decompressedBlock, int destinationPitch)
	{
		BitStream bitStream = default(BitStream);
		int[][] array = CreateRectangularArray<int>(6, 4);
		int[][] array2 = CreateRectangularArray<int>(4, 4);
		byte* ptr = decompressedBlock;
		bitStream.low = *(ulong*)compressedBlock;
		bitStream.high = *(ulong*)(compressedBlock + 8);
		int i;
		for (i = 0; i < 8; i++)
		{
			if (bitStream.ReadBit() != 0)
			{
				break;
			}
		}
		if (i >= 8)
		{
			for (int j = 0; j < 4; j++)
			{
				for (int k = 0; k < 4; k++)
				{
					ptr[k * 4] = 0;
					ptr[k * 4 + 1] = 0;
					ptr[k * 4 + 2] = 0;
					ptr[k * 4 + 3] = 0;
				}
				ptr += destinationPitch;
			}
			return;
		}
		int num = 0;
		int num2 = 1;
		int num3 = 0;
		int num4 = 0;
		if (i == 0 || i == 1 || i == 2 || i == 3 || i == 7)
		{
			num2 = ((i == 0 || i == 2) ? 3 : 2);
			num = bitStream.ReadBits((i == 0) ? 4 : 6);
		}
		int num5 = num2 * 2;
		if (i == 4 || i == 5)
		{
			num3 = bitStream.ReadBits(2);
			if (i == 4)
			{
				num4 = bitStream.ReadBit();
			}
		}
		for (int j = 0; j < 3; j++)
		{
			for (int k = 0; k < num5; k++)
			{
				array[k][j] = bitStream.ReadBits(Bc7Tables.bcdec_bc7_actual_bits_count[0][i]);
			}
		}
		if (Bc7Tables.bcdec_bc7_actual_bits_count[1][i] > 0)
		{
			for (int k = 0; k < num5; k++)
			{
				array[k][3] = bitStream.ReadBits(Bc7Tables.bcdec_bc7_actual_bits_count[1][i]);
			}
		}
		if (i == 0 || i == 1 || i == 3 || i == 6 || i == 7)
		{
			for (int j = 0; j < num5; j++)
			{
				for (int k = 0; k < 4; k++)
				{
					array[j][k] <<= 1;
				}
			}
			if (i == 1)
			{
				int j = bitStream.ReadBit();
				int k = bitStream.ReadBit();
				for (int l = 0; l < 3; l++)
				{
					array[0][l] |= j;
					array[1][l] |= j;
					array[2][l] |= k;
					array[3][l] |= k;
				}
			}
			else if ((0xCB & (1 << i)) != 0)
			{
				for (int j = 0; j < num5; j++)
				{
					int k = bitStream.ReadBit();
					for (int l = 0; l < 4; l++)
					{
						array[j][l] |= k;
					}
				}
			}
		}
		for (int j = 0; j < num5; j++)
		{
			int k = Bc7Tables.bcdec_bc7_actual_bits_count[0][i] + ((203 >> i) & 1);
			for (int l = 0; l < 3; l++)
			{
				array[j][l] = array[j][l] << 8 - k;
				array[j][l] = array[j][l] | (array[j][l] >> k);
			}
			k = Bc7Tables.bcdec_bc7_actual_bits_count[1][i] + ((203 >> i) & 1);
			array[j][3] = array[j][3] << 8 - k;
			array[j][3] = array[j][3] | (array[j][3] >> k);
		}
		if (Bc7Tables.bcdec_bc7_actual_bits_count[1][i] == 0)
		{
			for (int k = 0; k < num5; k++)
			{
				array[k][3] = 255;
			}
		}
		int num6;
		switch (i)
		{
		default:
			num6 = 2;
			break;
		case 6:
			num6 = 4;
			break;
		case 0:
		case 1:
			num6 = 3;
			break;
		}
		int num7 = num6;
		int num8 = i switch
		{
			5 => 2, 
			4 => 3, 
			_ => 0, 
		};
		int[] array3 = num7 switch
		{
			3 => Bc7Tables.AWeight3, 
			2 => Bc7Tables.AWeight2, 
			_ => Bc7Tables.AWeight4, 
		};
		int[] array4 = ((num8 == 2) ? Bc7Tables.AWeight2 : Bc7Tables.AWeight3);
		for (int j = 0; j < 4; j++)
		{
			for (int k = 0; k < 4; k++)
			{
				int num9 = ((num2 != 1) ? Bc7Tables.bcdec_bc7_partition_sets[num2 - 2][num][j][k] : (((j | k) == 0) ? 128 : 0));
				int num10;
				switch (i)
				{
				default:
					num10 = 2;
					break;
				case 6:
					num10 = 4;
					break;
				case 0:
				case 1:
					num10 = 3;
					break;
				}
				num7 = num10;
				if ((num9 & 0x80) != 0)
				{
					num7--;
				}
				array2[j][k] = bitStream.ReadBits(num7);
			}
		}
		for (int j = 0; j < 4; j++)
		{
			for (int k = 0; k < 4; k++)
			{
				int num9 = ((num2 != 1) ? Bc7Tables.bcdec_bc7_partition_sets[num2 - 2][num][j][k] : (((j | k) == 0) ? 128 : 0));
				num9 &= 3;
				int index = array2[j][k];
				int num11;
				int num12;
				int num13;
				int num14;
				if (num8 == 0)
				{
					num11 = Interpolate(array[num9 * 2][0], array[num9 * 2 + 1][0], array3, index);
					num12 = Interpolate(array[num9 * 2][1], array[num9 * 2 + 1][1], array3, index);
					num13 = Interpolate(array[num9 * 2][2], array[num9 * 2 + 1][2], array3, index);
					num14 = Interpolate(array[num9 * 2][3], array[num9 * 2 + 1][3], array3, index);
				}
				else
				{
					int index2 = bitStream.ReadBits(((j | k) != 0) ? num8 : (num8 - 1));
					if (num4 == 0)
					{
						num11 = Interpolate(array[num9 * 2][0], array[num9 * 2 + 1][0], array3, index);
						num12 = Interpolate(array[num9 * 2][1], array[num9 * 2 + 1][1], array3, index);
						num13 = Interpolate(array[num9 * 2][2], array[num9 * 2 + 1][2], array3, index);
						num14 = Interpolate(array[num9 * 2][3], array[num9 * 2 + 1][3], array4, index2);
					}
					else
					{
						num11 = Interpolate(array[num9 * 2][0], array[num9 * 2 + 1][0], array4, index2);
						num12 = Interpolate(array[num9 * 2][1], array[num9 * 2 + 1][1], array4, index2);
						num13 = Interpolate(array[num9 * 2][2], array[num9 * 2 + 1][2], array4, index2);
						num14 = Interpolate(array[num9 * 2][3], array[num9 * 2 + 1][3], array3, index);
					}
				}
				switch (num3)
				{
				case 1:
					SwapValues(&num14, &num11);
					break;
				case 2:
					SwapValues(&num14, &num12);
					break;
				case 3:
					SwapValues(&num14, &num13);
					break;
				}
				ptr[k * 4] = (byte)num11;
				ptr[k * 4 + 1] = (byte)num12;
				ptr[k * 4 + 2] = (byte)num13;
				ptr[k * 4 + 3] = (byte)num14;
			}
			ptr += destinationPitch;
		}
	}

	public static void ColorBlock(ReadOnlySpan<byte> compressedBlock, Span<byte> decompressedBlock, int destinationPitch, int onlyOpaqueMode)
	{
		Span<uint> span = stackalloc uint[4];
		ushort num = MemoryMarshal.Read<ushort>(compressedBlock);
		ushort num2 = MemoryMarshal.Read<ushort>(compressedBlock.Slice(2));
		uint num3 = (uint)(((num >> 11) & 0x1F) * 527 + 23 >> 6);
		uint num4 = (uint)(((num >> 5) & 0x3F) * 259 + 33 >> 6);
		uint num5 = (uint)((num & 0x1F) * 527 + 23 >> 6);
		span[0] = 0xFF000000u | (num5 << 16) | (num4 << 8) | num3;
		uint num6 = (uint)(((num2 >> 11) & 0x1F) * 527 + 23 >> 6);
		uint num7 = (uint)(((num2 >> 5) & 0x3F) * 259 + 33 >> 6);
		uint num8 = (uint)((num2 & 0x1F) * 527 + 23 >> 6);
		span[1] = 0xFF000000u | (num8 << 16) | (num7 << 8) | num6;
		if (num > num2 || onlyOpaqueMode != 0)
		{
			uint num9 = (2 * num3 + num6 + 1) / 3;
			uint num10 = (2 * num4 + num7 + 1) / 3;
			uint num11 = (2 * num5 + num8 + 1) / 3;
			span[2] = 0xFF000000u | (num11 << 16) | (num10 << 8) | num9;
			num9 = (num3 + 2 * num6 + 1) / 3;
			num10 = (num4 + 2 * num7 + 1) / 3;
			num11 = (num5 + 2 * num8 + 1) / 3;
			span[3] = 0xFF000000u | (num11 << 16) | (num10 << 8) | num9;
		}
		else
		{
			uint num9 = num3 + num6 + 1 >> 1;
			uint num10 = num4 + num7 + 1 >> 1;
			uint num11 = num5 + num8 + 1 >> 1;
			span[2] = 0xFF000000u | (num11 << 16) | (num10 << 8) | num9;
			span[3] = 0u;
		}
		uint num12 = MemoryMarshal.Read<uint>(compressedBlock.Slice(4));
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				int index = (int)(num12 & 3);
				int num13 = i * destinationPitch + j * 4;
				if (num13 + 4 > decompressedBlock.Length)
				{
					throw new Exception($"Not enough space in decompressed block.\nLength: {decompressedBlock.Length}\nOffset: {num13}\nPitch: {destinationPitch}\ni: {i}\nj: {j}");
				}
				MemoryMarshal.Write(decompressedBlock.Slice(num13), ref span[index]);
				num12 >>= 2;
			}
		}
	}

	public static void SharpAlphaBlock(ReadOnlySpan<byte> compressedBlock, Span<byte> decompressedBlock, int destinationPitch)
	{
		ReadOnlySpan<ushort> readOnlySpan = MemoryMarshal.Cast<byte, ushort>(compressedBlock);
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				decompressedBlock[j * 4 + i * destinationPitch] = (byte)(((readOnlySpan[i] >>> 4 * j) & 0xF) * 17);
			}
		}
	}

	public static void SmoothAlphaBlock(ReadOnlySpan<byte> compressedBlock, Span<byte> decompressedBlock, int destinationPitch, int pixelSize)
	{
		Span<byte> span = stackalloc byte[8];
		ulong num = MemoryMarshal.Read<ulong>(compressedBlock);
		span[0] = (byte)(num & 0xFF);
		span[1] = (byte)((num >> 8) & 0xFF);
		if (span[0] > span[1])
		{
			span[2] = (byte)((6 * span[0] + span[1] + 1) / 7);
			span[3] = (byte)((5 * span[0] + 2 * span[1] + 1) / 7);
			span[4] = (byte)((4 * span[0] + 3 * span[1] + 1) / 7);
			span[5] = (byte)((3 * span[0] + 4 * span[1] + 1) / 7);
			span[6] = (byte)((2 * span[0] + 5 * span[1] + 1) / 7);
			span[7] = (byte)((span[0] + 6 * span[1] + 1) / 7);
		}
		else
		{
			span[2] = (byte)((4 * span[0] + span[1] + 1) / 5);
			span[3] = (byte)((3 * span[0] + 2 * span[1] + 1) / 5);
			span[4] = (byte)((2 * span[0] + 3 * span[1] + 1) / 5);
			span[5] = (byte)((span[0] + 4 * span[1] + 1) / 5);
			span[6] = 0;
			span[7] = byte.MaxValue;
		}
		ulong num2 = num >> 16;
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				decompressedBlock[j * pixelSize + i * destinationPitch] = span[(int)(num2 & 7)];
				num2 >>= 3;
			}
		}
	}

	public static int ExtendSign(int val, int bits)
	{
		return val << 32 - bits >> 32 - bits;
	}

	public static int TransformInverse(int val, int a0, int bits, int isSigned)
	{
		val = (val + a0) & ((1 << bits) - 1);
		if (isSigned != 0)
		{
			val = ExtendSign(val, bits);
		}
		return val;
	}

	public static int Unquantize(int val, int bits, int isSigned)
	{
		int num = 0;
		int num2;
		if (isSigned == 0)
		{
			num2 = ((bits >= 15) ? val : ((val != 0) ? ((val != (1 << bits) - 1) ? ((val << 16) + 32768 >> bits) : 65535) : 0));
		}
		else if (bits >= 16)
		{
			num2 = val;
		}
		else
		{
			if (val < 0)
			{
				num = 1;
				val = -val;
			}
			num2 = ((val != 0) ? ((val < (1 << bits - 1) - 1) ? ((val << 15) + 16384 >> bits - 1) : 32767) : 0);
			if (num != 0)
			{
				num2 = -num2;
			}
		}
		return num2;
	}

	public static int Interpolate(int a, int b, ReadOnlySpan<int> weights, int index)
	{
		return a * (64 - weights[index]) + b * weights[index] + 32 >> 6;
	}

	public static ushort FinishUnquantize(int val, int isSigned)
	{
		if (isSigned == 0)
		{
			return (ushort)(val * 31 >> 6);
		}
		val = ((val < 0) ? (-(-val * 31 >> 5)) : (val * 31 >> 5));
		int num = 0;
		if (val < 0)
		{
			num = 32768;
			val = -val;
		}
		return (ushort)(num | val);
	}

	public static float HalfToFloatQuick(ushort half)
	{
		FP32 fP = default(FP32);
		fP.u = 947912704u;
		FP32 fP2 = fP;
		uint num = 260046848u;
		FP32 fP3 = default(FP32);
		fP3.u = (uint)((half & 0x7FFF) << 13);
		uint num2 = num & fP3.u;
		fP3.u += 939524096u;
		if (num2 == num)
		{
			fP3.u += 939524096u;
		}
		else if (num2 == 0)
		{
			fP3.u += 8388608u;
			fP3.f -= fP2.f;
		}
		fP3.u |= (uint)((half & 0x8000) << 16);
		return fP3.f;
	}

	public unsafe static void SwapValues(int* a, int* b)
	{
		*a ^= *b;
		*b ^= *a;
		*a ^= *b;
	}

	public static T[][] CreateRectangularArray<T>(int size1, int size2)
	{
		T[][] array = new T[size1][];
		for (int i = 0; i < size1; i++)
		{
			array[i] = new T[size2];
		}
		return array;
	}
}
