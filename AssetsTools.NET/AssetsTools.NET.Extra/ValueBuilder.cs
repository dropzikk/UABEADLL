using System.Collections.Generic;

namespace AssetsTools.NET.Extra;

public class ValueBuilder
{
	public static AssetTypeValueField DefaultValueFieldFromArrayTemplate(AssetTypeValueField arrayField)
	{
		return DefaultValueFieldFromArrayTemplate(arrayField.TemplateField);
	}

	public static AssetTypeValueField DefaultValueFieldFromArrayTemplate(AssetTypeTemplateField arrayField)
	{
		if (!arrayField.IsArray)
		{
			return null;
		}
		AssetTypeTemplateField templateField = arrayField.Children[1];
		return DefaultValueFieldFromTemplate(templateField);
	}

	public static AssetTypeValueField DefaultValueFieldFromTemplate(AssetTypeTemplateField templateField)
	{
		List<AssetTypeTemplateField> children = templateField.Children;
		List<AssetTypeValueField> list;
		if (templateField.IsArray || templateField.ValueType == AssetValueType.String)
		{
			list = new List<AssetTypeValueField>(0);
		}
		else
		{
			list = new List<AssetTypeValueField>(children.Count);
			for (int i = 0; i < children.Count; i++)
			{
				list.Add(DefaultValueFieldFromTemplate(children[i]));
			}
		}
		AssetTypeValue value = DefaultValueFromTemplate(templateField);
		return new AssetTypeValueField
		{
			Children = list,
			TemplateField = templateField,
			Value = value
		};
	}

	public static AssetTypeValue DefaultValueFromTemplate(AssetTypeTemplateField templateField)
	{
		object obj;
		switch (templateField.ValueType)
		{
		case AssetValueType.Int8:
			obj = (sbyte)0;
			break;
		case AssetValueType.UInt8:
			obj = (byte)0;
			break;
		case AssetValueType.Bool:
			obj = false;
			break;
		case AssetValueType.Int16:
			obj = (short)0;
			break;
		case AssetValueType.UInt16:
			obj = (ushort)0;
			break;
		case AssetValueType.Int32:
			obj = 0;
			break;
		case AssetValueType.UInt32:
			obj = 0u;
			break;
		case AssetValueType.Int64:
			obj = 0L;
			break;
		case AssetValueType.UInt64:
			obj = 0uL;
			break;
		case AssetValueType.Float:
			obj = 0f;
			break;
		case AssetValueType.Double:
			obj = 0.0;
			break;
		case AssetValueType.String:
		case AssetValueType.ByteArray:
			obj = new byte[0];
			break;
		case AssetValueType.Array:
			obj = default(AssetTypeArrayInfo);
			break;
		case AssetValueType.ManagedReferencesRegistry:
			obj = new ManagedReferencesRegistry();
			break;
		default:
			obj = null;
			break;
		}
		if (obj == null && templateField.IsArray)
		{
			obj = default(AssetTypeArrayInfo);
			return new AssetTypeValue(AssetValueType.Array, obj);
		}
		return new AssetTypeValue(templateField.ValueType, obj);
	}
}
