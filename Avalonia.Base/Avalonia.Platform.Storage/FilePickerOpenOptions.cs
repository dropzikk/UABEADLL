using System.Collections.Generic;

namespace Avalonia.Platform.Storage;

public class FilePickerOpenOptions : PickerOptions
{
	public bool AllowMultiple { get; set; }

	public IReadOnlyList<FilePickerFileType>? FileTypeFilter { get; set; }
}
