using System;

namespace AssetsTools.NET.Texture;

public static class RGBADecoders
{
	public static byte[] ReadR8(byte[] input, int width, int height)
	{
		byte[] array = new byte[width * height * 4];
		int num = 0;
		for (int i = 0; i < array.Length; i += 4)
		{
			byte b = input[num];
			array[i + 2] = b;
			array[i + 3] = byte.MaxValue;
			num++;
		}
		return array;
	}

	public static byte[] ReadAlpha8(byte[] input, int width, int height)
	{
		byte[] array = new byte[width * height * 4];
		int num = 0;
		for (int i = 0; i < array.Length; i += 4)
		{
			byte b = input[num];
			array[i] = byte.MaxValue;
			array[i + 1] = byte.MaxValue;
			array[i + 2] = byte.MaxValue;
			array[i + 3] = b;
			num++;
		}
		return array;
	}

	public static byte[] ReadR16(byte[] input, int width, int height)
	{
		byte[] array = new byte[width * height * 4];
		int num = 0;
		for (int i = 0; i < array.Length; i += 4)
		{
			byte b = input[num + 1];
			array[i + 2] = b;
			array[i + 3] = byte.MaxValue;
			num += 2;
		}
		return array;
	}

	public static byte[] ReadRHalf(byte[] input, int width, int height)
	{
		byte[] array = new byte[width * height * 4];
		int num = 0;
		for (int i = 0; i < array.Length; i += 4)
		{
			byte b = ReadHalf(input, num);
			array[i + 2] = b;
			array[i + 3] = byte.MaxValue;
			num += 2;
		}
		return array;
	}

	public static byte[] ReadRG16(byte[] input, int width, int height)
	{
		byte[] array = new byte[width * height * 4];
		int num = 0;
		for (int i = 0; i < array.Length; i += 4)
		{
			byte b = input[num];
			byte b2 = input[num + 1];
			array[i + 1] = b2;
			array[i + 2] = b;
			array[i + 3] = byte.MaxValue;
			num += 2;
		}
		return array;
	}

	public static byte[] ReadRGHalf(byte[] input, int width, int height)
	{
		byte[] array = new byte[width * height * 4];
		int num = 0;
		for (int i = 0; i < array.Length; i += 4)
		{
			byte b = ReadHalf(input, num);
			byte b2 = ReadHalf(input, num + 2);
			array[i + 1] = b2;
			array[i + 2] = b;
			array[i + 3] = byte.MaxValue;
			num += 4;
		}
		return array;
	}

	public static byte[] ReadRGB24(byte[] input, int width, int height)
	{
		byte[] array = new byte[width * height * 4];
		int num = 0;
		for (int i = 0; i < array.Length; i += 4)
		{
			byte b = input[num];
			byte b2 = input[num + 1];
			byte b3 = input[num + 2];
			array[i] = b3;
			array[i + 1] = b2;
			array[i + 2] = b;
			array[i + 3] = byte.MaxValue;
			num += 3;
		}
		return array;
	}

	public static byte[] ReadRGB565(byte[] input, int width, int height)
	{
		byte[] array = new byte[width * height * 4];
		int num = 0;
		for (int i = 0; i < array.Length; i += 4)
		{
			int num2 = BitConverter.ToUInt16(input, num);
			int num3 = (num2 >> 11) & 0x1F;
			num3 = (num3 << 3) | (num3 & 7);
			int num4 = (num2 >> 5) & 0x3F;
			num4 = (num4 << 2) | (num4 & 3);
			int num5 = num2 & 0x1F;
			num5 = (num5 << 3) | (num5 & 7);
			array[i] = (byte)num5;
			array[i + 1] = (byte)num4;
			array[i + 2] = (byte)num3;
			array[i + 3] = byte.MaxValue;
			num += 2;
		}
		return array;
	}

	public static byte[] ReadARGB4444(byte[] input, int width, int height)
	{
		byte[] array = new byte[width * height * 4];
		int num = 0;
		for (int i = 0; i < array.Length; i += 4)
		{
			int num2 = input[num + 1] >> 4;
			int num3 = input[num + 1] & 0xF;
			int num4 = input[num] >> 4;
			int num5 = input[num] & 0xF;
			array[i] = (byte)((num5 << 4) | num5);
			array[i + 1] = (byte)((num4 << 4) | num4);
			array[i + 2] = (byte)((num3 << 4) | num3);
			array[i + 3] = (byte)((num2 << 4) | num2);
			num += 2;
		}
		return array;
	}

	public static byte[] ReadRGBA4444(byte[] input, int width, int height)
	{
		byte[] array = new byte[width * height * 4];
		int num = 0;
		for (int i = 0; i < array.Length; i += 4)
		{
			int num2 = input[num + 1] >> 4;
			int num3 = input[num + 1] & 0xF;
			int num4 = input[num] >> 4;
			int num5 = input[num] & 0xF;
			array[i] = (byte)((num4 << 4) | num4);
			array[i + 1] = (byte)((num3 << 4) | num3);
			array[i + 2] = (byte)((num2 << 4) | num2);
			array[i + 3] = (byte)((num5 << 4) | num5);
			num += 2;
		}
		return array;
	}

	public static byte[] ReadRGBAHalf(byte[] input, int width, int height)
	{
		byte[] array = new byte[width * height * 4];
		int num = 0;
		for (int i = 0; i < array.Length; i += 4)
		{
			byte b = ReadHalf(input, num);
			byte b2 = ReadHalf(input, num + 2);
			byte b3 = ReadHalf(input, num + 4);
			byte b4 = ReadHalf(input, num + 6);
			array[i] = b3;
			array[i + 1] = b2;
			array[i + 2] = b;
			array[i + 3] = b4;
			num += 8;
		}
		return array;
	}

	public static byte[] ReadARGB32(byte[] input, int width, int height)
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

	public static byte[] ReadRGBA32(byte[] input, int width, int height)
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

	private static byte ReadHalf(byte[] input, int pos)
	{
		ushort half = BitConverter.ToUInt16(input, pos);
		float val = HalfHelper.HalfToSingle(half);
		return (byte)Math.Round(Math.Max(Math.Min(val, 1f), 0f) * 255f);
	}
}
