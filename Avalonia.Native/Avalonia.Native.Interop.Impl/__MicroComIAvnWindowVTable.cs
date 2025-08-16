using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Native.Interop.Impl;

internal class __MicroComIAvnWindowVTable : __MicroComIAvnWindowBaseVTable
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetEnabledDelegate(void* @this, int enable);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetParentDelegate(void* @this, void* parent);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetCanResizeDelegate(void* @this, int value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetDecorationsDelegate(void* @this, SystemDecorations value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetTitleDelegate(void* @this, byte* utf8Title);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetTitleBarColorDelegate(void* @this, AvnColor color);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetWindowStateDelegate(void* @this, AvnWindowState state);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetWindowStateDelegate(void* @this, AvnWindowState* ret);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int TakeFocusFromChildrenDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetExtendClientAreaDelegate(void* @this, int enable);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetExtendClientAreaHintsDelegate(void* @this, AvnExtendClientAreaChromeHints hints);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetExtendTitleBarHeightDelegate(void* @this, double* ret);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SetExtendTitleBarHeightDelegate(void* @this, double value);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetEnabled(void* @this, int enable)
	{
		IAvnWindow avnWindow = null;
		try
		{
			avnWindow = (IAvnWindow)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnWindow.SetEnabled(enable);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindow, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetParent(void* @this, void* parent)
	{
		IAvnWindow avnWindow = null;
		try
		{
			avnWindow = (IAvnWindow)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnWindow.SetParent(MicroComRuntime.CreateProxyOrNullFor<IAvnWindow>(parent, ownsHandle: false));
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindow, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetCanResize(void* @this, int value)
	{
		IAvnWindow avnWindow = null;
		try
		{
			avnWindow = (IAvnWindow)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnWindow.SetCanResize(value);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindow, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetDecorations(void* @this, SystemDecorations value)
	{
		IAvnWindow avnWindow = null;
		try
		{
			avnWindow = (IAvnWindow)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnWindow.SetDecorations(value);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindow, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetTitle(void* @this, byte* utf8Title)
	{
		IAvnWindow avnWindow = null;
		try
		{
			avnWindow = (IAvnWindow)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnWindow.SetTitle((utf8Title == null) ? null : Marshal.PtrToStringAnsi(new IntPtr(utf8Title)));
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindow, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetTitleBarColor(void* @this, AvnColor color)
	{
		IAvnWindow avnWindow = null;
		try
		{
			avnWindow = (IAvnWindow)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnWindow.SetTitleBarColor(color);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindow, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetWindowState(void* @this, AvnWindowState state)
	{
		IAvnWindow avnWindow = null;
		try
		{
			avnWindow = (IAvnWindow)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnWindow.SetWindowState(state);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindow, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetWindowState(void* @this, AvnWindowState* ret)
	{
		IAvnWindow avnWindow = null;
		try
		{
			avnWindow = (IAvnWindow)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			AvnWindowState windowState = avnWindow.WindowState;
			*ret = windowState;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindow, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int TakeFocusFromChildren(void* @this)
	{
		IAvnWindow avnWindow = null;
		try
		{
			avnWindow = (IAvnWindow)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnWindow.TakeFocusFromChildren();
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindow, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetExtendClientArea(void* @this, int enable)
	{
		IAvnWindow avnWindow = null;
		try
		{
			avnWindow = (IAvnWindow)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnWindow.SetExtendClientArea(enable);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindow, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetExtendClientAreaHints(void* @this, AvnExtendClientAreaChromeHints hints)
	{
		IAvnWindow avnWindow = null;
		try
		{
			avnWindow = (IAvnWindow)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnWindow.SetExtendClientAreaHints(hints);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindow, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetExtendTitleBarHeight(void* @this, double* ret)
	{
		IAvnWindow avnWindow = null;
		try
		{
			avnWindow = (IAvnWindow)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			double extendTitleBarHeight = avnWindow.ExtendTitleBarHeight;
			*ret = extendTitleBarHeight;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindow, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SetExtendTitleBarHeight(void* @this, double value)
	{
		IAvnWindow avnWindow = null;
		try
		{
			avnWindow = (IAvnWindow)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			avnWindow.SetExtendTitleBarHeight(value);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(avnWindow, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIAvnWindowVTable()
	{
		AddMethod((delegate*<void*, int, int>)(&SetEnabled));
		AddMethod((delegate*<void*, void*, int>)(&SetParent));
		AddMethod((delegate*<void*, int, int>)(&SetCanResize));
		AddMethod((delegate*<void*, SystemDecorations, int>)(&SetDecorations));
		AddMethod((delegate*<void*, byte*, int>)(&SetTitle));
		AddMethod((delegate*<void*, AvnColor, int>)(&SetTitleBarColor));
		AddMethod((delegate*<void*, AvnWindowState, int>)(&SetWindowState));
		AddMethod((delegate*<void*, AvnWindowState*, int>)(&GetWindowState));
		AddMethod((delegate*<void*, int>)(&TakeFocusFromChildren));
		AddMethod((delegate*<void*, int, int>)(&SetExtendClientArea));
		AddMethod((delegate*<void*, AvnExtendClientAreaChromeHints, int>)(&SetExtendClientAreaHints));
		AddMethod((delegate*<void*, double*, int>)(&GetExtendTitleBarHeight));
		AddMethod((delegate*<void*, double, int>)(&SetExtendTitleBarHeight));
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IAvnWindow), new __MicroComIAvnWindowVTable().CreateVTable());
	}
}
