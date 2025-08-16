using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AssetsTools.NET.Extra;

/// <summary>
/// A wrapper around an <see cref="T:AssetsTools.NET.AssetsFile" /> with information such as the path to the file
/// (used for handling dependencies) and the bundle it belongs to.
/// </summary>
public class AssetsFileInstance
{
	/// <summary>
	/// The full path to the file. This path can be fake it is not from disk.
	/// </summary>
	public string path;

	/// <summary>
	/// The name of the file. This is the file name part of the path.
	/// </summary>
	public string name;

	/// <summary>
	/// The base <see cref="T:AssetsTools.NET.AssetsFile" />.
	/// </summary>
	public AssetsFile file;

	/// <summary>
	/// The bundle this <see cref="T:AssetsTools.NET.AssetsFile" /> is a part of, if there is one.
	/// </summary>
	public BundleFileInstance parentBundle = null;

	internal Dictionary<int, AssetsFileInstance> dependencyCache;

	/// <summary>
	/// The stream the assets file uses.
	/// </summary>
	public Stream AssetsStream => file.Reader.BaseStream;

	public AssetsFileInstance(Stream stream, string filePath)
	{
		path = Path.GetFullPath(filePath);
		name = Path.GetFileName(path);
		file = new AssetsFile();
		file.Read(new AssetsFileReader(stream));
		dependencyCache = new Dictionary<int, AssetsFileInstance>();
	}

	public AssetsFileInstance(FileStream stream)
	{
		path = stream.Name;
		name = Path.GetFileName(path);
		file = new AssetsFile();
		file.Read(new AssetsFileReader(stream));
		dependencyCache = new Dictionary<int, AssetsFileInstance>();
	}

	public AssetsFileInstance GetDependency(AssetsManager am, int depIdx)
	{
		if (!dependencyCache.ContainsKey(depIdx) || dependencyCache[depIdx] == null)
		{
			string depPath = file.Metadata.Externals[depIdx].PathName;
			if (depPath == string.Empty)
			{
				return null;
			}
			if (!am.FileLookup.TryGetValue(am.GetFileLookupKey(depPath), out var value))
			{
				string directoryName = Path.GetDirectoryName(path);
				string text = Path.Combine(directoryName, depPath);
				string text2 = Path.Combine(directoryName, Path.GetFileName(depPath));
				if (File.Exists(text))
				{
					dependencyCache[depIdx] = am.LoadAssetsFile(text, loadDeps: true);
				}
				else if (File.Exists(text2))
				{
					dependencyCache[depIdx] = am.LoadAssetsFile(text2, loadDeps: true);
				}
				else
				{
					if (parentBundle == null)
					{
						return null;
					}
					AssetBundleFile assetBundleFile = parentBundle.file;
					if (assetBundleFile.BlockAndDirInfo.DirectoryInfos.Any((AssetBundleDirectoryInfo di) => di.Name == depPath))
					{
						dependencyCache[depIdx] = am.LoadAssetsFileFromBundle(parentBundle, depPath, loadDeps: true);
					}
					else
					{
						string directoryName2 = Path.GetDirectoryName(directoryName);
						string text3 = Path.Combine(directoryName2, Path.GetFileName(depPath));
						if (!File.Exists(text3))
						{
							return null;
						}
						dependencyCache[depIdx] = am.LoadAssetsFile(text3, loadDeps: true);
					}
				}
			}
			else
			{
				dependencyCache[depIdx] = value;
			}
		}
		return dependencyCache[depIdx];
	}
}
