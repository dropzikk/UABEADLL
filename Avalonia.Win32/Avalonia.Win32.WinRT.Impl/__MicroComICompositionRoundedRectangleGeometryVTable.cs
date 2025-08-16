using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositionRoundedRectangleGeometryVTable : __MicroComIInspectableVTable
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetCornerRadiusDelegate(void* @this, Vector2* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetCornerRadiusDelegate(void* @this, Vector2 value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetOffsetDelegate(void* @this, Vector2* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetOffsetDelegate(void* @this, Vector2 value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetSizeDelegate(void* @this, Vector2* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetSizeDelegate(void* @this, Vector2 value);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetCornerRadius(void* @this, Vector2* value)
	{
		ICompositionRoundedRectangleGeometry compositionRoundedRectangleGeometry = null;
		try
		{
			compositionRoundedRectangleGeometry = (ICompositionRoundedRectangleGeometry)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			Vector2 cornerRadius = compositionRoundedRectangleGeometry.CornerRadius;
			*value = cornerRadius;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionRoundedRectangleGeometry, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetCornerRadius(void* @this, Vector2 value)
	{
		ICompositionRoundedRectangleGeometry compositionRoundedRectangleGeometry = null;
		try
		{
			compositionRoundedRectangleGeometry = (ICompositionRoundedRectangleGeometry)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			compositionRoundedRectangleGeometry.SetCornerRadius(value);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionRoundedRectangleGeometry, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetOffset(void* @this, Vector2* value)
	{
		ICompositionRoundedRectangleGeometry compositionRoundedRectangleGeometry = null;
		try
		{
			compositionRoundedRectangleGeometry = (ICompositionRoundedRectangleGeometry)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			Vector2 offset = compositionRoundedRectangleGeometry.Offset;
			*value = offset;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionRoundedRectangleGeometry, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetOffset(void* @this, Vector2 value)
	{
		ICompositionRoundedRectangleGeometry compositionRoundedRectangleGeometry = null;
		try
		{
			compositionRoundedRectangleGeometry = (ICompositionRoundedRectangleGeometry)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			compositionRoundedRectangleGeometry.SetOffset(value);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionRoundedRectangleGeometry, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetSize(void* @this, Vector2* value)
	{
		ICompositionRoundedRectangleGeometry compositionRoundedRectangleGeometry = null;
		try
		{
			compositionRoundedRectangleGeometry = (ICompositionRoundedRectangleGeometry)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			Vector2 size = compositionRoundedRectangleGeometry.Size;
			*value = size;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionRoundedRectangleGeometry, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetSize(void* @this, Vector2 value)
	{
		ICompositionRoundedRectangleGeometry compositionRoundedRectangleGeometry = null;
		try
		{
			compositionRoundedRectangleGeometry = (ICompositionRoundedRectangleGeometry)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			compositionRoundedRectangleGeometry.SetSize(value);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionRoundedRectangleGeometry, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComICompositionRoundedRectangleGeometryVTable()
	{
		AddMethod((delegate*<void*, Vector2*, int>)(&GetCornerRadius));
		AddMethod((delegate*<void*, Vector2, int>)(&SetCornerRadius));
		AddMethod((delegate*<void*, Vector2*, int>)(&GetOffset));
		AddMethod((delegate*<void*, Vector2, int>)(&SetOffset));
		AddMethod((delegate*<void*, Vector2*, int>)(&GetSize));
		AddMethod((delegate*<void*, Vector2, int>)(&SetSize));
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(ICompositionRoundedRectangleGeometry), new __MicroComICompositionRoundedRectangleGeometryVTable().CreateVTable());
	}
}
