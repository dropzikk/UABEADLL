using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace AssetsTools.NET.Extra;

public class MonoCecilTempGenerator : IMonoBehaviourTemplateGenerator
{
	private UnityVersion unityVersion;

	private bool anyFieldIsManagedReference;

	public string managedPath;

	public Dictionary<string, AssemblyDefinition> loadedAssemblies = new Dictionary<string, AssemblyDefinition>();

	public MonoCecilTempGenerator(string managedPath)
	{
		this.managedPath = managedPath;
	}

	public void Dispose()
	{
		foreach (AssemblyDefinition value in loadedAssemblies.Values)
		{
			value.Dispose();
		}
		loadedAssemblies.Clear();
	}

	public AssetTypeTemplateField GetTemplateField(AssetTypeTemplateField baseField, string assemblyName, string nameSpace, string className, UnityVersion unityVersion)
	{
		if (!assemblyName.EndsWith(".dll"))
		{
			assemblyName += ".dll";
		}
		string text = Path.Combine(managedPath, assemblyName);
		if (!File.Exists(text))
		{
			return null;
		}
		List<AssetTypeTemplateField> collection = Read(text, nameSpace, className, unityVersion);
		AssetTypeTemplateField assetTypeTemplateField = baseField.Clone();
		assetTypeTemplateField.Children.AddRange(collection);
		return assetTypeTemplateField;
	}

	public List<AssetTypeTemplateField> Read(string assemblyPath, string nameSpace, string typeName, UnityVersion unityVersion)
	{
		AssemblyDefinition assemblyWithDependencies = GetAssemblyWithDependencies(assemblyPath);
		return Read(assemblyWithDependencies, nameSpace, typeName, unityVersion);
	}

	public List<AssetTypeTemplateField> Read(AssemblyDefinition assembly, string nameSpace, string typeName, UnityVersion unityVersion)
	{
		this.unityVersion = unityVersion;
		anyFieldIsManagedReference = false;
		List<AssetTypeTemplateField> list = new List<AssetTypeTemplateField>();
		RecursiveTypeLoad(assembly.MainModule, nameSpace, typeName, list, CommonMonoTemplateHelper.GetSerializationLimit(unityVersion));
		return list;
	}

	private AssemblyDefinition GetAssemblyWithDependencies(string path)
	{
		string fileName = Path.GetFileName(path);
		if (loadedAssemblies.ContainsKey(fileName))
		{
			return loadedAssemblies[fileName];
		}
		DefaultAssemblyResolver defaultAssemblyResolver = new DefaultAssemblyResolver();
		defaultAssemblyResolver.AddSearchDirectory(Path.GetDirectoryName(path));
		ReaderParameters parameters = new ReaderParameters
		{
			AssemblyResolver = defaultAssemblyResolver
		};
		AssemblyDefinition assemblyDefinition = AssemblyDefinition.ReadAssembly(path, parameters);
		loadedAssemblies[fileName] = assemblyDefinition;
		return assemblyDefinition;
	}

	private void RecursiveTypeLoad(ModuleDefinition module, string nameSpace, string typeName, List<AssetTypeTemplateField> attf, int availableDepth)
	{
		TypeDefinition typeDefinition;
		if (Enumerable.Contains(typeName, '/'))
		{
			string[] array = typeName.Split(new char[1] { '/' });
			typeDefinition = new TypeReference(nameSpace, array[0], module, module).Resolve();
			for (int i = 1; i < array.Length; i++)
			{
				TypeReference typeReference = new TypeReference("", array[i], module, module)
				{
					DeclaringType = typeDefinition
				};
				typeDefinition = typeReference.Resolve();
			}
		}
		else
		{
			TypeReference typeReference = new TypeReference(nameSpace, typeName, module, module);
			typeDefinition = typeReference.Resolve();
		}
		RecursiveTypeLoad(typeDefinition, attf, availableDepth, isRecursiveCall: true);
	}

	private void RecursiveTypeLoad(TypeDefWithSelfRef type, List<AssetTypeTemplateField> attf, int availableDepth, bool isRecursiveCall = false)
	{
		if (!isRecursiveCall)
		{
			availableDepth--;
		}
		string fullName = type.typeDef.BaseType.FullName;
		if (fullName != "System.Object" && fullName != "UnityEngine.Object" && fullName != "UnityEngine.MonoBehaviour" && fullName != "UnityEngine.ScriptableObject")
		{
			TypeDefWithSelfRef type2 = type.typeDef.BaseType;
			type2.AssignTypeParams(type);
			RecursiveTypeLoad(type2, attf, availableDepth, isRecursiveCall: true);
		}
		attf.AddRange(ReadTypes(type, availableDepth));
	}

	private List<AssetTypeTemplateField> ReadTypes(TypeDefWithSelfRef type, int availableDepth)
	{
		List<FieldDefinition> acceptableFields = GetAcceptableFields(type, availableDepth);
		List<AssetTypeTemplateField> list = new List<AssetTypeTemplateField>();
		for (int i = 0; i < acceptableFields.Count; i++)
		{
			AssetTypeTemplateField assetTypeTemplateField = new AssetTypeTemplateField();
			FieldDefinition fieldDefinition = acceptableFields[i];
			TypeDefWithSelfRef typeDefWithSelfRef = type.SolidifyType(fieldDefinition.FieldType);
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			if (typeDefWithSelfRef.typeRef.MetadataType == MetadataType.Array)
			{
				ArrayType arrayType = (ArrayType)typeDefWithSelfRef.typeRef;
				flag = arrayType.IsVector;
			}
			else if (typeDefWithSelfRef.typeDef.FullName == "System.Collections.Generic.List`1")
			{
				typeDefWithSelfRef = typeDefWithSelfRef.typeParamToArg.First().Value;
				flag = true;
			}
			assetTypeTemplateField.Name = fieldDefinition.Name;
			if (flag2 = typeDefWithSelfRef.typeDef.IsEnum)
			{
				string fullName = typeDefWithSelfRef.typeDef.GetEnumUnderlyingType().FullName;
				assetTypeTemplateField.Type = CommonMonoTemplateHelper.ConvertBaseToPrimitive(fullName);
			}
			else if (flag2 = typeDefWithSelfRef.typeDef.IsPrimitive)
			{
				assetTypeTemplateField.Type = CommonMonoTemplateHelper.ConvertBaseToPrimitive(typeDefWithSelfRef.typeDef.FullName);
			}
			else if (typeDefWithSelfRef.typeDef.FullName == "System.String")
			{
				assetTypeTemplateField.Type = "string";
			}
			else if (flag3 = DerivesFromUEObject(typeDefWithSelfRef))
			{
				assetTypeTemplateField.Type = "PPtr<$" + typeDefWithSelfRef.typeDef.Name + ">";
			}
			else if (flag4 = fieldDefinition.CustomAttributes.Any((CustomAttribute a) => a.AttributeType.Name == "SerializeReference"))
			{
				anyFieldIsManagedReference = true;
				assetTypeTemplateField.Type = "managedReference";
			}
			else
			{
				assetTypeTemplateField.Type = typeDefWithSelfRef.typeDef.Name;
			}
			if (flag2)
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
			else if (flag3)
			{
				assetTypeTemplateField.Children = CommonMonoTemplateHelper.PPtr(unityVersion);
			}
			else if (flag4)
			{
				assetTypeTemplateField.Children = CommonMonoTemplateHelper.ManagedReference(unityVersion);
			}
			else if (typeDefWithSelfRef.typeDef.IsSerializable)
			{
				assetTypeTemplateField.Children = Serialized(typeDefWithSelfRef, availableDepth);
			}
			assetTypeTemplateField.ValueType = AssetTypeValueField.GetValueTypeByTypeName(assetTypeTemplateField.Type);
			assetTypeTemplateField.IsAligned = CommonMonoTemplateHelper.TypeAligns(assetTypeTemplateField.ValueType);
			assetTypeTemplateField.HasValue = assetTypeTemplateField.ValueType != AssetValueType.None;
			if (flag)
			{
				assetTypeTemplateField = ((!(flag2 || flag3)) ? CommonMonoTemplateHelper.VectorWithType(assetTypeTemplateField) : CommonMonoTemplateHelper.Vector(assetTypeTemplateField));
			}
			list.Add(assetTypeTemplateField);
		}
		if (anyFieldIsManagedReference && DerivesFromUEObject(type))
		{
			list.Add(CommonMonoTemplateHelper.ManagedReferencesRegistry("references", unityVersion));
		}
		return list;
	}

	private List<FieldDefinition> GetAcceptableFields(TypeDefWithSelfRef parentType, int availableDepth)
	{
		List<FieldDefinition> list = new List<FieldDefinition>();
		foreach (FieldDefinition field in parentType.typeDef.Fields)
		{
			if ((!Net35Polyfill.HasFlag(field.Attributes, FieldAttributes.Public) && !field.CustomAttributes.Any((CustomAttribute a) => a.AttributeType.FullName == "UnityEngine.SerializeField" || a.AttributeType.FullName == "UnityEngine.SerializeReference")) || Net35Polyfill.HasFlag(field.Attributes, FieldAttributes.Static) || Net35Polyfill.HasFlag(field.Attributes, FieldAttributes.NotSerialized) || field.IsInitOnly || field.HasConstant)
			{
				continue;
			}
			TypeDefWithSelfRef fieldType2 = parentType.SolidifyType(field.FieldType);
			if (TryGetListOrArrayElement(fieldType2, out var elemType2))
			{
				if (availableDepth < 0 || TryGetListOrArrayElement(elemType2, out var _))
				{
					continue;
				}
				fieldType2 = elemType2;
			}
			else if (parentType.typeDef.FullName == fieldType2.typeDef.FullName && !DerivesFromUEObject(parentType))
			{
				continue;
			}
			TypeDefinition typeDef2 = fieldType2.typeDef;
			if (typeDef2 != null && IsValidDef(field, typeDef2, availableDepth))
			{
				list.Add(field);
			}
		}
		return list;
		bool IsValidDef(FieldDefinition fieldDef, TypeDefinition typeDef, int availableDepth)
		{
			if (typeDef.HasGenericParameters && unityVersion.major < 2020)
			{
				return false;
			}
			if (typeDef.IsPrimitive || typeDef.FullName == "System.String")
			{
				return true;
			}
			if (typeDef.IsEnum)
			{
				string fullName = typeDef.GetEnumUnderlyingType().FullName;
				return fullName != "System.Int64" && fullName != "System.UInt64";
			}
			if (availableDepth < 0)
			{
				return typeDef.IsValueType && (typeDef.IsSerializable || CommonMonoTemplateHelper.IsSpecialUnityType(typeDef.FullName));
			}
			if (DerivesFromUEObject(typeDef) || CommonMonoTemplateHelper.IsSpecialUnityType(typeDef.FullName))
			{
				return true;
			}
			if (fieldDef.CustomAttributes.Any((CustomAttribute a) => a.AttributeType.Name == "SerializeReference"))
			{
				if (unityVersion.major == 2019 && unityVersion.minor == 3 && unityVersion.patch < 8 && typeDef.FullName == "System.Object")
				{
					return false;
				}
				return !typeDef.IsValueType && !typeDef.HasGenericParameters;
			}
			if (CommonMonoTemplateHelper.IsAssemblyBlacklisted((typeDef.Scope as ModuleDefinition)?.Assembly.Name.Name ?? typeDef.Scope.Name, unityVersion))
			{
				return false;
			}
			return !typeDef.IsAbstract && typeDef.IsSerializable;
		}
		static bool TryGetListOrArrayElement(TypeDefWithSelfRef fieldType, out TypeDefWithSelfRef elemType)
		{
			if (fieldType.typeRef is ArrayType arrayType)
			{
				elemType = arrayType.ElementType;
				return true;
			}
			if (fieldType.typeRef is GenericInstanceType genericInstanceType && genericInstanceType.ElementType.FullName == "System.Collections.Generic.List`1")
			{
				elemType = fieldType.typeParamToArg["T"];
				return true;
			}
			elemType = default(TypeDefWithSelfRef);
			return false;
		}
	}

	private bool DerivesFromUEObject(TypeDefWithSelfRef typeDef)
	{
		if (typeDef.typeDef.BaseType == null)
		{
			return false;
		}
		if (typeDef.typeDef.IsInterface)
		{
			return false;
		}
		if (typeDef.typeDef.BaseType.FullName == "UnityEngine.Object" || typeDef.typeDef.FullName == "UnityEngine.Object")
		{
			return true;
		}
		if (typeDef.typeDef.BaseType.FullName != "System.Object")
		{
			return DerivesFromUEObject(typeDef.typeDef.BaseType.Resolve());
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
			"Gradient" => CommonMonoTemplateHelper.Gradient(unityVersion), 
			"AnimationCurve" => CommonMonoTemplateHelper.AnimationCurve(unityVersion), 
			"LayerMask" => CommonMonoTemplateHelper.BitField(), 
			"Bounds" => CommonMonoTemplateHelper.AABB(), 
			"BoundsInt" => CommonMonoTemplateHelper.BoundsInt(), 
			"Rect" => CommonMonoTemplateHelper.Rectf(), 
			"RectOffset" => CommonMonoTemplateHelper.RectOffset(), 
			"Color32" => CommonMonoTemplateHelper.RGBAi(), 
			"GUIStyle" => CommonMonoTemplateHelper.GUIStyle(unityVersion), 
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
