using System;
using Avalonia.Win32.Interop;

namespace Avalonia.Win32;

internal class EmbeddedWindowImpl : WindowImpl
{
	protected override IntPtr CreateWindowOverride(ushort atom)
	{
		return UnmanagedMethods.CreateWindowEx(0, atom, null, 1073741824u, 0, 0, 640, 480, OffscreenParentWindow.Handle, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
	}
}
