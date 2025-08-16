using System;
using LibCpp2IL.BinaryStructures;
using LibCpp2IL.Metadata;
using LibCpp2IL.Reflection;

namespace LibCpp2IL;

public class MetadataUsage
{
	public readonly MetadataUsageType Type;

	public readonly ulong Offset;

	private readonly uint _value;

	private string? _cachedName;

	private Il2CppType? _cachedType;

	private Il2CppTypeReflectionData? _cachedTypeReflectionData;

	private Il2CppMethodDefinition? _cachedMethod;

	private Il2CppFieldDefinition? _cachedField;

	private string? _cachedLiteral;

	private Il2CppGenericMethodRef? _cachedGenericMethod;

	public uint RawValue => _value;

	public object Value
	{
		get
		{
			switch (Type)
			{
			case MetadataUsageType.TypeInfo:
			case MetadataUsageType.Type:
				return AsType();
			case MetadataUsageType.MethodDef:
				return AsMethod();
			case MetadataUsageType.FieldInfo:
				return AsField();
			case MetadataUsageType.StringLiteral:
				return AsLiteral();
			case MetadataUsageType.MethodRef:
				return AsGenericMethodRef();
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
	}

	public bool IsValid
	{
		get
		{
			try
			{
				_ = Value;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}

	public MetadataUsage(MetadataUsageType type, ulong offset, uint value)
	{
		Type = type;
		_value = value;
		Offset = offset;
	}

	public Il2CppTypeReflectionData AsType()
	{
		if (_cachedTypeReflectionData == null)
		{
			MetadataUsageType type = Type;
			if (type - 1 > MetadataUsageType.TypeInfo)
			{
				throw new Exception($"Cannot cast metadata usage of kind {Type} to a Type");
			}
			try
			{
				_cachedType = LibCpp2IlMain.Binary.GetType((int)_value);
				_cachedTypeReflectionData = LibCpp2ILUtils.GetTypeReflectionData(_cachedType);
				_cachedName = LibCpp2ILUtils.GetTypeReflectionData(_cachedType)?.ToString();
			}
			catch (Exception innerException)
			{
				throw new Exception($"Failed to convert this metadata usage to a type, but it is of type {Type}, with a value of {_value} (0x{_value:X}). There are {LibCpp2IlMain.Binary.NumTypes} types", innerException);
			}
		}
		return _cachedTypeReflectionData;
	}

	public Il2CppMethodDefinition AsMethod()
	{
		if (_cachedMethod == null)
		{
			if (Type != MetadataUsageType.MethodDef)
			{
				throw new Exception($"Cannot cast metadata usage of kind {Type} to a Method Def");
			}
			_cachedMethod = LibCpp2IlMain.TheMetadata.methodDefs[_value];
			_cachedName = _cachedMethod.GlobalKey;
		}
		return _cachedMethod;
	}

	public Il2CppFieldDefinition AsField()
	{
		if (_cachedField == null)
		{
			if (Type != MetadataUsageType.FieldInfo)
			{
				throw new Exception($"Cannot cast metadata usage of kind {Type} to a Field");
			}
			Il2CppFieldRef il2CppFieldRef = LibCpp2IlMain.TheMetadata.fieldRefs[_value];
			_cachedField = il2CppFieldRef.FieldDefinition;
			_cachedName = il2CppFieldRef.DeclaringTypeDefinition.FullName + "." + _cachedField.Name;
		}
		return _cachedField;
	}

	public string AsLiteral()
	{
		if (_cachedLiteral == null)
		{
			if (Type != MetadataUsageType.StringLiteral)
			{
				throw new Exception($"Cannot cast metadata usage of kind {Type} to a String Literal");
			}
			_cachedName = (_cachedLiteral = LibCpp2IlMain.TheMetadata.GetStringLiteralFromIndex(_value));
		}
		return _cachedLiteral;
	}

	public Il2CppGenericMethodRef AsGenericMethodRef()
	{
		if (_cachedGenericMethod == null)
		{
			if (Type != MetadataUsageType.MethodRef)
			{
				throw new Exception($"Cannot cast metadata usage of kind {Type} to a Generic Method Ref");
			}
			Il2CppMethodSpec methodSpec = LibCpp2IlMain.Binary.GetMethodSpec((int)_value);
			_cachedGenericMethod = new Il2CppGenericMethodRef(methodSpec);
			_cachedName = _cachedGenericMethod.ToString();
		}
		return _cachedGenericMethod;
	}

	public override string ToString()
	{
		return $"Metadata Usage {{type={Type}, Value={Value}}}";
	}

	public static MetadataUsage? DecodeMetadataUsage(ulong encoded, ulong address)
	{
		MetadataUsageType metadataUsageType = (MetadataUsageType)((encoded & 0xE0000000u) >> 29);
		if (metadataUsageType <= MetadataUsageType.MethodRef && metadataUsageType >= MetadataUsageType.TypeInfo)
		{
			uint num = (uint)(encoded & 0x1FFFFFFF);
			if (LibCpp2IlMain.MetadataVersion >= 27f)
			{
				num >>= 1;
			}
			if ((metadataUsageType == MetadataUsageType.Type || metadataUsageType == MetadataUsageType.TypeInfo) && num > LibCpp2IlMain.Binary.NumTypes)
			{
				return null;
			}
			if (metadataUsageType == MetadataUsageType.MethodDef && num > LibCpp2IlMain.TheMetadata.methodDefs.Length)
			{
				return null;
			}
			return new MetadataUsage(metadataUsageType, address, num);
		}
		return null;
	}
}
