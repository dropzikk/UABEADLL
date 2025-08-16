namespace Avalonia.Win32.DirectX;

internal struct DXGI_MAPPED_RECT
{
	public int Pitch;

	public unsafe byte* pBits;
}
