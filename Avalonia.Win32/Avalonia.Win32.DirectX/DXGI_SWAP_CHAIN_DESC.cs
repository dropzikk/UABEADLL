using System;

namespace Avalonia.Win32.DirectX;

internal struct DXGI_SWAP_CHAIN_DESC
{
	public DXGI_MODE_DESC BufferDesc;

	public DXGI_SAMPLE_DESC SampleDesc;

	public uint BufferUsage;

	public ushort BufferCount;

	public IntPtr OutputWindow;

	public int Windowed;

	public DXGI_SWAP_EFFECT SwapEffect;

	public ushort Flags;
}
