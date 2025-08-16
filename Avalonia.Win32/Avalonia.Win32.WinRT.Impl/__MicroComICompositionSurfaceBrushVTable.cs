using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositionSurfaceBrushVTable : __MicroComIInspectableVTable
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetBitmapInterpolationModeDelegate(void* @this, CompositionBitmapInterpolationMode* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetBitmapInterpolationModeDelegate(void* @this, CompositionBitmapInterpolationMode value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetHorizontalAlignmentRatioDelegate(void* @this, float* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetHorizontalAlignmentRatioDelegate(void* @this, float value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetStretchDelegate(void* @this, CompositionStretch* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetStretchDelegate(void* @this, CompositionStretch value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetSurfaceDelegate(void* @this, void** value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetSurfaceDelegate(void* @this, void* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetVerticalAlignmentRatioDelegate(void* @this, float* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetVerticalAlignmentRatioDelegate(void* @this, float value);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetBitmapInterpolationMode(void* @this, CompositionBitmapInterpolationMode* value)
	{
		ICompositionSurfaceBrush compositionSurfaceBrush = null;
		try
		{
			compositionSurfaceBrush = (ICompositionSurfaceBrush)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			CompositionBitmapInterpolationMode bitmapInterpolationMode = compositionSurfaceBrush.BitmapInterpolationMode;
			*value = bitmapInterpolationMode;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionSurfaceBrush, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetBitmapInterpolationMode(void* @this, CompositionBitmapInterpolationMode value)
	{
		ICompositionSurfaceBrush compositionSurfaceBrush = null;
		try
		{
			compositionSurfaceBrush = (ICompositionSurfaceBrush)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			compositionSurfaceBrush.SetBitmapInterpolationMode(value);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionSurfaceBrush, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetHorizontalAlignmentRatio(void* @this, float* value)
	{
		ICompositionSurfaceBrush compositionSurfaceBrush = null;
		try
		{
			compositionSurfaceBrush = (ICompositionSurfaceBrush)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			float horizontalAlignmentRatio = compositionSurfaceBrush.HorizontalAlignmentRatio;
			*value = horizontalAlignmentRatio;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionSurfaceBrush, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetHorizontalAlignmentRatio(void* @this, float value)
	{
		ICompositionSurfaceBrush compositionSurfaceBrush = null;
		try
		{
			compositionSurfaceBrush = (ICompositionSurfaceBrush)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			compositionSurfaceBrush.SetHorizontalAlignmentRatio(value);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionSurfaceBrush, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetStretch(void* @this, CompositionStretch* value)
	{
		ICompositionSurfaceBrush compositionSurfaceBrush = null;
		try
		{
			compositionSurfaceBrush = (ICompositionSurfaceBrush)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			CompositionStretch stretch = compositionSurfaceBrush.Stretch;
			*value = stretch;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionSurfaceBrush, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetStretch(void* @this, CompositionStretch value)
	{
		ICompositionSurfaceBrush compositionSurfaceBrush = null;
		try
		{
			compositionSurfaceBrush = (ICompositionSurfaceBrush)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			compositionSurfaceBrush.SetStretch(value);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionSurfaceBrush, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetSurface(void* @this, void** value)
	{
		ICompositionSurfaceBrush compositionSurfaceBrush = null;
		try
		{
			compositionSurfaceBrush = (ICompositionSurfaceBrush)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			ICompositionSurface surface = compositionSurfaceBrush.Surface;
			*value = MicroComRuntime.GetNativePointer(surface, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionSurfaceBrush, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetSurface(void* @this, void* value)
	{
		ICompositionSurfaceBrush compositionSurfaceBrush = null;
		try
		{
			compositionSurfaceBrush = (ICompositionSurfaceBrush)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			compositionSurfaceBrush.SetSurface(MicroComRuntime.CreateProxyOrNullFor<ICompositionSurface>(value, ownsHandle: false));
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionSurfaceBrush, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetVerticalAlignmentRatio(void* @this, float* value)
	{
		ICompositionSurfaceBrush compositionSurfaceBrush = null;
		try
		{
			compositionSurfaceBrush = (ICompositionSurfaceBrush)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			float verticalAlignmentRatio = compositionSurfaceBrush.VerticalAlignmentRatio;
			*value = verticalAlignmentRatio;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionSurfaceBrush, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetVerticalAlignmentRatio(void* @this, float value)
	{
		ICompositionSurfaceBrush compositionSurfaceBrush = null;
		try
		{
			compositionSurfaceBrush = (ICompositionSurfaceBrush)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			compositionSurfaceBrush.SetVerticalAlignmentRatio(value);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionSurfaceBrush, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComICompositionSurfaceBrushVTable()
	{
		AddMethod((delegate*<void*, CompositionBitmapInterpolationMode*, int>)(&GetBitmapInterpolationMode));
		AddMethod((delegate*<void*, CompositionBitmapInterpolationMode, int>)(&SetBitmapInterpolationMode));
		AddMethod((delegate*<void*, float*, int>)(&GetHorizontalAlignmentRatio));
		AddMethod((delegate*<void*, float, int>)(&SetHorizontalAlignmentRatio));
		AddMethod((delegate*<void*, CompositionStretch*, int>)(&GetStretch));
		AddMethod((delegate*<void*, CompositionStretch, int>)(&SetStretch));
		AddMethod((delegate*<void*, void**, int>)(&GetSurface));
		AddMethod((delegate*<void*, void*, int>)(&SetSurface));
		AddMethod((delegate*<void*, float*, int>)(&GetVerticalAlignmentRatio));
		AddMethod((delegate*<void*, float, int>)(&SetVerticalAlignmentRatio));
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(ICompositionSurfaceBrush), new __MicroComICompositionSurfaceBrushVTable().CreateVTable());
	}
}
