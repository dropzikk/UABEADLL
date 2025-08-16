using System;
using System.Text;

namespace AssetsTools.NET;

public struct GUID128
{
	private const string HexToLiteral = "0123456789abcdef";

	public uint data0;

	public uint data1;

	public uint data2;

	public uint data3;

	public bool IsEmpty => data0 == 0 && data1 == 0 && data2 == 0 && data3 == 0;

	public uint this[int i]
	{
		get
		{
			if (1 == 0)
			{
			}
			uint result = i switch
			{
				0 => data0, 
				1 => data1, 
				2 => data2, 
				3 => data3, 
				_ => throw new IndexOutOfRangeException(), 
			};
			if (1 == 0)
			{
			}
			return result;
		}
		set
		{
			switch (i)
			{
			case 0:
				data0 = value;
				break;
			case 1:
				data1 = value;
				break;
			case 2:
				data2 = value;
				break;
			case 3:
				data3 = value;
				break;
			default:
				throw new IndexOutOfRangeException();
			}
		}
	}

	public GUID128(AssetsFileReader reader)
	{
		this = default(GUID128);
		Read(reader);
	}

	public void Read(AssetsFileReader reader)
	{
		data0 = reader.ReadUInt32();
		data1 = reader.ReadUInt32();
		data2 = reader.ReadUInt32();
		data3 = reader.ReadUInt32();
	}

	public void Write(AssetsFileWriter writer)
	{
		writer.Write(data0);
		writer.Write(data1);
		writer.Write(data2);
		writer.Write(data3);
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder(32);
		for (int num = 3; num >= 0; num--)
		{
			for (int num2 = 7; num2 >= 0; num2--)
			{
				uint num3 = this[num];
				num3 >>= num2 * 4;
				num3 &= 0xF;
				stringBuilder.Insert(0, "0123456789abcdef"[(int)num3]);
			}
		}
		return stringBuilder.ToString();
	}

	public static bool TryParse(string str, out GUID128 guid)
	{
		guid = default(GUID128);
		if (str.Length != 32)
		{
			return false;
		}
		for (int i = 0; i < 4; i++)
		{
			uint num = 0u;
			for (int num2 = 7; num2 >= 0; num2--)
			{
				uint num3 = LiteralToHex(str[i * 8 + num2]);
				if (num3 == uint.MaxValue)
				{
					return false;
				}
				num |= num3 << num2 * 4;
			}
			guid[i] = num;
		}
		return true;
	}

	private static uint LiteralToHex(char c)
	{
		if (1 == 0)
		{
		}
		uint result = c switch
		{
			'0' => 0u, 
			'1' => 1u, 
			'2' => 2u, 
			'3' => 3u, 
			'4' => 4u, 
			'5' => 5u, 
			'6' => 6u, 
			'7' => 7u, 
			'8' => 8u, 
			'9' => 9u, 
			'a' => 10u, 
			'b' => 11u, 
			'c' => 12u, 
			'd' => 13u, 
			'e' => 14u, 
			'f' => 15u, 
			_ => uint.MaxValue, 
		};
		if (1 == 0)
		{
		}
		return result;
	}
}
