using System;
using System.Runtime.InteropServices;
using System.Threading;
using Avalonia.OpenGL;
using Avalonia.Win32.Interop;

namespace Avalonia.Win32.OpenGl;

internal class WglRestoreContext : IDisposable
{
	private readonly object? _monitor;

	private readonly IntPtr _oldDc;

	private readonly IntPtr _oldContext;

	public WglRestoreContext(IntPtr gc, IntPtr context, object? monitor, bool takeMonitor = true)
	{
		_monitor = monitor;
		_oldDc = UnmanagedMethods.wglGetCurrentDC();
		_oldContext = UnmanagedMethods.wglGetCurrentContext();
		if (monitor != null && takeMonitor)
		{
			Monitor.Enter(monitor);
		}
		if (!UnmanagedMethods.wglMakeCurrent(gc, context))
		{
			int lastWin32Error = Marshal.GetLastWin32Error();
			int deviceCaps = UnmanagedMethods.GetDeviceCaps(gc, (UnmanagedMethods.DEVICECAP)12);
			if (monitor != null && takeMonitor)
			{
				Monitor.Exit(monitor);
			}
			throw new OpenGlException($"Unable to make the context current: {lastWin32Error}, DC valid: {deviceCaps != 0}");
		}
	}

	public void Dispose()
	{
		if (!UnmanagedMethods.wglMakeCurrent(_oldDc, _oldContext))
		{
			UnmanagedMethods.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
		}
		if (_monitor != null)
		{
			Monitor.Exit(_monitor);
		}
	}
}
