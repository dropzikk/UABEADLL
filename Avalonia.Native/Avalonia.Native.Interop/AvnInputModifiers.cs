namespace Avalonia.Native.Interop;

internal enum AvnInputModifiers
{
	AvnInputModifiersNone = 0,
	Alt = 1,
	Control = 2,
	Shift = 4,
	Windows = 8,
	LeftMouseButton = 0x10,
	RightMouseButton = 0x20,
	MiddleMouseButton = 0x40,
	XButton1MouseButton = 0x80,
	XButton2MouseButton = 0x100
}
