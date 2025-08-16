using System;
using System.Threading.Tasks;
using Avalonia.Metadata;

namespace Avalonia.Platform.Storage;

[NotClientImplementable]
public interface IStorageItem : IDisposable
{
	string Name { get; }

	Uri Path { get; }

	bool CanBookmark { get; }

	Task<StorageItemProperties> GetBasicPropertiesAsync();

	Task<string?> SaveBookmarkAsync();

	Task<IStorageFolder?> GetParentAsync();

	Task DeleteAsync();

	Task<IStorageItem?> MoveAsync(IStorageFolder destination);
}
