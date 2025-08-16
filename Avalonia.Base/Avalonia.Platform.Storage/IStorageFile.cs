using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Metadata;

namespace Avalonia.Platform.Storage;

[NotClientImplementable]
public interface IStorageFile : IStorageItem, IDisposable
{
	Task<Stream> OpenReadAsync();

	Task<Stream> OpenWriteAsync();
}
