using System.Collections.Generic;
using AssetsTools.NET.Extra;

namespace AssetsTools.NET;

public class AssetsFileMetadata
{
	private Dictionary<long, int> _quickLookup = null;

	/// <summary>
	/// Engine version this file uses.
	/// </summary>
	public string UnityVersion { get; set; }

	/// <summary>
	/// Target platform this file uses.
	/// </summary>
	public uint TargetPlatform { get; set; }

	/// <summary>
	/// Marks whether the type info contains type tree data.
	/// </summary>
	public bool TypeTreeEnabled { get; set; }

	/// <summary>
	/// List of type tree types.
	/// </summary>
	public List<TypeTreeType> TypeTreeTypes { get; set; }

	/// <summary>
	/// List of asset infos. Do not modify this directly. Instead, use
	/// <see cref="M:AssetsTools.NET.AssetsFile.Write(AssetsTools.NET.AssetsFileWriter,System.Int64,System.Collections.Generic.List{AssetsTools.NET.AssetsReplacer},AssetsTools.NET.ClassDatabaseFile)" />.
	/// </summary>
	public List<AssetFileInfo> AssetInfos { get; set; }

	/// <summary>
	/// List of script type pointers. This list should match up with ScriptTypeIndex in the type
	/// tree types list.
	/// </summary>
	public List<AssetPPtr> ScriptTypes { get; set; }

	/// <summary>
	/// List of externals (references to other files).
	/// </summary>
	public List<AssetsFileExternal> Externals { get; set; }

	/// <summary>
	/// List of reference types.
	/// </summary>
	public List<TypeTreeType> RefTypes { get; set; }

	/// <summary>
	/// Unknown.
	/// </summary>
	public string UserInformation { get; set; }

	/// <summary>
	/// Read the <see cref="T:AssetsTools.NET.AssetsFileMetadata" /> with the provided reader and file header.
	/// </summary>
	/// <param name="reader">The reader to use.</param>
	/// <param name="header">The header to use.</param>
	public void Read(AssetsFileReader reader, AssetsFileHeader header)
	{
		Read(reader, header.Version, header.DataOffset);
	}

	/// <summary>
	/// Read the <see cref="T:AssetsTools.NET.AssetsFileMetadata" /> with the provided reader and format version.
	/// This version is not recommended since no data offset is provided, so
	/// <see cref="P:AssetsTools.NET.AssetFileInfo.AbsoluteByteStart" /> is not set.
	/// </summary>
	/// <param name="reader">The reader to use.</param>
	/// <param name="version">The version of the file.</param>
	public void Read(AssetsFileReader reader, uint version)
	{
		Read(reader, version, -1L);
	}

	/// <summary>
	/// Read the <see cref="T:AssetsTools.NET.AssetsFileMetadata" /> with the provided reader and format version.
	/// </summary>
	/// <param name="reader">The reader to use.</param>
	/// <param name="version">The version of the file.</param>
	/// <param name="dataOffset">The version of the file.</param>
	public void Read(AssetsFileReader reader, uint version, long dataOffset)
	{
		_quickLookup = null;
		UnityVersion = reader.ReadNullTerminated();
		TargetPlatform = reader.ReadUInt32();
		if (version >= 13)
		{
			TypeTreeEnabled = reader.ReadBoolean();
		}
		int num = reader.ReadInt32();
		TypeTreeTypes = new List<TypeTreeType>(num);
		for (int i = 0; i < num; i++)
		{
			TypeTreeType typeTreeType = new TypeTreeType();
			typeTreeType.Read(reader, version, TypeTreeEnabled, isRefType: false);
			TypeTreeTypes.Add(typeTreeType);
		}
		int num2 = reader.ReadInt32();
		reader.Align();
		AssetInfos = new List<AssetFileInfo>(num2);
		for (int j = 0; j < num2; j++)
		{
			AssetFileInfo assetFileInfo = new AssetFileInfo();
			assetFileInfo.Read(reader, version);
			assetFileInfo.TypeId = assetFileInfo.GetTypeId(this, version);
			if (dataOffset != -1)
			{
				assetFileInfo.AbsoluteByteStart = assetFileInfo.GetAbsoluteByteStart(dataOffset);
			}
			AssetInfos.Add(assetFileInfo);
		}
		int num3 = reader.ReadInt32();
		ScriptTypes = new List<AssetPPtr>(num3);
		for (int k = 0; k < num3; k++)
		{
			int fileId = reader.ReadInt32();
			reader.Align();
			long pathId = reader.ReadInt64();
			AssetPPtr item = new AssetPPtr(fileId, pathId);
			ScriptTypes.Add(item);
		}
		int num4 = reader.ReadInt32();
		Externals = new List<AssetsFileExternal>(num4);
		for (int l = 0; l < num4; l++)
		{
			AssetsFileExternal assetsFileExternal = new AssetsFileExternal();
			assetsFileExternal.Read(reader);
			Externals.Add(assetsFileExternal);
		}
		if (version >= 20)
		{
			int num5 = reader.ReadInt32();
			RefTypes = new List<TypeTreeType>(num5);
			for (int m = 0; m < num5; m++)
			{
				TypeTreeType typeTreeType2 = new TypeTreeType();
				typeTreeType2.Read(reader, version, TypeTreeEnabled, isRefType: true);
				RefTypes.Add(typeTreeType2);
			}
		}
		if (version >= 5)
		{
			UserInformation = reader.ReadNullTerminated();
		}
	}

	/// <summary>
	/// Write the <see cref="T:AssetsTools.NET.AssetsFileMetadata" /> with the provided reader and format version.
	/// </summary>
	/// <param name="writer">The writer to use.</param>
	/// <param name="version">The version of the file.</param>
	public void Write(AssetsFileWriter writer, uint version)
	{
		writer.WriteNullTerminated(UnityVersion);
		writer.Write(TargetPlatform);
		if (version >= 13)
		{
			writer.Write(TypeTreeEnabled);
		}
		writer.Write(TypeTreeTypes.Count);
		for (int i = 0; i < TypeTreeTypes.Count; i++)
		{
			TypeTreeTypes[i].Write(writer, version, TypeTreeEnabled);
		}
		writer.Write(AssetInfos.Count);
		writer.Align();
		for (int j = 0; j < AssetInfos.Count; j++)
		{
			AssetInfos[j].Write(writer, version);
		}
		writer.Write(ScriptTypes.Count);
		for (int k = 0; k < ScriptTypes.Count; k++)
		{
			writer.Write(ScriptTypes[k].FileId);
			writer.Align();
			writer.Write(ScriptTypes[k].PathId);
		}
		writer.Write(Externals.Count);
		for (int l = 0; l < Externals.Count; l++)
		{
			Externals[l].Write(writer);
		}
		if (version >= 20)
		{
			writer.Write(RefTypes.Count);
			for (int m = 0; m < RefTypes.Count; m++)
			{
				RefTypes[m].Write(writer, version, TypeTreeEnabled);
			}
		}
		if (version >= 5)
		{
			writer.WriteNullTerminated(UserInformation);
		}
	}

	/// <summary>
	/// Get an <see cref="T:AssetsTools.NET.AssetFileInfo" /> from a path ID.
	/// </summary>
	/// <param name="pathId">The path ID to search for.</param>
	/// <returns>An info for that path ID.</returns>
	public AssetFileInfo GetAssetInfo(long pathId)
	{
		if (_quickLookup != null)
		{
			if (_quickLookup.ContainsKey(pathId))
			{
				return AssetInfos[_quickLookup[pathId]];
			}
		}
		else
		{
			for (int i = 0; i < AssetInfos.Count; i++)
			{
				AssetFileInfo assetFileInfo = AssetInfos[i];
				if (assetFileInfo.PathId == pathId)
				{
					return assetFileInfo;
				}
			}
		}
		return null;
	}

	/// <summary>
	/// Generate a dictionary lookup for assets instead of a brute force search.
	/// Takes a little bit more memory but results in quicker lookups.
	/// </summary>
	public void GenerateQuickLookup()
	{
		_quickLookup = new Dictionary<long, int>();
		for (int i = 0; i < AssetInfos.Count; i++)
		{
			AssetFileInfo assetFileInfo = AssetInfos[i];
			_quickLookup[assetFileInfo.PathId] = i;
		}
	}

	/// <summary>
	/// Get all assets of a specific type ID.
	/// </summary>
	/// <param name="typeId">The type ID to search for.</param>
	/// <returns>A list of infos for that type ID.</returns>
	public List<AssetFileInfo> GetAssetsOfType(int typeId)
	{
		List<AssetFileInfo> list = new List<AssetFileInfo>();
		foreach (AssetFileInfo assetInfo in AssetInfos)
		{
			if (assetInfo.TypeId == typeId)
			{
				list.Add(assetInfo);
			}
		}
		return list;
	}

	/// <summary>
	/// Get all assets of a specific type ID and script index. The script index of an asset can be
	/// found from <see cref="M:AssetsTools.NET.AssetsFile.GetScriptIndex(AssetsTools.NET.AssetFileInfo)" /> or <see cref="P:AssetsTools.NET.AssetsFileMetadata.ScriptTypes" />.
	/// </summary>
	/// <param name="typeId">The type ID to search for.</param>
	/// <param name="scriptIndex">The script index to search for.</param>
	/// <returns>A list of infos for that type ID and script index.</returns>
	public List<AssetFileInfo> GetAssetsOfType(int typeId, ushort scriptIndex)
	{
		List<AssetFileInfo> list = new List<AssetFileInfo>();
		foreach (AssetFileInfo assetInfo in AssetInfos)
		{
			if (scriptIndex != ushort.MaxValue)
			{
				if (assetInfo.TypeId < 0)
				{
					if (assetInfo.ScriptTypeIndex != scriptIndex || (typeId != 114 && (typeId >= 0 || assetInfo.TypeId != typeId)))
					{
						continue;
					}
				}
				else if (assetInfo.TypeId != 114 || FindTypeTreeTypeByID(assetInfo.TypeId, assetInfo.ScriptTypeIndex).ScriptTypeIndex != scriptIndex || typeId != 114)
				{
					continue;
				}
			}
			else if (assetInfo.TypeId != typeId)
			{
				continue;
			}
			list.Add(assetInfo);
		}
		return list;
	}

	/// <summary>
	/// Get all assets of a specific type ID.
	/// </summary>
	/// <param name="typeId">The type ID to search for.</param>
	/// <returns>A list of infos for that type ID.</returns>
	public List<AssetFileInfo> GetAssetsOfType(AssetClassID typeId)
	{
		return GetAssetsOfType((int)typeId);
	}

	/// <summary>
	/// Get all assets of a specific type ID and script index. The script index of an asset can be
	/// found from <see cref="M:AssetsTools.NET.AssetsFile.GetScriptIndex(AssetsTools.NET.AssetFileInfo)" /> or <see cref="P:AssetsTools.NET.AssetsFileMetadata.ScriptTypes" />.
	/// </summary>
	/// <param name="typeId">The type ID to search for.</param>
	/// <param name="scriptIndex">The script index to search for.</param>
	/// <returns>A list of infos for that type ID and script index.</returns>
	public List<AssetFileInfo> GetAssetsOfType(AssetClassID typeId, ushort scriptIndex)
	{
		return GetAssetsOfType((int)typeId, scriptIndex);
	}

	/// <summary>
	/// Get the type tree type by type ID.
	/// </summary>
	/// <param name="id">The type ID to search for.</param>
	/// <returns>The type tree type with this ID.</returns>
	public TypeTreeType FindTypeTreeTypeByID(int id)
	{
		foreach (TypeTreeType typeTreeType in TypeTreeTypes)
		{
			if (typeTreeType.TypeId == id)
			{
				return typeTreeType;
			}
		}
		return null;
	}

	/// <summary>
	/// Get the type tree type by type ID and script index. The script index of an asset can be
	/// found from <see cref="M:AssetsTools.NET.AssetsFile.GetScriptIndex(AssetsTools.NET.AssetFileInfo)" /> or <see cref="P:AssetsTools.NET.AssetsFileMetadata.ScriptTypes" />.
	/// For games before 5.5, <paramref name="scriptIndex" /> is ignored since this data is read
	/// from the negative value of <paramref name="id" />. In 5.5 and later, MonoBehaviours are always
	/// 0x72, so <paramref name="scriptIndex" /> is used instead.
	/// </summary>
	/// <param name="id">The type ID to search for.</param>
	/// <param name="scriptIndex">The script index to search for.</param>
	/// <returns>The type tree type with this ID and script index.</returns>
	public TypeTreeType FindTypeTreeTypeByID(int id, ushort scriptIndex)
	{
		foreach (TypeTreeType typeTreeType in TypeTreeTypes)
		{
			if (typeTreeType.TypeId == id)
			{
				if (typeTreeType.ScriptTypeIndex == scriptIndex)
				{
					return typeTreeType;
				}
				if (id < 0 && typeTreeType.ScriptTypeIndex == ushort.MaxValue)
				{
					return typeTreeType;
				}
			}
		}
		return null;
	}

	/// <summary>
	/// Get the type tree type by script index. The script index of an asset can be
	/// found from <see cref="M:AssetsTools.NET.AssetsFile.GetScriptIndex(AssetsTools.NET.AssetFileInfo)" /> or <see cref="P:AssetsTools.NET.AssetsFileMetadata.ScriptTypes" />.
	/// </summary>
	/// <param name="scriptIndex">The script index to search for.</param>
	/// <returns>The type tree type with this script index.</returns>
	public TypeTreeType FindTypeTreeTypeByScriptIndex(ushort scriptIndex)
	{
		foreach (TypeTreeType typeTreeType in TypeTreeTypes)
		{
			if (typeTreeType.ScriptTypeIndex == scriptIndex)
			{
				return typeTreeType;
			}
		}
		return null;
	}

	/// <summary>
	/// Get the type tree type by name.
	/// </summary>
	/// <param name="name">The type name to search for.</param>
	/// <returns>The type tree type with this name.</returns>
	public TypeTreeType FindTypeTreeTypeByName(string name)
	{
		foreach (TypeTreeType typeTreeType in TypeTreeTypes)
		{
			if (typeTreeType.Nodes[0].GetTypeString(typeTreeType.StringBuffer) == name)
			{
				return typeTreeType;
			}
		}
		return null;
	}

	/// <summary>
	/// Get the type tree ref type by script index.
	/// </summary>
	/// <param name="scriptIndex">The script index to search for.</param>
	/// <returns>The type tree ref type with this script index.</returns>
	public TypeTreeType FindRefTypeByIndex(ushort scriptIndex)
	{
		foreach (TypeTreeType refType in RefTypes)
		{
			if (refType.ScriptTypeIndex == scriptIndex)
			{
				return refType;
			}
		}
		return null;
	}
}
