using System;
using System.Linq;
using LibCpp2IL.Reflection;

namespace LibCpp2IL.Metadata;

public class Il2CppPropertyDefinition
{
	public int nameIndex;

	public int get;

	public int set;

	public uint attrs;

	[Version(Max = 24f)]
	public int customAttributeIndex;

	public uint token;

	[NonSerialized]
	private Il2CppTypeDefinition? _type;

	public int PropertyIndex => LibCpp2IlReflection.GetPropertyIndexFromProperty(this);

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
			_type = LibCpp2IlMain.TheMetadata.typeDefs.FirstOrDefault((Il2CppTypeDefinition t) => t.Properties.Contains(this));
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

	public Il2CppMethodDefinition? Getter
	{
		get
		{
			if (LibCpp2IlMain.TheMetadata != null && get >= 0 && DeclaringType != null)
			{
				return LibCpp2IlMain.TheMetadata.methodDefs[DeclaringType.firstMethodIdx + get];
			}
			return null;
		}
	}

	public Il2CppMethodDefinition? Setter
	{
		get
		{
			if (LibCpp2IlMain.TheMetadata != null && set >= 0 && DeclaringType != null)
			{
				return LibCpp2IlMain.TheMetadata.methodDefs[DeclaringType.firstMethodIdx + set];
			}
			return null;
		}
	}

	public Il2CppTypeReflectionData? PropertyType
	{
		get
		{
			if (LibCpp2IlMain.TheMetadata != null)
			{
				if (Getter != null)
				{
					return Getter.ReturnType;
				}
				return Setter.Parameters[0].Type;
			}
			return null;
		}
	}
}
