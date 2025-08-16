using System;
using System.ComponentModel;

namespace Avalonia.Controls;

[Obsolete("Use Window.StorageProvider API or TopLevel.StorageProvider API")]
[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class FileSystemDialog : SystemDialog
{
	public string? Directory { get; set; }
}
