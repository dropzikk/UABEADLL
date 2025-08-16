namespace Avalonia.Win32.DirectX;

internal struct DXGI_SWAP_CHAIN_DESC1
{
	public uint Width;

	public uint Height;

	public DXGI_FORMAT Format;

	public int Stereo;

	public DXGI_SAMPLE_DESC SampleDesc;

	public uint BufferUsage;

	public uint BufferCount;

	public DXGI_SCALING Scaling;

	public DXGI_SWAP_EFFECT SwapEffect;

	public DXGI_ALPHA_MODE AlphaMode;

	public uint Flags;
}
