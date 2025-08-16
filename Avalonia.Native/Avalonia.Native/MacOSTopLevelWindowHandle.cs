using System;
using Avalonia.Native.Interop;
using Avalonia.Platform;

namespace Avalonia.Native;

internal class MacOSTopLevelWindowHandle : IPlatformHandle, IMacOSTopLevelPlatformHandle
{
	private IAvnWindowBase _native;

	public IntPtr Handle => NSWindow;

	public string HandleDescriptor => "NSWindow";

	public IntPtr NSView => _native?.ObtainNSViewHandle() ?? IntPtr.Zero;

	public IntPtr NSWindow => _native?.ObtainNSWindowHandle() ?? IntPtr.Zero;

	public MacOSTopLevelWindowHandle(IAvnWindowBase native)
	{
		_native = native;
	}

	public IntPtr GetNSViewRetained()
	{
		return _native?.ObtainNSViewHandleRetained() ?? IntPtr.Zero;
	}

	public IntPtr GetNSWindowRetained()
	{
		return _native?.ObtainNSWindowHandleRetained() ?? IntPtr.Zero;
	}
}
