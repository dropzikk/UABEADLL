namespace Avalonia.X11;

internal enum XiEventType
{
	XI_DeviceChanged = 1,
	XI_KeyPress = 2,
	XI_KeyRelease = 3,
	XI_ButtonPress = 4,
	XI_ButtonRelease = 5,
	XI_Motion = 6,
	XI_Enter = 7,
	XI_Leave = 8,
	XI_FocusIn = 9,
	XI_FocusOut = 10,
	XI_HierarchyChanged = 11,
	XI_PropertyEvent = 12,
	XI_RawKeyPress = 13,
	XI_RawKeyRelease = 14,
	XI_RawButtonPress = 15,
	XI_RawButtonRelease = 16,
	XI_RawMotion = 17,
	XI_TouchBegin = 18,
	XI_TouchUpdate = 19,
	XI_TouchEnd = 20,
	XI_TouchOwnership = 21,
	XI_RawTouchBegin = 22,
	XI_RawTouchUpdate = 23,
	XI_RawTouchEnd = 24,
	XI_BarrierHit = 25,
	XI_BarrierLeave = 26,
	XI_LASTEVENT = 26
}
