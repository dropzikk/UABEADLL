using Avalonia.Metadata;

namespace Avalonia.Controls.Platform;

[Unstable]
public interface INativeMenuExporter
{
	void SetNativeMenu(NativeMenu? menu);
}
