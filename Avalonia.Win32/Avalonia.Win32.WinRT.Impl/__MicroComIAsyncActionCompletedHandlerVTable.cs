using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComIAsyncActionCompletedHandlerVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int InvokeDelegate(void* @this, void* asyncInfo, AsyncStatus asyncStatus);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int Invoke(void* @this, void* asyncInfo, AsyncStatus asyncStatus)
	{
		IAsyncActionCompletedHandler asyncActionCompletedHandler = null;
		try
		{
			asyncActionCompletedHandler = (IAsyncActionCompletedHandler)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			asyncActionCompletedHandler.Invoke(MicroComRuntime.CreateProxyOrNullFor<IAsyncAction>(asyncInfo, ownsHandle: false), asyncStatus);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(asyncActionCompletedHandler, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIAsyncActionCompletedHandlerVTable()
	{
		AddMethod((delegate*<void*, void*, AsyncStatus, int>)(&Invoke));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IAsyncActionCompletedHandler), new __MicroComIAsyncActionCompletedHandlerVTable().CreateVTable());
	}
}
