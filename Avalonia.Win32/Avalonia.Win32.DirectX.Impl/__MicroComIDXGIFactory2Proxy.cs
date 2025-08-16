using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.DirectX.Impl;

internal class __MicroComIDXGIFactory2Proxy : __MicroComIDXGIFactory1Proxy, IDXGIFactory2, IDXGIFactory1, IDXGIFactory, IDXGIObject, IUnknown, IDisposable
{
	protected override int VTableSize => base.VTableSize + 11;

	public unsafe int IsWindowedStereoEnabled()
	{
		return ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize])(base.PPV);
	}

	public unsafe IDXGISwapChain1 CreateSwapChainForHwnd(IUnknown pDevice, IntPtr hWnd, DXGI_SWAP_CHAIN_DESC1* pDesc, DXGI_SWAP_CHAIN_FULLSCREEN_DESC* pFullscreenDesc, IDXGIOutput pRestrictToOutput)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, IntPtr, void*, void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, MicroComRuntime.GetNativePointer(pDevice), hWnd, pDesc, pFullscreenDesc, MicroComRuntime.GetNativePointer(pRestrictToOutput), &pObject);
		if (num != 0)
		{
			throw new COMException("CreateSwapChainForHwnd failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IDXGISwapChain1>(pObject, ownsHandle: true);
	}

	public unsafe IDXGISwapChain1 CreateSwapChainForCoreWindow(IUnknown pDevice, IUnknown pWindow, DXGI_SWAP_CHAIN_DESC1* pDesc, IDXGIOutput pRestrictToOutput)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 2])(base.PPV, MicroComRuntime.GetNativePointer(pDevice), MicroComRuntime.GetNativePointer(pWindow), pDesc, MicroComRuntime.GetNativePointer(pRestrictToOutput), &pObject);
		if (num != 0)
		{
			throw new COMException("CreateSwapChainForCoreWindow failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IDXGISwapChain1>(pObject, ownsHandle: true);
	}

	public unsafe void GetSharedResourceAdapterLuid(IntPtr hResource, ulong* pLuid)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, IntPtr, void*, int>)(*base.PPV)[base.VTableSize + 3])(base.PPV, hResource, pLuid);
		if (num != 0)
		{
			throw new COMException("GetSharedResourceAdapterLuid failed", num);
		}
	}

	public unsafe int RegisterStereoStatusWindow(IntPtr WindowHandle, ushort wMsg)
	{
		int result = 0;
		int num = ((delegate* unmanaged[Stdcall]<void*, IntPtr, ushort, void*, int>)(*base.PPV)[base.VTableSize + 4])(base.PPV, WindowHandle, wMsg, &result);
		if (num != 0)
		{
			throw new COMException("RegisterStereoStatusWindow failed", num);
		}
		return result;
	}

	public unsafe int RegisterStereoStatusEvent(IntPtr hEvent)
	{
		int result = 0;
		int num = ((delegate* unmanaged[Stdcall]<void*, IntPtr, void*, int>)(*base.PPV)[base.VTableSize + 5])(base.PPV, hEvent, &result);
		if (num != 0)
		{
			throw new COMException("RegisterStereoStatusEvent failed", num);
		}
		return result;
	}

	public unsafe void UnregisterStereoStatus(int dwCookie)
	{
		((delegate* unmanaged[Stdcall]<void*, int, void>)(*base.PPV)[base.VTableSize + 6])(base.PPV, dwCookie);
	}

	public unsafe int RegisterOcclusionStatusWindow(IntPtr WindowHandle, ushort wMsg)
	{
		int result = 0;
		int num = ((delegate* unmanaged[Stdcall]<void*, IntPtr, ushort, void*, int>)(*base.PPV)[base.VTableSize + 7])(base.PPV, WindowHandle, wMsg, &result);
		if (num != 0)
		{
			throw new COMException("RegisterOcclusionStatusWindow failed", num);
		}
		return result;
	}

	public unsafe int RegisterOcclusionStatusEvent(IntPtr hEvent)
	{
		int result = 0;
		int num = ((delegate* unmanaged[Stdcall]<void*, IntPtr, void*, int>)(*base.PPV)[base.VTableSize + 8])(base.PPV, hEvent, &result);
		if (num != 0)
		{
			throw new COMException("RegisterOcclusionStatusEvent failed", num);
		}
		return result;
	}

	public unsafe void UnregisterOcclusionStatus(int dwCookie)
	{
		((delegate* unmanaged[Stdcall]<void*, int, void>)(*base.PPV)[base.VTableSize + 9])(base.PPV, dwCookie);
	}

	public unsafe IDXGISwapChain1 CreateSwapChainForComposition(IUnknown pDevice, DXGI_SWAP_CHAIN_DESC1* pDesc, IDXGIOutput pRestrictToOutput)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 10])(base.PPV, MicroComRuntime.GetNativePointer(pDevice), pDesc, MicroComRuntime.GetNativePointer(pRestrictToOutput), &pObject);
		if (num != 0)
		{
			throw new COMException("CreateSwapChainForComposition failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IDXGISwapChain1>(pObject, ownsHandle: true);
	}

	[ModuleInitializer]
	internal new static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(IDXGIFactory2), new Guid("50c83a1c-e072-4c48-87b0-3630fa36a6d0"), (IntPtr p, bool owns) => new __MicroComIDXGIFactory2Proxy(p, owns));
	}

	protected __MicroComIDXGIFactory2Proxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
