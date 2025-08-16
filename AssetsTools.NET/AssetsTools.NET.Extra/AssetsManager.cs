using System;
using System.Collections.Generic;
using System.IO;

namespace AssetsTools.NET.Extra;

public class AssetsManager
{
	private readonly Dictionary<int, AssetTypeTemplateField> templateFieldCache = new Dictionary<int, AssetTypeTemplateField>();

	private readonly Dictionary<AssetTypeReference, AssetTypeTemplateField> monoTemplateFieldCache = new Dictionary<AssetTypeReference, AssetTypeTemplateField>();

	private readonly Dictionary<AssetsFileInstance, Dictionary<ushort, AssetTypeTemplateField>> monoTypeTreeTemplateFieldCache = new Dictionary<AssetsFileInstance, Dictionary<ushort, AssetTypeTemplateField>>();

	private readonly Dictionary<AssetsFileInstance, Dictionary<long, AssetTypeTemplateField>> monoCldbTemplateFieldCache = new Dictionary<AssetsFileInstance, Dictionary<long, AssetTypeTemplateField>>();

	private readonly Dictionary<AssetsFileInstance, RefTypeManager> refTypeManagerCache = new Dictionary<AssetsFileInstance, RefTypeManager>();

	public bool UseTemplateFieldCache { get; set; } = false;

	public bool UseMonoTemplateFieldCache { get; set; } = false;

	public bool UseRefTypeManagerCache { get; set; } = false;

	public bool UseQuickLookup { get; set; } = false;

	public ClassDatabaseFile ClassDatabase { get; private set; }

	public ClassPackageFile ClassPackage { get; private set; }

	public List<AssetsFileInstance> Files { get; private set; } = new List<AssetsFileInstance>();

	public Dictionary<string, AssetsFileInstance> FileLookup { get; private set; } = new Dictionary<string, AssetsFileInstance>();

	public List<BundleFileInstance> Bundles { get; private set; } = new List<BundleFileInstance>();

	public Dictionary<string, BundleFileInstance> BundleLookup { get; private set; } = new Dictionary<string, BundleFileInstance>();

	public IMonoBehaviourTemplateGenerator MonoTempGenerator { get; set; } = null;

	internal string GetFileLookupKey(string path)
	{
		return Path.GetFullPath(path).ToLower();
	}

	private void LoadAssetsFileDependencies(AssetsFileInstance fileInst, string path, BundleFileInstance bunInst)
	{
		if (bunInst == null)
		{
			LoadDependencies(fileInst);
		}
		else
		{
			LoadBundleDependencies(fileInst, bunInst, Path.GetDirectoryName(path));
		}
	}

	private AssetsFileInstance LoadAssetsFileCacheless(Stream stream, string path, bool loadDeps, BundleFileInstance bunInst = null)
	{
		AssetsFileInstance assetsFileInstance = new AssetsFileInstance(stream, path);
		assetsFileInstance.parentBundle = bunInst;
		string fileLookupKey = GetFileLookupKey(path);
		FileLookup[fileLookupKey] = assetsFileInstance;
		Files.Add(assetsFileInstance);
		if (loadDeps)
		{
			LoadAssetsFileDependencies(assetsFileInstance, path, bunInst);
		}
		if (UseQuickLookup)
		{
			assetsFileInstance.file.GenerateQuickLookup();
		}
		return assetsFileInstance;
	}

	/// <summary>
	/// Load an <see cref="T:AssetsTools.NET.Extra.AssetsFileInstance" /> from a stream with a path.
	/// Use the <see cref="T:System.IO.FileStream" /> version of this method to skip the path argument.
	/// If a file with that name is already loaded, it will be returned instead.
	/// </summary>
	/// <param name="stream">The stream to read from.</param>
	/// <param name="path">The path to set on the <see cref="T:AssetsTools.NET.Extra.AssetsFileInstance" />.</param>
	/// <param name="loadDeps">Load all dependencies immediately?</param>
	/// <param name="bunInst">The parent bundle, if one exists.</param>
	/// <returns>The loaded <see cref="T:AssetsTools.NET.Extra.AssetsFileInstance" />.</returns>
	public AssetsFileInstance LoadAssetsFile(Stream stream, string path, bool loadDeps, BundleFileInstance bunInst = null)
	{
		string fileLookupKey = GetFileLookupKey(path);
		if (FileLookup.TryGetValue(fileLookupKey, out var value))
		{
			if (loadDeps)
			{
				LoadAssetsFileDependencies(value, path, bunInst);
			}
			return value;
		}
		return LoadAssetsFileCacheless(stream, path, loadDeps, bunInst);
	}

	/// <summary>
	/// Load an <see cref="T:AssetsTools.NET.Extra.AssetsFileInstance" /> from a stream.
	/// Assigns the <see cref="T:AssetsTools.NET.Extra.AssetsFileInstance" />'s path from the stream's file path.
	/// </summary>
	/// <param name="stream">The stream to read from.</param>
	/// <param name="loadDeps">Load all dependencies immediately?</param>
	/// <returns>The loaded <see cref="T:AssetsTools.NET.Extra.AssetsFileInstance" />.</returns>
	public AssetsFileInstance LoadAssetsFile(FileStream stream, bool loadDeps = false)
	{
		return LoadAssetsFileCacheless(stream, stream.Name, loadDeps);
	}

	/// <summary>
	/// Load an <see cref="T:AssetsTools.NET.Extra.AssetsFileInstance" /> from a path.
	/// If a file with that name is already loaded, it will be returned instead.
	/// </summary>
	/// <param name="path">The path of the file to read from.</param>
	/// <param name="loadDeps">Load all dependencies immediately?</param>
	/// <returns>The loaded <see cref="T:AssetsTools.NET.Extra.AssetsFileInstance" />.</returns>
	public AssetsFileInstance LoadAssetsFile(string path, bool loadDeps = false)
	{
		string fileLookupKey = GetFileLookupKey(path);
		if (FileLookup.TryGetValue(fileLookupKey, out var value))
		{
			return value;
		}
		return LoadAssetsFile(File.OpenRead(path), loadDeps);
	}

	/// <summary>
	/// Unload an <see cref="T:AssetsTools.NET.Extra.AssetsFileInstance" /> by path.
	/// </summary>
	/// <param name="path">The path of the <see cref="T:AssetsTools.NET.Extra.AssetsFileInstance" /> to unload.</param>
	/// <returns>True if the file was found and closed, and false if it wasn't found.</returns>
	public bool UnloadAssetsFile(string path)
	{
		string fileLookupKey = GetFileLookupKey(path);
		if (FileLookup.TryGetValue(fileLookupKey, out var value))
		{
			monoTypeTreeTemplateFieldCache.Remove(value);
			monoCldbTemplateFieldCache.Remove(value);
			refTypeManagerCache.Remove(value);
			Files.Remove(value);
			FileLookup.Remove(fileLookupKey);
			value.file.Close();
			return true;
		}
		return false;
	}

	/// <summary>
	/// Unload an <see cref="T:AssetsTools.NET.Extra.AssetsFileInstance" />.
	/// </summary>
	/// <param name="fileInst">The <see cref="T:AssetsTools.NET.Extra.AssetsFileInstance" /> to unload.</param>
	/// <returns>True if the file was found and closed, and false if it wasn't found.</returns>
	public bool UnloadAssetsFile(AssetsFileInstance fileInst)
	{
		fileInst.file.Close();
		if (Files.Contains(fileInst))
		{
			monoTypeTreeTemplateFieldCache.Remove(fileInst);
			monoCldbTemplateFieldCache.Remove(fileInst);
			refTypeManagerCache.Remove(fileInst);
			string fileLookupKey = GetFileLookupKey(fileInst.path);
			FileLookup.Remove(fileLookupKey);
			Files.Remove(fileInst);
			return true;
		}
		return false;
	}

	/// <summary>
	/// Unload all <see cref="T:AssetsTools.NET.Extra.AssetsFileInstance" />s.
	/// </summary>
	/// <param name="clearCache">Clear the cache? Recommended if you plan on reopening files later.</param>
	/// <returns>True if there are files that can be cleared, and false if no files are loaded.</returns>
	public bool UnloadAllAssetsFiles(bool clearCache = false)
	{
		if (clearCache)
		{
			templateFieldCache.Clear();
			monoTemplateFieldCache.Clear();
		}
		monoTypeTreeTemplateFieldCache.Clear();
		monoCldbTemplateFieldCache.Clear();
		refTypeManagerCache.Clear();
		if (Files.Count != 0)
		{
			foreach (AssetsFileInstance file in Files)
			{
				file.file.Close();
			}
			Files.Clear();
			FileLookup.Clear();
			return true;
		}
		return false;
	}

	/// <summary>
	/// Load a <see cref="T:AssetsTools.NET.Extra.BundleFileInstance" /> from a stream with a path.
	/// Use the <see cref="T:System.IO.FileStream" /> version of this method to skip the path argument.
	/// If the bundle is large, you may want to set <paramref name="unpackIfPacked" /> to false
	/// so you can manually decompress to file.
	/// </summary>
	/// <param name="stream">The stream to read from.</param>
	/// <param name="path">The path to set on the <see cref="T:AssetsTools.NET.Extra.AssetsFileInstance" />.</param>
	/// <param name="unpackIfPacked">Unpack the bundle if it's compressed?</param>
	/// <returns>The loaded <see cref="T:AssetsTools.NET.Extra.BundleFileInstance" />.</returns>
	public BundleFileInstance LoadBundleFile(Stream stream, string path, bool unpackIfPacked = true)
	{
		string fileLookupKey = GetFileLookupKey(path);
		if (BundleLookup.TryGetValue(fileLookupKey, out var value))
		{
			return value;
		}
		value = new BundleFileInstance(stream, path, unpackIfPacked);
		Bundles.Add(value);
		BundleLookup[fileLookupKey] = value;
		return value;
	}

	/// <summary>
	/// Load a <see cref="T:AssetsTools.NET.Extra.BundleFileInstance" /> from a stream.
	/// Assigns the <see cref="T:AssetsTools.NET.Extra.BundleFileInstance" />'s path from the stream's file path.
	/// If the bundle is large, you may want to set <paramref name="unpackIfPacked" /> to false
	/// so you can manually decompress to file.
	/// </summary>
	/// <param name="stream">The stream to read from.</param>
	/// <param name="unpackIfPacked">Unpack the bundle if it's compressed?</param>
	/// <returns>The loaded <see cref="T:AssetsTools.NET.Extra.BundleFileInstance" />.</returns>
	public BundleFileInstance LoadBundleFile(FileStream stream, bool unpackIfPacked = true)
	{
		return LoadBundleFile(stream, Path.GetFullPath(stream.Name), unpackIfPacked);
	}

	/// <summary>
	/// Load a <see cref="T:AssetsTools.NET.Extra.BundleFileInstance" /> from a path.
	/// If the bundle is large, you may want to set <paramref name="unpackIfPacked" /> to false
	/// so you can manually decompress to file.
	/// </summary>
	/// <param name="path">The path of the file to read from.</param>
	/// <param name="unpackIfPacked">Unpack the bundle if it's compressed?</param>
	/// <returns>The loaded <see cref="T:AssetsTools.NET.Extra.BundleFileInstance" />.</returns>
	public BundleFileInstance LoadBundleFile(string path, bool unpackIfPacked = true)
	{
		return LoadBundleFile(File.OpenRead(path), unpackIfPacked);
	}

	/// <summary>
	/// Unload an <see cref="T:AssetsTools.NET.Extra.BundleFileInstance" /> by path.
	/// </summary>
	/// <param name="path">The path of the <see cref="T:AssetsTools.NET.Extra.BundleFileInstance" /> to unload.</param>
	/// <returns>True if the file was found and closed, and false if it wasn't found.</returns>
	public bool UnloadBundleFile(string path)
	{
		string fileLookupKey = GetFileLookupKey(path);
		if (BundleLookup.TryGetValue(fileLookupKey, out var value))
		{
			value.file.Close();
			foreach (AssetsFileInstance loadedAssetsFile in value.loadedAssetsFiles)
			{
				loadedAssetsFile.file.Close();
			}
			Bundles.Remove(value);
			BundleLookup.Remove(fileLookupKey);
			return true;
		}
		return false;
	}

	/// <summary>
	/// Unload an <see cref="T:AssetsTools.NET.Extra.BundleFileInstance" />.
	/// </summary>
	/// <param name="bunInst">The <see cref="T:AssetsTools.NET.Extra.BundleFileInstance" /> to unload.</param>
	/// <returns>True if the file was found and closed, and false if it wasn't found.</returns>
	public bool UnloadBundleFile(BundleFileInstance bunInst)
	{
		bunInst.file.Close();
		foreach (AssetsFileInstance loadedAssetsFile in bunInst.loadedAssetsFiles)
		{
			UnloadAssetsFile(loadedAssetsFile);
		}
		bunInst.loadedAssetsFiles.Clear();
		if (Bundles.Contains(bunInst))
		{
			string fileLookupKey = GetFileLookupKey(bunInst.path);
			BundleLookup.Remove(fileLookupKey);
			Bundles.Remove(bunInst);
			return true;
		}
		return false;
	}

	/// <summary>
	/// Unload all <see cref="T:AssetsTools.NET.Extra.AssetsFileInstance" />s.
	/// </summary>
	/// <returns>True if there are files that can be cleared, and false if no files are loaded.</returns>
	public bool UnloadAllBundleFiles()
	{
		if (Bundles.Count != 0)
		{
			foreach (BundleFileInstance bundle in Bundles)
			{
				bundle.file.Close();
				foreach (AssetsFileInstance loadedAssetsFile in bundle.loadedAssetsFiles)
				{
					UnloadAssetsFile(loadedAssetsFile);
				}
				bundle.loadedAssetsFiles.Clear();
			}
			Bundles.Clear();
			BundleLookup.Clear();
			return true;
		}
		return false;
	}

	/// <summary>
	/// Load an <see cref="T:AssetsTools.NET.Extra.AssetsFileInstance" /> from a <see cref="T:AssetsTools.NET.Extra.BundleFileInstance" /> by index.
	/// </summary>
	/// <param name="bunInst">The bundle to load from.</param>
	/// <param name="index">The index of the file in the bundle to load from.</param>
	/// <param name="loadDeps">Load all dependencies immediately?</param>
	/// <returns>The loaded <see cref="T:AssetsTools.NET.Extra.AssetsFileInstance" />.</returns>
	public AssetsFileInstance LoadAssetsFileFromBundle(BundleFileInstance bunInst, int index, bool loadDeps = false)
	{
		string path = Path.Combine(bunInst.path, bunInst.file.GetFileName(index));
		string fileLookupKey = GetFileLookupKey(path);
		if (!FileLookup.TryGetValue(fileLookupKey, out var value))
		{
			if (bunInst.file.IsAssetsFile(index))
			{
				bunInst.file.GetFileRange(index, out var offset, out var length);
				SegmentStream stream = new SegmentStream(bunInst.DataStream, offset, length);
				AssetsFileInstance assetsFileInstance = LoadAssetsFile(stream, path, loadDeps, bunInst);
				bunInst.loadedAssetsFiles.Add(assetsFileInstance);
				return assetsFileInstance;
			}
			return null;
		}
		return value;
	}

	/// <summary>
	/// Load an <see cref="T:AssetsTools.NET.Extra.AssetsFileInstance" /> from a <see cref="T:AssetsTools.NET.Extra.BundleFileInstance" /> by name.
	/// </summary>
	/// <param name="bunInst">The bundle to load from.</param>
	/// <param name="name">The name of the file in the bundle to load from.</param>
	/// <param name="loadDeps">Load all dependencies immediately?</param>
	/// <returns>The loaded <see cref="T:AssetsTools.NET.Extra.AssetsFileInstance" />.</returns>
	public AssetsFileInstance LoadAssetsFileFromBundle(BundleFileInstance bunInst, string name, bool loadDeps = false)
	{
		int fileIndex = bunInst.file.GetFileIndex(name);
		if (fileIndex < 0)
		{
			return null;
		}
		return LoadAssetsFileFromBundle(bunInst, fileIndex, loadDeps);
	}

	public ClassDatabaseFile LoadClassDatabase(Stream stream)
	{
		ClassDatabase = new ClassDatabaseFile();
		ClassDatabase.Read(new AssetsFileReader(stream));
		return ClassDatabase;
	}

	public ClassDatabaseFile LoadClassDatabase(string path)
	{
		return LoadClassDatabase(File.OpenRead(path));
	}

	public ClassDatabaseFile LoadClassDatabaseFromPackage(UnityVersion version)
	{
		return ClassDatabase = ClassPackage.GetClassDatabase(version);
	}

	public ClassDatabaseFile LoadClassDatabaseFromPackage(string version)
	{
		return ClassDatabase = ClassPackage.GetClassDatabase(version);
	}

	public ClassPackageFile LoadClassPackage(Stream stream)
	{
		ClassPackage = new ClassPackageFile();
		ClassPackage.Read(new AssetsFileReader(stream));
		return ClassPackage;
	}

	public ClassPackageFile LoadClassPackage(string path)
	{
		return LoadClassPackage(File.OpenRead(path));
	}

	public void UnloadAll(bool unloadClassData = false)
	{
		UnloadAllAssetsFiles(clearCache: true);
		UnloadAllBundleFiles();
		MonoTempGenerator?.Dispose();
		if (unloadClassData)
		{
			ClassPackage = null;
			ClassDatabase = null;
		}
	}

	public void LoadDependencies(AssetsFileInstance ofFile)
	{
		string directoryName = Path.GetDirectoryName(ofFile.path);
		for (int i = 0; i < ofFile.file.Metadata.Externals.Count; i++)
		{
			string depPath = ofFile.file.Metadata.Externals[i].PathName;
			if (!(depPath == string.Empty) && Files.FindIndex((AssetsFileInstance f) => Path.GetFileName(f.path).ToLower() == Path.GetFileName(depPath).ToLower()) == -1)
			{
				string path = Path.Combine(directoryName, depPath);
				string path2 = Path.Combine(directoryName, Path.GetFileName(depPath));
				if (File.Exists(path))
				{
					LoadAssetsFile(path, loadDeps: true);
				}
				else if (File.Exists(path2))
				{
					LoadAssetsFile(path2, loadDeps: true);
				}
			}
		}
	}

	public void LoadBundleDependencies(AssetsFileInstance ofFile, BundleFileInstance ofBundle, string path)
	{
		for (int i = 0; i < ofFile.file.Metadata.Externals.Count; i++)
		{
			string depPath = ofFile.file.Metadata.Externals[i].PathName;
			if (Files.FindIndex((AssetsFileInstance f) => Path.GetFileName(f.path).ToLower() == Path.GetFileName(depPath).ToLower()) == -1)
			{
				string bunPath = Path.GetFileName(depPath);
				int num = Array.FindIndex(ofBundle.file.BlockAndDirInfo.DirectoryInfos, (AssetBundleDirectoryInfo d) => Path.GetFileName(d.Name) == bunPath);
				string path2 = Path.Combine(path, "..");
				string path3 = Path.Combine(path2, depPath);
				string path4 = Path.Combine(path2, Path.GetFileName(depPath));
				string path5 = Path.Combine(path, depPath);
				string path6 = Path.Combine(path, Path.GetFileName(depPath));
				if (num != -1)
				{
					LoadAssetsFileFromBundle(ofBundle, num, loadDeps: true);
				}
				else if (File.Exists(path5))
				{
					LoadAssetsFile(path5, loadDeps: true);
				}
				else if (File.Exists(path6))
				{
					LoadAssetsFile(path6, loadDeps: true);
				}
				else if (File.Exists(path3))
				{
					LoadAssetsFile(path3, loadDeps: true);
				}
				else if (File.Exists(path4))
				{
					LoadAssetsFile(path4, loadDeps: true);
				}
			}
		}
	}

	public RefTypeManager GetRefTypeManager(AssetsFileInstance inst)
	{
		if (UseRefTypeManagerCache && refTypeManagerCache.TryGetValue(inst, out var value))
		{
			return value;
		}
		value = new RefTypeManager();
		value.FromTypeTree(inst.file.Metadata);
		if (MonoTempGenerator != null)
		{
			value.WithMonoTemplateGenerator(inst.file.Metadata, MonoTempGenerator, UseMonoTemplateFieldCache ? monoTemplateFieldCache : null);
		}
		if (UseRefTypeManagerCache)
		{
			refTypeManagerCache[inst] = value;
		}
		return value;
	}

	public AssetTypeTemplateField GetTemplateBaseField(AssetsFileInstance inst, AssetFileInfo info, AssetReadFlags readFlags = AssetReadFlags.None)
	{
		long absoluteByteStart = info.AbsoluteByteStart;
		ushort scriptIndex = inst.file.GetScriptIndex(info);
		return GetTemplateBaseField(inst, inst.file.Reader, absoluteByteStart, info.TypeId, scriptIndex, readFlags);
	}

	public AssetTypeTemplateField GetTemplateBaseField(AssetsFileInstance inst, AssetsFileReader reader, long absByteStart, int typeId, ushort scriptIndex, AssetReadFlags readFlags)
	{
		AssetsFile file = inst.file;
		AssetTypeTemplateField value = null;
		bool typeTreeEnabled = inst.file.Metadata.TypeTreeEnabled;
		bool preferEditor = Net35Polyfill.HasFlag(readFlags, AssetReadFlags.PreferEditor);
		bool flag = Net35Polyfill.HasFlag(readFlags, AssetReadFlags.ForceFromCldb);
		bool flag2 = Net35Polyfill.HasFlag(readFlags, AssetReadFlags.SkipMonoBehaviourFields);
		if (UseTemplateFieldCache && typeId != 114 && templateFieldCache.TryGetValue(typeId, out value))
		{
			return value;
		}
		if (typeTreeEnabled && !flag)
		{
			if (UseMonoTemplateFieldCache && typeId == 114 && monoTypeTreeTemplateFieldCache.TryGetValue(inst, out var value2) && value2.TryGetValue(scriptIndex, out var value3))
			{
				return value3;
			}
			TypeTreeType typeTreeType = file.Metadata.FindTypeTreeTypeByID(typeId, scriptIndex);
			if (typeTreeType != null && typeTreeType.Nodes.Count > 0)
			{
				value = new AssetTypeTemplateField();
				value.FromTypeTree(typeTreeType);
				if (UseTemplateFieldCache && typeId != 114)
				{
					templateFieldCache[typeId] = value;
				}
				else if (UseMonoTemplateFieldCache && (long)typeId == 114)
				{
					if (!monoTypeTreeTemplateFieldCache.TryGetValue(inst, out var value4))
					{
						value4 = (monoTypeTreeTemplateFieldCache[inst] = new Dictionary<ushort, AssetTypeTemplateField>());
					}
					value4[scriptIndex] = value;
				}
				return value;
			}
		}
		if (UseTemplateFieldCache && UseMonoTemplateFieldCache && typeId == 114 && templateFieldCache.TryGetValue(typeId, out value))
		{
			value = value.Clone();
		}
		if (value == null)
		{
			ClassDatabaseType classDatabaseType = ClassDatabase.FindAssetClassByID(typeId);
			if (classDatabaseType == null)
			{
				return null;
			}
			value = new AssetTypeTemplateField();
			value.FromClassDatabase(ClassDatabase, classDatabaseType, preferEditor);
			if (UseTemplateFieldCache)
			{
				if (typeId == 114)
				{
					templateFieldCache[typeId] = value.Clone();
				}
				else
				{
					templateFieldCache[typeId] = value;
				}
			}
		}
		if (typeId == 114 && MonoTempGenerator != null && !flag2)
		{
			AssetTypeValueField assetTypeValueField = value.MakeValue(reader, absByteStart);
			AssetPPtr assetPPtr = AssetPPtr.FromField(assetTypeValueField["m_Script"]);
			if (!assetPPtr.IsNull())
			{
				AssetsFileInstance assetsFileInstance = ((assetPPtr.FileId != 0) ? inst.GetDependency(this, assetPPtr.FileId - 1) : inst);
				if (assetsFileInstance == null)
				{
					return value;
				}
				Dictionary<long, AssetTypeTemplateField> value5 = null;
				if (UseMonoTemplateFieldCache)
				{
					AssetTypeTemplateField value6;
					if (!monoCldbTemplateFieldCache.TryGetValue(assetsFileInstance, out value5))
					{
						value5 = (monoCldbTemplateFieldCache[assetsFileInstance] = new Dictionary<long, AssetTypeTemplateField>());
					}
					else if (value5.TryGetValue(assetPPtr.PathId, out value6))
					{
						return value6;
					}
				}
				AssetFileInfo assetInfo = assetsFileInstance.file.GetAssetInfo(assetPPtr.PathId);
				long absoluteByteStart = assetInfo.AbsoluteByteStart;
				int typeId2 = assetInfo.TypeId;
				ushort scriptIndex2 = assetsFileInstance.file.GetScriptIndex(assetInfo);
				string assemblyName;
				string nameSpace;
				string className;
				bool monoScriptInfo = GetMonoScriptInfo(assetsFileInstance, absoluteByteStart, typeId2, scriptIndex2, out assemblyName, out nameSpace, out className, readFlags);
				if (assemblyName.EndsWith(".dll"))
				{
					assemblyName = assemblyName.Substring(0, assemblyName.Length - 4);
				}
				if (monoScriptInfo)
				{
					AssetTypeReference key = new AssetTypeReference(className, nameSpace, assemblyName);
					if (UseMonoTemplateFieldCache && monoTemplateFieldCache.TryGetValue(key, out var value7))
					{
						value5[assetPPtr.PathId] = value7;
						return value7;
					}
					AssetTypeTemplateField templateField = MonoTempGenerator.GetTemplateField(value, assemblyName, nameSpace, className, new UnityVersion(file.Metadata.UnityVersion));
					if (templateField != null)
					{
						value = templateField;
						if (UseMonoTemplateFieldCache)
						{
							Dictionary<long, AssetTypeTemplateField> dictionary3 = value5;
							long pathId = assetPPtr.PathId;
							AssetTypeTemplateField value8 = (monoTemplateFieldCache[key] = value);
							dictionary3[pathId] = value8;
							return value;
						}
					}
				}
			}
		}
		return value;
	}

	private bool GetMonoScriptInfo(AssetsFileInstance inst, long absFilePos, int typeId, ushort scriptIndex, out string assemblyName, out string nameSpace, out string className, AssetReadFlags readFlags)
	{
		assemblyName = null;
		nameSpace = null;
		className = null;
		AssetTypeTemplateField templateBaseField = GetTemplateBaseField(inst, null, absFilePos, typeId, scriptIndex, readFlags);
		if (templateBaseField == null)
		{
			return false;
		}
		inst.file.Reader.Position = absFilePos;
		AssetTypeValueField assetTypeValueField = templateBaseField.MakeValue(inst.file.Reader);
		assemblyName = assetTypeValueField["m_AssemblyName"].AsString;
		nameSpace = assetTypeValueField["m_Namespace"].AsString;
		className = assetTypeValueField["m_ClassName"].AsString;
		return true;
	}

	public AssetTypeTemplateField CreateTemplateBaseField(AssetsFileInstance inst, int id, ushort scriptIndex = ushort.MaxValue)
	{
		AssetsFile file = inst.file;
		AssetTypeTemplateField assetTypeTemplateField = new AssetTypeTemplateField();
		if (file.Metadata.TypeTreeEnabled)
		{
			TypeTreeType typeTreeType = file.Metadata.FindTypeTreeTypeByID(id, scriptIndex);
			assetTypeTemplateField.FromTypeTree(typeTreeType);
		}
		else if (id != 114 || scriptIndex == ushort.MaxValue)
		{
			ClassDatabaseType cldbType = ClassDatabase.FindAssetClassByID(id);
			assetTypeTemplateField.FromClassDatabase(ClassDatabase, cldbType);
		}
		else
		{
			if (MonoTempGenerator == null)
			{
				throw new Exception("MonoTempGenerator must be non-null to create a MonoBehaviour!");
			}
			AssetTypeReference assetsFileScriptInfo = AssetHelper.GetAssetsFileScriptInfo(this, inst, scriptIndex);
			AssetTypeTemplateField templateBaseField = GetTemplateBaseField(inst, file.Reader, -1L, 114, scriptIndex, AssetReadFlags.SkipMonoBehaviourFields);
			UnityVersion unityVersion = new UnityVersion(file.Metadata.UnityVersion);
			assetTypeTemplateField = MonoTempGenerator.GetTemplateField(templateBaseField, assetsFileScriptInfo.AsmName, assetsFileScriptInfo.Namespace, assetsFileScriptInfo.ClassName, unityVersion);
		}
		return assetTypeTemplateField;
	}

	public AssetTypeValueField CreateValueBaseField(AssetsFileInstance inst, int id, ushort scriptIndex = ushort.MaxValue)
	{
		AssetTypeTemplateField templateField = CreateTemplateBaseField(inst, id, scriptIndex);
		return ValueBuilder.DefaultValueFieldFromTemplate(templateField);
	}

	public AssetTypeValueField GetBaseField(AssetsFileInstance inst, AssetFileInfo info, AssetReadFlags readFlags = AssetReadFlags.None)
	{
		AssetTypeTemplateField templateBaseField = GetTemplateBaseField(inst, info, readFlags);
		RefTypeManager refTypeManager = GetRefTypeManager(inst);
		return templateBaseField.MakeValue(inst.file.Reader, info.AbsoluteByteStart, refTypeManager);
	}

	public AssetTypeValueField GetBaseField(AssetsFileInstance inst, long pathId, AssetReadFlags readFlags = AssetReadFlags.None)
	{
		AssetFileInfo assetInfo = inst.file.GetAssetInfo(pathId);
		return GetBaseField(inst, assetInfo, readFlags);
	}

	public AssetExternal GetExtAsset(AssetsFileInstance relativeTo, int fileId, long pathId, bool onlyGetInfo = false, AssetReadFlags readFlags = AssetReadFlags.None)
	{
		AssetExternal assetExternal = default(AssetExternal);
		assetExternal.info = null;
		assetExternal.baseField = null;
		assetExternal.file = null;
		AssetExternal result = assetExternal;
		if (fileId == 0 && pathId == 0)
		{
			return result;
		}
		if (fileId != 0)
		{
			AssetsFileInstance dependency = relativeTo.GetDependency(this, fileId - 1);
			if (dependency == null)
			{
				return result;
			}
			result.file = dependency;
			result.info = dependency.file.GetAssetInfo(pathId);
			if (result.info == null)
			{
				return result;
			}
			if (!onlyGetInfo)
			{
				result.baseField = GetBaseField(dependency, result.info, readFlags);
			}
			else
			{
				result.baseField = null;
			}
			return result;
		}
		result.file = relativeTo;
		result.info = relativeTo.file.GetAssetInfo(pathId);
		if (result.info == null)
		{
			return result;
		}
		if (!onlyGetInfo)
		{
			result.baseField = GetBaseField(relativeTo, result.info, readFlags);
		}
		else
		{
			result.baseField = null;
		}
		return result;
	}

	public AssetExternal GetExtAsset(AssetsFileInstance relativeTo, AssetTypeValueField pptrField, bool onlyGetInfo = false, AssetReadFlags readFlags = AssetReadFlags.None)
	{
		int asInt = pptrField["m_FileID"].AsInt;
		long asLong = pptrField["m_PathID"].AsLong;
		return GetExtAsset(relativeTo, asInt, asLong, onlyGetInfo, readFlags);
	}
}
