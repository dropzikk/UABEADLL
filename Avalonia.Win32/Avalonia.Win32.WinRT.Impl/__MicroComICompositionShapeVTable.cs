using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositionShapeVTable : __MicroComIInspectableVTable
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetCenterPointDelegate(void* @this, Vector2* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetCenterPointDelegate(void* @this, Vector2 value);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetCenterPoint(void* @this, Vector2* value)
	{
		ICompositionShape compositionShape = null;
		try
		{
			compositionShape = (ICompositionShape)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			Vector2 centerPoint = compositionShape.CenterPoint;
			*value = centerPoint;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionShape, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetCenterPoint(void* @this, Vector2 value)
	{
		ICompositionShape compositionShape = null;
		try
		{
			compositionShape = (ICompositionShape)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			compositionShape.SetCenterPoint(value);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionShape, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComICompositionShapeVTable()
	{
		AddMethod((delegate*<void*, Vector2*, int>)(&GetCenterPoint));
		AddMethod((delegate*<void*, Vector2, int>)(&SetCenterPoint));
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(ICompositionShape), new __MicroComICompositionShapeVTable().CreateVTable());
	}
}
