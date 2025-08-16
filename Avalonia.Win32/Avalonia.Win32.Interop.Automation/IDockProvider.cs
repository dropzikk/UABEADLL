using System.Runtime.InteropServices;

namespace Avalonia.Win32.Interop.Automation;

[ComVisible(true)]
[Guid("159bc72c-4ad3-485e-9637-d7052edf0146")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IDockProvider
{
	DockPosition DockPosition { get; }

	void SetDockPosition(DockPosition dockPosition);
}
