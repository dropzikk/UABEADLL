namespace LibCpp2IL.Metadata;

public class Il2CppFieldDefaultValue
{
	public int fieldIndex;

	public int typeIndex;

	public int dataIndex;

	public object? Value
	{
		get
		{
			if (dataIndex > 0)
			{
				return LibCpp2ILUtils.GetDefaultValue(dataIndex, typeIndex);
			}
			return null;
		}
	}
}
