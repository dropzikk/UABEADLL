using System;

namespace Avalonia.Win32.Win32Com;

[Flags]
internal enum DropEffect
{
	None = 0,
	Copy = 1,
	Move = 2,
	Link = 4,
	Scroll = int.MinValue
}
