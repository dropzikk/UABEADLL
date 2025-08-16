using System;

namespace Avalonia.Remote.Protocol.Input;

[Flags]
public enum InputModifiers
{
	Alt = 0,
	Control = 1,
	Shift = 2,
	Windows = 3,
	LeftMouseButton = 4,
	RightMouseButton = 5,
	MiddleMouseButton = 6
}
