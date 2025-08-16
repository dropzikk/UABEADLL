using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.DirectX;

internal interface IDXGISwapChain : IDXGIDeviceSubObject, IDXGIObject, IUnknown, IDisposable
{
	DXGI_SWAP_CHAIN_DESC Desc { get; }

	IDXGIOutput ContainingOutput { get; }

	DXGI_FRAME_STATISTICS FrameStatistics { get; }

	ushort LastPresentCount { get; }

	void Present(ushort SyncInterval, ushort Flags);

	unsafe void* GetBuffer(ushort Buffer, Guid* riid);

	void SetFullscreenState(int Fullscreen, IDXGIOutput pTarget);

	unsafe IDXGIOutput GetFullscreenState(int* pFullscreen);

	void ResizeBuffers(ushort BufferCount, ushort Width, ushort Height, DXGI_FORMAT NewFormat, ushort SwapChainFlags);

	unsafe void ResizeTarget(DXGI_MODE_DESC* pNewTargetParameters);
}
