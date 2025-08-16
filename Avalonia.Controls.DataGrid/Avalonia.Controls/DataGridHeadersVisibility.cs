using System;

namespace Avalonia.Controls;

[Flags]
public enum DataGridHeadersVisibility
{
	All = 3,
	Column = 1,
	Row = 2,
	None = 0
}
