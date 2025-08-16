using System;
using Avalonia.Platform;
using Avalonia.Win32.Interop;

namespace Avalonia.Win32;

internal class CursorImpl : ICursorImpl, IDisposable, IPlatformHandle
{
	private readonly bool _isCustom;

	public IntPtr Handle { get; private set; }

	public string HandleDescriptor => "HCURSOR";

	public CursorImpl(IntPtr handle, bool isCustom)
	{
		Handle = handle;
		_isCustom = isCustom;
	}

	public void Dispose()
	{
		if (_isCustom && Handle != IntPtr.Zero)
		{
			UnmanagedMethods.DestroyIcon(Handle);
			Handle = IntPtr.Zero;
		}
	}
}
