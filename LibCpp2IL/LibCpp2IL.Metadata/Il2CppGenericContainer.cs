using System.Collections.Generic;

namespace LibCpp2IL.Metadata;

public class Il2CppGenericContainer
{
	public int ownerIndex;

	public int type_argc;

	public int is_method;

	public int genericParameterStart;

	public IEnumerable<Il2CppGenericParameter> GenericParameters
	{
		get
		{
			if (type_argc != 0)
			{
				int end = genericParameterStart + type_argc;
				for (int i = genericParameterStart; i < end; i++)
				{
					Il2CppGenericParameter il2CppGenericParameter = LibCpp2IlMain.TheMetadata.genericParameters[i];
					il2CppGenericParameter.Index = i;
					yield return il2CppGenericParameter;
				}
			}
		}
	}
}
