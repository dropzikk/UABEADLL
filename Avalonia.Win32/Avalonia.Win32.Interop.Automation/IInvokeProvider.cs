using System.Runtime.InteropServices;

namespace Avalonia.Win32.Interop.Automation;

[ComVisible(true)]
[Guid("54fcb24b-e18e-47a2-b4d3-eccbe77599a2")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IInvokeProvider
{
	void Invoke();
}
