namespace AssetsTools.NET.Texture;

public static class RGBAEncoders
{
	public static byte[] EncodeR8(byte[] input, int width, int height)
	{
		byte[] array = new byte[width * height];
		int num = 0;
		for (int i = 0; i < array.Length; i++)
		{
			byte b = input[num + 2];
			array[i] = b;
			num += 4;
		}
		return array;
	}

	public static byte[] EncodeAlpha8(byte[] input, int width, int height)
	{
		byte[] array = new byte[width * height];
		int num = 0;
		for (int i = 0; i < array.Length; i++)
		{
			byte b = input[num + 3];
			array[i] = b;
			num += 4;
		}
		return array;
	}

	public static byte[] EncodeR16(byte[] input, int width, int height)
	{
		byte[] array = new byte[width * height * 2];
		int num = 0;
		for (int i = 0; i < array.Length; i += 2)
		{
			byte b = input[num + 2];
			array[i + 1] = b;
			num += 4;
		}
		return array;
	}

	public static byte[] EncodeRHalf(byte[] input, int width, int height)
	{
		byte[] array = new byte[width * height * 2];
		int num = 0;
		for (int i = 0; i < array.Length; i += 2)
		{
			byte value = input[num + 2];
			WriteHalf(array, i, value);
			num += 4;
		}
		return array;
	}

	public static byte[] EncodeRG16(byte[] input, int width, int height)
	{
		byte[] array = new byte[width * height * 2];
		int num = 0;
		for (int i = 0; i < array.Length; i += 2)
		{
			byte b = input[num + 1];
			byte b2 = input[num + 2];
			array[i] = b2;
			array[i + 1] = b;
			num += 4;
		}
		return array;
	}

	public static byte[] EncodeRGHalf(byte[] input, int width, int height)
	{
		byte[] array = new byte[width * height * 4];
		int num = 0;
		for (int i = 0; i < array.Length; i += 4)
		{
			byte value = input[num + 1];
			byte value2 = input[num + 2];
			WriteHalf(array, i, value2);
			WriteHalf(array, i + 2, value);
			num += 4;
		}
		return array;
	}

	public static byte[] EncodeRGB24(byte[] input, int width, int height)
	{
		byte[] array = new byte[width * height * 3];
		int num = 0;
		for (int i = 0; i < array.Length; i += 3)
		{
			byte b = input[num];
			byte b2 = input[num + 1];
			byte b3 = input[num + 2];
			array[i] = b3;
			array[i + 1] = b2;
			array[i + 2] = b;
			num += 4;
		}
		return array;
	}

	public static byte[] EncodeRGB565(byte[] input, int width, int height)
	{
		byte[] array = new byte[width * height * 2];
		int num = 0;
		for (int i = 0; i < array.Length; i += 2)
		{
			byte b = input[num];
			byte b2 = input[num + 1];
			byte b3 = input[num + 2];
			int num2 = (b3 >> 3 << 11) | (b2 >> 2 << 5) | (b >> 3);
			array[i] = (byte)num2;
			array[i + 1] = (byte)(num2 >> 8);
			num += 4;
		}
		return array;
	}

	public static byte[] EncodeARGB4444(byte[] input, int width, int height)
	{
		byte[] array = new byte[width * height * 2];
		int num = 0;
		for (int i = 0; i < array.Length; i += 2)
		{
			byte b = input[num];
			byte b2 = input[num + 1];
			byte b3 = input[num + 2];
			byte b4 = input[num + 3];
			array[i] = (byte)((b2 & 0xF0) | (b >> 4));
			array[i + 1] = (byte)((b4 & 0xF0) | (b3 >> 4));
			num += 4;
		}
		return array;
	}

	public static byte[] EncodeRGBA4444(byte[] input, int width, int height)
	{
		byte[] array = new byte[width * height * 2];
		int num = 0;
		for (int i = 0; i < array.Length; i += 2)
		{
			byte b = input[num];
			byte b2 = input[num + 1];
			byte b3 = input[num + 2];
			byte b4 = input[num + 3];
			array[i] = (byte)((b & 0xF0) | (b4 >> 4));
			array[i + 1] = (byte)((b3 & 0xF0) | (b2 >> 4));
			num += 4;
		}
		return array;
	}

	public static byte[] EncodeRGBAHalf(byte[] input, int width, int height)
	{
		byte[] array = new byte[width * height * 8];
		int num = 0;
		for (int i = 0; i < array.Length; i += 8)
		{
			byte value = input[num];
			byte value2 = input[num + 1];
			byte value3 = input[num + 2];
			byte value4 = input[num + 3];
			WriteHalf(array, i, value3);
			WriteHalf(array, i + 2, value2);
			WriteHalf(array, i + 4, value);
			WriteHalf(array, i + 6, value4);
			num += 4;
		}
		return array;
	}

	public static byte[] EncodeARGB32(byte[] input, int width, int height)
	{
		byte[] array = new byte[width * height * 4];
		for (int i = 0; i < array.Length; i += 4)
		{
			byte b = input[i];
			byte b2 = input[i + 1];
			byte b3 = input[i + 2];
			byte b4 = input[i + 3];
			array[i] = b4;
			array[i + 1] = b3;
			array[i + 2] = b2;
			array[i + 3] = b;
		}
		return array;
	}

	public static byte[] EncodeRGBA32(byte[] input, int width, int height)
	{
		byte[] array = new byte[width * height * 4];
		for (int i = 0; i < array.Length; i += 4)
		{
			byte b = input[i];
			byte b2 = input[i + 1];
			byte b3 = input[i + 2];
			byte b4 = input[i + 3];
			array[i] = b3;
			array[i + 1] = b2;
			array[i + 2] = b;
			array[i + 3] = b4;
		}
		return array;
	}

	private static void WriteHalf(byte[] output, int pos, byte value)
	{
		float single = (float)(int)value / 255f;
		ushort num = HalfHelper.SingleToHalf(single);
		output[pos] = (byte)num;
		output[pos + 1] = (byte)(num >> 8);
	}
}
