using System;
using System.IO;
using System.Security;
using System.Threading.Tasks;

namespace Avalonia.Platform.Storage.FileIO;

internal class BclStorageFile : IStorageBookmarkFile, IStorageFile, IStorageItem, IDisposable, IStorageBookmarkItem
{
	public FileInfo FileInfo { get; }

	public string Name => FileInfo.Name;

	public virtual bool CanBookmark => true;

	public Uri Path
	{
		get
		{
			try
			{
				if (FileInfo.Directory != null)
				{
					return StorageProviderHelpers.FilePathToUri(FileInfo.FullName);
				}
			}
			catch (SecurityException)
			{
			}
			return new Uri(FileInfo.Name, UriKind.Relative);
		}
	}

	public BclStorageFile(FileInfo fileInfo)
	{
		FileInfo = fileInfo ?? throw new ArgumentNullException("fileInfo");
	}

	public Task<StorageItemProperties> GetBasicPropertiesAsync()
	{
		if (FileInfo.Exists)
		{
			return Task.FromResult(new StorageItemProperties((ulong)FileInfo.Length, FileInfo.CreationTimeUtc, FileInfo.LastAccessTimeUtc));
		}
		return Task.FromResult(new StorageItemProperties(null, null, null));
	}

	public Task<IStorageFolder?> GetParentAsync()
	{
		DirectoryInfo directory = FileInfo.Directory;
		if (directory != null)
		{
			return Task.FromResult((IStorageFolder)new BclStorageFolder(directory));
		}
		return Task.FromResult<IStorageFolder>(null);
	}

	public Task<Stream> OpenReadAsync()
	{
		return Task.FromResult((Stream)FileInfo.OpenRead());
	}

	public Task<Stream> OpenWriteAsync()
	{
		return Task.FromResult((Stream)new FileStream(FileInfo.FullName, FileMode.Create, FileAccess.Write, FileShare.Write));
	}

	public virtual Task<string?> SaveBookmarkAsync()
	{
		return Task.FromResult(FileInfo.FullName);
	}

	public Task ReleaseBookmarkAsync()
	{
		return Task.CompletedTask;
	}

	protected virtual void Dispose(bool disposing)
	{
	}

	~BclStorageFile()
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
		FileInfo.Delete();
	}

	public async Task<IStorageItem?> MoveAsync(IStorageFolder destination)
	{
		if (destination is BclStorageFolder bclStorageFolder)
		{
			string text = System.IO.Path.Combine(bclStorageFolder.DirectoryInfo.FullName, FileInfo.Name);
			FileInfo.MoveTo(text);
			return new BclStorageFile(new FileInfo(text));
		}
		return null;
	}
}
