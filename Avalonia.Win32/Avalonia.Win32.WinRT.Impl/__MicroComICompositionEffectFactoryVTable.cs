using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositionEffectFactoryVTable : __MicroComIInspectableVTable
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateBrushDelegate(void* @this, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetExtendedErrorDelegate(void* @this, int* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetLoadStatusDelegate(void* @this, CompositionEffectFactoryLoadStatus* value);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateBrush(void* @this, void** result)
	{
		ICompositionEffectFactory compositionEffectFactory = null;
		try
		{
			compositionEffectFactory = (ICompositionEffectFactory)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			ICompositionEffectBrush obj = compositionEffectFactory.CreateBrush();
			*result = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionEffectFactory, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetExtendedError(void* @this, int* value)
	{
		ICompositionEffectFactory compositionEffectFactory = null;
		try
		{
			compositionEffectFactory = (ICompositionEffectFactory)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			int extendedError = compositionEffectFactory.ExtendedError;
			*value = extendedError;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionEffectFactory, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetLoadStatus(void* @this, CompositionEffectFactoryLoadStatus* value)
	{
		ICompositionEffectFactory compositionEffectFactory = null;
		try
		{
			compositionEffectFactory = (ICompositionEffectFactory)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			CompositionEffectFactoryLoadStatus loadStatus = compositionEffectFactory.LoadStatus;
			*value = loadStatus;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionEffectFactory, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComICompositionEffectFactoryVTable()
	{
		AddMethod((delegate*<void*, void**, int>)(&CreateBrush));
		AddMethod((delegate*<void*, int*, int>)(&GetExtendedError));
		AddMethod((delegate*<void*, CompositionEffectFactoryLoadStatus*, int>)(&GetLoadStatus));
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(ICompositionEffectFactory), new __MicroComICompositionEffectFactoryVTable().CreateVTable());
	}
}
