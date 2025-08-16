using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Avalonia.Win32.Interop;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositionGraphicsDeviceVTable : __MicroComIInspectableVTable
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateDrawingSurfaceDelegate(void* @this, UnmanagedMethods.SIZE_F sizePixels, DirectXPixelFormat pixelFormat, DirectXAlphaMode alphaMode, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int AddRenderingDeviceReplacedDelegate(void* @this, void* handler, void* token);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int RemoveRenderingDeviceReplacedDelegate(void* @this, int token);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateDrawingSurface(void* @this, UnmanagedMethods.SIZE_F sizePixels, DirectXPixelFormat pixelFormat, DirectXAlphaMode alphaMode, void** result)
	{
		ICompositionGraphicsDevice compositionGraphicsDevice = null;
		try
		{
			compositionGraphicsDevice = (ICompositionGraphicsDevice)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			ICompositionDrawingSurface obj = compositionGraphicsDevice.CreateDrawingSurface(sizePixels, pixelFormat, alphaMode);
			*result = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionGraphicsDevice, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int AddRenderingDeviceReplaced(void* @this, void* handler, void* token)
	{
		ICompositionGraphicsDevice compositionGraphicsDevice = null;
		try
		{
			compositionGraphicsDevice = (ICompositionGraphicsDevice)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			compositionGraphicsDevice.AddRenderingDeviceReplaced(handler, token);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionGraphicsDevice, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int RemoveRenderingDeviceReplaced(void* @this, int token)
	{
		ICompositionGraphicsDevice compositionGraphicsDevice = null;
		try
		{
			compositionGraphicsDevice = (ICompositionGraphicsDevice)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			compositionGraphicsDevice.RemoveRenderingDeviceReplaced(token);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositionGraphicsDevice, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComICompositionGraphicsDeviceVTable()
	{
		AddMethod((delegate*<void*, UnmanagedMethods.SIZE_F, DirectXPixelFormat, DirectXAlphaMode, void**, int>)(&CreateDrawingSurface));
		AddMethod((delegate*<void*, void*, void*, int>)(&AddRenderingDeviceReplaced));
		AddMethod((delegate*<void*, int, int>)(&RemoveRenderingDeviceReplaced));
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(ICompositionGraphicsDevice), new __MicroComICompositionGraphicsDeviceVTable().CreateVTable());
	}
}
