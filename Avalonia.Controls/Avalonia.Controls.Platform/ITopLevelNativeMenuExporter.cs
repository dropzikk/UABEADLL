using System;
using Avalonia.Metadata;

namespace Avalonia.Controls.Platform;

[Unstable]
public interface ITopLevelNativeMenuExporter : INativeMenuExporter
{
	bool IsNativeMenuExported { get; }

	event EventHandler OnIsNativeMenuExportedChanged;
}
