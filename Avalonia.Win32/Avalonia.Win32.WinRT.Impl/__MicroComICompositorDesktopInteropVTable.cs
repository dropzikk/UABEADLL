using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositorDesktopInteropVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateDesktopWindowTargetDelegate(void* @this, IntPtr hwndTarget, int isTopmost, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int EnsureOnThreadDelegate(void* @this, int threadId);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateDesktopWindowTarget(void* @this, IntPtr hwndTarget, int isTopmost, void** result)
	{
		ICompositorDesktopInterop compositorDesktopInterop = null;
		try
		{
			compositorDesktopInterop = (ICompositorDesktopInterop)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IDesktopWindowTarget obj = compositorDesktopInterop.CreateDesktopWindowTarget(hwndTarget, isTopmost);
			*result = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositorDesktopInterop, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int EnsureOnThread(void* @this, int threadId)
	{
		ICompositorDesktopInterop compositorDesktopInterop = null;
		try
		{
			compositorDesktopInterop = (ICompositorDesktopInterop)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			compositorDesktopInterop.EnsureOnThread(threadId);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositorDesktopInterop, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComICompositorDesktopInteropVTable()
	{
		AddMethod((delegate*<void*, IntPtr, int, void**, int>)(&CreateDesktopWindowTarget));
		AddMethod((delegate*<void*, int, int>)(&EnsureOnThread));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(ICompositorDesktopInterop), new __MicroComICompositorDesktopInteropVTable().CreateVTable());
	}
}
