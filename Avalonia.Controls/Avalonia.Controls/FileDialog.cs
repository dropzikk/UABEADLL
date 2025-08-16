using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Avalonia.Controls;

[Obsolete("Use Window.StorageProvider API or TopLevel.StorageProvider API")]
[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class FileDialog : FileSystemDialog
{
	public List<FileDialogFilter> Filters { get; set; } = new List<FileDialogFilter>();

	public string? InitialFileName { get; set; }
}
