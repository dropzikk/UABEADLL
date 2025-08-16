using System;
using Avalonia.Platform;

namespace Avalonia.Win32.DirectX;

public interface IDirect3D11TexturePlatformSurface
{
	IDirect3D11TextureRenderTarget CreateRenderTarget(IPlatformGraphicsContext graphicsContext, IntPtr d3dDevice);
}
