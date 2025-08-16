using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.DirectX;

internal interface IDXGISwapChain1 : IDXGISwapChain, IDXGIDeviceSubObject, IDXGIObject, IUnknown, IDisposable
{
	DXGI_SWAP_CHAIN_DESC1 Desc1 { get; }

	DXGI_SWAP_CHAIN_FULLSCREEN_DESC FullscreenDesc { get; }

	IntPtr Hwnd { get; }

	IDXGIOutput RestrictToOutput { get; }

	DXGI_RGBA BackgroundColor { get; }

	DXGI_MODE_ROTATION Rotation { get; }

	unsafe void* GetCoreWindow(Guid* refiid);

	unsafe void Present1(ushort SyncInterval, ushort PresentFlags, DXGI_PRESENT_PARAMETERS* pPresentParameters);

	int IsTemporaryMonoSupported();

	unsafe void SetBackgroundColor(DXGI_RGBA* pColor);

	void SetRotation(DXGI_MODE_ROTATION Rotation);
}
