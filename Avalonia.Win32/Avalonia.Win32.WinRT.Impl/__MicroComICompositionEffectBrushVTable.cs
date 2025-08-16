using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositionEffectBrushVTable : __MicroComIInspectableVTable
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetSourceParameterDelegate(void* @this, IntPtr name, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetSourceParameterDelegate(void* @this, IntPtr name, void* source);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetSourceParameter(void* @this, IntPtr name, void** result)
	{
		ICompositionEffectBrush compositionEffectBrush = null;
		try
		{
			compositionEffectBrush = (ICompositionEffectBrush)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			ICompositionBrush sourceParameter = compositionEffectBrush.GetSourceParameter(name);
			*result = MicroComRuntime.GetNativePointer(sourceParameter, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionEffectBrush, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetSourceParameter(void* @this, IntPtr name, void* source)
	{
		ICompositionEffectBrush compositionEffectBrush = null;
		try
		{
			compositionEffectBrush = (ICompositionEffectBrush)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			compositionEffectBrush.SetSourceParameter(name, MicroComRuntime.CreateProxyOrNullFor<ICompositionBrush>(source, ownsHandle: false));
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionEffectBrush, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComICompositionEffectBrushVTable()
	{
		AddMethod((delegate*<void*, IntPtr, void**, int>)(&GetSourceParameter));
		AddMethod((delegate*<void*, IntPtr, void*, int>)(&SetSourceParameter));
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(ICompositionEffectBrush), new __MicroComICompositionEffectBrushVTable().CreateVTable());
	}
}
