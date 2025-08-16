using System;
using System.IO;

namespace LibCpp2IL;

public static class BinaryReaderHelpers
{
	public static byte[] Reverse(this byte[] b)
	{
		Array.Reverse((Array)b);
		return b;
	}

	public static ushort ReadUInt16WithReversedBits(this BinaryReader binRdr)
	{
		return BitConverter.ToUInt16(binRdr.ReadBytesRequired(2).Reverse(), 0);
	}

	public static short ReadInt16WithReversedBits(this BinaryReader binRdr)
	{
		return BitConverter.ToInt16(binRdr.ReadBytesRequired(2).Reverse(), 0);
	}

	public static uint ReadUInt32WithReversedBits(this BinaryReader binRdr)
	{
		return BitConverter.ToUInt32(binRdr.ReadBytesRequired(4).Reverse(), 0);
	}

	public static int ReadInt32WithReversedBits(this BinaryReader binRdr)
	{
		return BitConverter.ToInt32(binRdr.ReadBytesRequired(4).Reverse(), 0);
	}

	public static ulong ReadUInt64WithReversedBits(this BinaryReader binRdr)
	{
		return BitConverter.ToUInt64(binRdr.ReadBytesRequired(8).Reverse(), 0);
	}

	public static long ReadInt64WithReversedBits(this BinaryReader binRdr)
	{
		return BitConverter.ToInt64(binRdr.ReadBytesRequired(8).Reverse(), 0);
	}

	public static float ReadSingleWithReversedBits(this BinaryReader binRdr)
	{
		return BitConverter.ToSingle(binRdr.ReadBytesRequired(4).Reverse(), 0);
	}

	public static double ReadDoubleWithReversedBits(this BinaryReader binRdr)
	{
		return BitConverter.ToDouble(binRdr.ReadBytesRequired(8).Reverse(), 0);
	}

	private static byte[] ReadBytesRequired(this BinaryReader binRdr, int byteCount)
	{
		byte[] array = binRdr.ReadBytes(byteCount);
		if (array.Length != byteCount)
		{
			throw new EndOfStreamException($"{byteCount} bytes required from stream, but only {array.Length} returned.");
		}
		return array;
	}
}
