using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Avalonia.Win32.Interop;

namespace Avalonia.Win32;

internal class OffscreenParentWindow
{
	private static UnmanagedMethods.WndProc? s_wndProcDelegate;

	public static IntPtr Handle { get; } = CreateParentWindow();

	private static IntPtr CreateParentWindow()
	{
		s_wndProcDelegate = ParentWndProc;
		UnmanagedMethods.WNDCLASSEX wNDCLASSEX = default(UnmanagedMethods.WNDCLASSEX);
		wNDCLASSEX.cbSize = Marshal.SizeOf<UnmanagedMethods.WNDCLASSEX>();
		wNDCLASSEX.hInstance = UnmanagedMethods.GetModuleHandle(null);
		wNDCLASSEX.lpfnWndProc = s_wndProcDelegate;
		wNDCLASSEX.lpszClassName = "AvaloniaEmbeddedWindow-" + Guid.NewGuid();
		UnmanagedMethods.WNDCLASSEX lpwcx = wNDCLASSEX;
		ushort num = UnmanagedMethods.RegisterClassEx(ref lpwcx);
		if (num == 0)
		{
			throw new Win32Exception();
		}
		IntPtr intPtr = UnmanagedMethods.CreateWindowEx(0, num, null, 13565952u, int.MinValue, int.MinValue, int.MinValue, int.MinValue, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
		if (intPtr == IntPtr.Zero)
		{
			throw new Win32Exception();
		}
		return intPtr;
	}

	private static IntPtr ParentWndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
	{
		return UnmanagedMethods.DefWindowProc(hWnd, msg, wParam, lParam);
	}
}
