#define DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using AssetsTools.NET.Extra;
using LibCpp2IL;
using LibCpp2IL.Metadata;
using LibCpp2IL.Reflection;

namespace AssetsTools.NET.Cpp2IL;

public class Cpp2IlTempGenerator : IMonoBehaviourTemplateGenerator
{
	private readonly string _globalMetadataPath;

	private readonly string _assemblyPath;

	private int[] _il2cppUnityVersion;

	private AssetsTools.NET.Extra.UnityVersion _unityVersion;

	private bool _initialized;

	private bool anyFieldIsManagedReference;

	public Cpp2IlTempGenerator(string globalMetadataPath, string assemblyPath)
	{
		_globalMetadataPath = globalMetadataPath;
		_assemblyPath = assemblyPath;
		ResetCpp2IL();
	}

	public void Dispose()
	{
		ResetCpp2IL();
	}

	public void ResetCpp2IL()
	{
		LibCpp2IlMain.Reset();
		_il2cppUnityVersion = null;
		_initialized = false;
	}

	public void SetUnityVersion(AssetsTools.NET.Extra.UnityVersion unityVersion)
	{
		LibCpp2IlMain.Reset();
		_unityVersion = unityVersion;
		_il2cppUnityVersion = new int[3] { unityVersion.major, unityVersion.minor, unityVersion.patch };
		_initialized = false;
	}

	public void SetUnityVersion(int major, int minor, int patch)
	{
		LibCpp2IlMain.Reset();
		_unityVersion = new AssetsTools.NET.Extra.UnityVersion(major + "." + minor + "." + patch);
		_il2cppUnityVersion = new int[3] { major, minor, patch };
		_initialized = false;
	}

	public void InitializeCpp2IL()
	{
		if (!LibCpp2IlMain.LoadFromFile(_assemblyPath, _globalMetadataPath, _il2cppUnityVersion))
		{
			throw new Exception("CPP2IL initialization failed!");
		}
		_initialized = true;
	}

	public AssetTypeTemplateField GetTemplateField(AssetTypeTemplateField baseField, string assemblyName, string nameSpace, string className, AssetsTools.NET.Extra.UnityVersion unityVersion)
	{
		int[] array = new int[3] { unityVersion.major, unityVersion.minor, unityVersion.patch };
		if (_il2cppUnityVersion == null)
		{
			SetUnityVersion(unityVersion);
			InitializeCpp2IL();
		}
		else if (_il2cppUnityVersion[0] != array[0] && _il2cppUnityVersion[1] != array[1] && _il2cppUnityVersion[2] != array[2])
		{
			Debug.WriteLine("Warning: This unity version does not match what CPP2IL was registered with. Call ResetUnityVersion().");
		}
		_unityVersion = unityVersion;
		anyFieldIsManagedReference = false;
		Il2CppMetadata theMetadata = LibCpp2IlMain.TheMetadata;
		Il2CppAssemblyDefinition il2CppAssemblyDefinition = theMetadata.AssemblyDefinitions.ToList().First((Il2CppAssemblyDefinition a) => a.AssemblyName.Name == assemblyName);
		if (il2CppAssemblyDefinition == null)
		{
			throw new Exception("Assembly \"" + assemblyName + "\" was not found in the IL2CPP metadata.");
		}
		string fullName = nameSpace + (string.IsNullOrEmpty(nameSpace) ? "" : ".") + className;
		Il2CppTypeDefinition il2CppTypeDefinition = il2CppAssemblyDefinition.Image.Types.FirstOrDefault((Il2CppTypeDefinition t) => t.FullName == fullName);
		if (il2CppTypeDefinition == null)
		{
			throw new Exception("Type \"" + nameSpace + "::" + className + "\" was not found in the IL2CPP metadata.");
		}
		Il2CppTypeReflectionData il2CppTypeReflectionData = new Il2CppTypeReflectionData
		{
			baseType = il2CppTypeDefinition,
			genericParams = Array.Empty<Il2CppTypeReflectionData>(),
			arrayRank = 0,
			arrayType = null,
			isArray = false,
			isGenericType = (il2CppTypeDefinition.GenericContainer != null),
			isPointer = false,
			isType = true,
			variableGenericParamName = null
		};
		List<AssetTypeTemplateField> list = new List<AssetTypeTemplateField>();
		RecursiveTypeLoad(il2CppTypeReflectionData, list, CommonMonoTemplateHelper.GetSerializationLimit(_unityVersion), isRecursiveCall: true);
		AssetTypeTemplateField assetTypeTemplateField = baseField.Clone();
		assetTypeTemplateField.Children.AddRange(list);
		return assetTypeTemplateField;
	}

	private List<string> GetAttributeNamesOnField(Il2CppImageDefinition image, Il2CppFieldDefinition field)
	{
		List<string> list = new List<string>();
		Il2CppCustomAttributeTypeRange customAttributeData = LibCpp2IlMain.TheMetadata.GetCustomAttributeData(image, field.customAttributeIndex, field.token);
		if (customAttributeData == null)
		{
			return list;
		}
		for (int i = 0; i < customAttributeData.count; i++)
		{
			int attributeTypeIndex = LibCpp2IlMain.TheMetadata.attributeTypes[customAttributeData.start + i];
			Il2CppTypeDefinition il2CppTypeDefinition = LibCpp2IlMain.TheMetadata.typeDefs.FirstOrDefault((Il2CppTypeDefinition td) => td.byvalTypeIndex == attributeTypeIndex);
			if (il2CppTypeDefinition != null)
			{
				list.Add(il2CppTypeDefinition.FullName);
			}
		}
		return list;
	}

	private void RecursiveTypeLoad(TypeDefWithSelfRef type, List<AssetTypeTemplateField> templateFields, int availableDepth, bool isRecursiveCall = false)
	{
		if (!isRecursiveCall)
		{
			availableDepth--;
		}
		string fullName = type.typeDef.BaseType.baseType.FullName;
		if (fullName != "System.Object" && fullName != "UnityEngine.Object" && fullName != "UnityEngine.MonoBehaviour" && fullName != "UnityEngine.ScriptableObject")
		{
			TypeDefWithSelfRef type2 = type.typeDef.BaseType;
			type2.AssignTypeParams(type);
			RecursiveTypeLoad(type2, templateFields, availableDepth, isRecursiveCall: true);
		}
		templateFields.AddRange(ReadTypes(type, availableDepth));
	}

	private List<AssetTypeTemplateField> ReadTypes(TypeDefWithSelfRef type, int availableDepth)
	{
		List<Il2CppFieldDefinition> acceptableFields = GetAcceptableFields(type, availableDepth);
		List<AssetTypeTemplateField> list = new List<AssetTypeTemplateField>();
		for (int i = 0; i < acceptableFields.Count; i++)
		{
			AssetTypeTemplateField assetTypeTemplateField = new AssetTypeTemplateField();
			Il2CppFieldDefinition il2CppFieldDefinition = acceptableFields[i];
			TypeDefWithSelfRef typeDefWithSelfRef = type.SolidifyType(il2CppFieldDefinition.FieldType);
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			if (typeDefWithSelfRef.typeRef.isArray)
			{
				flag = typeDefWithSelfRef.typeRef.arrayRank == 1;
				typeDefWithSelfRef = typeDefWithSelfRef.typeRef.arrayType;
			}
			else if (typeDefWithSelfRef.typeDef.FullName == "System.Collections.Generic.List`1")
			{
				typeDefWithSelfRef = typeDefWithSelfRef.typeRef.genericParams[0];
				flag = true;
			}
			List<string> attributeNamesOnField = GetAttributeNamesOnField(type.typeDef.DeclaringAssembly, il2CppFieldDefinition);
			TypeAttributes flags = (TypeAttributes)typeDefWithSelfRef.typeDef.flags;
			bool flag4 = flags.HasFlag(TypeAttributes.Serializable);
			assetTypeTemplateField.Name = il2CppFieldDefinition.Name;
			bool flag5;
			if (flag5 = typeDefWithSelfRef.typeDef.IsEnumType)
			{
				string fullName = typeDefWithSelfRef.typeDef.GetEnumUnderlyingType().baseType.FullName;
				assetTypeTemplateField.Type = CommonMonoTemplateHelper.ConvertBaseToPrimitive(fullName);
			}
			else if (flag5 = CommonMonoTemplateHelper.IsPrimitiveType(typeDefWithSelfRef.typeDef.FullName))
			{
				assetTypeTemplateField.Type = CommonMonoTemplateHelper.ConvertBaseToPrimitive(typeDefWithSelfRef.typeDef.FullName);
			}
			else if (typeDefWithSelfRef.typeDef.FullName == "System.String")
			{
				assetTypeTemplateField.Type = "string";
			}
			else if (flag2 = DerivesFromUEObject(typeDefWithSelfRef))
			{
				assetTypeTemplateField.Type = "PPtr<$" + typeDefWithSelfRef.typeDef.Name + ">";
			}
			else if (flag3 = attributeNamesOnField.Contains("UnityEngine.SerializeReference"))
			{
				anyFieldIsManagedReference = true;
				assetTypeTemplateField.Type = "managedReference";
			}
			else
			{
				assetTypeTemplateField.Type = typeDefWithSelfRef.typeDef.Name;
			}
			if (flag5)
			{
				assetTypeTemplateField.Children = new List<AssetTypeTemplateField>();
			}
			else if (typeDefWithSelfRef.typeDef.FullName == "System.String")
			{
				assetTypeTemplateField.Children = CommonMonoTemplateHelper.String();
			}
			else if (CommonMonoTemplateHelper.IsSpecialUnityType(typeDefWithSelfRef.typeDef.FullName))
			{
				assetTypeTemplateField.Children = SpecialUnity(typeDefWithSelfRef, availableDepth);
			}
			else if (flag2)
			{
				assetTypeTemplateField.Children = CommonMonoTemplateHelper.PPtr(_unityVersion);
			}
			else if (flag3)
			{
				assetTypeTemplateField.Children = CommonMonoTemplateHelper.ManagedReference(_unityVersion);
			}
			else if (flag4)
			{
				assetTypeTemplateField.Children = Serialized(typeDefWithSelfRef, availableDepth);
			}
			else
			{
				Console.WriteLine("you wot mate");
			}
			assetTypeTemplateField.ValueType = AssetTypeValueField.GetValueTypeByTypeName(assetTypeTemplateField.Type);
			assetTypeTemplateField.IsAligned = CommonMonoTemplateHelper.TypeAligns(assetTypeTemplateField.ValueType);
			assetTypeTemplateField.HasValue = assetTypeTemplateField.ValueType != AssetValueType.None;
			if (flag)
			{
				assetTypeTemplateField = ((!(flag5 || flag2)) ? CommonMonoTemplateHelper.VectorWithType(assetTypeTemplateField) : CommonMonoTemplateHelper.Vector(assetTypeTemplateField));
			}
			list.Add(assetTypeTemplateField);
		}
		if (anyFieldIsManagedReference && DerivesFromUEObject(type))
		{
			list.Add(CommonMonoTemplateHelper.ManagedReferencesRegistry("references", _unityVersion));
		}
		return list;
	}

	private List<Il2CppFieldDefinition> GetAcceptableFields(TypeDefWithSelfRef parentType, int availableDepth)
	{
		List<Il2CppFieldDefinition> list = new List<Il2CppFieldDefinition>();
		for (int i = 0; i < parentType.typeDef.field_count; i++)
		{
			Il2CppFieldDefinition il2CppFieldDefinition = parentType.typeDef.Fields[i];
			FieldAttributes fieldAttributes = parentType.typeDef.FieldAttributes[i];
			List<string> attributeNamesOnField = GetAttributeNamesOnField(parentType.typeDef.DeclaringAssembly, il2CppFieldDefinition);
			if ((!fieldAttributes.HasFlag(FieldAttributes.Public) && !attributeNamesOnField.Contains("UnityEngine.SerializeField") && !attributeNamesOnField.Contains("UnityEngine.SerializeReference")) || fieldAttributes.HasFlag(FieldAttributes.Static) || fieldAttributes.HasFlag(FieldAttributes.NotSerialized) || fieldAttributes.HasFlag(FieldAttributes.InitOnly) || fieldAttributes.HasFlag(FieldAttributes.Literal))
			{
				continue;
			}
			TypeDefWithSelfRef typeDefWithSelfRef = parentType.SolidifyType(il2CppFieldDefinition.FieldType);
			if (TryGetListOrArrayElement(typeDefWithSelfRef, out var elemType2))
			{
				if (availableDepth < 0 || TryGetListOrArrayElement(elemType2, out var _))
				{
					continue;
				}
				typeDefWithSelfRef = elemType2;
			}
			else if (parentType.typeDef.FullName == typeDefWithSelfRef.typeDef.FullName && !DerivesFromUEObject(parentType))
			{
				continue;
			}
			if (IsValidDef(attributeNamesOnField, typeDefWithSelfRef, availableDepth))
			{
				list.Add(il2CppFieldDefinition);
			}
		}
		return list;
		bool IsValidDef(List<string> attributeNames, TypeDefWithSelfRef typeDef, int availableDepth)
		{
			if (typeDef.typeDef.GenericContainer != null && _unityVersion.major < 2020)
			{
				return false;
			}
			if (CommonMonoTemplateHelper.IsPrimitiveType(typeDef.typeDef.FullName) || typeDef.typeDef.FullName == "System.String")
			{
				return true;
			}
			if (typeDef.typeDef.IsEnumType)
			{
				string fullName = typeDef.typeDef.GetEnumUnderlyingType().baseType.FullName;
				return fullName != "System.Int64" && fullName != "System.UInt64";
			}
			TypeAttributes flags = (TypeAttributes)typeDef.typeDef.flags;
			if (availableDepth < 0)
			{
				return typeDef.typeDef.IsValueType && (flags.HasFlag(TypeAttributes.Serializable) || CommonMonoTemplateHelper.IsSpecialUnityType(typeDef.typeDef.FullName));
			}
			if (DerivesFromUEObject(typeDef) || CommonMonoTemplateHelper.IsSpecialUnityType(typeDef.typeDef.FullName))
			{
				return true;
			}
			if (attributeNames.Contains("UnityEngine.SerializeReference"))
			{
				if (_unityVersion.major == 2019 && _unityVersion.minor == 3 && _unityVersion.patch < 8 && typeDef.typeDef.FullName == "System.Object")
				{
					return false;
				}
				return !typeDef.typeDef.IsValueType && typeDef.typeDef.GenericContainer == null;
			}
			if (CommonMonoTemplateHelper.IsAssemblyBlacklisted(typeDef.typeDef.DeclaringAssembly.Name, _unityVersion))
			{
				return false;
			}
			return !typeDef.typeDef.IsAbstract && flags.HasFlag(TypeAttributes.Serializable);
		}
		static bool TryGetListOrArrayElement(TypeDefWithSelfRef fieldType, out Il2CppTypeReflectionData elemType)
		{
			if (fieldType.typeRef.isArray)
			{
				elemType = fieldType.typeRef.arrayType;
				return true;
			}
			if (fieldType.typeRef.isGenericType && fieldType.typeRef.baseType.FullName == "System.Collections.Generic.List`1")
			{
				elemType = fieldType.typeRef.genericParams[0];
				return true;
			}
			elemType = null;
			return false;
		}
	}

	private bool DerivesFromUEObject(TypeDefWithSelfRef typeDef)
	{
		TypeAttributes flags = (TypeAttributes)typeDef.typeDef.flags;
		if (typeDef.typeDef.BaseType == null)
		{
			return false;
		}
		if (flags.HasFlag(TypeAttributes.ClassSemanticsMask))
		{
			return false;
		}
		if (typeDef.typeDef.BaseType.baseType.FullName == "UnityEngine.Object" || typeDef.typeDef.FullName == "UnityEngine.Object")
		{
			return true;
		}
		if (typeDef.typeDef.BaseType.baseType.FullName != "System.Object")
		{
			return DerivesFromUEObject(typeDef.typeDef.BaseType);
		}
		return false;
	}

	private List<AssetTypeTemplateField> Serialized(TypeDefWithSelfRef type, int availableDepth)
	{
		List<AssetTypeTemplateField> list = new List<AssetTypeTemplateField>();
		RecursiveTypeLoad(type, list, availableDepth);
		return list;
	}

	private List<AssetTypeTemplateField> SpecialUnity(TypeDefWithSelfRef type, int availableDepth)
	{
		string name = type.typeDef.Name;
		if (1 == 0)
		{
		}
		List<AssetTypeTemplateField> result = name switch
		{
			"Gradient" => CommonMonoTemplateHelper.Gradient(_unityVersion), 
			"AnimationCurve" => CommonMonoTemplateHelper.AnimationCurve(_unityVersion), 
			"LayerMask" => CommonMonoTemplateHelper.BitField(), 
			"Bounds" => CommonMonoTemplateHelper.AABB(), 
			"BoundsInt" => CommonMonoTemplateHelper.BoundsInt(), 
			"Rect" => CommonMonoTemplateHelper.Rectf(), 
			"RectOffset" => CommonMonoTemplateHelper.RectOffset(), 
			"Color32" => CommonMonoTemplateHelper.RGBAi(), 
			"GUIStyle" => CommonMonoTemplateHelper.GUIStyle(_unityVersion), 
			"Vector2Int" => CommonMonoTemplateHelper.Vector2Int(), 
			"Vector3Int" => CommonMonoTemplateHelper.Vector3Int(), 
			_ => Serialized(type, availableDepth), 
		};
		if (1 == 0)
		{
		}
		return result;
	}
}
