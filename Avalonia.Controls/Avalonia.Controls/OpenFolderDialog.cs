using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls.Platform;
using Avalonia.Platform.Storage;
using Avalonia.Platform.Storage.FileIO;

namespace Avalonia.Controls;

[Obsolete("Use Window.StorageProvider API or TopLevel.StorageProvider API")]
[EditorBrowsable(EditorBrowsableState.Never)]
public class OpenFolderDialog : FileSystemDialog
{
	public Task<string?> ShowAsync(Window parent)
	{
		if (parent == null)
		{
			throw new ArgumentNullException("parent");
		}
		return AvaloniaLocator.Current.GetRequiredService<ISystemDialogImpl>().ShowFolderDialogAsync(this, parent);
	}

	public FolderPickerOpenOptions ToFolderPickerOpenOptions()
	{
		FolderPickerOpenOptions obj = new FolderPickerOpenOptions
		{
			Title = base.Title
		};
		string directory = base.Directory;
		obj.SuggestedStartLocation = ((directory != null) ? new BclStorageFolder(new DirectoryInfo(directory)) : null);
		return obj;
	}
}
