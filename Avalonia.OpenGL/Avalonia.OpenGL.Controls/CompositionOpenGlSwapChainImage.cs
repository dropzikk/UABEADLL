using System;
using System.Threading.Tasks;
using Avalonia.Rendering;
using Avalonia.Rendering.Composition;

namespace Avalonia.OpenGL.Controls;

internal class CompositionOpenGlSwapChainImage : IGlSwapchainImage, ISwapchainImage, IAsyncDisposable, IGlTexture
{
	private readonly ICompositionGpuInterop _interop;

	private readonly CompositionDrawingSurface _target;

	private readonly ICompositionImportableOpenGlSharedTexture _texture;

	private ICompositionImportedGpuImage? _imported;

	public int TextureId => _texture.TextureId;

	public int InternalFormat => _texture.InternalFormat;

	public PixelSize Size => _texture.Size;

	public Task? LastPresent { get; private set; }

	public CompositionOpenGlSwapChainImage(IGlContext context, IOpenGlTextureSharingRenderInterfaceContextFeature sharingFeature, PixelSize size, ICompositionGpuInterop interop, CompositionDrawingSurface target)
	{
		_interop = interop;
		_target = target;
		_texture = sharingFeature.CreateSharedTextureForComposition(context, size);
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
	}

	public void Present()
	{
		if (_imported == null)
		{
			_imported = _interop.ImportImage(_texture);
		}
		LastPresent = _target.UpdateAsync(_imported);
	}
}
