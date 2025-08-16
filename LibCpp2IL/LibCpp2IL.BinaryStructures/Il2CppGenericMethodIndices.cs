namespace LibCpp2IL.BinaryStructures;

public class Il2CppGenericMethodIndices
{
	public int methodIndex;

	public int invokerIndex;

	[Version(Min = 27.1f)]
	[Version(Min = 24.5f, Max = 24.5f)]
	public int adjustorThunk;
}
