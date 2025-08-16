namespace AssetsTools.NET.Texture;

public static class DXTDecoders
{
	public static byte[] ReadDXT1(byte[] data, int width, int height, bool dxt1a = false)
	{
		int num = width + 3 >> 2;
		int num2 = height + 3 >> 2;
		int num3 = num * num2 * 16 * 4;
		byte[] array = new byte[num3];
		int num4 = 0;
		uint[] array2 = new uint[4];
		uint[] array3 = new uint[4];
		uint[] array4 = new uint[4];
		byte b = (byte)((!dxt1a) ? 255u : 0u);
		for (int i = 0; i < num2; i++)
		{
			for (int j = 0; j < num; j++)
			{
				uint num5 = (uint)(data[num4] | (data[num4 + 1] << 8) | (data[num4 + 2] << 16) | (data[num4 + 3] << 24));
				uint num6 = (uint)(data[num4 + 4] | (data[num4 + 5] << 8) | (data[num4 + 6] << 16) | (data[num4 + 7] << 24));
				bool flag = (num5 & 0xFFFF) > (num5 & 0xFFFF0000u) >> 16;
				array4[0] = (num5 & 0x1F) << 3;
				array3[0] = (num5 & 0x7E0) >> 3;
				array2[0] = (num5 & 0xF800) >> 8;
				array4[1] = (num5 & 0x1F0000) >> 13;
				array3[1] = (num5 & 0x7E00000) >> 19;
				array2[1] = (num5 & 0xF8000000u) >> 24;
				int num7 = 255;
				if (flag)
				{
					array2[2] = DivisionTables.DivideBy3[2 * array2[0] + array2[1]];
					array3[2] = DivisionTables.DivideBy3[2 * array3[0] + array3[1]];
					array4[2] = DivisionTables.DivideBy3[2 * array4[0] + array4[1]];
					array2[3] = DivisionTables.DivideBy3[array2[0] + 2 * array2[1]];
					array3[3] = DivisionTables.DivideBy3[array3[0] + 2 * array3[1]];
					array4[3] = DivisionTables.DivideBy3[array4[0] + 2 * array4[1]];
				}
				else
				{
					array2[2] = (array2[0] + array2[1]) / 2;
					array3[2] = (array3[0] + array3[1]) / 2;
					array4[2] = (array4[0] + array4[1]) / 2;
					array2[3] = 0u;
					array3[3] = 0u;
					array4[3] = 0u;
					num7 = b;
				}
				for (int k = 0; k < 16; k++)
				{
					uint num8 = (num6 >> k * 2) & 3;
					int num9 = j * 4 * 4 + k % 4 * 4 + i * width * 4 * 4 + (k >> 2) * width * 4;
					array[num9] = (byte)array4[num8];
					array[num9 + 1] = (byte)array3[num8];
					array[num9 + 2] = (byte)array2[num8];
					array[num9 + 3] = (byte)num7;
				}
				num4 += 8;
			}
		}
		return array;
	}

	public static byte[] ReadDXT5(byte[] data, int width, int height)
	{
		int num = width + 3 >> 2;
		int num2 = height + 3 >> 2;
		int num3 = num * num2 * 16 * 4;
		byte[] array = new byte[num3];
		int num4 = 0;
		uint[] array2 = new uint[4];
		uint[] array3 = new uint[4];
		uint[] array4 = new uint[4];
		for (int i = 0; i < num2; i++)
		{
			for (int j = 0; j < num; j++)
			{
				int num5 = data[num4];
				int num6 = data[num4 + 1];
				ulong num7 = data[num4 + 2] | ((ulong)data[num4 + 3] << 8) | ((ulong)data[num4 + 4] << 16) | ((ulong)data[num4 + 5] << 24) | ((ulong)data[num4 + 6] << 32) | ((ulong)data[num4 + 7] << 40);
				uint num8 = (uint)(data[num4 + 8] | (data[num4 + 9] << 8) | (data[num4 + 10] << 16) | (data[num4 + 11] << 24));
				uint num9 = (uint)(data[num4 + 12] | (data[num4 + 13] << 8) | (data[num4 + 14] << 16) | (data[num4 + 15] << 24));
				array4[0] = (num8 & 0x1F) << 3;
				array3[0] = (num8 & 0x7E0) >> 3;
				array2[0] = (num8 & 0xF800) >> 8;
				array4[1] = (num8 & 0x1F0000) >> 13;
				array3[1] = (num8 & 0x7E00000) >> 19;
				array2[1] = (num8 & 0xF8000000u) >> 24;
				array2[2] = DivisionTables.DivideBy3[(array2[0] << 1) + array2[1]];
				array3[2] = DivisionTables.DivideBy3[(array3[0] << 1) + array3[1]];
				array4[2] = DivisionTables.DivideBy3[(array4[0] << 1) + array4[1]];
				array2[3] = DivisionTables.DivideBy3[array2[0] + (array2[1] << 1)];
				array3[3] = DivisionTables.DivideBy3[array3[0] + (array3[1] << 1)];
				array4[3] = DivisionTables.DivideBy3[array4[0] + (array4[1] << 1)];
				for (int k = 0; k < 16; k++)
				{
					uint num10 = (num9 >> k * 2) & 3;
					int num11 = (int)((num7 >> k * 3) & 7);
					int num12 = ((num5 <= num6) ? (num11 switch
					{
						0 => num5, 
						1 => num6, 
						2 => DivisionTables.DivideBy5[4 * num5 + num6], 
						3 => DivisionTables.DivideBy5[3 * num5 + 2 * num6], 
						4 => DivisionTables.DivideBy5[2 * num5 + 3 * num6], 
						5 => DivisionTables.DivideBy5[num5 + 4 * num6], 
						6 => 0, 
						7 => 255, 
						_ => 0, 
					}) : (num11 switch
					{
						0 => num5, 
						1 => num6, 
						2 => DivisionTables.DivideBy7[6 * num5 + num6], 
						3 => DivisionTables.DivideBy7[5 * num5 + 2 * num6], 
						4 => DivisionTables.DivideBy7[4 * num5 + 3 * num6], 
						5 => DivisionTables.DivideBy7[3 * num5 + 4 * num6], 
						6 => DivisionTables.DivideBy7[2 * num5 + 5 * num6], 
						7 => DivisionTables.DivideBy7[num5 + 6 * num6], 
						_ => 0, 
					}));
					int num13 = j * 4 * 4 + k % 4 * 4 + i * width * 4 * 4 + (k >> 2) * width * 4;
					array[num13] = (byte)array4[num10];
					array[num13 + 1] = (byte)array3[num10];
					array[num13 + 2] = (byte)array2[num10];
					array[num13 + 3] = (byte)num12;
				}
				num4 += 16;
			}
		}
		return array;
	}
}
