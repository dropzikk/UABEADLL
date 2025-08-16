using Avalonia.Metadata;
using Avalonia.Platform;

namespace Avalonia.Controls.Platform;

[Unstable]
public interface INativeControlHostDestroyableControlHandle : IPlatformHandle
{
	void Destroy();
}
