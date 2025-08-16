using System;
using Avalonia.OpenGL.Egl;
using Avalonia.Platform;
using Avalonia.Win32.DirectX;

namespace Avalonia.Win32.WinRT.Composition;

internal class WinUiCompositedWindowSurface : IDirect3D11TexturePlatformSurface, IDisposable, IBlurHost
{
	private readonly WinUiCompositionShared _shared;

	private readonly EglGlPlatformSurface.IEglWindowGlPlatformSurfaceInfo _info;

	private WinUiCompositedWindow? _window;

	private BlurEffect _blurEffect;

	public WinUiCompositedWindowSurface(WinUiCompositionShared shared, EglGlPlatformSurface.IEglWindowGlPlatformSurfaceInfo info)
	{
		_shared = shared;
		_info = info;
	}

	public IDirect3D11TextureRenderTarget CreateRenderTarget(IPlatformGraphicsContext context, IntPtr d3dDevice)
	{
		float? backdropCornerRadius = AvaloniaLocator.Current.GetService<Win32PlatformOptions>()?.WinUICompositionBackdropCornerRadius;
		if (_window == null)
		{
			_window = new WinUiCompositedWindow(_info, _shared, backdropCornerRadius);
		}
		_window.SetBlur(_blurEffect);
		return new WinUiCompositedWindowRenderTarget(context, _window, d3dDevice, _shared.Compositor);
	}

	public void Dispose()
	{
		_window?.Dispose();
		_window = null;
	}

	public void SetBlur(BlurEffect enable)
	{
		_blurEffect = enable;
		_window?.SetBlur(enable);
	}
}
