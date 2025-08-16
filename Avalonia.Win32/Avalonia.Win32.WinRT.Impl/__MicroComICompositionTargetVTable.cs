using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositionTargetVTable : __MicroComIInspectableVTable
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetRootDelegate(void* @this, void** value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetRootDelegate(void* @this, void* value);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetRoot(void* @this, void** value)
	{
		ICompositionTarget compositionTarget = null;
		try
		{
			compositionTarget = (ICompositionTarget)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IVisual root = compositionTarget.Root;
			*value = MicroComRuntime.GetNativePointer(root, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionTarget, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetRoot(void* @this, void* value)
	{
		ICompositionTarget compositionTarget = null;
		try
		{
			compositionTarget = (ICompositionTarget)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			compositionTarget.SetRoot(MicroComRuntime.CreateProxyOrNullFor<IVisual>(value, ownsHandle: false));
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionTarget, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComICompositionTargetVTable()
	{
		AddMethod((delegate*<void*, void**, int>)(&GetRoot));
		AddMethod((delegate*<void*, void*, int>)(&SetRoot));
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(ICompositionTarget), new __MicroComICompositionTargetVTable().CreateVTable());
	}
}
