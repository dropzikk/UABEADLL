using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositionEffectSourceParameterVTable : __MicroComIInspectableVTable
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetNameDelegate(void* @this, IntPtr* value);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetName(void* @this, IntPtr* value)
	{
		ICompositionEffectSourceParameter compositionEffectSourceParameter = null;
		try
		{
			compositionEffectSourceParameter = (ICompositionEffectSourceParameter)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IntPtr name = compositionEffectSourceParameter.Name;
			*value = name;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionEffectSourceParameter, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComICompositionEffectSourceParameterVTable()
	{
		AddMethod((delegate*<void*, IntPtr*, int>)(&GetName));
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(ICompositionEffectSourceParameter), new __MicroComICompositionEffectSourceParameterVTable().CreateVTable());
	}
}
