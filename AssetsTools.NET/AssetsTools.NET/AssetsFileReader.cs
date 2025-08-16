using System;
using System.IO;
using System.Linq;
using System.Text;

namespace AssetsTools.NET;

public class AssetsFileReader : BinaryReader
{
	public bool BigEndian { get; set; } = false;

	public long Position
	{
		get
		{
			return BaseStream.Position;
		}
		set
		{
			BaseStream.Position = value;
		}
	}

	public AssetsFileReader(string filePath)
		: base(File.OpenRead(filePath))
	{
	}

	public AssetsFileReader(Stream stream)
		: base(stream)
	{
	}

	public override short ReadInt16()
	{
		return BigEndian ? ((short)ReverseShort((ushort)base.ReadInt16())) : base.ReadInt16();
	}

	public override ushort ReadUInt16()
	{
		return BigEndian ? ReverseShort(base.ReadUInt16()) : base.ReadUInt16();
	}

	public int ReadInt24()
	{
		return BigEndian ? ((int)ReverseInt((uint)BitConverter.ToInt32(ReadBytes(3).Concat(new byte[1]).ToArray(), 0))) : BitConverter.ToInt32(ReadBytes(3).Concat(new byte[1]).ToArray(), 0);
	}

	public uint ReadUInt24()
	{
		return BigEndian ? ReverseInt(BitConverter.ToUInt32(ReadBytes(3).Concat(new byte[1]).ToArray(), 0)) : BitConverter.ToUInt32(ReadBytes(3).Concat(new byte[1]).ToArray(), 0);
	}

	public override int ReadInt32()
	{
		return BigEndian ? ((int)ReverseInt((uint)base.ReadInt32())) : base.ReadInt32();
	}

	public override uint ReadUInt32()
	{
		return BigEndian ? ReverseInt(base.ReadUInt32()) : base.ReadUInt32();
	}

	public override long ReadInt64()
	{
		return BigEndian ? ((long)ReverseLong((ulong)base.ReadInt64())) : base.ReadInt64();
	}

	public override ulong ReadUInt64()
	{
		return BigEndian ? ReverseLong(base.ReadUInt64()) : base.ReadUInt64();
	}

	public ushort ReverseShort(ushort value)
	{
		return (ushort)(((value & 0xFF00) >> 8) | ((value & 0xFF) << 8));
	}

	public uint ReverseInt(uint value)
	{
		value = (value >> 16) | (value << 16);
		return ((value & 0xFF00FF00u) >> 8) | ((value & 0xFF00FF) << 8);
	}

	public ulong ReverseLong(ulong value)
	{
		value = (value >> 32) | (value << 32);
		value = ((value & 0xFFFF0000FFFF0000uL) >> 16) | ((value & 0xFFFF0000FFFFL) << 16);
		return ((value & 0xFF00FF00FF00FF00uL) >> 8) | ((value & 0xFF00FF00FF00FFL) << 8);
	}

	public void Align()
	{
		long num = 4 - BaseStream.Position % 4;
		if (num != 4)
		{
			BaseStream.Position += num;
		}
	}

	public void Align8()
	{
		long num = 8 - BaseStream.Position % 8;
		if (num != 8)
		{
			BaseStream.Position += num;
		}
	}

	public void Align16()
	{
		long num = 16 - BaseStream.Position % 16;
		if (num != 16)
		{
			BaseStream.Position += num;
		}
	}

	public string ReadStringLength(int len)
	{
		return Encoding.UTF8.GetString(ReadBytes(len));
	}

	public string ReadNullTerminated()
	{
		string text = "";
		char c;
		while ((c = ReadChar()) != 0)
		{
			text += c;
		}
		return text;
	}

	public static string ReadNullTerminatedArray(byte[] bytes, uint pos)
	{
		StringBuilder stringBuilder = new StringBuilder();
		char value;
		while ((value = (char)bytes[pos]) != 0)
		{
			stringBuilder.Append(value);
			pos++;
		}
		return stringBuilder.ToString();
	}

	public string ReadCountString()
	{
		byte len = ReadByte();
		return ReadStringLength(len);
	}

	public string ReadCountStringInt16()
	{
		ushort len = ReadUInt16();
		return ReadStringLength(len);
	}

	public string ReadCountStringInt32()
	{
		int len = ReadInt32();
		return ReadStringLength(len);
	}
}
