using Avalonia.Platform.Storage;

namespace Avalonia.Controls.Platform;

public interface IStorageProviderFactory
{
	IStorageProvider CreateProvider(TopLevel topLevel);
}
