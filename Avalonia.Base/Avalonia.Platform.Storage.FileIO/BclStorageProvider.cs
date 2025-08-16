using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia.Compatibility;

namespace Avalonia.Platform.Storage.FileIO;

internal abstract class BclStorageProvider : IStorageProvider
{
	private static readonly Guid s_folderDownloads = new Guid("374DE290-123F-4565-9164-39C4925E467B");

	public abstract bool CanOpen { get; }

	public abstract bool CanSave { get; }

	public abstract bool CanPickFolder { get; }

	public abstract Task<IReadOnlyList<IStorageFile>> OpenFilePickerAsync(FilePickerOpenOptions options);

	public abstract Task<IStorageFile?> SaveFilePickerAsync(FilePickerSaveOptions options);

	public abstract Task<IReadOnlyList<IStorageFolder>> OpenFolderPickerAsync(FolderPickerOpenOptions options);

	public virtual Task<IStorageBookmarkFile?> OpenFileBookmarkAsync(string bookmark)
	{
		FileInfo fileInfo = new FileInfo(bookmark);
		if (!fileInfo.Exists)
		{
			return Task.FromResult<IStorageBookmarkFile>(null);
		}
		return Task.FromResult((IStorageBookmarkFile)new BclStorageFile(fileInfo));
	}

	public virtual Task<IStorageBookmarkFolder?> OpenFolderBookmarkAsync(string bookmark)
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(bookmark);
		if (!directoryInfo.Exists)
		{
			return Task.FromResult<IStorageBookmarkFolder>(null);
		}
		return Task.FromResult((IStorageBookmarkFolder)new BclStorageFolder(directoryInfo));
	}

	public virtual Task<IStorageFile?> TryGetFileFromPathAsync(Uri filePath)
	{
		if (filePath.IsAbsoluteUri)
		{
			FileInfo fileInfo = new FileInfo(filePath.LocalPath);
			if (fileInfo.Exists)
			{
				return Task.FromResult((IStorageFile)new BclStorageFile(fileInfo));
			}
		}
		return Task.FromResult<IStorageFile>(null);
	}

	public virtual Task<IStorageFolder?> TryGetFolderFromPathAsync(Uri folderPath)
	{
		if (folderPath.IsAbsoluteUri)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(folderPath.LocalPath);
			if (directoryInfo.Exists)
			{
				return Task.FromResult((IStorageFolder)new BclStorageFolder(directoryInfo));
			}
		}
		return Task.FromResult<IStorageFolder>(null);
	}

	public virtual Task<IStorageFolder?> TryGetWellKnownFolderAsync(WellKnownFolder wellKnownFolder)
	{
		string text = wellKnownFolder switch
		{
			WellKnownFolder.Desktop => GetFromSpecialFolder(Environment.SpecialFolder.Desktop), 
			WellKnownFolder.Documents => GetFromSpecialFolder(Environment.SpecialFolder.Personal), 
			WellKnownFolder.Downloads => GetDownloadsWellKnownFolder(), 
			WellKnownFolder.Music => GetFromSpecialFolder(Environment.SpecialFolder.MyMusic), 
			WellKnownFolder.Pictures => GetFromSpecialFolder(Environment.SpecialFolder.MyPictures), 
			WellKnownFolder.Videos => GetFromSpecialFolder(Environment.SpecialFolder.MyVideos), 
			_ => throw new ArgumentOutOfRangeException("wellKnownFolder", wellKnownFolder, null), 
		};
		if (text == null)
		{
			return Task.FromResult<IStorageFolder>(null);
		}
		DirectoryInfo directoryInfo = new DirectoryInfo(text);
		if (!directoryInfo.Exists)
		{
			return Task.FromResult<IStorageFolder>(null);
		}
		return Task.FromResult((IStorageFolder)new BclStorageFolder(directoryInfo));
		static string GetFromSpecialFolder(Environment.SpecialFolder folder)
		{
			return Environment.GetFolderPath(folder, Environment.SpecialFolderOption.Create);
		}
	}

	protected static string? GetDownloadsWellKnownFolder()
	{
		if (OperatingSystemEx.IsWindows())
		{
			if (Environment.OSVersion.Version.Major >= 6)
			{
				return SHGetKnownFolderPath(s_folderDownloads, 0, IntPtr.Zero);
			}
			return null;
		}
		if (OperatingSystemEx.IsLinux())
		{
			string environmentVariable = Environment.GetEnvironmentVariable("XDG_DOWNLOAD_DIR");
			if (environmentVariable != null && Directory.Exists(environmentVariable))
			{
				return environmentVariable;
			}
		}
		if (OperatingSystemEx.IsLinux() || OperatingSystemEx.IsMacOS())
		{
			return "~/Downloads";
		}
		return null;
	}

	[DllImport("shell32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = false)]
	private static extern string SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)] Guid id, int flags, IntPtr token);
}
