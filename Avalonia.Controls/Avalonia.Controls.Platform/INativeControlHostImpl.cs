using System;
using Avalonia.Metadata;
using Avalonia.Platform;

namespace Avalonia.Controls.Platform;

[Unstable]
public interface INativeControlHostImpl
{
	INativeControlHostDestroyableControlHandle CreateDefaultChild(IPlatformHandle parent);

	INativeControlHostControlTopLevelAttachment CreateNewAttachment(Func<IPlatformHandle, IPlatformHandle> create);

	INativeControlHostControlTopLevelAttachment CreateNewAttachment(IPlatformHandle handle);

	bool IsCompatibleWith(IPlatformHandle handle);
}
