using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComIVisual2VTable : __MicroComIInspectableVTable
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetParentForTransformDelegate(void* @this, void** value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetParentForTransformDelegate(void* @this, void* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetRelativeOffsetAdjustmentDelegate(void* @this, Vector3* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetRelativeOffsetAdjustmentDelegate(void* @this, Vector3 value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetRelativeSizeAdjustmentDelegate(void* @this, Vector2* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetRelativeSizeAdjustmentDelegate(void* @this, Vector2 value);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetParentForTransform(void* @this, void** value)
	{
		IVisual2 visual = null;
		try
		{
			visual = (IVisual2)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IVisual parentForTransform = visual.ParentForTransform;
			*value = MicroComRuntime.GetNativePointer(parentForTransform, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(visual, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetParentForTransform(void* @this, void* value)
	{
		IVisual2 visual = null;
		try
		{
			visual = (IVisual2)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			visual.SetParentForTransform(MicroComRuntime.CreateProxyOrNullFor<IVisual>(value, ownsHandle: false));
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(visual, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetRelativeOffsetAdjustment(void* @this, Vector3* value)
	{
		IVisual2 visual = null;
		try
		{
			visual = (IVisual2)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			Vector3 relativeOffsetAdjustment = visual.RelativeOffsetAdjustment;
			*value = relativeOffsetAdjustment;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(visual, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetRelativeOffsetAdjustment(void* @this, Vector3 value)
	{
		IVisual2 visual = null;
		try
		{
			visual = (IVisual2)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			visual.SetRelativeOffsetAdjustment(value);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(visual, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetRelativeSizeAdjustment(void* @this, Vector2* value)
	{
		IVisual2 visual = null;
		try
		{
			visual = (IVisual2)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			Vector2 relativeSizeAdjustment = visual.RelativeSizeAdjustment;
			*value = relativeSizeAdjustment;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(visual, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetRelativeSizeAdjustment(void* @this, Vector2 value)
	{
		IVisual2 visual = null;
		try
		{
			visual = (IVisual2)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			visual.SetRelativeSizeAdjustment(value);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(visual, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIVisual2VTable()
	{
		AddMethod((delegate*<void*, void**, int>)(&GetParentForTransform));
		AddMethod((delegate*<void*, void*, int>)(&SetParentForTransform));
		AddMethod((delegate*<void*, Vector3*, int>)(&GetRelativeOffsetAdjustment));
		AddMethod((delegate*<void*, Vector3, int>)(&SetRelativeOffsetAdjustment));
		AddMethod((delegate*<void*, Vector2*, int>)(&GetRelativeSizeAdjustment));
		AddMethod((delegate*<void*, Vector2, int>)(&SetRelativeSizeAdjustment));
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IVisual2), new __MicroComIVisual2VTable().CreateVTable());
	}
}
