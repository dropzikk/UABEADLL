using System;
using System.Collections.Generic;
using System.Linq;
using LibCpp2IL.Metadata;
using LibCpp2IL.Reflection;

namespace AssetsTools.NET.Cpp2IL;

internal struct TypeDefWithSelfRef
{
	public Il2CppTypeReflectionData typeRef;

	public Il2CppTypeDefinition typeDef;

	public Dictionary<string, TypeDefWithSelfRef> typeParamToArg;

	public string[] paramNames;

	public TypeDefWithSelfRef(Il2CppTypeReflectionData typeRef)
	{
		this.typeRef = typeRef;
		typeDef = typeRef.Resolve();
		typeParamToArg = new Dictionary<string, TypeDefWithSelfRef>();
		paramNames = Array.Empty<string>();
		Il2CppTypeReflectionData il2CppTypeReflectionData = typeRef;
		if (typeRef.isArray)
		{
			il2CppTypeReflectionData = typeRef.arrayType;
		}
		if (il2CppTypeReflectionData.isGenericType)
		{
			paramNames = il2CppTypeReflectionData.baseType.GenericContainer.GenericParameters.Select((Il2CppGenericParameter p) => p.Name).ToArray();
			for (int i = 0; i < il2CppTypeReflectionData.genericParams.Length; i++)
			{
				typeParamToArg.Add(paramNames[i], new TypeDefWithSelfRef(il2CppTypeReflectionData.genericParams[i]));
			}
		}
	}

	public void AssignTypeParams(TypeDefWithSelfRef parentTypeDef)
	{
		if (parentTypeDef.typeParamToArg.Count <= 0 || !typeRef.isGenericType)
		{
			return;
		}
		for (int i = 0; i < typeRef.genericParams.Length; i++)
		{
			TypeDefWithSelfRef value = new TypeDefWithSelfRef(typeRef.genericParams[i]);
			if (!string.IsNullOrEmpty(value.typeRef.variableGenericParamName))
			{
				if (parentTypeDef.typeParamToArg.TryGetValue(value.typeRef.variableGenericParamName, out var value2))
				{
					typeParamToArg[paramNames[i]] = value2;
				}
			}
			else
			{
				value.AssignTypeParams(parentTypeDef);
				typeParamToArg[paramNames[i]] = value;
			}
		}
	}

	public TypeDefWithSelfRef SolidifyType(TypeDefWithSelfRef typeDef)
	{
		typeDef.AssignTypeParams(this);
		if (typeParamToArg.TryGetValue(typeDef.typeRef.ToString(), out var value))
		{
			return value;
		}
		return typeDef;
	}

	public static implicit operator TypeDefWithSelfRef(Il2CppTypeReflectionData typeReference)
	{
		return new TypeDefWithSelfRef(typeReference);
	}
}
