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
public class SaveFileDialog : FileDialog
{
	public string? DefaultExtension { get; set; }

	public bool? ShowOverwritePrompt { get; set; }

	public async Task<string?> ShowAsync(Window parent)
	{
		if (parent == null)
		{
			throw new ArgumentNullException("parent");
		}
		return ((await AvaloniaLocator.Current.GetRequiredService<ISystemDialogImpl>().ShowFileDialogAsync(this, parent)) ?? Array.Empty<string>()).FirstOrDefault();
	}

	public FilePickerSaveOptions ToFilePickerSaveOptions()
	{
		FilePickerSaveOptions obj = new FilePickerSaveOptions
		{
			SuggestedFileName = base.InitialFileName,
			DefaultExtension = DefaultExtension,
			FileTypeChoices = base.Filters?.Select((FileDialogFilter f) => new FilePickerFileType(f.Name)
			{
				Patterns = f.Extensions.Select((string e) => "*." + e).ToArray()
			}).ToArray(),
			Title = base.Title
		};
		string directory = base.Directory;
		obj.SuggestedStartLocation = ((directory != null) ? new BclStorageFolder(new DirectoryInfo(directory)) : null);
		obj.ShowOverwritePrompt = ShowOverwritePrompt;
		return obj;
	}
}
