using System;

namespace Avalonia.FreeDesktop.DBusIme.Fcitx;

[Flags]
internal enum FcitxKeyState
{
	FcitxKeyState_None = 0,
	FcitxKeyState_Shift = 1,
	FcitxKeyState_CapsLock = 2,
	FcitxKeyState_Ctrl = 4,
	FcitxKeyState_Alt = 8,
	FcitxKeyState_Alt_Shift = 9,
	FcitxKeyState_Ctrl_Shift = 5,
	FcitxKeyState_Ctrl_Alt = 0xC,
	FcitxKeyState_Ctrl_Alt_Shift = 0xD,
	FcitxKeyState_NumLock = 0x10,
	FcitxKeyState_Super = 0x40,
	FcitxKeyState_ScrollLock = 0x80,
	FcitxKeyState_MousePressed = 0x100,
	FcitxKeyState_HandledMask = 0x1000000,
	FcitxKeyState_IgnoredMask = 0x2000000,
	FcitxKeyState_Super2 = 0x4000000,
	FcitxKeyState_Hyper = 0x8000000,
	FcitxKeyState_Meta = 0x10000000,
	FcitxKeyState_UsedMask = 0x5C001FFF
}
