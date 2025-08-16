using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Avalonia.Win32.Interop;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositionDrawingSurfaceVTable : __MicroComIInspectableVTable
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetAlphaModeDelegate(void* @this, DirectXAlphaMode* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetPixelFormatDelegate(void* @this, DirectXPixelFormat* value);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetSizeDelegate(void* @this, UnmanagedMethods.SIZE_F* value);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetAlphaMode(void* @this, DirectXAlphaMode* value)
	{
		ICompositionDrawingSurface compositionDrawingSurface = null;
		try
		{
			compositionDrawingSurface = (ICompositionDrawingSurface)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			DirectXAlphaMode alphaMode = compositionDrawingSurface.AlphaMode;
			*value = alphaMode;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionDrawingSurface, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetPixelFormat(void* @this, DirectXPixelFormat* value)
	{
		ICompositionDrawingSurface compositionDrawingSurface = null;
		try
		{
			compositionDrawingSurface = (ICompositionDrawingSurface)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			DirectXPixelFormat pixelFormat = compositionDrawingSurface.PixelFormat;
			*value = pixelFormat;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionDrawingSurface, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetSize(void* @this, UnmanagedMethods.SIZE_F* value)
	{
		ICompositionDrawingSurface compositionDrawingSurface = null;
		try
		{
			compositionDrawingSurface = (ICompositionDrawingSurface)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			UnmanagedMethods.SIZE_F size = compositionDrawingSurface.Size;
			*value = size;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionDrawingSurface, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComICompositionDrawingSurfaceVTable()
	{
		AddMethod((delegate*<void*, DirectXAlphaMode*, int>)(&GetAlphaMode));
		AddMethod((delegate*<void*, DirectXPixelFormat*, int>)(&GetPixelFormat));
		AddMethod((delegate*<void*, UnmanagedMethods.SIZE_F*, int>)(&GetSize));
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(ICompositionDrawingSurface), new __MicroComICompositionDrawingSurfaceVTable().CreateVTable());
	}
}
