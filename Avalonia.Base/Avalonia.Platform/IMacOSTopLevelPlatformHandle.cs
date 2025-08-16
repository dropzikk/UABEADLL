using System;
using Avalonia.Metadata;

namespace Avalonia.Platform;

[Unstable]
public interface IMacOSTopLevelPlatformHandle
{
	IntPtr NSView { get; }

	IntPtr NSWindow { get; }

	IntPtr GetNSViewRetained();

	IntPtr GetNSWindowRetained();
}
