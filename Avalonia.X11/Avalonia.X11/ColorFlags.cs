using System;

namespace Avalonia.X11;

[Flags]
internal enum ColorFlags
{
	DoRed = 1,
	DoGreen = 2,
	DoBlue = 4
}
