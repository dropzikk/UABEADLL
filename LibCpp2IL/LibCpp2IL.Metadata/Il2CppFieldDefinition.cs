using System;
using LibCpp2IL.BinaryStructures;
using LibCpp2IL.Reflection;

namespace LibCpp2IL.Metadata;

public class Il2CppFieldDefinition
{
	public int nameIndex;

	public int typeIndex;

	[Version(Max = 24f)]
	public int customAttributeIndex;

	public uint token;

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

	public Il2CppType? RawFieldType => LibCpp2IlMain.Binary?.GetType(typeIndex);

	public Il2CppTypeReflectionData? FieldType
	{
		get
		{
			if (RawFieldType != null)
			{
				return LibCpp2ILUtils.GetTypeReflectionData(RawFieldType);
			}
			return null;
		}
	}

	public int FieldIndex => LibCpp2IlReflection.GetFieldIndexFromField(this);

	public Il2CppFieldDefaultValue? DefaultValue => LibCpp2IlMain.TheMetadata?.GetFieldDefaultValue(this);

	public byte[] StaticArrayInitialValue
	{
		get
		{
			Il2CppTypeReflectionData fieldType = FieldType;
			if (fieldType == null || fieldType.isArray || fieldType.isPointer || !fieldType.isType || fieldType.isGenericType)
			{
				return Array.Empty<byte>();
			}
			string? name = FieldType.baseType.Name;
			if (name == null || !name.StartsWith("__StaticArrayInitTypeSize="))
			{
				return Array.Empty<byte>();
			}
			int count = int.Parse(FieldType.baseType.Name.Replace("__StaticArrayInitTypeSize=", ""));
			int item = LibCpp2IlMain.TheMetadata.GetFieldDefaultValue(FieldIndex).ptr;
			int defaultValueFromIndex = LibCpp2IlMain.TheMetadata.GetDefaultValueFromIndex(item);
			if (defaultValueFromIndex <= 0)
			{
				return Array.Empty<byte>();
			}
			return LibCpp2IlMain.TheMetadata.ReadByteArrayAtRawAddress(defaultValueFromIndex, count);
		}
	}

	public override string ToString()
	{
		if (LibCpp2IlMain.TheMetadata == null)
		{
			return base.ToString();
		}
		return $"Il2CppFieldDefinition[Name={Name}, FieldType={FieldType}]";
	}
}
