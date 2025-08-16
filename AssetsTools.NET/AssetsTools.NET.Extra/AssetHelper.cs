using System.Collections.Generic;

namespace AssetsTools.NET.Extra;

public static class AssetHelper
{
	public static Dictionary<int, AssetTypeReference> GetAssetsFileScriptInfos(AssetsManager am, AssetsFileInstance inst)
	{
		Dictionary<int, AssetTypeReference> dictionary = new Dictionary<int, AssetTypeReference>();
		List<AssetPPtr> scriptTypes = inst.file.Metadata.ScriptTypes;
		for (int i = 0; i < scriptTypes.Count; i++)
		{
			AssetTypeReference assetsFileScriptInfo = GetAssetsFileScriptInfo(am, inst, i);
			if (assetsFileScriptInfo != null)
			{
				dictionary[i] = assetsFileScriptInfo;
			}
		}
		return dictionary;
	}

	public static AssetTypeReference GetAssetsFileScriptInfo(AssetsManager am, AssetsFileInstance inst, int index)
	{
		List<AssetPPtr> scriptTypes = inst.file.Metadata.ScriptTypes;
		AssetPPtr assetPPtr = scriptTypes[index];
		AssetTypeValueField baseField;
		try
		{
			baseField = am.GetExtAsset(inst, assetPPtr.FileId, assetPPtr.PathId).baseField;
			if (baseField == null)
			{
				return null;
			}
		}
		catch
		{
			return null;
		}
		AssetTypeValueField assetTypeValueField = baseField["m_AssemblyName"];
		AssetTypeValueField assetTypeValueField2 = baseField["m_Namespace"];
		AssetTypeValueField assetTypeValueField3 = baseField["m_ClassName"];
		if (assetTypeValueField.IsDummy || assetTypeValueField2.IsDummy || assetTypeValueField3.IsDummy)
		{
			return null;
		}
		string asString = assetTypeValueField.AsString;
		string asString2 = assetTypeValueField2.AsString;
		string asString3 = assetTypeValueField3.AsString;
		return new AssetTypeReference(asString3, asString2, asString);
	}

	public static string GetAssetNameFast(AssetsFile file, ClassDatabaseFile cldb, AssetFileInfo info)
	{
		ClassDatabaseType classDatabaseType = cldb.FindAssetClassByID(info.TypeId);
		AssetsFileReader reader = file.Reader;
		if (file.Metadata.TypeTreeEnabled)
		{
			ushort scriptIndex = file.GetScriptIndex(info);
			TypeTreeType typeTreeType = file.Metadata.FindTypeTreeTypeByID(info.TypeId, scriptIndex);
			string typeString = typeTreeType.Nodes[0].GetTypeString(typeTreeType.StringBuffer);
			if (typeTreeType.Nodes.Count == 0)
			{
				return cldb.GetString(classDatabaseType.Name);
			}
			if (typeTreeType.Nodes.Count > 1 && typeTreeType.Nodes[1].GetNameString(typeTreeType.StringBuffer) == "m_Name")
			{
				reader.Position = info.AbsoluteByteStart;
				return reader.ReadCountStringInt32();
			}
			if (typeString == "GameObject")
			{
				reader.Position = info.AbsoluteByteStart;
				int num = reader.ReadInt32();
				int num2 = ((file.Header.Version > 16) ? 12 : 16);
				reader.Position += num * num2;
				reader.Position += 4L;
				return reader.ReadCountStringInt32();
			}
			if (typeString == "MonoBehaviour")
			{
				reader.Position = info.AbsoluteByteStart;
				reader.Position += 28L;
				string text = reader.ReadCountStringInt32();
				if (text != "")
				{
					return text;
				}
			}
			return typeString;
		}
		string @string = cldb.GetString(classDatabaseType.Name);
		if (classDatabaseType.ReleaseRootNode.Children.Count == 0)
		{
			return @string;
		}
		if (classDatabaseType.ReleaseRootNode.Children.Count > 1 && cldb.GetString(classDatabaseType.ReleaseRootNode.Children[0].FieldName) == "m_Name")
		{
			reader.Position = info.AbsoluteByteStart;
			return reader.ReadCountStringInt32();
		}
		if (@string == "GameObject")
		{
			reader.Position = info.AbsoluteByteStart;
			int num3 = reader.ReadInt32();
			int num4 = ((file.Header.Version > 16) ? 12 : 16);
			reader.Position += num3 * num4;
			reader.Position += 4L;
			return reader.ReadCountStringInt32();
		}
		if (@string == "MonoBehaviour")
		{
			reader.Position = info.AbsoluteByteStart;
			reader.Position += 28L;
			string text2 = reader.ReadCountStringInt32();
			if (text2 != "")
			{
				return text2;
			}
		}
		return @string;
	}
}
