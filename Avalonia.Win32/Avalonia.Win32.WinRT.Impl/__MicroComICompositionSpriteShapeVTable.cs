using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositionSpriteShapeVTable : __MicroComIInspectableVTable
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetFillBrushDelegate(void* @this, void** value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetFillBrushDelegate(void* @this, void* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetGeometryDelegate(void* @this, void** value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetGeometryDelegate(void* @this, void* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetIsStrokeNonScalingDelegate(void* @this, int* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetIsStrokeNonScalingDelegate(void* @this, int value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetStrokeBrushDelegate(void* @this, void** value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetStrokeBrushDelegate(void* @this, void* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetStrokeDashArrayDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetStrokeDashCapDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetStrokeDashCapDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetStrokeDashOffsetDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetStrokeDashOffsetDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetStrokeEndCapDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetStrokeEndCapDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetStrokeLineJoinDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetStrokeLineJoinDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetStrokeMiterLimitDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetStrokeMiterLimitDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetStrokeStartCapDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetStrokeStartCapDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetStrokeThicknessDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetStrokeThicknessDelegate(void* @this);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetFillBrush(void* @this, void** value)
	{
		ICompositionSpriteShape compositionSpriteShape = null;
		try
		{
			compositionSpriteShape = (ICompositionSpriteShape)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			ICompositionBrush fillBrush = compositionSpriteShape.FillBrush;
			*value = MicroComRuntime.GetNativePointer(fillBrush, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionSpriteShape, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetFillBrush(void* @this, void* value)
	{
		ICompositionSpriteShape compositionSpriteShape = null;
		try
		{
			compositionSpriteShape = (ICompositionSpriteShape)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			compositionSpriteShape.SetFillBrush(MicroComRuntime.CreateProxyOrNullFor<ICompositionBrush>(value, ownsHandle: false));
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionSpriteShape, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetGeometry(void* @this, void** value)
	{
		ICompositionSpriteShape compositionSpriteShape = null;
		try
		{
			compositionSpriteShape = (ICompositionSpriteShape)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			ICompositionGeometry geometry = compositionSpriteShape.Geometry;
			*value = MicroComRuntime.GetNativePointer(geometry, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionSpriteShape, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetGeometry(void* @this, void* value)
	{
		ICompositionSpriteShape compositionSpriteShape = null;
		try
		{
			compositionSpriteShape = (ICompositionSpriteShape)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			compositionSpriteShape.SetGeometry(MicroComRuntime.CreateProxyOrNullFor<ICompositionGeometry>(value, ownsHandle: false));
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionSpriteShape, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetIsStrokeNonScaling(void* @this, int* value)
	{
		ICompositionSpriteShape compositionSpriteShape = null;
		try
		{
			compositionSpriteShape = (ICompositionSpriteShape)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			int isStrokeNonScaling = compositionSpriteShape.IsStrokeNonScaling;
			*value = isStrokeNonScaling;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionSpriteShape, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetIsStrokeNonScaling(void* @this, int value)
	{
		ICompositionSpriteShape compositionSpriteShape = null;
		try
		{
			compositionSpriteShape = (ICompositionSpriteShape)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			compositionSpriteShape.SetIsStrokeNonScaling(value);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionSpriteShape, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetStrokeBrush(void* @this, void** value)
	{
		ICompositionSpriteShape compositionSpriteShape = null;
		try
		{
			compositionSpriteShape = (ICompositionSpriteShape)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			ICompositionBrush strokeBrush = compositionSpriteShape.StrokeBrush;
			*value = MicroComRuntime.GetNativePointer(strokeBrush, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionSpriteShape, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetStrokeBrush(void* @this, void* value)
	{
		ICompositionSpriteShape compositionSpriteShape = null;
		try
		{
			compositionSpriteShape = (ICompositionSpriteShape)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			compositionSpriteShape.SetStrokeBrush(MicroComRuntime.CreateProxyOrNullFor<ICompositionBrush>(value, ownsHandle: false));
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionSpriteShape, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetStrokeDashArray(void* @this)
	{
		ICompositionSpriteShape compositionSpriteShape = null;
		try
		{
			compositionSpriteShape = (ICompositionSpriteShape)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			compositionSpriteShape.GetStrokeDashArray();
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionSpriteShape, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetStrokeDashCap(void* @this)
	{
		ICompositionSpriteShape compositionSpriteShape = null;
		try
		{
			compositionSpriteShape = (ICompositionSpriteShape)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			compositionSpriteShape.GetStrokeDashCap();
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionSpriteShape, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetStrokeDashCap(void* @this)
	{
		ICompositionSpriteShape compositionSpriteShape = null;
		try
		{
			compositionSpriteShape = (ICompositionSpriteShape)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			compositionSpriteShape.SetStrokeDashCap();
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionSpriteShape, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetStrokeDashOffset(void* @this)
	{
		ICompositionSpriteShape compositionSpriteShape = null;
		try
		{
			compositionSpriteShape = (ICompositionSpriteShape)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			compositionSpriteShape.GetStrokeDashOffset();
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionSpriteShape, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetStrokeDashOffset(void* @this)
	{
		ICompositionSpriteShape compositionSpriteShape = null;
		try
		{
			compositionSpriteShape = (ICompositionSpriteShape)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			compositionSpriteShape.SetStrokeDashOffset();
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionSpriteShape, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetStrokeEndCap(void* @this)
	{
		ICompositionSpriteShape compositionSpriteShape = null;
		try
		{
			compositionSpriteShape = (ICompositionSpriteShape)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			compositionSpriteShape.GetStrokeEndCap();
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionSpriteShape, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetStrokeEndCap(void* @this)
	{
		ICompositionSpriteShape compositionSpriteShape = null;
		try
		{
			compositionSpriteShape = (ICompositionSpriteShape)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			compositionSpriteShape.SetStrokeEndCap();
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionSpriteShape, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetStrokeLineJoin(void* @this)
	{
		ICompositionSpriteShape compositionSpriteShape = null;
		try
		{
			compositionSpriteShape = (ICompositionSpriteShape)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			compositionSpriteShape.GetStrokeLineJoin();
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionSpriteShape, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetStrokeLineJoin(void* @this)
	{
		ICompositionSpriteShape compositionSpriteShape = null;
		try
		{
			compositionSpriteShape = (ICompositionSpriteShape)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			compositionSpriteShape.SetStrokeLineJoin();
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionSpriteShape, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetStrokeMiterLimit(void* @this)
	{
		ICompositionSpriteShape compositionSpriteShape = null;
		try
		{
			compositionSpriteShape = (ICompositionSpriteShape)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			compositionSpriteShape.GetStrokeMiterLimit();
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionSpriteShape, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetStrokeMiterLimit(void* @this)
	{
		ICompositionSpriteShape compositionSpriteShape = null;
		try
		{
			compositionSpriteShape = (ICompositionSpriteShape)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			compositionSpriteShape.SetStrokeMiterLimit();
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionSpriteShape, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetStrokeStartCap(void* @this)
	{
		ICompositionSpriteShape compositionSpriteShape = null;
		try
		{
			compositionSpriteShape = (ICompositionSpriteShape)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			compositionSpriteShape.GetStrokeStartCap();
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionSpriteShape, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetStrokeStartCap(void* @this)
	{
		ICompositionSpriteShape compositionSpriteShape = null;
		try
		{
			compositionSpriteShape = (ICompositionSpriteShape)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			compositionSpriteShape.SetStrokeStartCap();
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionSpriteShape, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetStrokeThickness(void* @this)
	{
		ICompositionSpriteShape compositionSpriteShape = null;
		try
		{
			compositionSpriteShape = (ICompositionSpriteShape)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			compositionSpriteShape.GetStrokeThickness();
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionSpriteShape, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetStrokeThickness(void* @this)
	{
		ICompositionSpriteShape compositionSpriteShape = null;
		try
		{
			compositionSpriteShape = (ICompositionSpriteShape)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			compositionSpriteShape.SetStrokeThickness();
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionSpriteShape, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComICompositionSpriteShapeVTable()
	{
		AddMethod((delegate*<void*, void**, int>)(&GetFillBrush));
		AddMethod((delegate*<void*, void*, int>)(&SetFillBrush));
		AddMethod((delegate*<void*, void**, int>)(&GetGeometry));
		AddMethod((delegate*<void*, void*, int>)(&SetGeometry));
		AddMethod((delegate*<void*, int*, int>)(&GetIsStrokeNonScaling));
		AddMethod((delegate*<void*, int, int>)(&SetIsStrokeNonScaling));
		AddMethod((delegate*<void*, void**, int>)(&GetStrokeBrush));
		AddMethod((delegate*<void*, void*, int>)(&SetStrokeBrush));
		AddMethod((delegate*<void*, int>)(&GetStrokeDashArray));
		AddMethod((delegate*<void*, int>)(&GetStrokeDashCap));
		AddMethod((delegate*<void*, int>)(&SetStrokeDashCap));
		AddMethod((delegate*<void*, int>)(&GetStrokeDashOffset));
		AddMethod((delegate*<void*, int>)(&SetStrokeDashOffset));
		AddMethod((delegate*<void*, int>)(&GetStrokeEndCap));
		AddMethod((delegate*<void*, int>)(&SetStrokeEndCap));
		AddMethod((delegate*<void*, int>)(&GetStrokeLineJoin));
		AddMethod((delegate*<void*, int>)(&SetStrokeLineJoin));
		AddMethod((delegate*<void*, int>)(&GetStrokeMiterLimit));
		AddMethod((delegate*<void*, int>)(&SetStrokeMiterLimit));
		AddMethod((delegate*<void*, int>)(&GetStrokeStartCap));
		AddMethod((delegate*<void*, int>)(&SetStrokeStartCap));
		AddMethod((delegate*<void*, int>)(&GetStrokeThickness));
		AddMethod((delegate*<void*, int>)(&SetStrokeThickness));
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(ICompositionSpriteShape), new __MicroComICompositionSpriteShapeVTable().CreateVTable());
	}
}
