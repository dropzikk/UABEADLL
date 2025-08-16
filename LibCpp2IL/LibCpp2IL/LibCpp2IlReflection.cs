using System.Collections.Generic;
using System.Linq;
using LibCpp2IL.BinaryStructures;
using LibCpp2IL.Metadata;

namespace LibCpp2IL;

public static class LibCpp2IlReflection
{
	private static readonly Dictionary<(string, string?), Il2CppTypeDefinition> _cachedTypes = new Dictionary<(string, string), Il2CppTypeDefinition>();

	private static readonly Dictionary<string, Il2CppTypeDefinition> _cachedTypesByFullName = new Dictionary<string, Il2CppTypeDefinition>();

	private static readonly Dictionary<Il2CppTypeDefinition, int> _typeIndices = new Dictionary<Il2CppTypeDefinition, int>();

	private static readonly Dictionary<Il2CppMethodDefinition, int> _methodIndices = new Dictionary<Il2CppMethodDefinition, int>();

	private static readonly Dictionary<Il2CppFieldDefinition, int> _fieldIndices = new Dictionary<Il2CppFieldDefinition, int>();

	private static readonly Dictionary<Il2CppPropertyDefinition, int> _propertyIndices = new Dictionary<Il2CppPropertyDefinition, int>();

	internal static void ResetCaches()
	{
		_cachedTypes.Clear();
		_cachedTypesByFullName.Clear();
		_typeIndices.Clear();
		_methodIndices.Clear();
		_fieldIndices.Clear();
		_propertyIndices.Clear();
	}

	public static Il2CppTypeDefinition? GetType(string name, string? @namespace = null)
	{
		if (LibCpp2IlMain.TheMetadata == null)
		{
			return null;
		}
		(string, string) key = (name, @namespace);
		if (!_cachedTypes.ContainsKey(key))
		{
			Il2CppTypeDefinition value = LibCpp2IlMain.TheMetadata.typeDefs.FirstOrDefault((Il2CppTypeDefinition td) => td.Name == name && (@namespace == null || @namespace == td.Namespace));
			_cachedTypes[key] = value;
		}
		return _cachedTypes[key];
	}

	public static Il2CppTypeDefinition? GetTypeByFullName(string fullName)
	{
		if (LibCpp2IlMain.TheMetadata == null)
		{
			return null;
		}
		if (!_cachedTypesByFullName.ContainsKey(fullName))
		{
			Il2CppTypeDefinition value = LibCpp2IlMain.TheMetadata.typeDefs.FirstOrDefault((Il2CppTypeDefinition td) => td.FullName == fullName);
			_cachedTypesByFullName[fullName] = value;
		}
		return _cachedTypesByFullName[fullName];
	}

	public static Il2CppTypeDefinition? GetTypeDefinitionByTypeIndex(int index)
	{
		if (LibCpp2IlMain.TheMetadata == null || LibCpp2IlMain.Binary == null)
		{
			return null;
		}
		if (index >= LibCpp2IlMain.Binary.NumTypes || index < 0)
		{
			return null;
		}
		Il2CppType type = LibCpp2IlMain.Binary.GetType(index);
		return LibCpp2IlMain.TheMetadata.typeDefs[type.data.classIndex];
	}

	public static int GetTypeIndexFromType(Il2CppTypeDefinition typeDefinition)
	{
		if (LibCpp2IlMain.TheMetadata == null)
		{
			return -1;
		}
		lock (_typeIndices)
		{
			if (!_typeIndices.ContainsKey(typeDefinition))
			{
				for (int i = 0; i < LibCpp2IlMain.TheMetadata.typeDefs.Length; i++)
				{
					if (LibCpp2IlMain.TheMetadata.typeDefs[i] == typeDefinition)
					{
						_typeIndices[typeDefinition] = i;
					}
				}
			}
			return _typeIndices.GetValueOrDefault(typeDefinition, -1);
		}
	}

	public static int GetMethodIndexFromMethod(Il2CppMethodDefinition methodDefinition)
	{
		if (LibCpp2IlMain.TheMetadata == null)
		{
			return -1;
		}
		if (_methodIndices.Count == 0)
		{
			lock (_methodIndices)
			{
				if (_methodIndices.Count == 0)
				{
					for (int i = 0; i < LibCpp2IlMain.TheMetadata.methodDefs.Length; i++)
					{
						Il2CppMethodDefinition key = LibCpp2IlMain.TheMetadata.methodDefs[i];
						_methodIndices[key] = i;
					}
				}
			}
		}
		return _methodIndices.GetValueOrDefault(methodDefinition, -1);
	}

	public static int GetFieldIndexFromField(Il2CppFieldDefinition fieldDefinition)
	{
		if (LibCpp2IlMain.TheMetadata == null)
		{
			return -1;
		}
		if (_fieldIndices.Count == 0)
		{
			lock (_fieldIndices)
			{
				if (_fieldIndices.Count == 0)
				{
					for (int i = 0; i < LibCpp2IlMain.TheMetadata.fieldDefs.Length; i++)
					{
						Il2CppFieldDefinition key = LibCpp2IlMain.TheMetadata.fieldDefs[i];
						_fieldIndices[key] = i;
					}
				}
			}
		}
		return _fieldIndices[fieldDefinition];
	}

	public static int GetPropertyIndexFromProperty(Il2CppPropertyDefinition propertyDefinition)
	{
		if (LibCpp2IlMain.TheMetadata == null)
		{
			return -1;
		}
		if (_propertyIndices.Count == 0)
		{
			lock (_propertyIndices)
			{
				if (_propertyIndices.Count == 0)
				{
					for (int i = 0; i < LibCpp2IlMain.TheMetadata.propertyDefs.Length; i++)
					{
						Il2CppPropertyDefinition key = LibCpp2IlMain.TheMetadata.propertyDefs[i];
						_propertyIndices[key] = i;
					}
				}
			}
		}
		return _propertyIndices[propertyDefinition];
	}
}
