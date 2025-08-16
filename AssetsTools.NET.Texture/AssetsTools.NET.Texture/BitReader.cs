namespace AssetsTools.NET.Texture;

internal class BitReader
{
	public int index;

	public ulong data0;

	public ulong data1;

	public void Reset(ulong data0, ulong data1)
	{
		index = 0;
		this.data0 = data0;
		this.data1 = data1;
	}

	public int ReadNumber(int bitCount)
	{
		ulong num = 0uL;
		for (int i = 0; i < bitCount; i++)
		{
			if (index < 64)
			{
				int num2 = index - i;
				num = ((num2 >= 0) ? (num | ((data0 & (ulong)(1L << index)) >> num2)) : (num | ((data0 & (ulong)(1L << index)) << -num2)));
			}
			else
			{
				int num3 = index - 64 - i;
				num = ((num3 >= 0) ? (num | ((data1 & (ulong)(1L << index - 64)) >> num3)) : (num | ((data1 & (ulong)(1L << index - 64)) << -num3)));
			}
			index++;
		}
		return (int)num;
	}
}
