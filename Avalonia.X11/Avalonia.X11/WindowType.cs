using System;

namespace Avalonia.X11;

[Flags]
internal enum WindowType
{
	Client = 1,
	Whole = 2,
	Both = 3
}
