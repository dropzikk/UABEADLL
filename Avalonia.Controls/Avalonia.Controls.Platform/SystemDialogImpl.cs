using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;

namespace Avalonia.Controls.Platform;

[Obsolete]
[EditorBrowsable(EditorBrowsableState.Never)]
internal class SystemDialogImpl : ISystemDialogImpl
{
	public async Task<string[]?> ShowFileDialogAsync(FileDialog dialog, Window parent)
	{
		if (dialog is OpenFileDialog openFileDialog)
		{
			IStorageProvider storageProvider = parent.StorageProvider;
			if (!storageProvider.CanOpen)
			{
				return null;
			}
			FilePickerOpenOptions options = openFileDialog.ToFilePickerOpenOptions();
			return (await storageProvider.OpenFilePickerAsync(options)).Select((IStorageFile file) => file.TryGetLocalPath() ?? file.Name).ToArray();
		}
		if (dialog is SaveFileDialog saveFileDialog)
		{
			IStorageProvider storageProvider2 = parent.StorageProvider;
			if (!storageProvider2.CanSave)
			{
				return null;
			}
			FilePickerSaveOptions options2 = saveFileDialog.ToFilePickerSaveOptions();
			IStorageFile storageFile = await storageProvider2.SaveFilePickerAsync(options2);
			if (storageFile == null)
			{
				return null;
			}
			string text = storageFile.TryGetLocalPath() ?? storageFile.Name;
			return new string[1] { text };
		}
		return null;
	}

	public async Task<string?> ShowFolderDialogAsync(OpenFolderDialog dialog, Window parent)
	{
		IStorageProvider storageProvider = parent.StorageProvider;
		if (!storageProvider.CanPickFolder)
		{
			return null;
		}
		FolderPickerOpenOptions options = dialog.ToFolderPickerOpenOptions();
		return (await storageProvider.OpenFolderPickerAsync(options)).Select((IStorageFolder folder) => folder.TryGetLocalPath() ?? folder.Name).FirstOrDefault((string u) => u != null);
	}
}
