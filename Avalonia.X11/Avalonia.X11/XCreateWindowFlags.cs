using System;

namespace Avalonia.X11;

[Flags]
internal enum XCreateWindowFlags
{
	CWBackPixmap = 1,
	CWBackPixel = 2,
	CWBorderPixmap = 4,
	CWBorderPixel = 8,
	CWBitGravity = 0x10,
	CWWinGravity = 0x20,
	CWBackingStore = 0x40,
	CWBackingPlanes = 0x80,
	CWBackingPixel = 0x100,
	CWOverrideRedirect = 0x200,
	CWSaveUnder = 0x400,
	CWEventMask = 0x800,
	CWDontPropagate = 0x1000,
	CWColormap = 0x2000,
	CWCursor = 0x4000
}
