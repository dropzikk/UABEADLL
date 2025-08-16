using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AssetsTools.NET.Extra;

namespace AssetsTools.NET;

public class AssetsFile
{
	/// <summary>
	/// Assets file header.
	/// </summary>
	public AssetsFileHeader Header { get; set; }

	/// <summary>
	/// Contains metadata about the file (TypeTree, engine version, dependencies, etc.)
	/// </summary>
	public AssetsFileMetadata Metadata { get; set; }

	/// <summary>
	/// The <see cref="T:AssetsTools.NET.AssetsFileReader" /> that reads the file.
	/// </summary>
	public AssetsFileReader Reader { get; set; }

	/// <summary>
	/// A list of all asset infos in this file.
	/// </summary>
	public List<AssetFileInfo> AssetInfos => Metadata.AssetInfos;

	/// <summary>
	/// Closes the reader.
	/// </summary>
	public void Close()
	{
		Reader.Close();
	}

	/// <summary>
	/// Read the <see cref="T:AssetsTools.NET.AssetsFile" /> with the provided reader.
	/// </summary>
	/// <param name="reader">The reader to use.</param>
	public void Read(AssetsFileReader reader)
	{
		Reader = reader;
		Header = new AssetsFileHeader();
		Header.Read(reader);
		Metadata = new AssetsFileMetadata();
		Metadata.Read(reader, Header);
	}

	/// <summary>
	/// Read the <see cref="T:AssetsTools.NET.AssetsFile" /> with the provided stream.
	/// </summary>
	/// <param name="stream">The stream to use.</param>
	public void Read(Stream stream)
	{
		Read(new AssetsFileReader(stream));
	}

	/// <summary>
	/// Write the <see cref="T:AssetsTools.NET.AssetsFile" /> with the provided writer.
	/// </summary>
	/// <param name="writer">The writer to use.</param>
	/// <param name="filePos">Where in the stream to start writing. Use -1 to start writing at the current stream position.</param>
	/// <param name="replacers">The list of asset replacers to use. Do not use null if you want no replacers, instead use an empty list.</param>
	/// <param name="typeMeta">The class database to use if any new asset types are used. Do not rely on this for MonoBehaviours.</param>
	public void Write(AssetsFileWriter writer, long filePos, List<AssetsReplacer> replacers, ClassDatabaseFile typeMeta = null)
	{
		long num = filePos;
		if (filePos == -1)
		{
			num = writer.Position;
		}
		else
		{
			writer.Position = filePos;
		}
		Header.Write(writer);
		List<TypeTreeType> typeTreeTypes = Metadata.TypeTreeTypes;
		foreach (AssetsReplacer item2 in replacers.Where((AssetsReplacer r) => r.GetReplacementType() == AssetsReplacementType.AddOrModify))
		{
			int replacerClassId = item2.GetClassID();
			ushort replacerScriptIndex = item2.GetMonoScriptID();
			if ((Header.Version < 16) ? typeTreeTypes.Any((TypeTreeType t) => t.TypeId == replacerClassId) : typeTreeTypes.Any((TypeTreeType t) => t.TypeId == replacerClassId && t.ScriptTypeIndex == replacerScriptIndex))
			{
				continue;
			}
			TypeTreeType typeTreeType = null;
			if (typeMeta != null)
			{
				ClassDatabaseType classDatabaseType = typeMeta.FindAssetClassByID(replacerClassId);
				if (classDatabaseType != null)
				{
					typeTreeType = ClassDatabaseToTypeTree.Convert(typeMeta, classDatabaseType);
					typeTreeType.ScriptTypeIndex = replacerScriptIndex;
				}
			}
			if (typeTreeType == null)
			{
				TypeTreeType typeTreeType2 = new TypeTreeType();
				typeTreeType2.TypeId = replacerClassId;
				typeTreeType2.IsStrippedType = false;
				typeTreeType2.ScriptTypeIndex = replacerScriptIndex;
				typeTreeType2.ScriptIdHash = Hash128.NewBlankHash();
				typeTreeType2.TypeHash = Hash128.NewBlankHash();
				typeTreeType2.Nodes = new List<TypeTreeNode>();
				typeTreeType2.StringBufferBytes = new byte[0];
				typeTreeType2.TypeDependencies = new int[0];
				typeTreeType = typeTreeType2;
			}
			typeTreeTypes.Add(typeTreeType);
		}
		Dictionary<long, AssetFileInfo> dictionary = new Dictionary<long, AssetFileInfo>();
		Dictionary<long, AssetsReplacer> dictionary2 = replacers.ToDictionary((AssetsReplacer r) => r.GetPathID());
		List<AssetFileInfo> list = new List<AssetFileInfo>();
		foreach (AssetFileInfo assetInfo in Metadata.AssetInfos)
		{
			dictionary.Add(assetInfo.PathId, assetInfo);
			if (!dictionary2.ContainsKey(assetInfo.PathId))
			{
				AssetFileInfo item = new AssetFileInfo
				{
					PathId = assetInfo.PathId,
					TypeIdOrIndex = assetInfo.TypeIdOrIndex,
					ClassId = assetInfo.ClassId,
					ScriptTypeIndex = assetInfo.ScriptTypeIndex,
					Stripped = assetInfo.Stripped
				};
				list.Add(item);
			}
		}
		foreach (AssetsReplacer replacer in replacers.Where((AssetsReplacer r) => r.GetReplacementType() == AssetsReplacementType.AddOrModify))
		{
			AssetFileInfo assetFileInfo = new AssetFileInfo
			{
				PathId = replacer.GetPathID(),
				ClassId = (ushort)replacer.GetClassID(),
				ScriptTypeIndex = replacer.GetMonoScriptID(),
				Stripped = 0
			};
			if (Header.Version < 16)
			{
				if (replacer.GetClassID() < 0)
				{
					assetFileInfo.ClassId = 114;
				}
				assetFileInfo.TypeIdOrIndex = replacer.GetClassID();
			}
			else if (replacer.GetMonoScriptID() == ushort.MaxValue)
			{
				assetFileInfo.TypeIdOrIndex = typeTreeTypes.FindIndex((TypeTreeType t) => t.TypeId == replacer.GetClassID());
			}
			else
			{
				assetFileInfo.TypeIdOrIndex = typeTreeTypes.FindIndex((TypeTreeType t) => t.TypeId == replacer.GetClassID() && t.ScriptTypeIndex == replacer.GetMonoScriptID());
			}
			list.Add(assetFileInfo);
		}
		list.Sort((AssetFileInfo i1, AssetFileInfo i2) => i1.PathId.CompareTo(i2.PathId));
		AssetsFileMetadata assetsFileMetadata = new AssetsFileMetadata
		{
			UnityVersion = Metadata.UnityVersion,
			TargetPlatform = Metadata.TargetPlatform,
			TypeTreeEnabled = Metadata.TypeTreeEnabled,
			TypeTreeTypes = typeTreeTypes,
			AssetInfos = list,
			ScriptTypes = Metadata.ScriptTypes,
			Externals = Metadata.Externals,
			RefTypes = Metadata.RefTypes,
			UserInformation = Metadata.UserInformation
		};
		long position = writer.Position;
		assetsFileMetadata.Write(writer, Header.Version);
		int num2 = (int)(writer.Position - position);
		if (writer.Position < 4096)
		{
			while (writer.Position < 4096)
			{
				writer.Write((byte)0);
			}
		}
		else if (writer.Position % 16 == 0)
		{
			writer.Position += 16L;
		}
		else
		{
			writer.Align16();
		}
		long position2 = writer.Position;
		for (int j = 0; j < list.Count; j++)
		{
			AssetFileInfo assetFileInfo2 = list[j];
			assetFileInfo2.ByteStart = writer.Position - position2;
			if (dictionary2.TryGetValue(assetFileInfo2.PathId, out var value))
			{
				value.Write(writer);
			}
			else
			{
				AssetFileInfo assetFileInfo3 = dictionary[assetFileInfo2.PathId];
				Reader.Position = Header.DataOffset + assetFileInfo3.ByteStart;
				Reader.BaseStream.CopyToCompat(writer.BaseStream, assetFileInfo3.ByteSize);
			}
			assetFileInfo2.ByteSize = (uint)(writer.Position - (position2 + assetFileInfo2.ByteStart));
			if (j != list.Count - 1)
			{
				writer.Align8();
			}
		}
		long num3 = writer.Position - num;
		AssetsFileHeader assetsFileHeader = new AssetsFileHeader
		{
			MetadataSize = num2,
			FileSize = num3,
			Version = Header.Version,
			DataOffset = position2,
			Endianness = Header.Endianness
		};
		writer.Position = num;
		assetsFileHeader.Write(writer);
		writer.Position = position;
		assetsFileMetadata.Write(writer, Header.Version);
		writer.Position = num + num3;
	}

	/// <summary>
	/// Get the script index for an <see cref="T:AssetsTools.NET.AssetFileInfo" />.
	/// Always use this method instead of ScriptTypeIndex, as it handles all versions.
	/// </summary>
	/// <param name="info">The file info to check.</param>
	/// <returns>The script index of the asset.</returns>
	public ushort GetScriptIndex(AssetFileInfo info)
	{
		if (Header.Version < 16)
		{
			return info.ScriptTypeIndex;
		}
		return Metadata.TypeTreeTypes[info.TypeIdOrIndex].ScriptTypeIndex;
	}

	/// <summary>
	/// Check if a file at a path is an <see cref="T:AssetsTools.NET.AssetsFile" /> or not.
	/// </summary>
	/// <param name="filePath">The file path to read from and check.</param>
	/// <returns>True if the file is an assets file, otherwise false.</returns>
	public static bool IsAssetsFile(string filePath)
	{
		using AssetsFileReader assetsFileReader = new AssetsFileReader(filePath);
		return IsAssetsFile(assetsFileReader, 0L, assetsFileReader.BaseStream.Length);
	}

	/// <summary>
	/// Check if a file at a position in a stream is an <see cref="T:AssetsTools.NET.AssetsFile" /> or not.
	/// </summary>
	/// <param name="reader">The reader to use.</param>
	/// <param name="offset">The offset to start at (this value cannot be -1).</param>
	/// <param name="length">The length of the file. You can use <c>reader.BaseStream.Length</c> for this.</param>
	/// <returns></returns>
	public static bool IsAssetsFile(AssetsFileReader reader, long offset, long length)
	{
		reader.BigEndian = true;
		if (length < 48)
		{
			return false;
		}
		reader.Position = offset;
		string text = reader.ReadStringLength(5);
		if (text == "Unity")
		{
			return false;
		}
		reader.Position = offset + 8;
		int num = reader.ReadInt32();
		if (num > 99)
		{
			return false;
		}
		reader.Position = offset + 20;
		if (num >= 22)
		{
			reader.Position += 28L;
		}
		string text2 = "";
		char c;
		while (reader.Position < reader.BaseStream.Length && (c = (char)reader.ReadByte()) != 0)
		{
			text2 += c;
			if (text2.Length > 255)
			{
				return false;
			}
		}
		string text3 = Regex.Replace(text2, "[a-zA-Z0-9\\.\\n]", "");
		string text4 = Regex.Replace(text2, "[^a-zA-Z0-9\\.\\n]", "");
		return text3 == "" && text4.Length > 0;
	}

	/// <summary>
	/// Get an <see cref="T:AssetsTools.NET.AssetFileInfo" /> from a path ID.
	/// </summary>
	/// <param name="pathId">The path ID to search for.</param>
	/// <returns>An info for that path ID.</returns>
	public AssetFileInfo GetAssetInfo(long pathId)
	{
		return Metadata.GetAssetInfo(pathId);
	}

	/// <summary>
	/// Generate a dictionary lookup for assets instead of a brute force search.
	/// Takes a little bit more memory but results in quicker lookups.
	/// </summary>
	public void GenerateQuickLookup()
	{
		Metadata.GenerateQuickLookup();
	}

	/// <summary>
	/// Get all assets of a specific type ID.
	/// </summary>
	/// <param name="typeId">The type ID to search for.</param>
	/// <returns>A list of infos for that type ID.</returns>
	public List<AssetFileInfo> GetAssetsOfType(int typeId)
	{
		return Metadata.GetAssetsOfType(typeId);
	}

	/// <summary>
	/// Get all assets of a specific type ID.
	/// </summary>
	/// <param name="typeId">The type ID to search for.</param>
	/// <returns>A list of infos for that type ID.</returns>
	public List<AssetFileInfo> GetAssetsOfType(AssetClassID typeId)
	{
		return Metadata.GetAssetsOfType(typeId);
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
		return Metadata.GetAssetsOfType(typeId, scriptIndex);
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
		return Metadata.GetAssetsOfType(typeId, scriptIndex);
	}
}
