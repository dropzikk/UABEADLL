using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LibCpp2IL.BinaryStructures;
using LibCpp2IL.Logging;
using LibCpp2IL.Metadata;
using LibCpp2IL.Reflection;

namespace LibCpp2IL;

public static class LibCpp2ILUtils
{
	private static readonly Dictionary<int, string> TypeString = new Dictionary<int, string>
	{
		{ 1, "void" },
		{ 2, "bool" },
		{ 3, "char" },
		{ 4, "sbyte" },
		{ 5, "byte" },
		{ 6, "short" },
		{ 7, "ushort" },
		{ 8, "int" },
		{ 9, "uint" },
		{ 10, "long" },
		{ 11, "ulong" },
		{ 12, "float" },
		{ 13, "double" },
		{ 14, "string" },
		{ 22, "TypedReference" },
		{ 24, "IntPtr" },
		{ 25, "UIntPtr" },
		{ 28, "object" }
	};

	private static readonly Dictionary<string, ulong> PrimitiveSizes = new Dictionary<string, ulong>
	{
		{ "Byte", 1uL },
		{ "SByte", 1uL },
		{ "Boolean", 1uL },
		{ "Int16", 2uL },
		{ "UInt16", 2uL },
		{ "Char", 2uL },
		{ "Int32", 4uL },
		{ "UInt32", 4uL },
		{ "Single", 4uL },
		{ "Int64", 8uL },
		{ "UInt64", 8uL },
		{ "Double", 8uL },
		{ "IntPtr", 8uL },
		{ "UIntPtr", 8uL }
	};

	private static Dictionary<FieldInfo, VersionAttribute[]> _cachedVersionAttributes = new Dictionary<FieldInfo, VersionAttribute[]>();

	internal static void Reset()
	{
		_cachedVersionAttributes.Clear();
	}

	internal static string GetTypeName(Il2CppMetadata metadata, Il2CppBinary cppAssembly, Il2CppTypeDefinition typeDef, bool fullName = false)
	{
		string text = string.Empty;
		if (fullName)
		{
			text = typeDef.Namespace;
			if (text != string.Empty)
			{
				text += ".";
			}
		}
		if (typeDef.declaringTypeIndex != -1)
		{
			text = text + GetTypeName(metadata, cppAssembly, cppAssembly.GetType(typeDef.declaringTypeIndex)) + ".";
		}
		text += metadata.GetStringFromIndex(typeDef.nameIndex);
		List<string> list = new List<string>();
		if (typeDef.genericContainerIndex < 0)
		{
			return text;
		}
		Il2CppGenericContainer il2CppGenericContainer = metadata.genericContainers[typeDef.genericContainerIndex];
		for (int i = 0; i < il2CppGenericContainer.type_argc; i++)
		{
			int num = il2CppGenericContainer.genericParameterStart + i;
			Il2CppGenericParameter il2CppGenericParameter = metadata.genericParameters[num];
			list.Add(metadata.GetStringFromIndex(il2CppGenericParameter.nameIndex));
		}
		text = text.Replace($"`{il2CppGenericContainer.type_argc}", "");
		return text + "<" + string.Join(", ", list) + ">";
	}

	internal static Il2CppTypeReflectionData[]? GetGenericTypeParams(Il2CppGenericInst genericInst)
	{
		if (LibCpp2IlMain.Binary == null || LibCpp2IlMain.TheMetadata == null)
		{
			return null;
		}
		List<Il2CppTypeReflectionData> list = new List<Il2CppTypeReflectionData>();
		ulong[] array = LibCpp2IlMain.Binary.ReadClassArrayAtVirtualAddress<ulong>(genericInst.pointerStart, (long)genericInst.pointerCount);
		for (uint num = 0u; num < genericInst.pointerCount; num++)
		{
			Il2CppType il2CppTypeFromPointer = LibCpp2IlMain.Binary.GetIl2CppTypeFromPointer(array[num]);
			list.Add(GetTypeReflectionData(il2CppTypeFromPointer));
		}
		return list.ToArray();
	}

	internal static string GetGenericTypeParamNames(Il2CppMetadata metadata, Il2CppBinary cppAssembly, Il2CppGenericInst genericInst)
	{
		List<string> list = new List<string>();
		ulong[] array = cppAssembly.ReadClassArrayAtVirtualAddress<ulong>(genericInst.pointerStart, (long)genericInst.pointerCount);
		for (uint num = 0u; num < genericInst.pointerCount; num++)
		{
			Il2CppType il2CppTypeFromPointer = cppAssembly.GetIl2CppTypeFromPointer(array[num]);
			list.Add(GetTypeName(metadata, cppAssembly, il2CppTypeFromPointer));
		}
		return "<" + string.Join(", ", list) + ">";
	}

	public static string GetTypeName(Il2CppMetadata metadata, Il2CppBinary cppAssembly, Il2CppType type, bool fullName = false)
	{
		switch (type.type)
		{
		case Il2CppTypeEnum.IL2CPP_TYPE_VALUETYPE:
		case Il2CppTypeEnum.IL2CPP_TYPE_CLASS:
		{
			Il2CppTypeDefinition typeDef = metadata.typeDefs[type.data.classIndex];
			string stringFromIndex = string.Empty;
			return stringFromIndex + GetTypeName(metadata, cppAssembly, typeDef, fullName);
		}
		case Il2CppTypeEnum.IL2CPP_TYPE_GENERICINST:
		{
			Il2CppGenericClass il2CppGenericClass = cppAssembly.ReadClassAtVirtualAddress<Il2CppGenericClass>(type.data.generic_class);
			Il2CppTypeDefinition il2CppTypeDefinition = metadata.typeDefs[il2CppGenericClass.typeDefinitionIndex];
			string stringFromIndex = metadata.GetStringFromIndex(il2CppTypeDefinition.nameIndex);
			Il2CppGenericInst il2CppGenericInst = cppAssembly.ReadClassAtVirtualAddress<Il2CppGenericInst>(il2CppGenericClass.context.class_inst);
			stringFromIndex = stringFromIndex.Replace($"`{il2CppGenericInst.pointerCount}", "");
			return stringFromIndex + GetGenericTypeParamNames(metadata, cppAssembly, il2CppGenericInst);
		}
		case Il2CppTypeEnum.IL2CPP_TYPE_VAR:
		case Il2CppTypeEnum.IL2CPP_TYPE_MVAR:
		{
			Il2CppGenericParameter il2CppGenericParameter = metadata.genericParameters[type.data.genericParameterIndex];
			return metadata.GetStringFromIndex(il2CppGenericParameter.nameIndex);
		}
		case Il2CppTypeEnum.IL2CPP_TYPE_ARRAY:
		{
			Il2CppArrayType il2CppArrayType = cppAssembly.ReadClassAtVirtualAddress<Il2CppArrayType>(type.data.array);
			Il2CppType il2CppTypeFromPointer3 = cppAssembly.GetIl2CppTypeFromPointer(il2CppArrayType.etype);
			return GetTypeName(metadata, cppAssembly, il2CppTypeFromPointer3) + "[" + new string(',', il2CppArrayType.rank - 1) + "]";
		}
		case Il2CppTypeEnum.IL2CPP_TYPE_SZARRAY:
		{
			Il2CppType il2CppTypeFromPointer2 = cppAssembly.GetIl2CppTypeFromPointer(type.data.type);
			return GetTypeName(metadata, cppAssembly, il2CppTypeFromPointer2) + "[]";
		}
		case Il2CppTypeEnum.IL2CPP_TYPE_PTR:
		{
			Il2CppType il2CppTypeFromPointer = cppAssembly.GetIl2CppTypeFromPointer(type.data.type);
			return GetTypeName(metadata, cppAssembly, il2CppTypeFromPointer) + "*";
		}
		default:
			return TypeString[(int)type.type];
		}
	}

	internal static object? GetDefaultValue(int dataIndex, int typeIndex)
	{
		Il2CppMetadata theMetadata = LibCpp2IlMain.TheMetadata;
		Il2CppBinary binary = LibCpp2IlMain.Binary;
		if (dataIndex == -1)
		{
			return null;
		}
		int defaultValueFromIndex = theMetadata.GetDefaultValueFromIndex(dataIndex);
		if (defaultValueFromIndex <= 0)
		{
			return null;
		}
		int bytesRead2;
		switch (binary.GetType(typeIndex).type)
		{
		case Il2CppTypeEnum.IL2CPP_TYPE_BOOLEAN:
			return theMetadata.ReadClassAtRawAddr<bool>(defaultValueFromIndex);
		case Il2CppTypeEnum.IL2CPP_TYPE_U1:
			return theMetadata.ReadClassAtRawAddr<byte>(defaultValueFromIndex);
		case Il2CppTypeEnum.IL2CPP_TYPE_I1:
			return theMetadata.ReadClassAtRawAddr<sbyte>(defaultValueFromIndex);
		case Il2CppTypeEnum.IL2CPP_TYPE_CHAR:
			return BitConverter.ToChar(theMetadata.ReadByteArrayAtRawAddress(defaultValueFromIndex, 2), 0);
		case Il2CppTypeEnum.IL2CPP_TYPE_U2:
			return theMetadata.ReadClassAtRawAddr<ushort>(defaultValueFromIndex);
		case Il2CppTypeEnum.IL2CPP_TYPE_I2:
			return theMetadata.ReadClassAtRawAddr<short>(defaultValueFromIndex);
		case Il2CppTypeEnum.IL2CPP_TYPE_U4:
			if (LibCpp2IlMain.MetadataVersion < 29f)
			{
				return theMetadata.ReadClassAtRawAddr<uint>(defaultValueFromIndex);
			}
			return theMetadata.ReadUnityCompressedUIntAtRawAddr(defaultValueFromIndex, out bytesRead2);
		case Il2CppTypeEnum.IL2CPP_TYPE_I4:
			if (LibCpp2IlMain.MetadataVersion < 29f)
			{
				return theMetadata.ReadClassAtRawAddr<int>(defaultValueFromIndex);
			}
			return theMetadata.ReadUnityCompressedIntAtRawAddr(defaultValueFromIndex, out bytesRead2);
		case Il2CppTypeEnum.IL2CPP_TYPE_U8:
			return theMetadata.ReadClassAtRawAddr<ulong>(defaultValueFromIndex, overrideArchCheck: true);
		case Il2CppTypeEnum.IL2CPP_TYPE_I8:
			return theMetadata.ReadClassAtRawAddr<long>(defaultValueFromIndex, overrideArchCheck: true);
		case Il2CppTypeEnum.IL2CPP_TYPE_R4:
			return theMetadata.ReadClassAtRawAddr<float>(defaultValueFromIndex);
		case Il2CppTypeEnum.IL2CPP_TYPE_R8:
			return theMetadata.ReadClassAtRawAddr<double>(defaultValueFromIndex);
		case Il2CppTypeEnum.IL2CPP_TYPE_STRING:
		{
			int bytesRead = 4;
			int num = ((!(LibCpp2IlMain.MetadataVersion < 29f)) ? theMetadata.ReadUnityCompressedIntAtRawAddr(defaultValueFromIndex, out bytesRead) : theMetadata.ReadClassAtRawAddr<int>(defaultValueFromIndex));
			if (num > 65536)
			{
				LibLogger.WarnNewline("[GetDefaultValue] String length is really large: " + num);
			}
			return Encoding.UTF8.GetString(theMetadata.ReadByteArrayAtRawAddress(defaultValueFromIndex + bytesRead, num));
		}
		default:
			return null;
		}
	}

	public static Il2CppTypeReflectionData WrapType(Il2CppTypeDefinition what)
	{
		return new Il2CppTypeReflectionData
		{
			baseType = what,
			genericParams = new Il2CppTypeReflectionData[0],
			isGenericType = false,
			isType = true
		};
	}

	public static Il2CppTypeReflectionData GetTypeReflectionData(Il2CppType forWhat)
	{
		if (LibCpp2IlMain.Binary == null || LibCpp2IlMain.TheMetadata == null)
		{
			throw new Exception("Can't get type reflection data when not initialized. How did you even get the type?");
		}
		switch (forWhat.type)
		{
		case Il2CppTypeEnum.IL2CPP_TYPE_OBJECT:
			return WrapType(LibCpp2IlReflection.GetType("Object", "System"));
		case Il2CppTypeEnum.IL2CPP_TYPE_VOID:
			return WrapType(LibCpp2IlReflection.GetType("Void", "System"));
		case Il2CppTypeEnum.IL2CPP_TYPE_BOOLEAN:
			return WrapType(LibCpp2IlReflection.GetType("Boolean", "System"));
		case Il2CppTypeEnum.IL2CPP_TYPE_CHAR:
			return WrapType(LibCpp2IlReflection.GetType("Char", "System"));
		case Il2CppTypeEnum.IL2CPP_TYPE_I1:
			return WrapType(LibCpp2IlReflection.GetType("SByte", "System"));
		case Il2CppTypeEnum.IL2CPP_TYPE_U1:
			return WrapType(LibCpp2IlReflection.GetType("Byte", "System"));
		case Il2CppTypeEnum.IL2CPP_TYPE_I2:
			return WrapType(LibCpp2IlReflection.GetType("Int16", "System"));
		case Il2CppTypeEnum.IL2CPP_TYPE_U2:
			return WrapType(LibCpp2IlReflection.GetType("UInt16", "System"));
		case Il2CppTypeEnum.IL2CPP_TYPE_I4:
			return WrapType(LibCpp2IlReflection.GetType("Int32", "System"));
		case Il2CppTypeEnum.IL2CPP_TYPE_U4:
			return WrapType(LibCpp2IlReflection.GetType("UInt32", "System"));
		case Il2CppTypeEnum.IL2CPP_TYPE_I:
			return WrapType(LibCpp2IlReflection.GetType("IntPtr", "System"));
		case Il2CppTypeEnum.IL2CPP_TYPE_U:
			return WrapType(LibCpp2IlReflection.GetType("UIntPtr", "System"));
		case Il2CppTypeEnum.IL2CPP_TYPE_I8:
			return WrapType(LibCpp2IlReflection.GetType("Int64", "System"));
		case Il2CppTypeEnum.IL2CPP_TYPE_U8:
			return WrapType(LibCpp2IlReflection.GetType("UInt64", "System"));
		case Il2CppTypeEnum.IL2CPP_TYPE_R4:
			return WrapType(LibCpp2IlReflection.GetType("Single", "System"));
		case Il2CppTypeEnum.IL2CPP_TYPE_R8:
			return WrapType(LibCpp2IlReflection.GetType("Double", "System"));
		case Il2CppTypeEnum.IL2CPP_TYPE_STRING:
			return WrapType(LibCpp2IlReflection.GetType("String", "System"));
		case Il2CppTypeEnum.IL2CPP_TYPE_TYPEDBYREF:
			return WrapType(LibCpp2IlReflection.GetType("TypedReference", "System"));
		case Il2CppTypeEnum.IL2CPP_TYPE_VALUETYPE:
		case Il2CppTypeEnum.IL2CPP_TYPE_CLASS:
			return new Il2CppTypeReflectionData
			{
				baseType = LibCpp2IlMain.TheMetadata.typeDefs[forWhat.data.classIndex],
				genericParams = new Il2CppTypeReflectionData[0],
				isType = true,
				isGenericType = false
			};
		case Il2CppTypeEnum.IL2CPP_TYPE_GENERICINST:
		{
			Il2CppGenericClass il2CppGenericClass = LibCpp2IlMain.Binary.ReadClassAtVirtualAddress<Il2CppGenericClass>(forWhat.data.generic_class);
			Il2CppTypeDefinition baseType;
			if (LibCpp2IlMain.MetadataVersion < 27f)
			{
				baseType = LibCpp2IlMain.TheMetadata.typeDefs[il2CppGenericClass.typeDefinitionIndex];
			}
			else
			{
				Il2CppType il2CppType = LibCpp2IlMain.Binary.ReadClassAtVirtualAddress<Il2CppType>((ulong)il2CppGenericClass.typeDefinitionIndex);
				il2CppType.Init();
				baseType = LibCpp2IlMain.TheMetadata.typeDefs[il2CppType.data.classIndex];
			}
			Il2CppGenericInst il2CppGenericInst = LibCpp2IlMain.Binary.ReadClassAtVirtualAddress<Il2CppGenericInst>(il2CppGenericClass.context.class_inst);
			List<Il2CppTypeReflectionData> list = (from pointer in LibCpp2IlMain.Binary.GetPointers(il2CppGenericInst.pointerStart, (long)il2CppGenericInst.pointerCount)
				select LibCpp2IlMain.Binary.GetIl2CppTypeFromPointer(pointer) into type
				select GetTypeReflectionData(type)).ToList();
			return new Il2CppTypeReflectionData
			{
				baseType = baseType,
				genericParams = list.ToArray(),
				isType = true,
				isGenericType = true
			};
		}
		case Il2CppTypeEnum.IL2CPP_TYPE_VAR:
		case Il2CppTypeEnum.IL2CPP_TYPE_MVAR:
		{
			Il2CppGenericParameter il2CppGenericParameter = LibCpp2IlMain.TheMetadata.genericParameters[forWhat.data.genericParameterIndex];
			string stringFromIndex = LibCpp2IlMain.TheMetadata.GetStringFromIndex(il2CppGenericParameter.nameIndex);
			return new Il2CppTypeReflectionData
			{
				baseType = null,
				genericParams = new Il2CppTypeReflectionData[0],
				isType = false,
				isGenericType = false,
				variableGenericParamName = stringFromIndex
			};
		}
		case Il2CppTypeEnum.IL2CPP_TYPE_SZARRAY:
		{
			Il2CppType il2CppTypeFromPointer2 = LibCpp2IlMain.Binary.GetIl2CppTypeFromPointer(forWhat.data.type);
			return new Il2CppTypeReflectionData
			{
				baseType = null,
				arrayType = GetTypeReflectionData(il2CppTypeFromPointer2),
				arrayRank = 1,
				isArray = true,
				isType = false,
				isGenericType = false,
				genericParams = new Il2CppTypeReflectionData[0]
			};
		}
		case Il2CppTypeEnum.IL2CPP_TYPE_ARRAY:
		{
			Il2CppArrayType il2CppArrayType = LibCpp2IlMain.Binary.ReadClassAtVirtualAddress<Il2CppArrayType>(forWhat.data.array);
			Il2CppType il2CppTypeFromPointer = LibCpp2IlMain.Binary.GetIl2CppTypeFromPointer(il2CppArrayType.etype);
			return new Il2CppTypeReflectionData
			{
				baseType = null,
				arrayType = GetTypeReflectionData(il2CppTypeFromPointer),
				isArray = true,
				isType = false,
				arrayRank = il2CppArrayType.rank,
				isGenericType = false,
				genericParams = new Il2CppTypeReflectionData[0]
			};
		}
		case Il2CppTypeEnum.IL2CPP_TYPE_PTR:
		{
			Il2CppTypeReflectionData typeReflectionData = GetTypeReflectionData(LibCpp2IlMain.Binary.GetIl2CppTypeFromPointer(forWhat.data.type));
			typeReflectionData.isPointer = true;
			return typeReflectionData;
		}
		default:
			throw new ArgumentException($"Unknown type {forWhat.type}");
		}
	}

	public static int VersionAwareSizeOf(Type type, bool dontCheckVersionAttributes = false, bool downsize = true)
	{
		if (type.IsEnum)
		{
			type = type.GetEnumUnderlyingType();
		}
		if (type.IsPrimitive)
		{
			return (int)PrimitiveSizes[type.Name];
		}
		bool flag = downsize && LibCpp2IlMain.Binary.is32Bit;
		int num = 0;
		FieldInfo[] fields = type.GetFields();
		foreach (FieldInfo fieldInfo in fields)
		{
			if (!dontCheckVersionAttributes && !ShouldReadFieldOnThisVersion(fieldInfo))
			{
				continue;
			}
			switch (fieldInfo.FieldType.Name)
			{
			case "Int64":
			case "UInt64":
				num += (flag ? 4 : 8);
				continue;
			case "Int32":
			case "UInt32":
				num += 4;
				continue;
			case "Int16":
			case "UInt16":
				num += 2;
				continue;
			case "Byte":
			case "SByte":
				num++;
				continue;
			}
			if (fieldInfo.FieldType == type)
			{
				throw new Exception($"Infinite recursion is not allowed. Field {fieldInfo} of type {type} has the same type as its parent.");
			}
			num += VersionAwareSizeOf(fieldInfo.FieldType, dontCheckVersionAttributes, downsize);
		}
		return num;
	}

	internal static IEnumerable<int> Range(int start, int count)
	{
		for (int i = start; i < start + count; i++)
		{
			yield return i;
		}
	}

	internal static bool ShouldReadFieldOnThisVersion(FieldInfo i)
	{
		if (!_cachedVersionAttributes.TryGetValue(i, out VersionAttribute[] value))
		{
			value = Attribute.GetCustomAttributes(i, typeof(VersionAttribute)).Cast<VersionAttribute>().ToArray();
			_cachedVersionAttributes[i] = value;
		}
		if (value.Length != 0)
		{
			return value.Any((VersionAttribute attr) => LibCpp2IlMain.MetadataVersion >= attr.Min && LibCpp2IlMain.MetadataVersion <= attr.Max);
		}
		return true;
	}

	internal static void PopulateDeclaringAssemblyCache()
	{
		Il2CppImageDefinition[] imageDefinitions = LibCpp2IlMain.TheMetadata.imageDefinitions;
		foreach (Il2CppImageDefinition il2CppImageDefinition in imageDefinitions)
		{
			Il2CppTypeDefinition[] types = il2CppImageDefinition.Types;
			for (int j = 0; j < types.Length; j++)
			{
				types[j].DeclaringAssembly = il2CppImageDefinition;
			}
		}
	}
}
