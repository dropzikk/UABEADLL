using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace Avalonia.Platform.Storage.FileIO;

internal class BclStorageFolder : IStorageBookmarkFolder, IStorageFolder, IStorageItem, IDisposable, IStorageBookmarkItem
{
	public string Name => DirectoryInfo.Name;

	public DirectoryInfo DirectoryInfo { get; }

	public bool CanBookmark => true;

	public Uri Path
	{
		get
		{
			try
			{
				return StorageProviderHelpers.FilePathToUri(DirectoryInfo.FullName);
			}
			catch (SecurityException)
			{
				return new Uri(DirectoryInfo.Name, UriKind.Relative);
			}
		}
	}

	public BclStorageFolder(DirectoryInfo directoryInfo)
	{
		DirectoryInfo = directoryInfo ?? throw new ArgumentNullException("directoryInfo");
		if (!DirectoryInfo.Exists)
		{
			throw new ArgumentException("Directory must exist", "directoryInfo");
		}
	}

	public Task<StorageItemProperties> GetBasicPropertiesAsync()
	{
		return Task.FromResult(new StorageItemProperties(null, DirectoryInfo.CreationTimeUtc, DirectoryInfo.LastAccessTimeUtc));
	}

	public Task<IStorageFolder?> GetParentAsync()
	{
		DirectoryInfo parent = DirectoryInfo.Parent;
		if (parent != null)
		{
			return Task.FromResult((IStorageFolder)new BclStorageFolder(parent));
		}
		return Task.FromResult<IStorageFolder>(null);
	}

	public async IAsyncEnumerable<IStorageItem> GetItemsAsync()
	{
		IEnumerable<IStorageItem> enumerable = DirectoryInfo.EnumerateDirectories().Select((Func<DirectoryInfo, IStorageItem>)((DirectoryInfo d) => new BclStorageFolder(d))).Concat(from f in DirectoryInfo.EnumerateFiles()
			select new BclStorageFile(f));
		foreach (IStorageItem item in enumerable)
		{
			yield return item;
		}
	}

	public virtual Task<string?> SaveBookmarkAsync()
	{
		return Task.FromResult(DirectoryInfo.FullName);
	}

	public Task ReleaseBookmarkAsync()
	{
		return Task.CompletedTask;
	}

	protected virtual void Dispose(bool disposing)
	{
	}

	~BclStorageFolder()
	{
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	public async Task DeleteAsync()
	{
		DirectoryInfo.Delete(recursive: true);
	}

	public async Task<IStorageItem?> MoveAsync(IStorageFolder destination)
	{
		if (destination is BclStorageFolder bclStorageFolder)
		{
			string text = System.IO.Path.Combine(bclStorageFolder.DirectoryInfo.FullName, DirectoryInfo.Name);
			DirectoryInfo.MoveTo(text);
			return new BclStorageFolder(new DirectoryInfo(text));
		}
		return null;
	}

	public async Task<IStorageFile?> CreateFileAsync(string name)
	{
		FileInfo fileInfo = new FileInfo(System.IO.Path.Combine(DirectoryInfo.FullName, name));
		using (fileInfo.Create())
		{
			return new BclStorageFile(fileInfo);
		}
	}

	public async Task<IStorageFolder?> CreateFolderAsync(string name)
	{
		return new BclStorageFolder(DirectoryInfo.CreateSubdirectory(name));
	}
}
