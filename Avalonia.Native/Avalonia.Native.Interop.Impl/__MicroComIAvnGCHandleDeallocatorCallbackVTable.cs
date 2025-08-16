using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnGCHandleDeallocatorCallbackVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void FreeGCHandleDelegate(void* @this, IntPtr handle);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void FreeGCHandle(void* @this, IntPtr handle)
	{
		IAvnGCHandleDeallocatorCallback avnGCHandleDeallocatorCallback = null;
		try
		{
			avnGCHandleDeallocatorCallback = (IAvnGCHandleDeallocatorCallback)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnGCHandleDeallocatorCallback.FreeGCHandle(handle);
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnGCHandleDeallocatorCallback, e);
		}
	}

	protected unsafe __MicroComIAvnGCHandleDeallocatorCallbackVTable()
	{
		AddMethod((delegate*<void*, IntPtr, void>)(&FreeGCHandle));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IAvnGCHandleDeallocatorCallback), new __MicroComIAvnGCHandleDeallocatorCallbackVTable().CreateVTable());
	}
}
