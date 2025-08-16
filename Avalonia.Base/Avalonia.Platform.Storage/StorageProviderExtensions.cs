using System;
using System.Threading.Tasks;
using Avalonia.Platform.Storage.FileIO;

namespace Avalonia.Platform.Storage;

public static class StorageProviderExtensions
{
	public static Task<IStorageFile?> TryGetFileFromPathAsync(this IStorageProvider provider, string filePath)
	{
		if (provider is BclStorageProvider)
		{
			return Task.FromResult(StorageProviderHelpers.TryCreateBclStorageItem(filePath) as IStorageFile);
		}
		if (StorageProviderHelpers.TryFilePathToUri(filePath, out Uri uri))
		{
			return provider.TryGetFileFromPathAsync(uri);
		}
		return Task.FromResult<IStorageFile>(null);
	}

	public static Task<IStorageFolder?> TryGetFolderFromPathAsync(this IStorageProvider provider, string folderPath)
	{
		if (provider is BclStorageProvider)
		{
			return Task.FromResult(StorageProviderHelpers.TryCreateBclStorageItem(folderPath) as IStorageFolder);
		}
		if (StorageProviderHelpers.TryFilePathToUri(folderPath, out Uri uri))
		{
			return provider.TryGetFolderFromPathAsync(uri);
		}
		return Task.FromResult<IStorageFolder>(null);
	}

	public static string? TryGetLocalPath(this IStorageItem item)
	{
		if (item is BclStorageFolder bclStorageFolder)
		{
			return bclStorageFolder.DirectoryInfo.FullName;
		}
		if (item is BclStorageFile bclStorageFile)
		{
			return bclStorageFile.FileInfo.FullName;
		}
		Uri path = item.Path;
		if ((object)path != null && path.IsAbsoluteUri && path.Scheme == "file")
		{
			return path.LocalPath;
		}
		return null;
	}
}
