using System;
using System.Text;

namespace AssetsTools.NET;

public class AssetTypeValue
{
	public AssetValueType ValueType { get; set; }

	private object Value { get; set; }

	public bool AsBool
	{
		get
		{
			if (Value is bool result)
			{
				return result;
			}
			if (Value is byte b)
			{
				return b == 1;
			}
			return false;
		}
		set
		{
			Value = value;
		}
	}

	public sbyte AsSByte
	{
		get
		{
			if (Value is sbyte result)
			{
				return result;
			}
			return (sbyte)Convert.ChangeType(Value, typeof(sbyte));
		}
		set
		{
			Value = value;
		}
	}

	public byte AsByte
	{
		get
		{
			if (Value is byte result)
			{
				return result;
			}
			return (byte)Convert.ChangeType(Value, typeof(byte));
		}
		set
		{
			Value = value;
		}
	}

	public short AsShort
	{
		get
		{
			if (Value is short result)
			{
				return result;
			}
			return (short)Convert.ChangeType(Value, typeof(short));
		}
		set
		{
			Value = value;
		}
	}

	public ushort AsUShort
	{
		get
		{
			if (Value is ushort result)
			{
				return result;
			}
			return (ushort)Convert.ChangeType(Value, typeof(ushort));
		}
		set
		{
			Value = value;
		}
	}

	public int AsInt
	{
		get
		{
			if (Value is int result)
			{
				return result;
			}
			return (int)Convert.ChangeType(Value, typeof(int));
		}
		set
		{
			Value = value;
		}
	}

	public uint AsUInt
	{
		get
		{
			if (Value is uint result)
			{
				return result;
			}
			return (uint)Convert.ChangeType(Value, typeof(uint));
		}
		set
		{
			Value = value;
		}
	}

	public long AsLong
	{
		get
		{
			if (Value is long result)
			{
				return result;
			}
			return (long)Convert.ChangeType(Value, typeof(long));
		}
		set
		{
			Value = value;
		}
	}

	public ulong AsULong
	{
		get
		{
			if (Value is ulong result)
			{
				return result;
			}
			return (ulong)Convert.ChangeType(Value, typeof(ulong));
		}
		set
		{
			Value = value;
		}
	}

	public float AsFloat
	{
		get
		{
			if (Value is float result)
			{
				return result;
			}
			return (float)Convert.ChangeType(Value, typeof(float));
		}
		set
		{
			Value = value;
		}
	}

	public double AsDouble
	{
		get
		{
			if (Value is double result)
			{
				return result;
			}
			return (double)Convert.ChangeType(Value, typeof(double));
		}
		set
		{
			Value = value;
		}
	}

	public string AsString
	{
		get
		{
			if (ValueType == AssetValueType.String)
			{
				return Encoding.UTF8.GetString((byte[])Value);
			}
			if (ValueType == AssetValueType.Bool)
			{
				return ((bool)Value) ? "true" : "false";
			}
			if (ValueType == AssetValueType.ByteArray)
			{
				return SimpleHexDump((byte[])Value);
			}
			return Value.ToString();
		}
		set
		{
			Value = Encoding.UTF8.GetBytes(value);
		}
	}

	public AssetTypeArrayInfo AsArray
	{
		get
		{
			return (AssetTypeArrayInfo)Value;
		}
		set
		{
			Value = value;
		}
	}

	public byte[] AsByteArray
	{
		get
		{
			return (byte[])Value;
		}
		set
		{
			Value = value;
		}
	}

	public ManagedReferencesRegistry AsManagedReferencesRegistry
	{
		get
		{
			return (ManagedReferencesRegistry)Value;
		}
		set
		{
			Value = value;
		}
	}

	public object AsObject
	{
		get
		{
			return Value;
		}
		set
		{
			if (value is string s)
			{
				Value = Encoding.UTF8.GetBytes(s);
			}
			else
			{
				Value = value;
			}
		}
	}

	public AssetTypeValue(bool value)
	{
		ValueType = AssetValueType.Bool;
		Value = value;
	}

	public AssetTypeValue(sbyte value)
	{
		ValueType = AssetValueType.Int8;
		Value = value;
	}

	public AssetTypeValue(byte value)
	{
		ValueType = AssetValueType.UInt8;
		Value = value;
	}

	public AssetTypeValue(short value)
	{
		ValueType = AssetValueType.Int16;
		Value = value;
	}

	public AssetTypeValue(ushort value)
	{
		ValueType = AssetValueType.UInt16;
		Value = value;
	}

	public AssetTypeValue(int value)
	{
		ValueType = AssetValueType.Int32;
		Value = value;
	}

	public AssetTypeValue(uint value)
	{
		ValueType = AssetValueType.UInt32;
		Value = value;
	}

	public AssetTypeValue(long value)
	{
		ValueType = AssetValueType.Int64;
		Value = value;
	}

	public AssetTypeValue(ulong value)
	{
		ValueType = AssetValueType.UInt64;
		Value = value;
	}

	public AssetTypeValue(float value)
	{
		ValueType = AssetValueType.Float;
		Value = value;
	}

	public AssetTypeValue(double value)
	{
		ValueType = AssetValueType.Double;
		Value = value;
	}

	public AssetTypeValue(string value)
	{
		ValueType = AssetValueType.String;
		Value = Encoding.UTF8.GetBytes(value);
	}

	public AssetTypeValue(byte[] value, bool asString)
	{
		ValueType = (asString ? AssetValueType.String : AssetValueType.ByteArray);
		Value = value;
	}

	public AssetTypeValue(ManagedReferencesRegistry value)
	{
		ValueType = AssetValueType.ManagedReferencesRegistry;
		Value = value;
	}

	public AssetTypeValue(AssetValueType valueType, object value = null)
	{
		ValueType = valueType;
		if (value is string s)
		{
			Value = Encoding.UTF8.GetBytes(s);
		}
		else
		{
			Value = value;
		}
	}

	public override string ToString()
	{
		return AsString;
	}

	private string SimpleHexDump(byte[] byteArray)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (byteArray.Length == 0)
		{
			return string.Empty;
		}
		int i;
		for (i = 0; i < byteArray.Length - 1; i++)
		{
			stringBuilder.Append(byteArray[i].ToString("x2"));
			stringBuilder.Append(" ");
		}
		stringBuilder.Append(byteArray[i].ToString("x2"));
		return stringBuilder.ToString();
	}
}
