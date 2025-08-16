using System;

namespace Avalonia.Win32.Interop;

[Flags]
internal enum NIF : uint
{
	MESSAGE = 1u,
	ICON = 2u,
	TIP = 4u,
	STATE = 8u,
	INFO = 0x10u,
	GUID = 0x20u,
	REALTIME = 0x40u,
	SHOWTIP = 0x80u
}
