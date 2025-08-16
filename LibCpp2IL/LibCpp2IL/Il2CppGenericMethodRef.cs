using System.Linq;
using System.Text;
using LibCpp2IL.BinaryStructures;
using LibCpp2IL.Metadata;
using LibCpp2IL.Reflection;

namespace LibCpp2IL;

public class Il2CppGenericMethodRef
{
	public readonly Il2CppTypeDefinition DeclaringType;

	public readonly Il2CppTypeReflectionData[] TypeGenericParams;

	public readonly Il2CppMethodDefinition BaseMethod;

	public readonly Il2CppTypeReflectionData[] MethodGenericParams;

	public ulong GenericVariantPtr;

	public Il2CppGenericMethodRef(Il2CppMethodSpec methodSpec)
	{
		Il2CppTypeReflectionData[] typeGenericParams = new Il2CppTypeReflectionData[0];
		if (methodSpec.classIndexIndex != -1)
		{
			typeGenericParams = LibCpp2ILUtils.GetGenericTypeParams(methodSpec.GenericClassInst);
		}
		Il2CppTypeReflectionData[] methodGenericParams = new Il2CppTypeReflectionData[0];
		if (methodSpec.methodIndexIndex != -1)
		{
			methodGenericParams = LibCpp2ILUtils.GetGenericTypeParams(methodSpec.GenericMethodInst);
		}
		BaseMethod = methodSpec.MethodDefinition;
		DeclaringType = methodSpec.MethodDefinition.DeclaringType;
		TypeGenericParams = typeGenericParams;
		MethodGenericParams = methodGenericParams;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(BaseMethod?.ReturnType).Append(" ");
		stringBuilder.Append(DeclaringType.FullName);
		if (TypeGenericParams.Length != 0)
		{
			stringBuilder.Append("<").Append(string.Join(", ", TypeGenericParams.AsEnumerable())).Append(">");
		}
		stringBuilder.Append(".").Append(BaseMethod?.Name);
		if (MethodGenericParams.Length != 0)
		{
			stringBuilder.Append("<").Append(string.Join(", ", MethodGenericParams.AsEnumerable())).Append(">");
		}
		return stringBuilder.ToString();
	}
}
