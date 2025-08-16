using System;

namespace Avalonia.Win32.Interop;

[Flags]
internal enum NIIF : uint
{
	NONE = 0u,
	INFO = 1u,
	WARNING = 2u,
	ERROR = 3u,
	USER = 4u,
	ICON_MASK = 0xFu,
	NOSOUND = 0x10u,
	LARGE_ICON = 0x20u,
	RESPECT_QUIET_TIME = 0x80u
}
