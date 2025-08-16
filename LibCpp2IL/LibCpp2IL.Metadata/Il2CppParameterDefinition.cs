namespace LibCpp2IL.Metadata;

public class Il2CppParameterDefinition
{
	public int nameIndex;

	public uint token;

	[Version(Max = 24f)]
	public int customAttributeIndex;

	public int typeIndex;
}
