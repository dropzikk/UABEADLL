using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls.Platform;
using Avalonia.Platform.Storage;
using Avalonia.Platform.Storage.FileIO;

namespace Avalonia.Controls;

[Obsolete("Use Window.StorageProvider API or TopLevel.StorageProvider API")]
[EditorBrowsable(EditorBrowsableState.Never)]
public class OpenFileDialog : FileDialog
{
	public bool AllowMultiple { get; set; }

	public Task<string[]?> ShowAsync(Window parent)
	{
		if (parent == null)
		{
			throw new ArgumentNullException("parent");
		}
		return AvaloniaLocator.Current.GetRequiredService<ISystemDialogImpl>().ShowFileDialogAsync(this, parent);
	}

	public FilePickerOpenOptions ToFilePickerOpenOptions()
	{
		FilePickerOpenOptions obj = new FilePickerOpenOptions
		{
			AllowMultiple = AllowMultiple,
			FileTypeFilter = base.Filters?.Select((FileDialogFilter f) => new FilePickerFileType(f.Name)
			{
				Patterns = f.Extensions.Select((string e) => "*." + e).ToArray()
			}).ToArray(),
			Title = base.Title
		};
		string directory = base.Directory;
		obj.SuggestedStartLocation = ((directory != null) ? new BclStorageFolder(new DirectoryInfo(directory)) : null);
		return obj;
	}
}
