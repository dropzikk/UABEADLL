using System;
using System.Threading.Tasks;
using Avalonia.Platform;
using Avalonia.Rendering;
using Avalonia.Rendering.Composition;

namespace Avalonia.OpenGL.Controls;

internal class DxgiMutexOpenGlSwapChainImage : IGlSwapchainImage, ISwapchainImage, IAsyncDisposable, IGlTexture
{
	private readonly ICompositionGpuInterop _interop;

	private readonly CompositionDrawingSurface _surface;

	private readonly IGlExportableExternalImageTexture _texture;

	private Task? _lastPresent;

	private ICompositionImportedGpuImage? _imported;

	public int TextureId => _texture.TextureId;

	public int InternalFormat => _texture.InternalFormat;

	public PixelSize Size => new PixelSize(_texture.Properties.Width, _texture.Properties.Height);

	public Task? LastPresent => _lastPresent;

	public DxgiMutexOpenGlSwapChainImage(ICompositionGpuInterop interop, CompositionDrawingSurface surface, IGlContextExternalObjectsFeature externalObjects, PixelSize size)
	{
		_interop = interop;
		_surface = surface;
		_texture = externalObjects.CreateImage("D3D11TextureGlobalSharedHandle", size, PlatformGraphicsExternalImageFormat.R8G8B8A8UNorm);
	}

	public async ValueTask DisposeAsync()
	{
		if (_imported != null)
		{
			try
			{
				await _imported.DisposeAsync();
			}
			catch
			{
			}
		}
		_texture.Dispose();
	}

	public void BeginDraw()
	{
		_texture.AcquireKeyedMutex(0u);
	}

	public void Present()
	{
		_texture.ReleaseKeyedMutex(1u);
		if (_imported == null)
		{
			_imported = _interop.ImportImage(_texture.GetHandle(), _texture.Properties);
		}
		_lastPresent = _surface.UpdateWithKeyedMutexAsync(_imported, 1u, 0u);
	}
}
