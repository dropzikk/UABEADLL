using System;
using System.Linq;
using System.Reflection;
using LibCpp2IL.BinaryStructures;
using LibCpp2IL.Reflection;

namespace LibCpp2IL.Metadata;

public class Il2CppEventDefinition
{
	public int nameIndex;

	public int typeIndex;

	public int add;

	public int remove;

	public int raise;

	[Version(Max = 24f)]
	public int customAttributeIndex;

	public uint token;

	[NonSerialized]
	private Il2CppTypeDefinition? _type;

	public Il2CppTypeDefinition? DeclaringType
	{
		get
		{
			if (_type != null)
			{
				return _type;
			}
			if (LibCpp2IlMain.TheMetadata == null)
			{
				return null;
			}
			_type = LibCpp2IlMain.TheMetadata.typeDefs.FirstOrDefault((Il2CppTypeDefinition t) => t.Events.Contains(this));
			return _type;
		}
		internal set
		{
			_type = value;
		}
	}

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

	public Il2CppType? RawType => LibCpp2IlMain.Binary?.GetType(typeIndex);

	public Il2CppTypeReflectionData? EventType
	{
		get
		{
			if (LibCpp2IlMain.Binary != null)
			{
				return LibCpp2ILUtils.GetTypeReflectionData(RawType);
			}
			return null;
		}
	}

	public EventAttributes EventAttributes => (EventAttributes)RawType.attrs;

	public Il2CppMethodDefinition? Adder
	{
		get
		{
			if (LibCpp2IlMain.TheMetadata != null && add >= 0 && DeclaringType != null)
			{
				return LibCpp2IlMain.TheMetadata.methodDefs[DeclaringType.firstMethodIdx + add];
			}
			return null;
		}
	}

	public Il2CppMethodDefinition? Remover
	{
		get
		{
			if (LibCpp2IlMain.TheMetadata != null && remove >= 0 && DeclaringType != null)
			{
				return LibCpp2IlMain.TheMetadata.methodDefs[DeclaringType.firstMethodIdx + remove];
			}
			return null;
		}
	}

	public Il2CppMethodDefinition? Invoker
	{
		get
		{
			if (LibCpp2IlMain.TheMetadata != null && raise >= 0 && DeclaringType != null)
			{
				return LibCpp2IlMain.TheMetadata.methodDefs[DeclaringType.firstMethodIdx + raise];
			}
			return null;
		}
	}
}
