using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositionEffectSourceParameterFactoryVTable : __MicroComIInspectableVTable
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateDelegate(void* @this, IntPtr name, void** instance);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int Create(void* @this, IntPtr name, void** instance)
	{
		ICompositionEffectSourceParameterFactory compositionEffectSourceParameterFactory = null;
		try
		{
			compositionEffectSourceParameterFactory = (ICompositionEffectSourceParameterFactory)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			ICompositionEffectSourceParameter obj = compositionEffectSourceParameterFactory.Create(name);
			*instance = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionEffectSourceParameterFactory, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComICompositionEffectSourceParameterFactoryVTable()
	{
		AddMethod((delegate*<void*, IntPtr, void**, int>)(&Create));
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(ICompositionEffectSourceParameterFactory), new __MicroComICompositionEffectSourceParameterFactoryVTable().CreateVTable());
	}
}
