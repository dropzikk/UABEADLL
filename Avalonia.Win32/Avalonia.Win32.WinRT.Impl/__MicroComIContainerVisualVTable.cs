using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComIContainerVisualVTable : __MicroComIInspectableVTable
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetChildrenDelegate(void* @this, void** value);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetChildren(void* @this, void** value)
	{
		IContainerVisual containerVisual = null;
		try
		{
			containerVisual = (IContainerVisual)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IVisualCollection children = containerVisual.Children;
			*value = MicroComRuntime.GetNativePointer(children, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(containerVisual, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIContainerVisualVTable()
	{
		AddMethod((delegate*<void*, void**, int>)(&GetChildren));
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IContainerVisual), new __MicroComIContainerVisualVTable().CreateVTable());
	}
}
