using System.Text;

namespace AssetsTools.NET;

public struct Hash128
{
	public byte[] data;

	public Hash128(byte[] data)
	{
		this.data = data;
	}

	public Hash128(AssetsFileReader reader)
	{
		data = reader.ReadBytes(16);
	}

	public bool IsZero()
	{
		if (data == null)
		{
			return true;
		}
		for (int i = 0; i < data.Length; i++)
		{
			if (data[i] != 0)
			{
				return false;
			}
		}
		return true;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder(data.Length * 2);
		byte[] array = data;
		foreach (byte b in array)
		{
			stringBuilder.AppendFormat("{0:x2}", b);
		}
		return stringBuilder.ToString();
	}

	public static Hash128 NewBlankHash()
	{
		Hash128 result = default(Hash128);
		result.data = new byte[16];
		return result;
	}
}
