using System;

namespace Avalonia.FreeDesktop.DBusIme.IBus;

[Flags]
internal enum IBusModifierMask
{
	ShiftMask = 1,
	LockMask = 2,
	ControlMask = 4,
	Mod1Mask = 8,
	Mod2Mask = 0x10,
	Mod3Mask = 0x20,
	Mod4Mask = 0x40,
	Mod5Mask = 0x80,
	Button1Mask = 0x100,
	Button2Mask = 0x200,
	Button3Mask = 0x400,
	Button4Mask = 0x800,
	Button5Mask = 0x1000,
	HandledMask = 0x1000000,
	ForwardMask = 0x2000000,
	IgnoredMask = 0x2000000,
	SuperMask = 0x4000000,
	HyperMask = 0x8000000,
	MetaMask = 0x10000000,
	ReleaseMask = 0x40000000,
	ModifierMask = 0x5C001FFF
}
