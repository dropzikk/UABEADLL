using System.Collections.Generic;

namespace Avalonia.Platform.Storage;

public class FilePickerSaveOptions : PickerOptions
{
	public string? SuggestedFileName { get; set; }

	public string? DefaultExtension { get; set; }

	public IReadOnlyList<FilePickerFileType>? FileTypeChoices { get; set; }

	public bool? ShowOverwritePrompt { get; set; }
}
