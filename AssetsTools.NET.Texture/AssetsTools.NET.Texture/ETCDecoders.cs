namespace AssetsTools.NET.Texture;

public class ETCDecoders
{
	public static byte[] ReadETC(byte[] data, int width, int height, bool etc2 = false)
	{
		int num = width + 3 >> 2;
		int num2 = height + 3 >> 2;
		int num3 = num * num2 * 16 * 4;
		byte[] array = new byte[num3];
		int num4 = 0;
		int[] array2 = new int[3];
		int[] array3 = new int[3];
		int[][] array4 = new int[2][];
		uint[] array5 = new uint[2];
		byte[] array6 = new byte[4];
		byte[] array7 = new byte[4];
		byte[] array8 = new byte[4];
		for (int i = 0; i < num2; i++)
		{
			for (int j = 0; j < num; j++)
			{
				byte b = data[num4];
				byte b2 = data[num4 + 1];
				byte b3 = data[num4 + 2];
				byte b4 = data[num4 + 3];
				int num5 = 0;
				if (etc2 && (b4 & 2) != 0)
				{
					int num6 = b & 0xF8;
					num6 += ETCTables.Comp3BitShiftedTable[b & 7];
					int num7 = b2 & 0xF8;
					num7 += ETCTables.Comp3BitShiftedTable[b2 & 7];
					int num8 = b3 & 0xF8;
					num8 += ETCTables.Comp3BitShiftedTable[b3 & 7];
					if ((num6 & 0xFF07) != 0)
					{
						num5 = 1;
					}
					else if ((num7 & 0xFF07) != 0)
					{
						num5 = 2;
					}
					else if ((num8 & 0xFF07) != 0)
					{
						num5 = 3;
					}
				}
				if (!etc2 || (etc2 && (b4 & 2) == 0) || num5 == 0)
				{
					if ((b4 & 2) > 0)
					{
						array2[0] = b & 0xF8;
						array2[0] |= (array2[0] & 0xE0) >> 5;
						array2[1] = b2 & 0xF8;
						array2[1] |= (array2[1] & 0xE0) >> 5;
						array2[2] = b3 & 0xF8;
						array2[2] |= (array2[2] & 0xE0) >> 5;
						array3[0] = b & 0xF8;
						array3[0] += ETCTables.Comp3BitShiftedTable[b & 7];
						array3[0] |= (array3[0] & 0xE0) >> 5;
						array3[1] = b2 & 0xF8;
						array3[1] += ETCTables.Comp3BitShiftedTable[b2 & 7];
						array3[1] |= (array3[1] & 0xE0) >> 5;
						array3[2] = b3 & 0xF8;
						array3[2] += ETCTables.Comp3BitShiftedTable[b3 & 7];
						array3[2] |= (array3[2] & 0xE0) >> 5;
					}
					else
					{
						array2[0] = b & 0xF0;
						array2[0] |= array2[0] >> 4;
						array2[1] = b2 & 0xF0;
						array2[1] |= array2[1] >> 4;
						array2[2] = b3 & 0xF0;
						array2[2] |= array2[2] >> 4;
						array3[0] = b & 0xF;
						array3[0] |= array3[0] << 4;
						array3[1] = b2 & 0xF;
						array3[1] |= array3[1] << 4;
						array3[2] = b3 & 0xF;
						array3[2] |= array3[2] << 4;
					}
					array4[0] = array2;
					array4[1] = array3;
					array5[0] = (uint)((b4 & 0xE0) >> 5);
					array5[1] = (uint)((b4 & 0x1C) >> 2);
					uint num9 = (uint)((data[num4 + 4] << 24) | (data[num4 + 5] << 16) | (data[num4 + 6] << 8) | data[num4 + 7]);
					if ((b4 & 1) == 0)
					{
						for (int k = 0; k < 16; k++)
						{
							uint num10 = array5[k >> 3];
							int[] array9 = array4[k >> 3];
							int num11 = (int)(((num9 & (1 << k)) >> k) | ((num9 & (65536L << k)) >> 16 + k - 1));
							int num12 = ETCTables.ModifierTable[num10][num11];
							int num13 = j * 4 * 4 + (k >> 2) * 4 + i * width * 4 * 4 + k % 4 * width * 4;
							array[num13] = ETCTables.Clamp0To255Table[array9[2] + num12 + 255];
							array[num13 + 1] = ETCTables.Clamp0To255Table[array9[1] + num12 + 255];
							array[num13 + 2] = ETCTables.Clamp0To255Table[array9[0] + num12 + 255];
							array[num13 + 3] = byte.MaxValue;
						}
					}
					else
					{
						for (int k = 0; k < 16; k++)
						{
							uint num10 = array5[(k & 2) >> 1];
							int[] array9 = array4[(k & 2) >> 1];
							int num11 = (int)(((num9 & (1 << k)) >> k) | ((num9 & (65536L << k)) >> 16 + k - 1));
							int num12 = ETCTables.ModifierTable[num10][num11];
							int num14 = j * 4 * 4 + (k >> 2) * 4 + i * width * 4 * 4 + k % 4 * width * 4;
							array[num14] = ETCTables.Clamp0To255Table[array9[2] + num12 + 255];
							array[num14 + 1] = ETCTables.Clamp0To255Table[array9[1] + num12 + 255];
							array[num14 + 2] = ETCTables.Clamp0To255Table[array9[0] + num12 + 255];
							array[num14 + 3] = byte.MaxValue;
						}
					}
				}
				else if (num5 == 1 || num5 == 2)
				{
					if (num5 == 1)
					{
						int num15 = ((b & 0x18) >> 1) | (b & 3);
						num15 |= num15 << 4;
						int num16 = b2 & 0xF0;
						num16 |= num16 >> 4;
						int num17 = b2 & 0xF;
						num17 |= num17 << 4;
						int num18 = b3 & 0xF0;
						num18 |= num18 >> 4;
						int num19 = b3 & 0xF;
						num19 |= num19 << 4;
						int num20 = b4 & 0xF0;
						num20 |= num20 >> 4;
						int num21 = ETCTables.Etc2DistanceTable[((b4 & 0xC) >> 1) | (b4 & 1)];
						array6[0] = (byte)num15;
						array7[0] = (byte)num16;
						array8[0] = (byte)num17;
						array6[2] = (byte)num18;
						array7[2] = (byte)num19;
						array8[2] = (byte)num20;
						array6[1] = ETCTables.Clamp0To255Table[num18 + num21 + 255];
						array7[1] = ETCTables.Clamp0To255Table[num19 + num21 + 255];
						array8[1] = ETCTables.Clamp0To255Table[num20 + num21 + 255];
						array6[3] = ETCTables.Clamp0To255Table[num18 - num21 + 255];
						array7[3] = ETCTables.Clamp0To255Table[num19 - num21 + 255];
						array8[3] = ETCTables.Clamp0To255Table[num20 - num21 + 255];
					}
					else
					{
						int num15 = (b & 0x78) >> 3;
						num15 |= num15 << 4;
						int num16 = ((b & 7) << 1) | ((b2 & 0x10) >> 4);
						num16 |= num16 << 4;
						int num17 = (b2 & 8) | ((b2 & 3) << 1) | ((b3 & 0x80) >> 7);
						num17 |= num17 << 4;
						int num18 = (b3 & 0x78) >> 3;
						num18 |= num18 << 4;
						int num19 = ((b3 & 7) << 1) | ((b4 & 0x80) >> 7);
						num19 |= num19 << 4;
						int num20 = (b4 & 0x78) >> 3;
						num20 |= num20 << 4;
						int num22 = (num15 << 16) + (num16 << 8) + num17;
						int num23 = (num18 << 16) + (num19 << 8) + num20;
						int num21 = ETCTables.Etc2DistanceTable[(b4 & 4) | ((b4 & 1) << 1) | ((num22 >= num23) ? 1 : 0)];
						array6[0] = ETCTables.Clamp0To255Table[num15 + num21 + 255];
						array7[0] = ETCTables.Clamp0To255Table[num16 + num21 + 255];
						array8[0] = ETCTables.Clamp0To255Table[num17 + num21 + 255];
						array6[1] = ETCTables.Clamp0To255Table[num15 - num21 + 255];
						array7[1] = ETCTables.Clamp0To255Table[num16 - num21 + 255];
						array8[1] = ETCTables.Clamp0To255Table[num17 - num21 + 255];
						array6[2] = ETCTables.Clamp0To255Table[num18 + num21 + 255];
						array7[2] = ETCTables.Clamp0To255Table[num19 + num21 + 255];
						array8[2] = ETCTables.Clamp0To255Table[num20 + num21 + 255];
						array6[3] = ETCTables.Clamp0To255Table[num18 - num21 + 255];
						array7[3] = ETCTables.Clamp0To255Table[num19 - num21 + 255];
						array8[3] = ETCTables.Clamp0To255Table[num20 - num21 + 255];
					}
					uint num9 = (uint)((data[num4 + 4] << 24) | (data[num4 + 5] << 16) | (data[num4 + 6] << 8) | data[num4 + 7]);
					for (int k = 0; k < 16; k++)
					{
						int num11 = (int)(((num9 & (1 << k)) >> k) | ((num9 & (65536L << k)) >> 16 + k - 1));
						int num24 = j * 4 * 4 + (k >> 2) * 4 + i * width * 4 * 4 + k % 4 * width * 4;
						array[num24] = array8[num11];
						array[num24 + 1] = array7[num11];
						array[num24 + 2] = array6[num11];
						array[num24 + 3] = byte.MaxValue;
					}
				}
				else if (num5 == 3)
				{
					int num25 = (b & 0x7E) >> 1;
					int num26 = ((b & 1) << 6) | ((b2 & 0x7E) >> 1);
					int num27 = ((b2 & 1) << 5) | (b3 & 0x18) | ((b3 & 3) << 1) | ((b4 & 0x80) >> 7);
					int num28 = ((b4 & 0x7C) >> 1) | (b4 & 1);
					int num29 = (data[num4 + 4] & 0xFE) >> 1;
					int num30 = ((data[num4 + 4] & 1) << 5) | ((data[num4 + 5] & 0xF8) >> 3);
					int num31 = ((data[num4 + 5] & 7) << 3) | ((data[num4 + 6] & 0xE0) >> 5);
					int num32 = ((data[num4 + 6] & 0x1F) << 2) | ((data[num4 + 7] & 0xC0) >> 6);
					int num33 = data[num4 + 7] & 0x3F;
					num25 = (num25 << 2) | ((num25 & 0x30) >> 4);
					num26 = (num26 << 1) | ((num26 & 0x40) >> 6);
					num27 = (num27 << 2) | ((num27 & 0x30) >> 4);
					num28 = (num28 << 2) | ((num28 & 0x30) >> 4);
					num29 = (num29 << 1) | ((num29 & 0x40) >> 6);
					num30 = (num30 << 2) | ((num30 & 0x30) >> 4);
					num31 = (num31 << 2) | ((num31 & 0x30) >> 4);
					num32 = (num32 << 1) | ((num32 & 0x40) >> 6);
					num33 = (num33 << 2) | ((num33 & 0x30) >> 4);
					for (int l = 0; l < 4; l++)
					{
						for (int m = 0; m < 4; m++)
						{
							int num34 = j * 4 * 4 + m * 4 + i * width * 4 * 4 + l * width * 4;
							array[num34] = ETCTables.Clamp0To255Table[(m * (num30 - num27) + l * (num33 - num27) + 4 * num27 + 2 >> 2) + 255];
							array[num34 + 1] = ETCTables.Clamp0To255Table[(m * (num29 - num26) + l * (num32 - num26) + 4 * num26 + 2 >> 2) + 255];
							array[num34 + 2] = ETCTables.Clamp0To255Table[(m * (num28 - num25) + l * (num31 - num25) + 4 * num25 + 2 >> 2) + 255];
							array[num34 + 3] = byte.MaxValue;
						}
					}
				}
				num4 += 8;
			}
		}
		return array;
	}
}
