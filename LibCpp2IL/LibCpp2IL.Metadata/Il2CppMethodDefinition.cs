using System;
using System.Linq;
using System.Reflection;
using LibCpp2IL.BinaryStructures;
using LibCpp2IL.Logging;
using LibCpp2IL.Reflection;

namespace LibCpp2IL.Metadata;

public class Il2CppMethodDefinition
{
	public int nameIndex;

	public int declaringTypeIdx;

	public int returnTypeIdx;

	public int parameterStart;

	[Version(Max = 24f)]
	public int customAttributeIndex;

	public int genericContainerIndex;

	[Version(Max = 24.15f)]
	public int methodIndex;

	[Version(Max = 24.15f)]
	public int invokerIndex;

	[Version(Max = 24.15f)]
	public int delegateWrapperIndex;

	[Version(Max = 24.15f)]
	public int rgctxStartIndex;

	[Version(Max = 24.15f)]
	public int rgctxCount;

	public uint token;

	public ushort flags;

	public ushort iflags;

	public ushort slot;

	public ushort parameterCount;

	private ulong? _methodPointer;

	private Il2CppParameterReflectionData[]? _cachedParameters;

	public MethodAttributes Attributes => (MethodAttributes)flags;

	public bool IsStatic => (Attributes & MethodAttributes.Static) != 0;

	public int MethodIndex => LibCpp2IlReflection.GetMethodIndexFromMethod(this);

	public string? Name
	{
		get
		{
			if (LibCpp2IlMain.TheMetadata != null)
			{
				return LibCpp2IlMain.TheMetadata.GetStringFromIndex(nameIndex);
			}
			return null;
		}
	}

	public string? GlobalKey
	{
		get
		{
			if (DeclaringType != null)
			{
				return DeclaringType.Name + "." + Name + "()";
			}
			return null;
		}
	}

	public Il2CppType? RawReturnType => LibCpp2IlMain.Binary?.GetType(returnTypeIdx);

	public Il2CppTypeReflectionData? ReturnType
	{
		get
		{
			if (LibCpp2IlMain.Binary != null)
			{
				return LibCpp2ILUtils.GetTypeReflectionData(LibCpp2IlMain.Binary.GetType(returnTypeIdx));
			}
			return null;
		}
	}

	public Il2CppTypeDefinition? DeclaringType
	{
		get
		{
			if (LibCpp2IlMain.TheMetadata != null)
			{
				return LibCpp2IlMain.TheMetadata.typeDefs[declaringTypeIdx];
			}
			return null;
		}
	}

	public ulong MethodPointer
	{
		get
		{
			if (!_methodPointer.HasValue)
			{
				if (LibCpp2IlMain.Binary == null || LibCpp2IlMain.TheMetadata == null || DeclaringType == null)
				{
					LibLogger.WarnNewline($"Couldn't get method pointer for {Name}. Binary is {LibCpp2IlMain.Binary}, Meta is {LibCpp2IlMain.TheMetadata}, DeclaringType is {DeclaringType}");
					return 0uL;
				}
				int imageIndex = 0;
				if (LibCpp2IlMain.MetadataVersion >= 27f)
				{
					imageIndex = LibCpp2IlMain.Binary.GetCodegenModuleIndexByName(DeclaringType.DeclaringAssembly.Name);
				}
				else if (LibCpp2IlMain.MetadataVersion >= 24.2f)
				{
					imageIndex = DeclaringType.DeclaringAssembly.assemblyIndex;
				}
				_methodPointer = LibCpp2IlMain.Binary.GetMethodPointer(methodIndex, MethodIndex, imageIndex, token);
			}
			return _methodPointer.Value;
		}
	}

	public long MethodOffsetInFile
	{
		get
		{
			if (MethodPointer != 0L && LibCpp2IlMain.Binary != null)
			{
				if (!LibCpp2IlMain.Binary.TryMapVirtualAddressToRaw(MethodPointer, out var result))
				{
					return 0L;
				}
				return result;
			}
			return 0L;
		}
	}

	public ulong Rva
	{
		get
		{
			if (MethodPointer != 0L && LibCpp2IlMain.Binary != null)
			{
				return LibCpp2IlMain.Binary.GetRVA(MethodPointer);
			}
			return 0uL;
		}
	}

	public string? HumanReadableSignature
	{
		get
		{
			if (ReturnType != null && Parameters != null && Name != null)
			{
				return string.Format("{0} {1}({2})", ReturnType, Name, string.Join(", ", Parameters.AsEnumerable()));
			}
			return null;
		}
	}

	public Il2CppParameterDefinition[]? InternalParameterData
	{
		get
		{
			if (LibCpp2IlMain.TheMetadata == null || LibCpp2IlMain.Binary == null)
			{
				return null;
			}
			if (parameterStart < 0 || parameterCount == 0)
			{
				return Array.Empty<Il2CppParameterDefinition>();
			}
			Il2CppParameterDefinition[] array = new Il2CppParameterDefinition[parameterCount];
			Array.Copy(LibCpp2IlMain.TheMetadata.parameterDefs, parameterStart, array, 0, parameterCount);
			return array;
		}
	}

	public Il2CppType[]? InternalParameterTypes
	{
		get
		{
			if (InternalParameterData != null)
			{
				return InternalParameterData.Select((Il2CppParameterDefinition paramDef) => LibCpp2IlMain.Binary.GetType(paramDef.typeIndex)).ToArray();
			}
			return null;
		}
	}

	public Il2CppParameterReflectionData[]? Parameters
	{
		get
		{
			if (_cachedParameters == null && InternalParameterData != null)
			{
				_cachedParameters = InternalParameterData.Select(delegate(Il2CppParameterDefinition paramDef, int idx)
				{
					Il2CppType type = LibCpp2IlMain.Binary.GetType(paramDef.typeIndex);
					ParameterAttributes attrs = (ParameterAttributes)type.attrs;
					Il2CppParameterDefaultValue il2CppParameterDefaultValue = (((attrs & ParameterAttributes.HasDefault) != 0) ? LibCpp2IlMain.TheMetadata.GetParameterDefaultValueFromIndex(parameterStart + idx) : null);
					return new Il2CppParameterReflectionData
					{
						Type = LibCpp2ILUtils.GetTypeReflectionData(type),
						ParameterName = LibCpp2IlMain.TheMetadata.GetStringFromIndex(paramDef.nameIndex),
						ParameterAttributes = attrs,
						RawType = type,
						DefaultValue = ((il2CppParameterDefaultValue == null) ? null : LibCpp2ILUtils.GetDefaultValue(il2CppParameterDefaultValue.dataIndex, il2CppParameterDefaultValue.typeIndex))
					};
				}).ToArray();
			}
			return _cachedParameters;
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
		return "Il2CppMethodDefinition[Name='" + Name + "']";
	}
}
