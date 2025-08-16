using System;

namespace Avalonia.Controls;

[Flags]
public enum SizeToContent
{
	Manual = 0,
	Width = 1,
	Height = 2,
	WidthAndHeight = 3
}
