using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositor3VTable : __MicroComIInspectableVTable
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateHostBackdropBrushDelegate(void* @this, void** result);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateHostBackdropBrush(void* @this, void** result)
	{
		ICompositor3 compositor = null;
		try
		{
			compositor = (ICompositor3)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			ICompositionBackdropBrush obj = compositor.CreateHostBackdropBrush();
			*result = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositor, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComICompositor3VTable()
	{
		AddMethod((delegate*<void*, void**, int>)(&CreateHostBackdropBrush));
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(ICompositor3), new __MicroComICompositor3VTable().CreateVTable());
	}
}
