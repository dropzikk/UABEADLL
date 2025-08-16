using System;

namespace Avalonia.X11;

[Flags]
internal enum XiDeviceEventFlags
{
	None = 0,
	XIPointerEmulated = 0x10000
}
