using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;

namespace Avalonia.X11.NativeDialogs;

internal class CompositeStorageProvider : IStorageProvider
{
	private readonly IEnumerable<Func<Task<IStorageProvider?>>> _factories;

	public bool CanOpen => true;

	public bool CanSave => true;

	public bool CanPickFolder => true;

	public CompositeStorageProvider(IEnumerable<Func<Task<IStorageProvider?>>> factories)
	{
		_factories = factories;
	}

	private async Task<IStorageProvider> EnsureStorageProvider()
	{
		foreach (Func<Task<IStorageProvider>> factory in _factories)
		{
			IStorageProvider storageProvider = await factory();
			if (storageProvider != null)
			{
				return storageProvider;
			}
		}
		throw new InvalidOperationException("Neither DBus nor GTK are available on the system");
	}

	public async Task<IReadOnlyList<IStorageFile>> OpenFilePickerAsync(FilePickerOpenOptions options)
	{
		return await (await EnsureStorageProvider().ConfigureAwait(continueOnCapturedContext: false)).OpenFilePickerAsync(options).ConfigureAwait(continueOnCapturedContext: false);
	}

	public async Task<IStorageFile?> SaveFilePickerAsync(FilePickerSaveOptions options)
	{
		return await (await EnsureStorageProvider().ConfigureAwait(continueOnCapturedContext: false)).SaveFilePickerAsync(options).ConfigureAwait(continueOnCapturedContext: false);
	}

	public async Task<IReadOnlyList<IStorageFolder>> OpenFolderPickerAsync(FolderPickerOpenOptions options)
	{
		return await (await EnsureStorageProvider().ConfigureAwait(continueOnCapturedContext: false)).OpenFolderPickerAsync(options).ConfigureAwait(continueOnCapturedContext: false);
	}

	public async Task<IStorageBookmarkFile?> OpenFileBookmarkAsync(string bookmark)
	{
		return await (await EnsureStorageProvider().ConfigureAwait(continueOnCapturedContext: false)).OpenFileBookmarkAsync(bookmark).ConfigureAwait(continueOnCapturedContext: false);
	}

	public async Task<IStorageBookmarkFolder?> OpenFolderBookmarkAsync(string bookmark)
	{
		return await (await EnsureStorageProvider().ConfigureAwait(continueOnCapturedContext: false)).OpenFolderBookmarkAsync(bookmark).ConfigureAwait(continueOnCapturedContext: false);
	}

	public async Task<IStorageFile?> TryGetFileFromPathAsync(Uri filePath)
	{
		return await (await EnsureStorageProvider().ConfigureAwait(continueOnCapturedContext: false)).TryGetFileFromPathAsync(filePath).ConfigureAwait(continueOnCapturedContext: false);
	}

	public async Task<IStorageFolder?> TryGetFolderFromPathAsync(Uri folderPath)
	{
		return await (await EnsureStorageProvider().ConfigureAwait(continueOnCapturedContext: false)).TryGetFolderFromPathAsync(folderPath).ConfigureAwait(continueOnCapturedContext: false);
	}

	public async Task<IStorageFolder?> TryGetWellKnownFolderAsync(WellKnownFolder wellKnownFolder)
	{
		return await (await EnsureStorageProvider().ConfigureAwait(continueOnCapturedContext: false)).TryGetWellKnownFolderAsync(wellKnownFolder).ConfigureAwait(continueOnCapturedContext: false);
	}
}
