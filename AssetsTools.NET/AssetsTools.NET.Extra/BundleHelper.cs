using System.Collections.Generic;
using System.IO;

namespace AssetsTools.NET.Extra;

public static class BundleHelper
{
	public static byte[] LoadAssetDataFromBundle(AssetBundleFile bundle, int index)
	{
		bundle.GetFileRange(index, out var offset, out var length);
		AssetsFileReader dataReader = bundle.DataReader;
		dataReader.Position = offset;
		return dataReader.ReadBytes((int)length);
	}

	public static byte[] LoadAssetDataFromBundle(AssetBundleFile bundle, string name)
	{
		int fileIndex = bundle.GetFileIndex(name);
		if (fileIndex < 0)
		{
			return null;
		}
		return LoadAssetDataFromBundle(bundle, fileIndex);
	}

	public static AssetsFile LoadAssetFromBundle(AssetBundleFile bundle, int index)
	{
		bundle.GetFileRange(index, out var offset, out var length);
		Stream stream = new SegmentStream(bundle.DataReader.BaseStream, offset, length);
		AssetsFileReader reader = new AssetsFileReader(stream);
		AssetsFile assetsFile = new AssetsFile();
		assetsFile.Read(reader);
		return assetsFile;
	}

	public static AssetsFile LoadAssetFromBundle(AssetBundleFile bundle, string name)
	{
		int fileIndex = bundle.GetFileIndex(name);
		if (fileIndex < 0)
		{
			return null;
		}
		return LoadAssetFromBundle(bundle, fileIndex);
	}

	public static List<byte[]> LoadAllAssetsDataFromBundle(AssetBundleFile bundle)
	{
		List<byte[]> list = new List<byte[]>();
		int num = bundle.BlockAndDirInfo.DirectoryInfos.Length;
		for (int i = 0; i < num; i++)
		{
			if (bundle.IsAssetsFile(i))
			{
				list.Add(LoadAssetDataFromBundle(bundle, i));
			}
		}
		return list;
	}

	public static List<AssetsFile> LoadAllAssetsFromBundle(AssetBundleFile bundle)
	{
		List<AssetsFile> list = new List<AssetsFile>();
		int num = bundle.BlockAndDirInfo.DirectoryInfos.Length;
		for (int i = 0; i < num; i++)
		{
			if (bundle.IsAssetsFile(i))
			{
				list.Add(LoadAssetFromBundle(bundle, i));
			}
		}
		return list;
	}

	public static AssetBundleFile UnpackBundle(AssetBundleFile file, bool freeOriginalStream = true)
	{
		MemoryStream memoryStream = new MemoryStream();
		file.Unpack(new AssetsFileWriter(memoryStream));
		memoryStream.Position = 0L;
		AssetBundleFile assetBundleFile = new AssetBundleFile();
		assetBundleFile.Read(new AssetsFileReader(memoryStream));
		if (freeOriginalStream)
		{
			file.Reader.Close();
			file.DataReader.Close();
		}
		return assetBundleFile;
	}

	public static AssetBundleFile UnpackBundleToStream(AssetBundleFile file, Stream stream, bool freeOriginalStream = true)
	{
		file.Unpack(new AssetsFileWriter(stream));
		stream.Position = 0L;
		AssetBundleFile assetBundleFile = new AssetBundleFile();
		assetBundleFile.Read(new AssetsFileReader(stream));
		if (freeOriginalStream)
		{
			file.Reader.Close();
			file.DataReader.Close();
		}
		return assetBundleFile;
	}

	public static AssetBundleDirectoryInfo GetDirInfo(AssetBundleFile bundle, int index)
	{
		AssetBundleDirectoryInfo[] directoryInfos = bundle.BlockAndDirInfo.DirectoryInfos;
		return directoryInfos[index];
	}

	public static AssetBundleDirectoryInfo GetDirInfo(AssetBundleFile bundle, string name)
	{
		AssetBundleDirectoryInfo[] directoryInfos = bundle.BlockAndDirInfo.DirectoryInfos;
		foreach (AssetBundleDirectoryInfo assetBundleDirectoryInfo in directoryInfos)
		{
			if (assetBundleDirectoryInfo.Name == name)
			{
				return assetBundleDirectoryInfo;
			}
		}
		return null;
	}
}
