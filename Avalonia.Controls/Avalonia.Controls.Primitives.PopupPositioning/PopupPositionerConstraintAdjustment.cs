using System;

namespace Avalonia.Controls.Primitives.PopupPositioning;

[Flags]
public enum PopupPositionerConstraintAdjustment
{
	None = 0,
	SlideX = 1,
	SlideY = 2,
	FlipX = 4,
	FlipY = 8,
	ResizeX = 0x10,
	ResizeY = 0x10,
	All = 0x1F
}
