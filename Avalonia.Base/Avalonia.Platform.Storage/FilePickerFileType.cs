using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Avalonia.Platform.Storage;

public sealed class FilePickerFileType
{
	public string Name { get; }

	public IReadOnlyList<string>? Patterns { get; set; }

	public IReadOnlyList<string>? MimeTypes { get; set; }

	public IReadOnlyList<string>? AppleUniformTypeIdentifiers { get; set; }

	public FilePickerFileType(string? name)
	{
		Name = name ?? string.Empty;
	}

	internal IReadOnlyList<string>? TryGetExtensions()
	{
		return (from e in Patterns?.Select(Path.GetExtension)
			where !string.IsNullOrEmpty(e) && !e.Contains('*') && e.StartsWith(".")
			select e).ToArray();
	}
}
