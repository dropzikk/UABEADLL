using System.Text;
using LibCpp2IL.Metadata;

namespace LibCpp2IL.Reflection;

public class Il2CppTypeReflectionData
{
	public Il2CppTypeDefinition? baseType;

	public Il2CppTypeReflectionData[] genericParams;

	public bool isType;

	public bool isGenericType;

	public bool isArray;

	public Il2CppTypeReflectionData? arrayType;

	public byte arrayRank;

	public string variableGenericParamName;

	public bool isPointer;

	private string GetPtrSuffix()
	{
		if (!isPointer)
		{
			return "";
		}
		return "*";
	}

	public override string ToString()
	{
		if (isArray)
		{
			return arrayType?.ToString() + "[]".Repeat(arrayRank) + GetPtrSuffix();
		}
		if (!isType)
		{
			return variableGenericParamName + GetPtrSuffix();
		}
		if (!isGenericType)
		{
			return baseType.FullName + GetPtrSuffix();
		}
		StringBuilder stringBuilder = new StringBuilder(baseType.FullName + "<");
		Il2CppTypeReflectionData[] array = genericParams;
		foreach (Il2CppTypeReflectionData value in array)
		{
			stringBuilder.Append(value).Append(", ");
		}
		stringBuilder.Remove(stringBuilder.Length - 2, 2);
		stringBuilder.Append(">");
		return stringBuilder?.ToString() + GetPtrSuffix();
	}
}
