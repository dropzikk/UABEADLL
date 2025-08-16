using System.Runtime.InteropServices;

namespace Avalonia.Win32.Interop.Automation;

[ComVisible(true)]
[Guid("70d46e77-e3a8-449d-913c-e30eb2afecdb")]
public enum DockPosition
{
	Top,
	Left,
	Bottom,
	Right,
	Fill,
	None
}
