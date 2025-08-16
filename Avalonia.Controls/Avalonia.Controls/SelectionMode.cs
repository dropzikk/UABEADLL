using System;

namespace Avalonia.Controls;

[Flags]
public enum SelectionMode
{
	Single = 0,
	Multiple = 1,
	Toggle = 2,
	AlwaysSelected = 4
}
