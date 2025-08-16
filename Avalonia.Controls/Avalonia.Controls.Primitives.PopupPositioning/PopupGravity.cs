using System;

namespace Avalonia.Controls.Primitives.PopupPositioning;

[Flags]
public enum PopupGravity
{
	None = 0,
	Top = 1,
	Bottom = 2,
	Left = 4,
	Right = 8,
	TopLeft = 5,
	TopRight = 9,
	BottomLeft = 6,
	BottomRight = 0xA
}
