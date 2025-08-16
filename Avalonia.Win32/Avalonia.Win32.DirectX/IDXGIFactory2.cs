using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.DirectX;

internal interface IDXGIFactory2 : IDXGIFactory1, IDXGIFactory, IDXGIObject, IUnknown, IDisposable
{
	int IsWindowedStereoEnabled();

	unsafe IDXGISwapChain1 CreateSwapChainForHwnd(IUnknown pDevice, IntPtr hWnd, DXGI_SWAP_CHAIN_DESC1* pDesc, DXGI_SWAP_CHAIN_FULLSCREEN_DESC* pFullscreenDesc, IDXGIOutput pRestrictToOutput);

	unsafe IDXGISwapChain1 CreateSwapChainForCoreWindow(IUnknown pDevice, IUnknown pWindow, DXGI_SWAP_CHAIN_DESC1* pDesc, IDXGIOutput pRestrictToOutput);

	unsafe void GetSharedResourceAdapterLuid(IntPtr hResource, ulong* pLuid);

	int RegisterStereoStatusWindow(IntPtr WindowHandle, ushort wMsg);

	int RegisterStereoStatusEvent(IntPtr hEvent);

	void UnregisterStereoStatus(int dwCookie);

	int RegisterOcclusionStatusWindow(IntPtr WindowHandle, ushort wMsg);

	int RegisterOcclusionStatusEvent(IntPtr hEvent);

	void UnregisterOcclusionStatus(int dwCookie);

	unsafe IDXGISwapChain1 CreateSwapChainForComposition(IUnknown pDevice, DXGI_SWAP_CHAIN_DESC1* pDesc, IDXGIOutput pRestrictToOutput);
}
