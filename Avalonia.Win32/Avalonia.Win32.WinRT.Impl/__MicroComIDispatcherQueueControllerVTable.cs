using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComIDispatcherQueueControllerVTable : __MicroComIInspectableVTable
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetDispatcherQueueDelegate(void* @this, void** value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int ShutdownQueueAsyncDelegate(void* @this, void** operation);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetDispatcherQueue(void* @this, void** value)
	{
		IDispatcherQueueController dispatcherQueueController = null;
		try
		{
			dispatcherQueueController = (IDispatcherQueueController)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IDispatcherQueue dispatcherQueue = dispatcherQueueController.DispatcherQueue;
			*value = MicroComRuntime.GetNativePointer(dispatcherQueue, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(dispatcherQueueController, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int ShutdownQueueAsync(void* @this, void** operation)
	{
		IDispatcherQueueController dispatcherQueueController = null;
		try
		{
			dispatcherQueueController = (IDispatcherQueueController)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IAsyncAction obj = dispatcherQueueController.ShutdownQueueAsync();
			*operation = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(dispatcherQueueController, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIDispatcherQueueControllerVTable()
	{
		AddMethod((delegate*<void*, void**, int>)(&GetDispatcherQueue));
		AddMethod((delegate*<void*, void**, int>)(&ShutdownQueueAsync));
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IDispatcherQueueController), new __MicroComIDispatcherQueueControllerVTable().CreateVTable());
	}
}
