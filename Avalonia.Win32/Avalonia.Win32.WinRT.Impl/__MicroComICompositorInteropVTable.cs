using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Impl;

internal class __MicroComICompositorInteropVTable : MicroComVtblBase
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateCompositionSurfaceForHandleDelegate(void* @this, IntPtr swapChain, void** res);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateCompositionSurfaceForSwapChainDelegate(void* @this, void* swapChain, void** result);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateGraphicsDeviceDelegate(void* @this, void* renderingDevice, void** result);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateCompositionSurfaceForHandle(void* @this, IntPtr swapChain, void** res)
	{
		ICompositorInterop compositorInterop = null;
		try
		{
			compositorInterop = (ICompositorInterop)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			ICompositionSurface obj = compositorInterop.CreateCompositionSurfaceForHandle(swapChain);
			*res = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositorInterop, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateCompositionSurfaceForSwapChain(void* @this, void* swapChain, void** result)
	{
		ICompositorInterop compositorInterop = null;
		try
		{
			compositorInterop = (ICompositorInterop)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			ICompositionSurface obj = compositorInterop.CreateCompositionSurfaceForSwapChain(MicroComRuntime.CreateProxyOrNullFor<IUnknown>(swapChain, ownsHandle: false));
			*result = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositorInterop, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateGraphicsDevice(void* @this, void* renderingDevice, void** result)
	{
		ICompositorInterop compositorInterop = null;
		try
		{
			compositorInterop = (ICompositorInterop)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			ICompositionGraphicsDevice obj = compositorInterop.CreateGraphicsDevice(MicroComRuntime.CreateProxyOrNullFor<IUnknown>(renderingDevice, ownsHandle: false));
			*result = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(compositorInterop, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComICompositorInteropVTable()
	{
		AddMethod((delegate*<void*, IntPtr, void**, int>)(&CreateCompositionSurfaceForHandle));
		AddMethod((delegate*<void*, void*, void**, int>)(&CreateCompositionSurfaceForSwapChain));
		AddMethod((delegate*<void*, void*, void**, int>)(&CreateGraphicsDevice));
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(ICompositorInterop), new __MicroComICompositorInteropVTable().CreateVTable());
	}
}
