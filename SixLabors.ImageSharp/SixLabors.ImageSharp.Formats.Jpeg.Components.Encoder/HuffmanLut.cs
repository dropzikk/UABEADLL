namespace SixLabors.ImageSharp.Formats.Jpeg.Components.Encoder;

internal readonly struct HuffmanLut
{
	public int[] Values { get; }

	public HuffmanLut(HuffmanSpec spec)
	{
		int num = 0;
		byte[] values = spec.Values;
		foreach (byte b in values)
		{
			if (b > num)
			{
				num = b;
			}
		}
		Values = new int[num + 1];
		int num2 = 0;
		int num3 = 0;
		for (int j = 0; j < spec.Count.Length; j++)
		{
			int num4 = j + 1;
			for (int k = 0; k < spec.Count[j]; k++)
			{
				Values[spec.Values[num3]] = num4 | (num2 << 32 - num4);
				num2++;
				num3++;
			}
			num2 <<= 1;
		}
	}
}
