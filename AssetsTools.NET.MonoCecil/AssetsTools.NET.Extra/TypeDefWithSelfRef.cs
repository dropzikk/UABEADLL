using System.Collections.Generic;
using Mono.Cecil;

namespace AssetsTools.NET.Extra;

internal struct TypeDefWithSelfRef
{
	public TypeReference typeRef;

	public TypeDefinition typeDef;

	public Dictionary<string, TypeDefWithSelfRef> typeParamToArg;

	public TypeDefWithSelfRef(TypeReference typeRef)
	{
		this.typeRef = typeRef;
		typeDef = typeRef.Resolve();
		typeParamToArg = new Dictionary<string, TypeDefWithSelfRef>();
		TypeReference typeReference = typeRef;
		if (typeReference is ArrayType arrayType)
		{
			typeReference = arrayType.ElementType;
		}
		if (typeReference is GenericInstanceType { HasGenericArguments: not false } genericInstanceType)
		{
			for (int i = 0; i < genericInstanceType.GenericArguments.Count; i++)
			{
				typeParamToArg.Add(typeDef.GenericParameters[i].Name, new TypeDefWithSelfRef(genericInstanceType.GenericArguments[i]));
			}
		}
	}

	public void AssignTypeParams(TypeDefWithSelfRef parentTypeDef)
	{
		if (parentTypeDef.typeParamToArg.Count <= 0 || !(typeRef is GenericInstanceType genericInstanceType))
		{
			return;
		}
		for (int i = 0; i < genericInstanceType.GenericArguments.Count; i++)
		{
			TypeDefWithSelfRef value = new TypeDefWithSelfRef(genericInstanceType.GenericArguments[i]);
			if (value.typeRef.IsGenericParameter)
			{
				if (parentTypeDef.typeParamToArg.TryGetValue(value.typeRef.Name, out var value2))
				{
					typeParamToArg[typeDef.GenericParameters[i].Name] = value2;
				}
			}
			else
			{
				value.AssignTypeParams(parentTypeDef);
				typeParamToArg[typeDef.GenericParameters[i].Name] = value;
			}
		}
	}

	public TypeDefWithSelfRef SolidifyType(TypeDefWithSelfRef typeDef)
	{
		typeDef.AssignTypeParams(this);
		if (typeParamToArg.TryGetValue(typeDef.typeRef.Name, out var value))
		{
			return value;
		}
		return typeDef;
	}

	public static implicit operator TypeDefWithSelfRef(TypeReference typeReference)
	{
		return new TypeDefWithSelfRef(typeReference);
	}
}
