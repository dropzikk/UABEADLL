using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace AssetsTools.NET;

public class AssetTypeValueField : IEnumerable<AssetTypeValueField>, IEnumerable
{
	/// <summary>
	/// The field which indicates that a field that was accessed does not exist.
	/// </summary>
	public static readonly AssetTypeValueField DUMMY_FIELD = new AssetTypeValueField
	{
		TemplateField = new AssetTypeTemplateField
		{
			Name = "DUMMY",
			HasValue = false,
			IsAligned = false,
			IsArray = false,
			Type = "DUMMY",
			ValueType = AssetValueType.None,
			Children = new List<AssetTypeTemplateField>(0)
		},
		Value = null,
		IsDummy = true,
		Children = new List<AssetTypeValueField>(0)
	};

	/// <summary>
	/// Template field corresponding to this value field.
	/// </summary>
	public AssetTypeTemplateField TemplateField { get; set; }

	/// <summary>
	/// Value of this field.
	/// </summary>
	public AssetTypeValue Value { get; set; }

	/// <summary>
	/// Children of this field.
	/// </summary>
	public List<AssetTypeValueField> Children { get; set; }

	public bool IsDummy { get; set; }

	public AssetTypeValueField this[string name]
	{
		get
		{
			if (IsDummy)
			{
				throw new DummyFieldAccessException("Cannot access fields of a dummy field!");
			}
			if (name.Contains("."))
			{
				string[] array = name.Split(new char[1] { '.' });
				AssetTypeValueField assetTypeValueField = this;
				string[] array2 = array;
				foreach (string text in array2)
				{
					bool flag = false;
					foreach (AssetTypeValueField child in assetTypeValueField.Children)
					{
						if (child.TemplateField.Name == text)
						{
							flag = true;
							assetTypeValueField = child;
							break;
						}
					}
					if (!flag)
					{
						return DUMMY_FIELD;
					}
				}
				return assetTypeValueField;
			}
			foreach (AssetTypeValueField child2 in Children)
			{
				if (child2.TemplateField.Name == name)
				{
					return child2;
				}
			}
			return DUMMY_FIELD;
		}
	}

	public AssetTypeValueField this[int index]
	{
		get
		{
			if (IsDummy)
			{
				throw new DummyFieldAccessException("Cannot access fields of a dummy field!");
			}
			return Children[index];
		}
	}

	public bool AsBool
	{
		get
		{
			return Value.AsBool;
		}
		set
		{
			Value.AsBool = value;
		}
	}

	public sbyte AsSByte
	{
		get
		{
			return Value.AsSByte;
		}
		set
		{
			Value.AsSByte = value;
		}
	}

	public byte AsByte
	{
		get
		{
			return Value.AsByte;
		}
		set
		{
			Value.AsByte = value;
		}
	}

	public short AsShort
	{
		get
		{
			return Value.AsShort;
		}
		set
		{
			Value.AsShort = value;
		}
	}

	public ushort AsUShort
	{
		get
		{
			return Value.AsUShort;
		}
		set
		{
			Value.AsUShort = value;
		}
	}

	public int AsInt
	{
		get
		{
			return Value.AsInt;
		}
		set
		{
			Value.AsInt = value;
		}
	}

	public uint AsUInt
	{
		get
		{
			return Value.AsUInt;
		}
		set
		{
			Value.AsUInt = value;
		}
	}

	public long AsLong
	{
		get
		{
			return Value.AsLong;
		}
		set
		{
			Value.AsLong = value;
		}
	}

	public ulong AsULong
	{
		get
		{
			return Value.AsULong;
		}
		set
		{
			Value.AsULong = value;
		}
	}

	public float AsFloat
	{
		get
		{
			return Value.AsFloat;
		}
		set
		{
			Value.AsFloat = value;
		}
	}

	public double AsDouble
	{
		get
		{
			return Value.AsDouble;
		}
		set
		{
			Value.AsDouble = value;
		}
	}

	public string AsString
	{
		get
		{
			return Value.AsString;
		}
		set
		{
			Value.AsString = value;
		}
	}

	public object AsObject
	{
		get
		{
			return Value.AsObject;
		}
		set
		{
			Value.AsObject = value;
		}
	}

	public AssetTypeArrayInfo AsArray
	{
		get
		{
			return Value.AsArray;
		}
		set
		{
			Value.AsArray = value;
		}
	}

	public byte[] AsByteArray
	{
		get
		{
			return Value.AsByteArray;
		}
		set
		{
			Value.AsByteArray = value;
		}
	}

	public ManagedReferencesRegistry AsManagedReferencesRegistry
	{
		get
		{
			return Value.AsManagedReferencesRegistry;
		}
		set
		{
			Value.AsManagedReferencesRegistry = value;
		}
	}

	public string TypeName => TemplateField.Type;

	public string FieldName => TemplateField.Name;

	/// <summary>
	/// Read the <see cref="T:AssetsTools.NET.AssetTypeValueField" /> from a value, template field, and children.
	/// </summary>
	/// <param name="value">The value to use.</param>
	/// <param name="templateField">The template field to use.</param>
	/// <param name="children">The children to use.</param>
	public void Read(AssetTypeValue value, AssetTypeTemplateField templateField, List<AssetTypeValueField> children)
	{
		Value = value;
		TemplateField = templateField;
		Children = children;
		IsDummy = false;
	}

	public AssetTypeValueField Get(string name)
	{
		return this[name];
	}

	public AssetTypeValueField Get(int index)
	{
		return this[index];
	}

	public static AssetValueType GetValueTypeByTypeName(string type)
	{
		switch (type)
		{
		case "string":
			return AssetValueType.String;
		case "SInt8":
		case "char":
			return AssetValueType.Int8;
		case "UInt8":
		case "unsigned char":
			return AssetValueType.UInt8;
		case "SInt16":
		case "short":
			return AssetValueType.Int16;
		case "UInt16":
		case "unsigned short":
			return AssetValueType.UInt16;
		case "SInt32":
		case "Type*":
		case "int":
			return AssetValueType.Int32;
		case "UInt32":
		case "unsigned int":
			return AssetValueType.UInt32;
		case "SInt64":
		case "long":
			return AssetValueType.Int64;
		case "UInt64":
		case "unsigned long long":
		case "FileSize":
			return AssetValueType.UInt64;
		case "float":
			return AssetValueType.Float;
		case "double":
			return AssetValueType.Double;
		case "bool":
			return AssetValueType.Bool;
		case "Array":
			return AssetValueType.Array;
		case "TypelessData":
			return AssetValueType.ByteArray;
		case "ManagedReferencesRegistry":
			return AssetValueType.ManagedReferencesRegistry;
		default:
			return AssetValueType.None;
		}
	}

	/// <summary>
	/// Write the <see cref="T:AssetsTools.NET.AssetTypeValueField" /> with the provided writer.
	/// </summary>
	/// <param name="writer">The writer to use.</param>
	public void Write(AssetsFileWriter writer)
	{
		if (TemplateField.IsArray)
		{
			if (TemplateField.ValueType == AssetValueType.ByteArray)
			{
				byte[] asByteArray = AsByteArray;
				writer.Write(asByteArray.Length);
				writer.Write(asByteArray);
				if (TemplateField.IsAligned)
				{
					writer.Align();
				}
				return;
			}
			int count = Children.Count;
			writer.Write(count);
			for (int i = 0; i < count; i++)
			{
				this[i].Write(writer);
			}
			if (TemplateField.IsAligned)
			{
				writer.Align();
			}
		}
		else if (Children.Count == 0)
		{
			switch (TemplateField.ValueType)
			{
			case AssetValueType.Int8:
				writer.Write(AsSByte);
				if (TemplateField.IsAligned)
				{
					writer.Align();
				}
				break;
			case AssetValueType.UInt8:
				writer.Write(AsByte);
				if (TemplateField.IsAligned)
				{
					writer.Align();
				}
				break;
			case AssetValueType.Bool:
				writer.Write(AsBool);
				if (TemplateField.IsAligned)
				{
					writer.Align();
				}
				break;
			case AssetValueType.Int16:
				writer.Write(AsShort);
				if (TemplateField.IsAligned)
				{
					writer.Align();
				}
				break;
			case AssetValueType.UInt16:
				writer.Write(AsUShort);
				if (TemplateField.IsAligned)
				{
					writer.Align();
				}
				break;
			case AssetValueType.Int32:
				writer.Write(AsInt);
				break;
			case AssetValueType.UInt32:
				writer.Write(AsUInt);
				break;
			case AssetValueType.Int64:
				writer.Write(AsLong);
				break;
			case AssetValueType.UInt64:
				writer.Write(AsULong);
				break;
			case AssetValueType.Float:
				writer.Write(AsFloat);
				break;
			case AssetValueType.Double:
				writer.Write(AsDouble);
				break;
			case AssetValueType.String:
				writer.Write(AsByteArray.Length);
				writer.Write(AsByteArray);
				writer.Align();
				break;
			case AssetValueType.ManagedReferencesRegistry:
			{
				writer.Write(AsManagedReferencesRegistry.version);
				int count2 = AsManagedReferencesRegistry.references.Count;
				for (int j = 0; j < count2; j++)
				{
					AssetTypeReferencedObject assetTypeReferencedObject = AsManagedReferencesRegistry.references[j];
					if (AsManagedReferencesRegistry.version != 1)
					{
						writer.Write(assetTypeReferencedObject.rid);
					}
					assetTypeReferencedObject.type.WriteAsset(writer);
					assetTypeReferencedObject.data.Write(writer);
				}
				if (AsManagedReferencesRegistry.version == 1)
				{
					AssetTypeReference.TERMINUS.WriteAsset(writer);
				}
				break;
			}
			case AssetValueType.Array:
			case AssetValueType.ByteArray:
				break;
			}
		}
		else
		{
			for (int k = 0; k < Children.Count; k++)
			{
				this[k].Write(writer);
			}
			if (TemplateField.IsAligned)
			{
				writer.Align();
			}
		}
	}

	/// <summary>
	/// Write the <see cref="T:AssetsTools.NET.AssetTypeValueField" /> with a new writer to a byte array.
	/// </summary>
	/// <param name="bigEndian">Write in big endian?</param>
	public byte[] WriteToByteArray(bool bigEndian = false)
	{
		using MemoryStream memoryStream = new MemoryStream();
		using AssetsFileWriter assetsFileWriter = new AssetsFileWriter(memoryStream);
		assetsFileWriter.BigEndian = bigEndian;
		Write(assetsFileWriter);
		return memoryStream.ToArray();
	}

	public IEnumerator<AssetTypeValueField> GetEnumerator()
	{
		return Children.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return Children.GetEnumerator();
	}

	public override string ToString()
	{
		if (TemplateField != null)
		{
			return TemplateField.ToString();
		}
		return null;
	}
}
