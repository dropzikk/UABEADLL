using System;
using System.ComponentModel;
using System.Threading;
using Avalonia.Platform;
using Avalonia.Threading;
using Avalonia.Win32.Interop;
using Avalonia.Win32.Win32Com;
using MicroCom.Runtime;

namespace Avalonia.Win32;

internal class OleContext
{
	private static OleContext? s_current;

	internal static OleContext? Current
	{
		get
		{
			if (!IsValidOleThread())
			{
				return null;
			}
			return s_current ?? (s_current = new OleContext());
		}
	}

	private OleContext()
	{
		UnmanagedMethods.HRESULT hRESULT = UnmanagedMethods.OleInitialize(IntPtr.Zero);
		if (hRESULT != 0 && hRESULT != UnmanagedMethods.HRESULT.S_FALSE)
		{
			throw new Win32Exception((int)hRESULT, "Failed to initialize OLE");
		}
	}

	private static bool IsValidOleThread()
	{
		if (Dispatcher.UIThread.CheckAccess())
		{
			return Thread.CurrentThread.GetApartmentState() == ApartmentState.STA;
		}
		return false;
	}

	internal bool RegisterDragDrop(IPlatformHandle? hwnd, IDropTarget? target)
	{
		if (hwnd?.HandleDescriptor != "HWND" || target == null)
		{
			return false;
		}
		IntPtr nativeIntPtr = target.GetNativeIntPtr();
		return UnmanagedMethods.RegisterDragDrop(hwnd.Handle, nativeIntPtr) == UnmanagedMethods.HRESULT.S_OK;
	}

	internal bool UnregisterDragDrop(IPlatformHandle? hwnd)
	{
		if (hwnd?.HandleDescriptor != "HWND")
		{
			return false;
		}
		return UnmanagedMethods.RevokeDragDrop(hwnd.Handle) == UnmanagedMethods.HRESULT.S_OK;
	}
}
