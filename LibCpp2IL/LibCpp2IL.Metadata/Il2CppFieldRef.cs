using LibCpp2IL.BinaryStructures;

namespace LibCpp2IL.Metadata;

public class Il2CppFieldRef
{
	public int typeIndex;

	public int fieldIndex;

	public Il2CppType? DeclaringType => LibCpp2IlMain.Binary?.GetType(typeIndex);

	public Il2CppTypeDefinition? DeclaringTypeDefinition
	{
		get
		{
			Il2CppMetadata? theMetadata = LibCpp2IlMain.TheMetadata;
			if (theMetadata == null)
			{
				return null;
			}
			return theMetadata.typeDefs[DeclaringType.data.classIndex];
		}
	}

	public Il2CppFieldDefinition? FieldDefinition
	{
		get
		{
			Il2CppMetadata? theMetadata = LibCpp2IlMain.TheMetadata;
			if (theMetadata == null)
			{
				return null;
			}
			return theMetadata.fieldDefs[DeclaringTypeDefinition.firstFieldIdx + fieldIndex];
		}
	}
}
