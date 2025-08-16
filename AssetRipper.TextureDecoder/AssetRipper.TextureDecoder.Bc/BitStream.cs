namespace AssetRipper.TextureDecoder.Bc;

internal struct BitStream
{
	public ulong low;

	public ulong high;

	public int ReadBits(int numBits)
	{
		uint num = (uint)((1 << numBits) - 1);
		int result = (int)(low & num);
		low >>= numBits;
		low |= (high & num) << 64 - numBits;
		high >>= numBits;
		return result;
	}

	public int ReadBit()
	{
		return ReadBits(1);
	}

	public int ReadBitsReversed(int numBits)
	{
		int num = ReadBits(numBits);
		int num2 = 0;
		while (numBits-- != 0)
		{
			num2 <<= 1;
			num2 |= num & 1;
			num >>= 1;
		}
		return num2;
	}
}
