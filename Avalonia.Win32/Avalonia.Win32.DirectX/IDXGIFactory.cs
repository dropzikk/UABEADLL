using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.DirectX;

internal interface IDXGIFactory : IDXGIObject, IUnknown, IDisposable
{
	IntPtr WindowAssociation { get; }

	unsafe int EnumAdapters(ushort Adapter, void* ppAdapter);

	void MakeWindowAssociation(IntPtr WindowHandle, ushort Flags);

	unsafe IDXGISwapChain CreateSwapChain(IUnknown pDevice, DXGI_SWAP_CHAIN_DESC* pDesc);

	unsafe IDXGIAdapter CreateSoftwareAdapter(void* Module);
}
