using System;

namespace Avalonia.Win32.Interop.Automation;

[Flags]
public enum ProviderOptions
{
	ClientSideProvider = 1,
	ServerSideProvider = 2,
	NonClientAreaProvider = 4,
	OverrideProvider = 8,
	ProviderOwnsSetFocus = 0x10,
	UseComThreading = 0x20
}
