using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.DirectX.Impl;

internal class __MicroComIDXGIFactory2VTable : __MicroComIDXGIFactory1VTable
{
	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int IsWindowedStereoEnabledDelegate(void* @this);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateSwapChainForHwndDelegate(void* @this, void* pDevice, IntPtr hWnd, DXGI_SWAP_CHAIN_DESC1* pDesc, DXGI_SWAP_CHAIN_FULLSCREEN_DESC* pFullscreenDesc, void* pRestrictToOutput, void** ppSwapChain);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateSwapChainForCoreWindowDelegate(void* @this, void* pDevice, void* pWindow, DXGI_SWAP_CHAIN_DESC1* pDesc, void* pRestrictToOutput, void** ppSwapChain);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int GetSharedResourceAdapterLuidDelegate(void* @this, IntPtr hResource, ulong* pLuid);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int RegisterStereoStatusWindowDelegate(void* @this, IntPtr WindowHandle, ushort wMsg, int* pdwCookie);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int RegisterStereoStatusEventDelegate(void* @this, IntPtr hEvent, int* pdwCookie);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void UnregisterStereoStatusDelegate(void* @this, int dwCookie);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int RegisterOcclusionStatusWindowDelegate(void* @this, IntPtr WindowHandle, ushort wMsg, int* pdwCookie);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int RegisterOcclusionStatusEventDelegate(void* @this, IntPtr hEvent, int* pdwCookie);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate void UnregisterOcclusionStatusDelegate(void* @this, int dwCookie);

	[UnmanagedFunctionPointer(CallingConvention.StdCall)]
	private unsafe delegate int CreateSwapChainForCompositionDelegate(void* @this, void* pDevice, DXGI_SWAP_CHAIN_DESC1* pDesc, void* pRestrictToOutput, void** ppSwapChain);

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int IsWindowedStereoEnabled(void* @this)
	{
		IDXGIFactory2 iDXGIFactory = null;
		try
		{
			iDXGIFactory = (IDXGIFactory2)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			return iDXGIFactory.IsWindowedStereoEnabled();
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGIFactory, e);
			return 0;
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateSwapChainForHwnd(void* @this, void* pDevice, IntPtr hWnd, DXGI_SWAP_CHAIN_DESC1* pDesc, DXGI_SWAP_CHAIN_FULLSCREEN_DESC* pFullscreenDesc, void* pRestrictToOutput, void** ppSwapChain)
	{
		IDXGIFactory2 iDXGIFactory = null;
		try
		{
			iDXGIFactory = (IDXGIFactory2)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IDXGISwapChain1 obj = iDXGIFactory.CreateSwapChainForHwnd(MicroComRuntime.CreateProxyOrNullFor<IUnknown>(pDevice, ownsHandle: false), hWnd, pDesc, pFullscreenDesc, MicroComRuntime.CreateProxyOrNullFor<IDXGIOutput>(pRestrictToOutput, ownsHandle: false));
			*ppSwapChain = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGIFactory, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateSwapChainForCoreWindow(void* @this, void* pDevice, void* pWindow, DXGI_SWAP_CHAIN_DESC1* pDesc, void* pRestrictToOutput, void** ppSwapChain)
	{
		IDXGIFactory2 iDXGIFactory = null;
		try
		{
			iDXGIFactory = (IDXGIFactory2)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IDXGISwapChain1 obj = iDXGIFactory.CreateSwapChainForCoreWindow(MicroComRuntime.CreateProxyOrNullFor<IUnknown>(pDevice, ownsHandle: false), MicroComRuntime.CreateProxyOrNullFor<IUnknown>(pWindow, ownsHandle: false), pDesc, MicroComRuntime.CreateProxyOrNullFor<IDXGIOutput>(pRestrictToOutput, ownsHandle: false));
			*ppSwapChain = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGIFactory, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int GetSharedResourceAdapterLuid(void* @this, IntPtr hResource, ulong* pLuid)
	{
		IDXGIFactory2 iDXGIFactory = null;
		try
		{
			iDXGIFactory = (IDXGIFactory2)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			iDXGIFactory.GetSharedResourceAdapterLuid(hResource, pLuid);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGIFactory, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int RegisterStereoStatusWindow(void* @this, IntPtr WindowHandle, ushort wMsg, int* pdwCookie)
	{
		IDXGIFactory2 iDXGIFactory = null;
		try
		{
			iDXGIFactory = (IDXGIFactory2)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			int num = iDXGIFactory.RegisterStereoStatusWindow(WindowHandle, wMsg);
			*pdwCookie = num;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGIFactory, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int RegisterStereoStatusEvent(void* @this, IntPtr hEvent, int* pdwCookie)
	{
		IDXGIFactory2 iDXGIFactory = null;
		try
		{
			iDXGIFactory = (IDXGIFactory2)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			int num = iDXGIFactory.RegisterStereoStatusEvent(hEvent);
			*pdwCookie = num;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGIFactory, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void UnregisterStereoStatus(void* @this, int dwCookie)
	{
		IDXGIFactory2 iDXGIFactory = null;
		try
		{
			iDXGIFactory = (IDXGIFactory2)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			iDXGIFactory.UnregisterStereoStatus(dwCookie);
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGIFactory, e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int RegisterOcclusionStatusWindow(void* @this, IntPtr WindowHandle, ushort wMsg, int* pdwCookie)
	{
		IDXGIFactory2 iDXGIFactory = null;
		try
		{
			iDXGIFactory = (IDXGIFactory2)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			int num = iDXGIFactory.RegisterOcclusionStatusWindow(WindowHandle, wMsg);
			*pdwCookie = num;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGIFactory, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int RegisterOcclusionStatusEvent(void* @this, IntPtr hEvent, int* pdwCookie)
	{
		IDXGIFactory2 iDXGIFactory = null;
		try
		{
			iDXGIFactory = (IDXGIFactory2)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			int num = iDXGIFactory.RegisterOcclusionStatusEvent(hEvent);
			*pdwCookie = num;
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGIFactory, e);
			return -2147467259;
		}
		return 0;
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static void UnregisterOcclusionStatus(void* @this, int dwCookie)
	{
		IDXGIFactory2 iDXGIFactory = null;
		try
		{
			iDXGIFactory = (IDXGIFactory2)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			iDXGIFactory.UnregisterOcclusionStatus(dwCookie);
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGIFactory, e);
		}
	}

	[UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvStdcall) })]
	private unsafe static int CreateSwapChainForComposition(void* @this, void* pDevice, DXGI_SWAP_CHAIN_DESC1* pDesc, void* pRestrictToOutput, void** ppSwapChain)
	{
		IDXGIFactory2 iDXGIFactory = null;
		try
		{
			iDXGIFactory = (IDXGIFactory2)MicroComRuntime.GetObjectFromCcw(new IntPtr(@this));
			IDXGISwapChain1 obj = iDXGIFactory.CreateSwapChainForComposition(MicroComRuntime.CreateProxyOrNullFor<IUnknown>(pDevice, ownsHandle: false), pDesc, MicroComRuntime.CreateProxyOrNullFor<IDXGIOutput>(pRestrictToOutput, ownsHandle: false));
			*ppSwapChain = MicroComRuntime.GetNativePointer(obj, owned: true);
		}
		catch (COMException ex)
		{
			return ex.ErrorCode;
		}
		catch (Exception e)
		{
			MicroComRuntime.UnhandledException(iDXGIFactory, e);
			return -2147467259;
		}
		return 0;
	}

	protected unsafe __MicroComIDXGIFactory2VTable()
	{
		AddMethod((delegate*<void*, int>)(&IsWindowedStereoEnabled));
		AddMethod((delegate*<void*, void*, IntPtr, DXGI_SWAP_CHAIN_DESC1*, DXGI_SWAP_CHAIN_FULLSCREEN_DESC*, void*, void**, int>)(&CreateSwapChainForHwnd));
		AddMethod((delegate*<void*, void*, void*, DXGI_SWAP_CHAIN_DESC1*, void*, void**, int>)(&CreateSwapChainForCoreWindow));
		AddMethod((delegate*<void*, IntPtr, ulong*, int>)(&GetSharedResourceAdapterLuid));
		AddMethod((delegate*<void*, IntPtr, ushort, int*, int>)(&RegisterStereoStatusWindow));
		AddMethod((delegate*<void*, IntPtr, int*, int>)(&RegisterStereoStatusEvent));
		AddMethod((delegate*<void*, int, void>)(&UnregisterStereoStatus));
		AddMethod((delegate*<void*, IntPtr, ushort, int*, int>)(&RegisterOcclusionStatusWindow));
		AddMethod((delegate*<void*, IntPtr, int*, int>)(&RegisterOcclusionStatusEvent));
		AddMethod((delegate*<void*, int, void>)(&UnregisterOcclusionStatus));
		AddMethod((delegate*<void*, void*, DXGI_SWAP_CHAIN_DESC1*, void*, void**, int>)(&CreateSwapChainForComposition));
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.RegisterVTable(typeof(IDXGIFactory2), new __MicroComIDXGIFactory2VTable().CreateVTable());
	}
}
