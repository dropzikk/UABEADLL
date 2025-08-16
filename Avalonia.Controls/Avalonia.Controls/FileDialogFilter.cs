using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Avalonia.Controls;

[Obsolete("Use Window.StorageProvider API or TopLevel.StorageProvider API")]
[EditorBrowsable(EditorBrowsableState.Never)]
public class FileDialogFilter
{
	public string? Name { get; set; }

	public List<string> Extensions { get; set; } = new List<string>();
}
