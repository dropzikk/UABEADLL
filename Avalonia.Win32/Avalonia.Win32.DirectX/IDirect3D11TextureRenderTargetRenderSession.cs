using System;

namespace Avalonia.Win32.DirectX;

public interface IDirect3D11TextureRenderTargetRenderSession : IDisposable
{
	IntPtr D3D11Texture2D { get; }

	PixelSize Size { get; }

	PixelPoint Offset { get; }

	double Scaling { get; }
}
