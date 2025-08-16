namespace Avalonia.Win32.DirectX;

internal struct D3D11_TEXTURE2D_DESC
{
	public uint Width;

	public uint Height;

	public uint MipLevels;

	public uint ArraySize;

	public DXGI_FORMAT Format;

	public DXGI_SAMPLE_DESC SampleDesc;

	public D3D11_USAGE Usage;

	public D3D11_BIND_FLAG BindFlags;

	public uint CPUAccessFlags;

	public D3D11_RESOURCE_MISC_FLAG MiscFlags;
}
