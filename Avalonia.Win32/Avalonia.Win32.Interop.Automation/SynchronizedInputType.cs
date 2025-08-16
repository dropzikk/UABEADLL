using System.Runtime.InteropServices;

namespace Avalonia.Win32.Interop.Automation;

[ComVisible(true)]
[Guid("fdc8f176-aed2-477a-8c89-5604c66f278d")]
public enum SynchronizedInputType
{
	KeyUp = 1,
	KeyDown = 2,
	MouseLeftButtonUp = 4,
	MouseLeftButtonDown = 8,
	MouseRightButtonUp = 0x10,
	MouseRightButtonDown = 0x20
}
