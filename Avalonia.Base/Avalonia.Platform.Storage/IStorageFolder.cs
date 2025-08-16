using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Metadata;

namespace Avalonia.Platform.Storage;

[NotClientImplementable]
public interface IStorageFolder : IStorageItem, IDisposable
{
	IAsyncEnumerable<IStorageItem> GetItemsAsync();

	Task<IStorageFile?> CreateFileAsync(string name);

	Task<IStorageFolder?> CreateFolderAsync(string name);
}
