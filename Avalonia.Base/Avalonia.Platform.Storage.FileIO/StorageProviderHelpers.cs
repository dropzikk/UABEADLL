using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;

namespace Avalonia.Platform.Storage.FileIO;

internal static class StorageProviderHelpers
{
	public static IStorageItem? TryCreateBclStorageItem(string path)
	{
		if (!string.IsNullOrWhiteSpace(path))
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(path);
			if (directoryInfo.Exists)
			{
				return new BclStorageFolder(directoryInfo);
			}
			FileInfo fileInfo = new FileInfo(path);
			if (fileInfo.Exists)
			{
				return new BclStorageFile(fileInfo);
			}
		}
		return null;
	}

	public static Uri FilePathToUri(string path)
	{
		string path2 = new StringBuilder(path).Replace("%", $"%{37:X2}").Replace("[", $"%{91:X2}").Replace("]", $"%{93:X2}")
			.ToString();
		return new UriBuilder("file", string.Empty)
		{
			Path = path2
		}.Uri;
	}

	public static bool TryFilePathToUri(string path, [NotNullWhen(true)] out Uri? uri)
	{
		try
		{
			uri = FilePathToUri(path);
			return true;
		}
		catch
		{
			uri = null;
			return false;
		}
	}

	[return: NotNullIfNotNull("path")]
	public static string? NameWithExtension(string? path, string? defaultExtension, FilePickerFileType? filter)
	{
		string fileName = Path.GetFileName(path);
		if (fileName != null && !Path.HasExtension(fileName))
		{
			if (filter != null && filter.Patterns?.Count > 0)
			{
				if (defaultExtension != null && filter.Patterns.Contains<string>(defaultExtension))
				{
					return Path.ChangeExtension(path, defaultExtension.TrimStart('.'));
				}
				string text = filter.Patterns.FirstOrDefault((string x) => x != "*.*")?.Split(new string[1] { "*." }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
				if (text != null)
				{
					return Path.ChangeExtension(path, text);
				}
			}
			if (defaultExtension != null)
			{
				return Path.ChangeExtension(path, defaultExtension);
			}
		}
		return path;
	}
}
