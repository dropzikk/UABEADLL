using System;
using Avalonia.Rendering;
using Avalonia.Rendering.Composition;

namespace Avalonia.OpenGL.Controls;

internal class CompositionOpenGlSwapchain : SwapchainBase<IGlSwapchainImage>
{
	private readonly IGlContext _context;

	private readonly IGlContextExternalObjectsFeature? _externalObjectsFeature;

	private readonly IOpenGlTextureSharingRenderInterfaceContextFeature? _sharingFeature;

	public CompositionOpenGlSwapchain(IGlContext context, ICompositionGpuInterop interop, CompositionDrawingSurface target, IOpenGlTextureSharingRenderInterfaceContextFeature sharingFeature)
		: base(interop, target)
	{
		_context = context;
		_sharingFeature = sharingFeature;
	}

	public CompositionOpenGlSwapchain(IGlContext context, ICompositionGpuInterop interop, CompositionDrawingSurface target, IGlContextExternalObjectsFeature? externalObjectsFeature)
		: base(interop, target)
	{
		_context = context;
		_externalObjectsFeature = externalObjectsFeature;
	}

	protected override IGlSwapchainImage CreateImage(PixelSize size)
	{
		if (_sharingFeature != null)
		{
			return new CompositionOpenGlSwapChainImage(_context, _sharingFeature, size, base.Interop, base.Target);
		}
		return new DxgiMutexOpenGlSwapChainImage(base.Interop, base.Target, _externalObjectsFeature, size);
	}

	public IDisposable BeginDraw(PixelSize size, out IGlTexture texture)
	{
		IGlSwapchainImage image;
		IDisposable result = BeginDrawCore(size, out image);
		texture = image;
		return result;
	}
}
