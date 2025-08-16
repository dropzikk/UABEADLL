using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComIDesktopWindowContentBridgeInteropVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int InitializeDelegate(void* @this, void* compositor, IntPtr parentHwnd);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetHWndDelegate(void* @this, IntPtr* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetAppliedScaleFactorDelegate(void* @this, float* value);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int Initialize(void* @this, void* compositor, IntPtr parentHwnd)
	{
		IDesktopWindowContentBridgeInterop desktopWindowContentBridgeInterop = null;
		try
		{
			desktopWindowContentBridgeInterop = (IDesktopWindowContentBridgeInterop)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			desktopWindowContentBridgeInterop.Initialize(MicroComRuntime.CreateProxyOrNullFor<ICompositor>(compositor, ownsHandle: false), parentHwnd);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(desktopWindowContentBridgeInterop, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetHWnd(void* @this, IntPtr* value)
	{
		IDesktopWindowContentBridgeInterop desktopWindowContentBridgeInterop = null;
		try
		{
			desktopWindowContentBridgeInterop = (IDesktopWindowContentBridgeInterop)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IntPtr hWnd = desktopWindowContentBridgeInterop.HWnd;
			*value = hWnd;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(desktopWindowContentBridgeInterop, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetAppliedScaleFactor(void* @this, float* value)
	{
		IDesktopWindowContentBridgeInterop desktopWindowContentBridgeInterop = null;
		try
		{
			desktopWindowContentBridgeInterop = (IDesktopWindowContentBridgeInterop)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			float appliedScaleFactor = desktopWindowContentBridgeInterop.AppliedScaleFactor;
			*value = appliedScaleFactor;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(desktopWindowContentBridgeInterop, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIDesktopWindowContentBridgeInteropVTable()
	{
		AddMethod((delegate*<void*, void*, IntPtr, int>)(&Initialize));
		AddMethod((delegate*<void*, IntPtr*, int>)(&GetHWnd));
		AddMethod((delegate*<void*, float*, int>)(&GetAppliedScaleFactor));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IDesktopWindowContentBridgeInterop), new __MicroComIDesktopWindowContentBridgeInteropVTable().CreateVTable());
	}
}
