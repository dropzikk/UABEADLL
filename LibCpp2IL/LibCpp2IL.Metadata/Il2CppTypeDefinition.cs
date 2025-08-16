using System;
using System.Linq;
using System.Reflection;
using LibCpp2IL.BinaryStructures;
using LibCpp2IL.Reflection;

namespace LibCpp2IL.Metadata;

public class Il2CppTypeDefinition
{
	public int nameIndex;

	public int namespaceIndex;

	[Version(Max = 24f)]
	public int customAttributeIndex;

	public int byvalTypeIndex;

	[Version(Max = 24.5f)]
	public int byrefTypeIndex;

	public int declaringTypeIndex;

	public int parentIndex;

	public int elementTypeIndex;

	[Version(Max = 24.15f)]
	public int rgctxStartIndex;

	[Version(Max = 24.15f)]
	public int rgctxCount;

	public int genericContainerIndex;

	public uint flags;

	public int firstFieldIdx;

	public int firstMethodIdx;

	public int firstEventId;

	public int firstPropertyId;

	public int nestedTypesStart;

	public int interfacesStart;

	public int vtableStart;

	public int interfaceOffsetsStart;

	public ushort method_count;

	public ushort propertyCount;

	public ushort field_count;

	public ushort eventCount;

	public ushort nested_type_count;

	public ushort vtable_count;

	public ushort interfaces_count;

	public ushort interface_offsets_count;

	public uint bitfield;

	public uint token;

	private Il2CppImageDefinition? _cachedDeclaringAssembly;

	private string? _cachedNamespace;

	private string? _cachedName;

	public bool IsValueType => (bitfield & 1) == 1;

	public bool IsEnumType => ((bitfield >> 1) & 1) == 1;

	public bool HasFinalizer => ((bitfield >> 2) & 1) == 1;

	public bool HasCctor => ((bitfield >> 3) & 1) == 1;

	public bool IsBlittable => ((bitfield >> 4) & 1) == 1;

	public bool IsImportOrWindowsRuntime => ((bitfield >> 5) & 1) == 1;

	public uint PackingSize => ((Il2CppPackingSizeEnum)((bitfield >> 6) & 0xF)).NumericalValue();

	public bool PackingSizeIsDefault => ((bitfield >> 10) & 1) == 1;

	public bool ClassSizeIsDefault => ((bitfield >> 11) & 1) == 1;

	public uint SpecifiedPackingSize => ((Il2CppPackingSizeEnum)((bitfield >> 12) & 0xF)).NumericalValue();

	public bool IsByRefLike => ((bitfield >> 16) & 1) == 1;

	public Il2CppTypeDefinitionSizes RawSizes
	{
		get
		{
			ulong addr = LibCpp2IlMain.Binary.TypeDefinitionSizePointers[TypeIndex];
			return LibCpp2IlMain.Binary.ReadClassAtVirtualAddress<Il2CppTypeDefinitionSizes>(addr);
		}
	}

	public int Size => RawSizes.native_size;

	public Il2CppInterfaceOffset[] InterfaceOffsets
	{
		get
		{
			if (interfaceOffsetsStart < 0)
			{
				return new Il2CppInterfaceOffset[0];
			}
			return LibCpp2IlMain.TheMetadata.interfaceOffsets.SubArray(interfaceOffsetsStart, interface_offsets_count);
		}
	}

	public MetadataUsage?[] VTable
	{
		get
		{
			if (vtableStart < 0)
			{
				return new MetadataUsage[0];
			}
			return (from v in LibCpp2IlMain.TheMetadata.VTableMethodIndices.SubArray(vtableStart, vtable_count)
				select MetadataUsage.DecodeMetadataUsage(v, 0uL)).ToArray();
		}
	}

	public int TypeIndex => LibCpp2IlReflection.GetTypeIndexFromType(this);

	public bool IsAbstract => (flags & 0x80) != 0;

	public Il2CppImageDefinition? DeclaringAssembly
	{
		get
		{
			if (_cachedDeclaringAssembly == null)
			{
				if (LibCpp2IlMain.TheMetadata == null)
				{
					return null;
				}
				LibCpp2ILUtils.PopulateDeclaringAssemblyCache();
			}
			return _cachedDeclaringAssembly;
		}
		internal set
		{
			_cachedDeclaringAssembly = value;
		}
	}

	public Il2CppCodeGenModule? CodeGenModule
	{
		get
		{
			if (LibCpp2IlMain.Binary != null)
			{
				return LibCpp2IlMain.Binary.GetCodegenModuleByName(DeclaringAssembly.Name);
			}
			return null;
		}
	}

	public Il2CppRGCTXDefinition[] RGCTXs
	{
		get
		{
			if (LibCpp2IlMain.MetadataVersion < 24.2f)
			{
				return LibCpp2IlMain.TheMetadata.RgctxDefinitions.Skip(rgctxStartIndex).Take(rgctxCount).ToArray();
			}
			Il2CppCodeGenModule codeGenModule = CodeGenModule;
			if (codeGenModule == null)
			{
				return new Il2CppRGCTXDefinition[0];
			}
			Il2CppTokenRangePair il2CppTokenRangePair = codeGenModule.RGCTXRanges.FirstOrDefault((Il2CppTokenRangePair r) => r.token == token);
			if (il2CppTokenRangePair == null)
			{
				return new Il2CppRGCTXDefinition[0];
			}
			return LibCpp2IlMain.Binary.GetRGCTXDataForPair(codeGenModule, il2CppTokenRangePair);
		}
	}

	public ulong[] RGCTXMethodPointers
	{
		get
		{
			int codegenModuleIndexByName = LibCpp2IlMain.Binary.GetCodegenModuleIndexByName(DeclaringAssembly.Name);
			if (codegenModuleIndexByName < 0)
			{
				return new ulong[0];
			}
			ulong[] pointers = LibCpp2IlMain.Binary.GetCodegenModuleMethodPointers(codegenModuleIndexByName);
			return (from r in RGCTXs
				where r.type == Il2CppRGCTXDataType.IL2CPP_RGCTX_DATA_METHOD
				select pointers[r.MethodIndex]).ToArray();
		}
	}

	public string? Namespace
	{
		get
		{
			if (_cachedNamespace == null)
			{
				_cachedNamespace = ((LibCpp2IlMain.TheMetadata == null) ? null : LibCpp2IlMain.TheMetadata.GetStringFromIndex(namespaceIndex));
			}
			return _cachedNamespace;
		}
	}

	public string? Name
	{
		get
		{
			if (_cachedName == null)
			{
				_cachedName = ((LibCpp2IlMain.TheMetadata == null) ? null : LibCpp2IlMain.TheMetadata.GetStringFromIndex(nameIndex));
			}
			return _cachedName;
		}
	}

	public string? FullName
	{
		get
		{
			if (LibCpp2IlMain.TheMetadata == null)
			{
				return null;
			}
			if (DeclaringType != null)
			{
				return DeclaringType.FullName + "/" + Name;
			}
			return (string.IsNullOrEmpty(Namespace) ? "" : (Namespace + ".")) + Name;
		}
	}

	public Il2CppType? RawBaseType
	{
		get
		{
			if (parentIndex != -1)
			{
				return LibCpp2IlMain.Binary.GetType(parentIndex);
			}
			return null;
		}
	}

	public Il2CppTypeReflectionData? BaseType
	{
		get
		{
			if (parentIndex != -1)
			{
				return LibCpp2ILUtils.GetTypeReflectionData(LibCpp2IlMain.Binary.GetType(parentIndex));
			}
			return null;
		}
	}

	public Il2CppFieldDefinition[]? Fields
	{
		get
		{
			if (LibCpp2IlMain.TheMetadata == null)
			{
				return null;
			}
			if (firstFieldIdx < 0 || field_count == 0)
			{
				return Array.Empty<Il2CppFieldDefinition>();
			}
			Il2CppFieldDefinition[] array = new Il2CppFieldDefinition[field_count];
			Array.Copy(LibCpp2IlMain.TheMetadata.fieldDefs, firstFieldIdx, array, 0, field_count);
			return array;
		}
	}

	public FieldAttributes[]? FieldAttributes => (from idx in Fields?.Select((Il2CppFieldDefinition f) => f.typeIndex)
		select LibCpp2IlMain.Binary.GetType(idx) into t
		select (FieldAttributes)t.attrs).ToArray();

	public object?[]? FieldDefaults => (from tuple in Fields?.Select((Il2CppFieldDefinition f, int idx) => (FieldIndex: f.FieldIndex, FieldAttributes[idx]))
		select ((tuple.Item2 & System.Reflection.FieldAttributes.HasDefault) == 0) ? null : LibCpp2IlMain.TheMetadata.GetFieldDefaultValueFromIndex(tuple.FieldIndex) into def
		select (def != null) ? LibCpp2ILUtils.GetDefaultValue(def.dataIndex, def.typeIndex) : null).ToArray();

	public Il2CppFieldReflectionData[]? FieldInfos
	{
		get
		{
			Il2CppFieldDefinition[]? fields = Fields;
			FieldAttributes[] attributes = FieldAttributes;
			object?[] defaults = FieldDefaults;
			return fields?.Select(delegate(Il2CppFieldDefinition t, int i)
			{
				Il2CppFieldReflectionData result = default(Il2CppFieldReflectionData);
				result.attributes = attributes[i];
				result.field = t;
				result.defaultValue = defaults[i];
				return result;
			}).ToArray();
		}
	}

	public Il2CppMethodDefinition[]? Methods
	{
		get
		{
			if (LibCpp2IlMain.TheMetadata == null)
			{
				return null;
			}
			if (firstMethodIdx < 0 || method_count == 0)
			{
				return Array.Empty<Il2CppMethodDefinition>();
			}
			Il2CppMethodDefinition[] array = new Il2CppMethodDefinition[method_count];
			Array.Copy(LibCpp2IlMain.TheMetadata.methodDefs, firstMethodIdx, array, 0, method_count);
			return array;
		}
	}

	public Il2CppPropertyDefinition[]? Properties
	{
		get
		{
			if (LibCpp2IlMain.TheMetadata == null)
			{
				return null;
			}
			if (firstPropertyId < 0 || propertyCount == 0)
			{
				return Array.Empty<Il2CppPropertyDefinition>();
			}
			Il2CppPropertyDefinition[] array = new Il2CppPropertyDefinition[propertyCount];
			Array.Copy(LibCpp2IlMain.TheMetadata.propertyDefs, firstPropertyId, array, 0, propertyCount);
			return array.Select(delegate(Il2CppPropertyDefinition p)
			{
				p.DeclaringType = this;
				return p;
			}).ToArray();
		}
	}

	public Il2CppEventDefinition[]? Events
	{
		get
		{
			if (LibCpp2IlMain.TheMetadata != null)
			{
				return LibCpp2IlMain.TheMetadata.eventDefs.Skip(firstEventId).Take(eventCount).Select(delegate(Il2CppEventDefinition e)
				{
					e.DeclaringType = this;
					return e;
				})
					.ToArray();
			}
			return null;
		}
	}

	public Il2CppTypeDefinition[]? NestedTypes
	{
		get
		{
			if (LibCpp2IlMain.TheMetadata != null)
			{
				return (from idx in LibCpp2IlMain.TheMetadata.nestedTypeIndices.Skip(nestedTypesStart).Take(nested_type_count)
					select LibCpp2IlMain.TheMetadata.typeDefs[idx]).ToArray();
			}
			return null;
		}
	}

	public Il2CppType[] RawInterfaces
	{
		get
		{
			if (LibCpp2IlMain.TheMetadata != null && LibCpp2IlMain.Binary != null)
			{
				return (from idx in LibCpp2IlMain.TheMetadata.interfaceIndices.Skip(interfacesStart).Take(interfaces_count)
					select LibCpp2IlMain.Binary.GetType(idx)).ToArray();
			}
			return Array.Empty<Il2CppType>();
		}
	}

	public Il2CppTypeReflectionData[]? Interfaces
	{
		get
		{
			if (LibCpp2IlMain.TheMetadata != null && LibCpp2IlMain.Binary != null)
			{
				return RawInterfaces.Select(LibCpp2ILUtils.GetTypeReflectionData).ToArray();
			}
			return null;
		}
	}

	public Il2CppTypeDefinition? DeclaringType
	{
		get
		{
			if (LibCpp2IlMain.TheMetadata != null && LibCpp2IlMain.Binary != null && declaringTypeIndex >= 0)
			{
				return LibCpp2IlMain.TheMetadata.typeDefs[LibCpp2IlMain.Binary.GetType(declaringTypeIndex).data.classIndex];
			}
			return null;
		}
	}

	public Il2CppGenericContainer? GenericContainer
	{
		get
		{
			if (genericContainerIndex >= 0)
			{
				Il2CppMetadata? theMetadata = LibCpp2IlMain.TheMetadata;
				if (theMetadata == null)
				{
					return null;
				}
				return theMetadata.genericContainers[genericContainerIndex];
			}
			return null;
		}
	}

	public override string ToString()
	{
		if (LibCpp2IlMain.TheMetadata == null)
		{
			return base.ToString();
		}
		return "Il2CppTypeDefinition[namespace='" + Namespace + "', name='" + Name + "', parentType=" + (BaseType?.ToString() ?? "null") + "]";
	}
}
