using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComIDesktopWindowTargetInteropVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetHWndDelegate(void* @this, IntPtr* value);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetHWnd(void* @this, IntPtr* value)
	{
		IDesktopWindowTargetInterop desktopWindowTargetInterop = null;
		try
		{
			desktopWindowTargetInterop = (IDesktopWindowTargetInterop)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IntPtr hWnd = desktopWindowTargetInterop.HWnd;
			*value = hWnd;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(desktopWindowTargetInterop, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIDesktopWindowTargetInteropVTable()
	{
		AddMethod((delegate*<void*, IntPtr*, int>)(&GetHWnd));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IDesktopWindowTargetInterop), new __MicroComIDesktopWindowTargetInteropVTable().CreateVTable());
	}
}
