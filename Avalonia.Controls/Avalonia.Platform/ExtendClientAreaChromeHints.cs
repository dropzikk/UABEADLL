using System;

namespace Avalonia.Platform;

[Flags]
public enum ExtendClientAreaChromeHints
{
	NoChrome = 0,
	Default = 2,
	SystemChrome = 1,
	PreferSystemChrome = 2,
	OSXThickTitleBar = 8
}
