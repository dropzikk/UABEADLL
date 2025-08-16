using System;

namespace Avalonia.X11;

[Flags]
internal enum XModifierMask
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
	AnyModifier = 0x8000
}
