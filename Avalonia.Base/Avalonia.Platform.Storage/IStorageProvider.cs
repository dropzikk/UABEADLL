using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Metadata;

namespace Avalonia.Platform.Storage;

[NotClientImplementable]
public interface IStorageProvider
{
	bool CanOpen { get; }

	bool CanSave { get; }

	bool CanPickFolder { get; }

	Task<IReadOnlyList<IStorageFile>> OpenFilePickerAsync(FilePickerOpenOptions options);

	Task<IStorageFile?> SaveFilePickerAsync(FilePickerSaveOptions options);

	Task<IReadOnlyList<IStorageFolder>> OpenFolderPickerAsync(FolderPickerOpenOptions options);

	Task<IStorageBookmarkFile?> OpenFileBookmarkAsync(string bookmark);

	Task<IStorageBookmarkFolder?> OpenFolderBookmarkAsync(string bookmark);

	Task<IStorageFile?> TryGetFileFromPathAsync(Uri filePath);

	Task<IStorageFolder?> TryGetFolderFromPathAsync(Uri folderPath);

	Task<IStorageFolder?> TryGetWellKnownFolderAsync(WellKnownFolder wellKnownFolder);
}
