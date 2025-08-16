using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositionScopedBatchVTable : __MicroComIInspectableVTable
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetIsActiveDelegate(void* @this, int* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetIsEndedDelegate(void* @this, int* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int EndDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int ResumeDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SuspendDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int AddCompletedDelegate(void* @this, void* handler, int* token);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int RemoveCompletedDelegate(void* @this, int token);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetIsActive(void* @this, int* value)
	{
		ICompositionScopedBatch compositionScopedBatch = null;
		try
		{
			compositionScopedBatch = (ICompositionScopedBatch)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			int isActive = compositionScopedBatch.IsActive;
			*value = isActive;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionScopedBatch, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetIsEnded(void* @this, int* value)
	{
		ICompositionScopedBatch compositionScopedBatch = null;
		try
		{
			compositionScopedBatch = (ICompositionScopedBatch)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			int isEnded = compositionScopedBatch.IsEnded;
			*value = isEnded;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionScopedBatch, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int End(void* @this)
	{
		ICompositionScopedBatch compositionScopedBatch = null;
		try
		{
			compositionScopedBatch = (ICompositionScopedBatch)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			compositionScopedBatch.End();
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionScopedBatch, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int Resume(void* @this)
	{
		ICompositionScopedBatch compositionScopedBatch = null;
		try
		{
			compositionScopedBatch = (ICompositionScopedBatch)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			compositionScopedBatch.Resume();
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionScopedBatch, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int Suspend(void* @this)
	{
		ICompositionScopedBatch compositionScopedBatch = null;
		try
		{
			compositionScopedBatch = (ICompositionScopedBatch)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			compositionScopedBatch.Suspend();
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionScopedBatch, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int AddCompleted(void* @this, void* handler, int* token)
	{
		ICompositionScopedBatch compositionScopedBatch = null;
		try
		{
			compositionScopedBatch = (ICompositionScopedBatch)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			int num = compositionScopedBatch.AddCompleted(handler);
			*token = num;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionScopedBatch, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int RemoveCompleted(void* @this, int token)
	{
		ICompositionScopedBatch compositionScopedBatch = null;
		try
		{
			compositionScopedBatch = (ICompositionScopedBatch)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			compositionScopedBatch.RemoveCompleted(token);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionScopedBatch, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComICompositionScopedBatchVTable()
	{
		AddMethod((delegate*<void*, int*, int>)(&GetIsActive));
		AddMethod((delegate*<void*, int*, int>)(&GetIsEnded));
		AddMethod((delegate*<void*, int>)(&End));
		AddMethod((delegate*<void*, int>)(&Resume));
		AddMethod((delegate*<void*, int>)(&Suspend));
		AddMethod((delegate*<void*, void*, int*, int>)(&AddCompleted));
		AddMethod((delegate*<void*, int, int>)(&RemoveCompleted));
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(ICompositionScopedBatch), new __MicroComICompositionScopedBatchVTable().CreateVTable());
	}
}
