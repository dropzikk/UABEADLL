using System;

namespace Avalonia.FreeDesktop.DBusIme.IBus;

[Flags]
internal enum IBusCapability
{
	CapPreeditText = 1,
	CapAuxiliaryText = 2,
	CapLookupTable = 4,
	CapFocus = 8,
	CapProperty = 0x10,
	CapSurroundingText = 0x20
}
