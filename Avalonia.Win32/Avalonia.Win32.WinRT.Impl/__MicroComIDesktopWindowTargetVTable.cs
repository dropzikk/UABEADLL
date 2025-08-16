using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComIDesktopWindowTargetVTable : __MicroComIInspectableVTable
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetIsTopmostDelegate(void* @this, int* value);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetIsTopmost(void* @this, int* value)
	{
		IDesktopWindowTarget desktopWindowTarget = null;
		try
		{
			desktopWindowTarget = (IDesktopWindowTarget)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			int isTopmost = desktopWindowTarget.IsTopmost;
			*value = isTopmost;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(desktopWindowTarget, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIDesktopWindowTargetVTable()
	{
		AddMethod((delegate*<void*, int*, int>)(&GetIsTopmost));
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IDesktopWindowTarget), new __MicroComIDesktopWindowTargetVTable().CreateVTable());
	}
}
