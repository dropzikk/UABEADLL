using System;
using System.Collections.Generic;
using System.Linq;
using AssetsTools.NET.Extra;

namespace AssetsTools.NET;

public class AssetTypeTemplateField
{
	/// <summary>
	/// Name of the field.
	/// </summary>
	public string Name { get; set; }

	/// <summary>
	/// Type name of the field.
	/// </summary>
	public string Type { get; set; }

	/// <summary>
	/// Type of the field (as an enum).
	/// </summary>
	public AssetValueType ValueType { get; set; }

	/// <summary>
	/// Is the field an array?
	/// </summary>
	public bool IsArray { get; set; }

	/// <summary>
	/// Is the field aligned? This aligns four bytes after all children have been read/written.
	/// </summary>
	public bool IsAligned { get; set; }

	/// <summary>
	/// Does the field have value? (i.e. is the field a numeric / string / array type?)
	/// </summary>
	public bool HasValue { get; set; }

	/// <summary>
	/// Children of the field.
	/// </summary>
	public List<AssetTypeTemplateField> Children { get; set; }

	/// <summary>
	/// Read the template field from a type tree type.
	/// </summary>
	/// <param name="typeTreeType">The type tree type to read from.</param>
	public void FromTypeTree(TypeTreeType typeTreeType)
	{
		int fieldIndex = 0;
		FromTypeTree(typeTreeType, ref fieldIndex);
	}

	private void FromTypeTree(TypeTreeType typeTreeType, ref int fieldIndex)
	{
		TypeTreeNode typeTreeNode = typeTreeType.Nodes[fieldIndex];
		Name = typeTreeNode.GetNameString(typeTreeType.StringBuffer);
		Type = typeTreeNode.GetTypeString(typeTreeType.StringBuffer);
		ValueType = AssetTypeValueField.GetValueTypeByTypeName(Type);
		IsArray = Net35Polyfill.HasFlag(typeTreeNode.TypeFlags, TypeTreeNodeFlags.Array);
		IsAligned = (typeTreeNode.MetaFlags & 0x4000) != 0;
		HasValue = ValueType != AssetValueType.None;
		Children = new List<AssetTypeTemplateField>();
		fieldIndex++;
		while (fieldIndex < typeTreeType.Nodes.Count)
		{
			TypeTreeNode typeTreeNode2 = typeTreeType.Nodes[fieldIndex];
			if (typeTreeNode2.Level <= typeTreeNode.Level)
			{
				fieldIndex--;
				break;
			}
			AssetTypeTemplateField assetTypeTemplateField = new AssetTypeTemplateField();
			assetTypeTemplateField.FromTypeTree(typeTreeType, ref fieldIndex);
			Children.Add(assetTypeTemplateField);
			fieldIndex++;
		}
		if (ValueType == AssetValueType.String && !Children[0].IsArray && Children[0].ValueType != 0)
		{
			Type = Children[0].Type;
			ValueType = Children[0].ValueType;
			Children.Clear();
		}
		if (IsArray)
		{
			ValueType = ((Children[1].ValueType == AssetValueType.UInt8) ? AssetValueType.ByteArray : AssetValueType.Array);
		}
		Children.TrimExcess();
	}

	/// <summary>
	/// Read the template field from a class database type.
	/// </summary>
	/// <param name="cldbFile">The class database file to read from.</param>
	/// <param name="cldbType">The class database type to read.</param>
	/// <param name="preferEditor">Read from the editor version of this type if available?</param>
	public void FromClassDatabase(ClassDatabaseFile cldbFile, ClassDatabaseType cldbType, bool preferEditor = false)
	{
		if (cldbType.EditorRootNode == null && cldbType.ReleaseRootNode == null)
		{
			throw new Exception("No root nodes were found!");
		}
		ClassDatabaseTypeNode preferredNode = cldbType.GetPreferredNode(preferEditor);
		FromClassDatabase(cldbFile.StringTable, preferredNode);
	}

	private void FromClassDatabase(ClassDatabaseStringTable strTable, ClassDatabaseTypeNode node)
	{
		Name = strTable.GetString(node.FieldName);
		Type = strTable.GetString(node.TypeName);
		if (Type == "SInt32")
		{
			Type = "int";
		}
		else if (Type == "UInt32")
		{
			Type = "unsigned int";
		}
		ValueType = AssetTypeValueField.GetValueTypeByTypeName(Type);
		IsArray = node.TypeFlags == 1;
		IsAligned = (node.MetaFlag & 0x4000) != 0;
		HasValue = ValueType != AssetValueType.None;
		Children = new List<AssetTypeTemplateField>(node.Children.Count);
		foreach (ClassDatabaseTypeNode child in node.Children)
		{
			AssetTypeTemplateField assetTypeTemplateField = new AssetTypeTemplateField();
			assetTypeTemplateField.FromClassDatabase(strTable, child);
			Children.Add(assetTypeTemplateField);
		}
		if (ValueType == AssetValueType.String && !Children[0].IsArray && Children[0].ValueType != 0)
		{
			Type = Children[0].Type;
			ValueType = Children[0].ValueType;
			Children.Clear();
			Children.TrimExcess();
		}
		if (IsArray)
		{
			ValueType = ((Children[1].ValueType == AssetValueType.UInt8) ? AssetValueType.ByteArray : AssetValueType.Array);
		}
	}

	/// <summary>
	/// Deserialize an asset into a value field.
	/// </summary>
	/// <param name="reader">The reader to use.</param>
	/// <param name="refMan">The ref type manager to use, if reading a MonoBehaviour using a ref type.</param>
	/// <returns>The deserialized base field.</returns>
	public AssetTypeValueField MakeValue(AssetsFileReader reader, RefTypeManager refMan = null)
	{
		AssetTypeValueField valueField = new AssetTypeValueField
		{
			TemplateField = this
		};
		return ReadType(reader, valueField, refMan);
	}

	/// <summary>
	/// Deserialize an asset into a value field.
	/// </summary>
	/// <param name="reader">The reader to use.</param>
	/// <param name="position">The position to start reading from.</param>
	/// <param name="refMan">The ref type manager to use, if reading a MonoBehaviour using a ref type.</param>
	/// <returns>The deserialized value field.</returns>
	public AssetTypeValueField MakeValue(AssetsFileReader reader, long position, RefTypeManager refMan = null)
	{
		reader.Position = position;
		return MakeValue(reader, refMan);
	}

	/// <summary>
	/// Deserialize a single field and its children.
	/// </summary>
	/// <param name="reader">The reader to use.</param>
	/// <param name="valueField">The empty base value field to use.</param>
	/// <param name="refMan">The ref type manager to use, if reading a MonoBehaviour using a ref type.</param>
	/// <returns>The deserialized base field.</returns>
	public AssetTypeValueField ReadType(AssetsFileReader reader, AssetTypeValueField valueField, RefTypeManager refMan)
	{
		if (valueField.TemplateField.IsArray)
		{
			int count = valueField.TemplateField.Children.Count;
			if (count != 2)
			{
				throw new Exception($"Expected array to have two children, found {count} instead!");
			}
			AssetValueType valueType = valueField.TemplateField.Children[0].ValueType;
			if (valueType != AssetValueType.Int32 && valueType != AssetValueType.UInt32)
			{
				throw new Exception($"Expected int array size type, found {valueType} instead!");
			}
			if (valueField.TemplateField.ValueType == AssetValueType.ByteArray)
			{
				valueField.Children = new List<AssetTypeValueField>(0);
				int count2 = reader.ReadInt32();
				byte[] value = reader.ReadBytes(count2);
				if (valueField.TemplateField.IsAligned)
				{
					reader.Align();
				}
				valueField.Value = new AssetTypeValue(AssetValueType.ByteArray, value);
			}
			else
			{
				int num = reader.ReadInt32();
				valueField.Children = new List<AssetTypeValueField>(num);
				for (int i = 0; i < num; i++)
				{
					AssetTypeValueField assetTypeValueField = new AssetTypeValueField();
					assetTypeValueField.TemplateField = valueField.TemplateField.Children[1];
					valueField.Children.Add(ReadType(reader, assetTypeValueField, refMan));
				}
				valueField.Children.TrimExcess();
				if (valueField.TemplateField.IsAligned)
				{
					reader.Align();
				}
				AssetTypeArrayInfo assetTypeArrayInfo = default(AssetTypeArrayInfo);
				assetTypeArrayInfo.size = num;
				AssetTypeArrayInfo assetTypeArrayInfo2 = assetTypeArrayInfo;
				valueField.Value = new AssetTypeValue(AssetValueType.Array, assetTypeArrayInfo2);
			}
		}
		else
		{
			switch (valueField.TemplateField.ValueType)
			{
			case AssetValueType.None:
			{
				int count5 = valueField.TemplateField.Children.Count;
				valueField.Children = new List<AssetTypeValueField>(count5);
				for (int k = 0; k < count5; k++)
				{
					AssetTypeValueField assetTypeValueField2 = new AssetTypeValueField();
					assetTypeValueField2.TemplateField = valueField.TemplateField.Children[k];
					valueField.Children.Add(ReadType(reader, assetTypeValueField2, refMan));
				}
				valueField.Children.TrimExcess();
				valueField.Value = null;
				if (valueField.TemplateField.IsAligned)
				{
					reader.Align();
				}
				break;
			}
			case AssetValueType.String:
			{
				valueField.Children = new List<AssetTypeValueField>(0);
				int count3 = reader.ReadInt32();
				valueField.Value = new AssetTypeValue(reader.ReadBytes(count3), asString: true);
				reader.Align();
				break;
			}
			case AssetValueType.ManagedReferencesRegistry:
			{
				if (refMan == null)
				{
					throw new Exception("refMan MUST be set to deserialize objects with ref types!");
				}
				valueField.Children = new List<AssetTypeValueField>(0);
				ManagedReferencesRegistry managedReferencesRegistry = new ManagedReferencesRegistry();
				valueField.Value = new AssetTypeValue(managedReferencesRegistry);
				int count4 = valueField.TemplateField.Children.Count;
				if (count4 != 2)
				{
					throw new Exception($"Expected ManagedReferencesRegistry to have two children, found {count4} instead!");
				}
				managedReferencesRegistry.version = reader.ReadInt32();
				managedReferencesRegistry.references = new List<AssetTypeReferencedObject>(0);
				if (managedReferencesRegistry.version == 1)
				{
					while (true)
					{
						AssetTypeReferencedObject assetTypeReferencedObject = MakeReferencedObject(reader, managedReferencesRegistry.version, managedReferencesRegistry.references.Count, refMan);
						if (assetTypeReferencedObject.type.Equals(AssetTypeReference.TERMINUS))
						{
							break;
						}
						managedReferencesRegistry.references.Add(assetTypeReferencedObject);
					}
				}
				else
				{
					int num2 = reader.ReadInt32();
					for (int j = 0; j < num2; j++)
					{
						AssetTypeReferencedObject item = MakeReferencedObject(reader, managedReferencesRegistry.version, j, refMan);
						managedReferencesRegistry.references.Add(item);
					}
				}
				break;
			}
			default:
				if (valueField.TemplateField.Children.Count == 0)
				{
					valueField.Children = new List<AssetTypeValueField>(0);
					switch (valueField.TemplateField.ValueType)
					{
					case AssetValueType.Int8:
						valueField.Value = new AssetTypeValue(reader.ReadSByte());
						break;
					case AssetValueType.UInt8:
						valueField.Value = new AssetTypeValue(reader.ReadByte());
						break;
					case AssetValueType.Bool:
						valueField.Value = new AssetTypeValue(reader.ReadBoolean());
						break;
					case AssetValueType.Int16:
						valueField.Value = new AssetTypeValue(reader.ReadInt16());
						break;
					case AssetValueType.UInt16:
						valueField.Value = new AssetTypeValue(reader.ReadUInt16());
						break;
					case AssetValueType.Int32:
						valueField.Value = new AssetTypeValue(reader.ReadInt32());
						break;
					case AssetValueType.UInt32:
						valueField.Value = new AssetTypeValue(reader.ReadUInt32());
						break;
					case AssetValueType.Int64:
						valueField.Value = new AssetTypeValue(reader.ReadInt64());
						break;
					case AssetValueType.UInt64:
						valueField.Value = new AssetTypeValue(reader.ReadUInt64());
						break;
					case AssetValueType.Float:
						valueField.Value = new AssetTypeValue(reader.ReadSingle());
						break;
					case AssetValueType.Double:
						valueField.Value = new AssetTypeValue(reader.ReadDouble());
						break;
					}
					if (valueField.TemplateField.IsAligned)
					{
						reader.Align();
					}
				}
				else if (valueField.TemplateField.ValueType != 0)
				{
					throw new Exception("Cannot read value of field with children!");
				}
				break;
			}
		}
		return valueField;
	}

	/// <summary>
	/// Clone the field.
	/// </summary>
	/// <returns>The cloned field.</returns>
	public AssetTypeTemplateField Clone()
	{
		return new AssetTypeTemplateField
		{
			Name = Name,
			Type = Type,
			ValueType = ValueType,
			IsArray = IsArray,
			IsAligned = IsAligned,
			HasValue = HasValue,
			Children = Children.Select((AssetTypeTemplateField c) => c.Clone()).ToList()
		};
	}

	private AssetTypeReferencedObject MakeReferencedObject(AssetsFileReader reader, int registryVersion, int referenceIndex, RefTypeManager refMan)
	{
		AssetTypeReferencedObject assetTypeReferencedObject = new AssetTypeReferencedObject();
		if (registryVersion == 1)
		{
			assetTypeReferencedObject.rid = referenceIndex;
		}
		else
		{
			assetTypeReferencedObject.rid = reader.ReadInt64();
		}
		AssetTypeReference assetTypeReference = new AssetTypeReference();
		assetTypeReference.ReadAsset(reader);
		assetTypeReferencedObject.type = assetTypeReference;
		AssetTypeTemplateField templateField = refMan.GetTemplateField(assetTypeReference);
		if (templateField != null)
		{
			assetTypeReferencedObject.data = new AssetTypeValueField
			{
				TemplateField = templateField
			};
			assetTypeReferencedObject.data = ReadType(reader, assetTypeReferencedObject.data, refMan);
		}
		else
		{
			assetTypeReferencedObject.data = AssetTypeValueField.DUMMY_FIELD;
		}
		return assetTypeReferencedObject;
	}

	public override string ToString()
	{
		return Type + " " + Name;
	}
}
