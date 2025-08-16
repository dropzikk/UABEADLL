using System;

namespace AssetsTools.NET.Texture;

public static class BC7Decoder
{
	public static byte[] ReadBC7(byte[] data, int width, int height)
	{
		int num = width + 3 >> 2;
		int num2 = height + 3 >> 2;
		int num3 = num * num2 * 16 * 4;
		byte[] array = new byte[num3];
		int num4 = 0;
		BitReader bitReader = new BitReader();
		byte[] array2 = new byte[12];
		byte[] array3 = new byte[24];
		byte[] array4 = new byte[16];
		byte[] array5 = new byte[4];
		byte[] array6 = new byte[16];
		byte[] array7 = new byte[16];
		byte[] array8 = new byte[4];
		byte[] array9 = new byte[4];
		for (int i = 0; i < num2; i++)
		{
			for (int j = 0; j < num; j++)
			{
				int num5 = data[num4];
				ulong num6 = BitConverter.ToUInt64(data, num4);
				ulong num7 = BitConverter.ToUInt64(data, num4 + 8);
				bitReader.Reset(num6, num7);
				int num8 = (((num5 & 1) != 1) ? ((((num5 >> 1) & 1) == 1) ? 1 : ((((num5 >> 2) & 1) == 1) ? 2 : ((((num5 >> 3) & 1) == 1) ? 3 : ((((num5 >> 4) & 1) == 1) ? 4 : ((((num5 >> 5) & 1) == 1) ? 5 : ((((num5 >> 6) & 1) == 1) ? 6 : ((((num5 >> 7) & 1) != 1) ? (-1) : 7))))))) : 0);
				bitReader.index += num8 + 1;
				if (num8 == 1)
				{
					int num9 = (byte)GetBits64(num6, 2, 7);
					array2[0] = (byte)GetBits64(num6, 8, 13);
					array2[3] = (byte)GetBits64(num6, 14, 19);
					array2[6] = (byte)GetBits64(num6, 20, 25);
					array2[9] = (byte)GetBits64(num6, 26, 31);
					array2[1] = (byte)GetBits64(num6, 32, 37);
					array2[4] = (byte)GetBits64(num6, 38, 43);
					array2[7] = (byte)GetBits64(num6, 44, 49);
					array2[10] = (byte)GetBits64(num6, 50, 55);
					array2[2] = (byte)GetBits64(num6, 56, 61);
					array2[5] = (byte)(GetBits64(num6, 62, 63) | (GetBits64(num7, 0, 3) << 2));
					array2[8] = (byte)GetBits64(num7, 4, 9);
					array2[11] = (byte)GetBits64(num7, 10, 15);
					for (int k = 0; k < 4; k++)
					{
						array2[k * 3] <<= 2;
						array2[k * 3 + 1] <<= 2;
						array2[k * 3 + 2] <<= 2;
					}
					byte b = (byte)((byte)GetBits64(num7, 16, 16) << 1);
					byte b2 = (byte)((byte)GetBits64(num7, 17, 17) << 1);
					for (int k = 0; k < 3; k++)
					{
						array2[k] |= b;
						array2[3 + k] |= b;
						array2[6 + k] |= b2;
						array2[9 + k] |= b2;
					}
					for (int k = 0; k < 4; k++)
					{
						array2[k * 3] |= (byte)(array2[k * 3] >> 7);
						array2[k * 3 + 1] |= (byte)(array2[k * 3 + 1] >> 7);
						array2[k * 3 + 2] |= (byte)(array2[k * 3 + 2] >> 7);
					}
					for (int k = 0; k < 16; k++)
					{
						array4[k] = BPTCTables.P2[num9 * 16 + k];
					}
					array5[0] = 0;
					array5[1] = BPTCTables.AnchorIndexSecondSubset[num9];
					num7 >>= 18;
					for (int k = 0; k < 16; k++)
					{
						array6[k] = 0;
					}
					for (int k = 0; k < 16; k++)
					{
						if (k == array5[array4[k]])
						{
							array6[k] = (byte)(num7 & 3);
							num7 >>= 2;
						}
						else
						{
							array6[k] = (byte)(num7 & 7);
							num7 >>= 3;
						}
					}
					for (int k = 0; k < 16; k++)
					{
						for (int l = 0; l < 3; l++)
						{
							array8[l] = array2[2 * array4[k] * 3 + l];
							array9[l] = array2[(2 * array4[k] + 1) * 3 + l];
						}
						int num10 = j * 4 * 4 + k % 4 * 4 + i * width * 4 * 4 + (k >> 2) * width * 4;
						array[num10] = Interpolate(array8[2], array9[2], array6[k], 3);
						array[num10 + 1] = Interpolate(array8[1], array9[1], array6[k], 3);
						array[num10 + 2] = Interpolate(array8[0], array9[0], array6[k], 3);
						array[num10 + 3] = byte.MaxValue;
					}
				}
				else
				{
					int num11 = 1;
					int num9 = 0;
					if (BPTCTables.ModeHasPartitionBits[num8])
					{
						num11 = BPTCTables.NumberOfSubsets[num8];
						num9 = bitReader.ReadNumber(BPTCTables.NumberOfPartitionBits[num8]);
					}
					int num12 = bitReader.ReadNumber(BPTCTables.NumberOfRotationBits[num8]);
					int num13 = 0;
					if (num8 == 4)
					{
						num13 = bitReader.ReadNumber(1);
					}
					int indexPrecision = BPTCTables.AlphaIndexBitCount[num8] - num13;
					int indexPrecision2 = BPTCTables.ColorIndexBitCount[num8] + num13;
					int num14 = BPTCTables.ComponentsInData0[num8];
					long num15 = (long)(num6 >> bitReader.index);
					int num16 = BPTCTables.ColorPrecision[num8];
					int num17 = (1 << num16) - 1;
					int num18 = num11 * 2 * num16;
					for (int k = 0; k < num14; k++)
					{
						for (int l = 0; l < num11; l++)
						{
							for (int m = 0; m < 2; m++)
							{
								array3[l * 8 + m * 4 + k] = (byte)(num15 & num17);
								num15 >>= num16;
							}
						}
					}
					bitReader.index += num14 * num18;
					if (num14 < 3)
					{
						num15 = (long)(num6 >> bitReader.index);
						num15 |= (long)(num7 << 64 - bitReader.index);
						int k = num14;
						for (int l = 0; l < num11; l++)
						{
							for (int m = 0; m < 2; m++)
							{
								array3[l * 8 + m * 4 + k] = (byte)(num15 & num17);
								num15 >>= num16;
							}
						}
						bitReader.index += num18;
					}
					if (num14 < 2)
					{
						num15 = (long)(num7 >> bitReader.index - 64);
						int k = 2;
						for (int l = 0; l < num11; l++)
						{
							for (int m = 0; m < 2; m++)
							{
								array3[l * 8 + m * 4 + k] = (byte)(num15 & num17);
								num15 >>= num16;
							}
						}
						bitReader.index += num18;
					}
					int num19 = BPTCTables.AlphaPrecision[num8];
					if (num19 > 0)
					{
						num15 = num8 switch
						{
							7 => (long)(num7 >> bitReader.index - 64), 
							5 => (long)((num6 >> bitReader.index) | ((num7 & 3) << 14)), 
							_ => (long)(num6 >> bitReader.index), 
						};
						num17 = (1 << num19) - 1;
						for (int l = 0; l < num11; l++)
						{
							for (int m = 0; m < 2; m++)
							{
								array3[l * 8 + m * 4 + 3] = (byte)(num15 & num17);
								num15 >>= num19;
							}
						}
						bitReader.index += num11 * 2 * num19;
					}
					if (BPTCTables.ModeHasPBits[num8])
					{
						uint num20;
						if (bitReader.index < 64)
						{
							num20 = (uint)(num6 >> bitReader.index);
							if (bitReader.index + num11 * 2 > 64)
							{
								num20 |= (uint)(int)(num7 << 64 - bitReader.index);
							}
						}
						else
						{
							num20 = (uint)(num7 >> bitReader.index - 64);
						}
						for (int k = 0; k < num11 * 2; k++)
						{
							array3[k * 4] <<= 1;
							array3[k * 4 + 1] <<= 1;
							array3[k * 4 + 2] <<= 1;
							array3[k * 4 + 3] <<= 1;
							array3[k * 4] |= (byte)(num20 & 1);
							array3[k * 4 + 1] |= (byte)(num20 & 1);
							array3[k * 4 + 2] |= (byte)(num20 & 1);
							array3[k * 4 + 3] |= (byte)(num20 & 1);
							num20 >>= 1;
						}
						bitReader.index += num11 * 2;
					}
					int num21 = BPTCTables.ColorPrecisionPlusPBit[num8];
					int num22 = BPTCTables.AlphaPrecisionPlusPBit[num8];
					for (int k = 0; k < num11 * 2; k++)
					{
						ref byte reference = ref array3[k * 4];
						reference = (byte)(reference << 8 - num21);
						ref byte reference2 = ref array3[k * 4 + 1];
						reference2 = (byte)(reference2 << 8 - num21);
						ref byte reference3 = ref array3[k * 4 + 2];
						reference3 = (byte)(reference3 << 8 - num21);
						ref byte reference4 = ref array3[k * 4 + 3];
						reference4 = (byte)(reference4 << 8 - num22);
						array3[k * 4] |= (byte)(array3[k * 4] >> num21);
						array3[k * 4 + 1] |= (byte)(array3[k * 4 + 1] >> num21);
						array3[k * 4 + 2] |= (byte)(array3[k * 4 + 2] >> num21);
						array3[k * 4 + 3] |= (byte)(array3[k * 4 + 3] >> num22);
					}
					if (num8 <= 3)
					{
						for (int k = 0; k < num11 * 2; k++)
						{
							array3[k * 4 + 3] = byte.MaxValue;
						}
					}
					for (int k = 0; k < 16; k++)
					{
						switch (num11)
						{
						case 1:
							array4[k] = 0;
							break;
						case 2:
							array4[k] = BPTCTables.P2[num9 * 16 + k];
							break;
						default:
							array4[k] = BPTCTables.P3[num9 * 16 + k];
							break;
						}
					}
					for (int k = 0; k < num11; k++)
					{
						if (k == 0)
						{
							array5[k] = 0;
						}
						else if (num11 == 2)
						{
							array5[k] = BPTCTables.AnchorIndexSecondSubset[num9];
						}
						else if (k == 1)
						{
							array5[k] = BPTCTables.AnchorIndexSecondSubsetOfThree[num9];
						}
						else
						{
							array5[k] = BPTCTables.AnchorIndexThirdSubset[num9];
						}
					}
					for (int k = 0; k < 16; k++)
					{
						array6[k] = 0;
						array7[k] = 0;
					}
					ulong num23;
					if (bitReader.index >= 64)
					{
						num23 = num7 >> bitReader.index - 64;
						uint num24 = (uint)((1 << (int)BPTCTables.IndexBits[num8]) - 1);
						uint num25 = (uint)((1 << BPTCTables.IndexBits[num8] - 1) - 1);
						for (int k = 0; k < 16; k++)
						{
							if (k == array5[array4[k]])
							{
								array6[k] = (byte)(num23 & num25);
								num23 >>= BPTCTables.IndexBits[num8] - 1;
								array7[k] = array6[k];
							}
							else
							{
								array6[k] = (byte)(num23 & num24);
								num23 >>= (int)BPTCTables.IndexBits[num8];
								array7[k] = array6[k];
							}
						}
					}
					else
					{
						num23 = num6 >> 50;
						num23 |= num7 << 14;
						for (int k = 0; k < 16; k++)
						{
							if (k == array5[array4[k]])
							{
								if (num13 == 1)
								{
									array7[k] = (byte)(num23 & 1);
									num23 >>= 1;
								}
								else
								{
									array6[k] = (byte)(num23 & 1);
									num23 >>= 1;
								}
							}
							else if (num13 == 1)
							{
								array7[k] = (byte)(num23 & 3);
								num23 >>= 2;
							}
							else
							{
								array6[k] = (byte)(num23 & 3);
								num23 >>= 2;
							}
						}
						num23 = num7 >> 17;
					}
					if (BPTCTables.IndexBits2[num8] > 0)
					{
						uint num24 = (uint)((1 << (int)BPTCTables.IndexBits2[num8]) - 1);
						uint num25 = (uint)((1 << BPTCTables.IndexBits2[num8] - 1) - 1);
						for (int k = 0; k < 16; k++)
						{
							if (k == array5[array4[k]])
							{
								if (num13 == 1)
								{
									array6[k] = (byte)(num23 & 3);
									num23 >>= 2;
								}
								else
								{
									array7[k] = (byte)(num23 & num25);
									num23 >>= BPTCTables.IndexBits2[num8] - 1;
								}
							}
							else if (num13 == 1)
							{
								array6[k] = (byte)(num23 & 7);
								num23 >>= 3;
							}
							else
							{
								array7[k] = (byte)(num23 & num24);
								num23 >>= (int)BPTCTables.IndexBits2[num8];
							}
						}
					}
					for (int k = 0; k < 16; k++)
					{
						for (int l = 0; l < 4; l++)
						{
							array8[l] = array3[2 * array4[k] * 4 + l];
							array9[l] = array3[(2 * array4[k] + 1) * 4 + l];
						}
						int num26 = j * 4 * 4 + k % 4 * 4 + i * width * 4 * 4 + (k >> 2) * width * 4;
						switch (num12)
						{
						case 0:
							array[num26] = Interpolate(array8[2], array9[2], array6[k], indexPrecision2);
							array[num26 + 1] = Interpolate(array8[1], array9[1], array6[k], indexPrecision2);
							array[num26 + 2] = Interpolate(array8[0], array9[0], array6[k], indexPrecision2);
							array[num26 + 3] = Interpolate(array8[3], array9[3], array7[k], indexPrecision);
							break;
						case 1:
							array[num26] = Interpolate(array8[2], array9[2], array6[k], indexPrecision2);
							array[num26 + 1] = Interpolate(array8[1], array9[1], array6[k], indexPrecision2);
							array[num26 + 2] = Interpolate(array8[3], array9[3], array7[k], indexPrecision);
							array[num26 + 3] = Interpolate(array8[0], array9[0], array6[k], indexPrecision2);
							break;
						case 2:
							array[num26] = Interpolate(array8[2], array9[2], array6[k], indexPrecision2);
							array[num26 + 1] = Interpolate(array8[3], array9[3], array7[k], indexPrecision);
							array[num26 + 2] = Interpolate(array8[0], array9[0], array6[k], indexPrecision2);
							array[num26 + 3] = Interpolate(array8[1], array9[1], array6[k], indexPrecision2);
							break;
						default:
							array[num26] = Interpolate(array8[3], array9[3], array7[k], indexPrecision);
							array[num26 + 1] = Interpolate(array8[1], array9[1], array6[k], indexPrecision2);
							array[num26 + 2] = Interpolate(array8[0], array9[0], array6[k], indexPrecision2);
							array[num26 + 3] = Interpolate(array8[2], array9[2], array6[k], indexPrecision2);
							break;
						}
					}
				}
				num4 += 16;
			}
		}
		return array;
	}

	private static uint GetBits64(ulong data, int bit0, int bit1)
	{
		ulong num = (ulong)((bit1 != 63) ? ((1L << bit1 + 1) - 1) : (-1));
		return (uint)((data & num) >> bit0);
	}

	private static byte Interpolate(byte e0, byte e1, byte index, int indexPrecision)
	{
		return indexPrecision switch
		{
			2 => (byte)((long)(64 - BPTCTables.AlphaWeight2[index]) * (long)e0 + (uint)(BPTCTables.AlphaWeight2[index] * e1) + 32 >> 6), 
			3 => (byte)((long)(64 - BPTCTables.AlphaWeight3[index]) * (long)e0 + (uint)(BPTCTables.AlphaWeight3[index] * e1) + 32 >> 6), 
			_ => (byte)((long)(64 - BPTCTables.AlphaWeight4[index]) * (long)e0 + (uint)(BPTCTables.AlphaWeight4[index] * e1) + 32 >> 6), 
		};
	}
}
