using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositionGeometryVTable : __MicroComIInspectableVTable
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetTrimEndDelegate(void* @this, float* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetTrimEndDelegate(void* @this, float value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetTrimOffsetDelegate(void* @this, float* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetTrimOffsetDelegate(void* @this, float value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetTrimStartDelegate(void* @this, float* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetTrimStartDelegate(void* @this, float value);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetTrimEnd(void* @this, float* value)
	{
		ICompositionGeometry compositionGeometry = null;
		try
		{
			compositionGeometry = (ICompositionGeometry)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			float trimEnd = compositionGeometry.TrimEnd;
			*value = trimEnd;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionGeometry, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetTrimEnd(void* @this, float value)
	{
		ICompositionGeometry compositionGeometry = null;
		try
		{
			compositionGeometry = (ICompositionGeometry)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			compositionGeometry.SetTrimEnd(value);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionGeometry, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetTrimOffset(void* @this, float* value)
	{
		ICompositionGeometry compositionGeometry = null;
		try
		{
			compositionGeometry = (ICompositionGeometry)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			float trimOffset = compositionGeometry.TrimOffset;
			*value = trimOffset;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionGeometry, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetTrimOffset(void* @this, float value)
	{
		ICompositionGeometry compositionGeometry = null;
		try
		{
			compositionGeometry = (ICompositionGeometry)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			compositionGeometry.SetTrimOffset(value);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionGeometry, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetTrimStart(void* @this, float* value)
	{
		ICompositionGeometry compositionGeometry = null;
		try
		{
			compositionGeometry = (ICompositionGeometry)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			float trimStart = compositionGeometry.TrimStart;
			*value = trimStart;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionGeometry, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetTrimStart(void* @this, float value)
	{
		ICompositionGeometry compositionGeometry = null;
		try
		{
			compositionGeometry = (ICompositionGeometry)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			compositionGeometry.SetTrimStart(value);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionGeometry, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComICompositionGeometryVTable()
	{
		AddMethod((delegate*<void*, float*, int>)(&GetTrimEnd));
		AddMethod((delegate*<void*, float, int>)(&SetTrimEnd));
		AddMethod((delegate*<void*, float*, int>)(&GetTrimOffset));
		AddMethod((delegate*<void*, float, int>)(&SetTrimOffset));
		AddMethod((delegate*<void*, float*, int>)(&GetTrimStart));
		AddMethod((delegate*<void*, float, int>)(&SetTrimStart));
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(ICompositionGeometry), new __MicroComICompositionGeometryVTable().CreateVTable());
	}
}
