using System;
using System.IO;

namespace LibCpp2IL;

public static class LEB128
{
	private const long SIGN_EXTEND_MASK = -1L;

	private const int INT64_BITSIZE = 64;

	public static void WriteLEB128Signed(this Stream stream, long value)
	{
		stream.WriteLEB128Signed(value, out var _);
	}

	public static void WriteLEB128Signed(this Stream stream, long value, out int bytes)
	{
		bytes = 0;
		bool flag = true;
		while (flag)
		{
			byte b = (byte)(value & 0x7F);
			value >>= 7;
			bool flag2 = (b & 0x40) != 0;
			flag = (value != 0L || flag2) && !(value == -1 && flag2);
			if (flag)
			{
				b |= 0x80;
			}
			stream.WriteByte(b);
			bytes++;
		}
	}

	public static void WriteLEB128Unsigned(this Stream stream, ulong value)
	{
		stream.WriteLEB128Unsigned(value, out var _);
	}

	public static void WriteLEB128Unsigned(this Stream stream, ulong value, out int bytes)
	{
		bytes = 0;
		bool flag = true;
		while (flag)
		{
			byte b = (byte)(value & 0x7F);
			value >>= 7;
			flag = value != 0;
			if (flag)
			{
				b |= 0x80;
			}
			stream.WriteByte(b);
			bytes++;
		}
	}

	public static long ReadLEB128Signed(this Stream stream)
	{
		int bytes;
		return stream.ReadLEB128Signed(out bytes);
	}

	public static long ReadLEB128Signed(this Stream stream, out int bytes)
	{
		bytes = 0;
		long num = 0L;
		int num2 = 0;
		bool flag = true;
		bool flag2 = false;
		while (flag)
		{
			int num3 = stream.ReadByte();
			if (num3 < 0)
			{
				throw new InvalidOperationException("Unexpected end of stream");
			}
			byte num4 = (byte)num3;
			bytes++;
			flag = (num4 & 0x80) != 0;
			flag2 = (num4 & 0x40) != 0;
			long num5 = (long)num4 & 0x7FL;
			num |= num5 << num2;
			num2 += 7;
		}
		if (num2 < 64 && flag2)
		{
			num |= -1L << num2;
		}
		return num;
	}

	public static ulong ReadLEB128Unsigned(this Stream stream)
	{
		int bytes;
		return stream.ReadLEB128Unsigned(out bytes);
	}

	public static ulong ReadLEB128Unsigned(this Stream stream, out int bytes)
	{
		bytes = 0;
		ulong num = 0uL;
		int num2 = 0;
		bool flag = true;
		while (flag)
		{
			int num3 = stream.ReadByte();
			if (num3 < 0)
			{
				throw new InvalidOperationException("Unexpected end of stream");
			}
			byte num4 = (byte)num3;
			bytes++;
			flag = (num4 & 0x80) != 0;
			ulong num5 = (ulong)num4 & 0x7FuL;
			num |= num5 << num2;
			num2 += 7;
		}
		return num;
	}
}
