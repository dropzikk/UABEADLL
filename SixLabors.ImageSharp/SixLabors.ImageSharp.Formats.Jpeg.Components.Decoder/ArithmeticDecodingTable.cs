namespace SixLabors.ImageSharp.Formats.Jpeg.Components.Decoder;

internal class ArithmeticDecodingTable
{
	public byte TableClass { get; }

	public byte Identifier { get; }

	public byte ConditioningTableValue { get; private set; }

	public int DcL { get; private set; }

	public int DcU { get; private set; }

	public int AcKx { get; private set; }

	public ArithmeticDecodingTable(byte tableClass, byte identifier)
	{
		TableClass = tableClass;
		Identifier = identifier;
	}

	public void Configure(byte conditioningTableValue)
	{
		ConditioningTableValue = conditioningTableValue;
		if (TableClass == 0)
		{
			DcL = conditioningTableValue & 0xF;
			DcU = conditioningTableValue >> 4;
			AcKx = 0;
		}
		else
		{
			DcL = 0;
			DcU = 0;
			AcKx = conditioningTableValue;
		}
	}
}
