using Avalonia.Metadata;

namespace Avalonia.Controls.Platform;

[Unstable]
public interface INativeMenuExporterProvider
{
	INativeMenuExporter? NativeMenuExporter { get; }
}
