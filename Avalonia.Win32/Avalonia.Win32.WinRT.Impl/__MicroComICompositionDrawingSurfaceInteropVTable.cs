using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Avalonia.Win32.Interop;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositionDrawingSurfaceInteropVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int BeginDrawDelegate(void* @this, UnmanagedMethods.RECT* updateRect, Guid* iid, void** updateObject, UnmanagedMethods.POINT* updateOffset);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int EndDrawDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int ResizeDelegate(void* @this, UnmanagedMethods.POINT sizePixels);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int ScrollDelegate(void* @this, UnmanagedMethods.RECT* scrollRect, UnmanagedMethods.RECT* clipRect, int offsetX, int offsetY);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int ResumeDrawDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int SuspendDrawDelegate(void* @this);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int BeginDraw(void* @this, UnmanagedMethods.RECT* updateRect, Guid* iid, void** updateObject, UnmanagedMethods.POINT* updateOffset)
	{
		ICompositionDrawingSurfaceInterop compositionDrawingSurfaceInterop = null;
		try
		{
			compositionDrawingSurfaceInterop = (ICompositionDrawingSurfaceInterop)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			UnmanagedMethods.POINT pOINT = compositionDrawingSurfaceInterop.BeginDraw(updateRect, iid, updateObject);
			*updateOffset = pOINT;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionDrawingSurfaceInterop, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int EndDraw(void* @this)
	{
		ICompositionDrawingSurfaceInterop compositionDrawingSurfaceInterop = null;
		try
		{
			compositionDrawingSurfaceInterop = (ICompositionDrawingSurfaceInterop)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			compositionDrawingSurfaceInterop.EndDraw();
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionDrawingSurfaceInterop, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int Resize(void* @this, UnmanagedMethods.POINT sizePixels)
	{
		ICompositionDrawingSurfaceInterop compositionDrawingSurfaceInterop = null;
		try
		{
			compositionDrawingSurfaceInterop = (ICompositionDrawingSurfaceInterop)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			compositionDrawingSurfaceInterop.Resize(sizePixels);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionDrawingSurfaceInterop, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int Scroll(void* @this, UnmanagedMethods.RECT* scrollRect, UnmanagedMethods.RECT* clipRect, int offsetX, int offsetY)
	{
		ICompositionDrawingSurfaceInterop compositionDrawingSurfaceInterop = null;
		try
		{
			compositionDrawingSurfaceInterop = (ICompositionDrawingSurfaceInterop)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			compositionDrawingSurfaceInterop.Scroll(scrollRect, clipRect, offsetX, offsetY);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionDrawingSurfaceInterop, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int ResumeDraw(void* @this)
	{
		ICompositionDrawingSurfaceInterop compositionDrawingSurfaceInterop = null;
		try
		{
			compositionDrawingSurfaceInterop = (ICompositionDrawingSurfaceInterop)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			compositionDrawingSurfaceInterop.ResumeDraw();
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionDrawingSurfaceInterop, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int SuspendDraw(void* @this)
	{
		ICompositionDrawingSurfaceInterop compositionDrawingSurfaceInterop = null;
		try
		{
			compositionDrawingSurfaceInterop = (ICompositionDrawingSurfaceInterop)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			compositionDrawingSurfaceInterop.SuspendDraw();
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionDrawingSurfaceInterop, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComICompositionDrawingSurfaceInteropVTable()
	{
		AddMethod((delegate*<void*, UnmanagedMethods.RECT*, Guid*, void**, UnmanagedMethods.POINT*, int>)(&BeginDraw));
		AddMethod((delegate*<void*, int>)(&EndDraw));
		AddMethod((delegate*<void*, UnmanagedMethods.POINT, int>)(&Resize));
		AddMethod((delegate*<void*, UnmanagedMethods.RECT*, UnmanagedMethods.RECT*, int, int, int>)(&Scroll));
		AddMethod((delegate*<void*, int>)(&ResumeDraw));
		AddMethod((delegate*<void*, int>)(&SuspendDraw));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(ICompositionDrawingSurfaceInterop), new __MicroComICompositionDrawingSurfaceInteropVTable().CreateVTable());
	}
}
