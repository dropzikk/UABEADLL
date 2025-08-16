using System.Collections.Generic;
using System.IO;

namespace AssetsTools.NET.Extra;

public class BundleCreator
{
	public static void CreateBlankAssets(MemoryStream ms, string engineVersion, uint formatVersion, uint typeTreeVersion, bool hasTypeTree = false)
	{
		AssetsFileWriter assetsFileWriter = new AssetsFileWriter(ms);
		AssetsFileHeader assetsFileHeader = new AssetsFileHeader
		{
			MetadataSize = 0L,
			FileSize = -1L,
			Version = formatVersion,
			DataOffset = -1L,
			Endianness = false
		};
		AssetsFileMetadata assetsFileMetadata = new AssetsFileMetadata
		{
			UnityVersion = engineVersion,
			TargetPlatform = typeTreeVersion,
			TypeTreeEnabled = hasTypeTree,
			TypeTreeTypes = new List<TypeTreeType>(),
			AssetInfos = new List<AssetFileInfo>(),
			ScriptTypes = new List<AssetPPtr>(),
			Externals = new List<AssetsFileExternal>(),
			RefTypes = new List<TypeTreeType>()
		};
		assetsFileHeader.Write(assetsFileWriter);
		assetsFileMetadata.Write(assetsFileWriter, formatVersion);
		assetsFileWriter.Write(0u);
		assetsFileWriter.Align();
		assetsFileWriter.Write(0u);
		assetsFileWriter.Write(0u);
		if (assetsFileHeader.Version >= 20)
		{
			assetsFileWriter.Write(0);
		}
		uint num = (uint)(assetsFileWriter.Position - 19);
		if (assetsFileHeader.Version >= 22)
		{
			num -= 28;
		}
		if (assetsFileWriter.Position < 4096)
		{
			while (assetsFileWriter.Position < 4096)
			{
				assetsFileWriter.Write((byte)0);
			}
		}
		else if (assetsFileWriter.Position % 16 == 0)
		{
			assetsFileWriter.Position += 16L;
		}
		else
		{
			assetsFileWriter.Align16();
		}
		long position2 = (assetsFileHeader.DataOffset = (assetsFileHeader.FileSize = assetsFileWriter.Position));
		assetsFileHeader.MetadataSize = num;
		assetsFileWriter.Position = 0L;
		assetsFileHeader.Write(assetsFileWriter);
		assetsFileWriter.Position = position2;
	}
}
