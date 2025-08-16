using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Native.Interop;
using Avalonia.Platform.Storage;
using Avalonia.Platform.Storage.FileIO;

namespace Avalonia.Native;

internal class SystemDialogs : BclStorageProvider
{
	private readonly WindowBaseImpl _window;

	private readonly IAvnSystemDialogs _native;

	public override bool CanOpen => true;

	public override bool CanSave => true;

	public override bool CanPickFolder => true;

	public SystemDialogs(WindowBaseImpl window, IAvnSystemDialogs native)
	{
		_window = window;
		_native = native;
	}

	public override async Task<IReadOnlyList<IStorageFile>> OpenFilePickerAsync(FilePickerOpenOptions options)
	{
		using SystemDialogEvents events = new SystemDialogEvents();
		string initialDirectory = options.SuggestedStartLocation?.TryGetLocalPath() ?? string.Empty;
		_native.OpenFileDialog((IAvnWindow)_window.Native, events, options.AllowMultiple.AsComBool(), options.Title ?? string.Empty, initialDirectory, string.Empty, PrepareFilterParameter(options.FileTypeFilter));
		IStorageFile[] array = (await events.Task.ConfigureAwait(continueOnCapturedContext: false))?.Select((string f) => new BclStorageFile(new FileInfo(f))).ToArray();
		return array ?? Array.Empty<IStorageFile>();
	}

	public override async Task<IStorageFile?> SaveFilePickerAsync(FilePickerSaveOptions options)
	{
		using SystemDialogEvents events = new SystemDialogEvents();
		string initialDirectory = options.SuggestedStartLocation?.TryGetLocalPath() ?? string.Empty;
		_native.SaveFileDialog((IAvnWindow)_window.Native, events, options.Title ?? string.Empty, initialDirectory, options.SuggestedFileName ?? string.Empty, PrepareFilterParameter(options.FileTypeChoices));
		string text = (await events.Task.ConfigureAwait(continueOnCapturedContext: false)).FirstOrDefault();
		return (text != null) ? new BclStorageFile(new FileInfo(text)) : null;
	}

	public override async Task<IReadOnlyList<IStorageFolder>> OpenFolderPickerAsync(FolderPickerOpenOptions options)
	{
		using SystemDialogEvents events = new SystemDialogEvents();
		string initialPath = options.SuggestedStartLocation?.TryGetLocalPath() ?? string.Empty;
		_native.SelectFolderDialog((IAvnWindow)_window.Native, events, options.AllowMultiple.AsComBool(), options.Title ?? "", initialPath);
		IStorageFolder[] array = (await events.Task.ConfigureAwait(continueOnCapturedContext: false))?.Select((string f) => new BclStorageFolder(new DirectoryInfo(f))).ToArray();
		return array ?? Array.Empty<IStorageFolder>();
	}

	private static string PrepareFilterParameter(IReadOnlyList<FilePickerFileType>? fileTypes)
	{
		return string.Join(";", fileTypes?.SelectMany(delegate(FilePickerFileType f)
		{
			IReadOnlyList<string>? appleUniformTypeIdentifiers = f.AppleUniformTypeIdentifiers;
			if (appleUniformTypeIdentifiers != null && appleUniformTypeIdentifiers.Any())
			{
				return f.AppleUniformTypeIdentifiers;
			}
			IReadOnlyList<string>? mimeTypes = f.MimeTypes;
			return (mimeTypes != null && mimeTypes.Any()) ? f.MimeTypes.Where((string t) => t != "*/*") : Array.Empty<string>();
		}) ?? Array.Empty<string>());
	}
}
