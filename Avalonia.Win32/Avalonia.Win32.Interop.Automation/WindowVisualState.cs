using System.Runtime.InteropServices;

namespace Avalonia.Win32.Interop.Automation;

[ComVisible(true)]
[Guid("fdc8f176-aed2-477a-8c89-ea04cc5f278d")]
public enum WindowVisualState
{
	Normal,
	Maximized,
	Minimized
}
