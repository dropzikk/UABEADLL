using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComIAsyncActionVTable : __MicroComIInspectableVTable
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetCompletedDelegate(void* @this, void* handler);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetCompletedDelegate(void* @this, void** ppv);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetResultsDelegate(void* @this);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetCompleted(void* @this, void* handler)
	{
		IAsyncAction asyncAction = null;
		try
		{
			asyncAction = (IAsyncAction)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			asyncAction.SetCompleted(MicroComRuntime.CreateProxyOrNullFor<IAsyncActionCompletedHandler>(handler, ownsHandle: false));
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(asyncAction, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetCompleted(void* @this, void** ppv)
	{
		IAsyncAction asyncAction = null;
		try
		{
			asyncAction = (IAsyncAction)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IAsyncActionCompletedHandler completed = asyncAction.Completed;
			*ppv = MicroComRuntime.GetNativePointer(completed, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(asyncAction, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetResults(void* @this)
	{
		IAsyncAction asyncAction = null;
		try
		{
			asyncAction = (IAsyncAction)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			asyncAction.GetResults();
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(asyncAction, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIAsyncActionVTable()
	{
		AddMethod((delegate*<void*, void*, int>)(&SetCompleted));
		AddMethod((delegate*<void*, void**, int>)(&GetCompleted));
		AddMethod((delegate*<void*, int>)(&GetResults));
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IAsyncAction), new __MicroComIAsyncActionVTable().CreateVTable());
	}
}
