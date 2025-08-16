using System;

namespace Avalonia.Win32.DirectX;

public interface IDirect3D11TextureRenderTarget : IDisposable
{
	bool IsCorrupted { get; }

	IDirect3D11TextureRenderTargetRenderSession BeginDraw();
}
