using System;
using System.ComponentModel;

namespace Avalonia.Input;

public static class DataFormats
{
	public static readonly string Text = "Text";

	public static readonly string Files = "Files";

	[Obsolete("Use DataFormats.Files, this format is supported only on desktop platforms.")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static readonly string FileNames = "FileNames";
}
