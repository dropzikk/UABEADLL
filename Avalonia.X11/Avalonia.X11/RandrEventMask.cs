using System;

namespace Avalonia.X11;

[Flags]
internal enum RandrEventMask
{
	RRScreenChangeNotify = 1,
	RRCrtcChangeNotifyMask = 2,
	RROutputChangeNotifyMask = 4,
	RROutputPropertyNotifyMask = 8,
	RRProviderChangeNotifyMask = 0x10,
	RRProviderPropertyNotifyMask = 0x20,
	RRResourceChangeNotifyMask = 0x40,
	RRLeaseNotifyMask = 0x80
}
