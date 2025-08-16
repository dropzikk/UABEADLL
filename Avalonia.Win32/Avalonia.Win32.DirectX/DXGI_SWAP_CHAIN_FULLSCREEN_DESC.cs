namespace Avalonia.Win32.DirectX;

internal struct DXGI_SWAP_CHAIN_FULLSCREEN_DESC
{
	public DXGI_RATIONAL RefreshRate;

	public DXGI_MODE_SCANLINE_ORDER ScanlineOrdering;

	public DXGI_MODE_SCALING Scaling;

	public int Windowed;
}
