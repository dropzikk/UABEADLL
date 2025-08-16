using System;
using Avalonia.Controls.Platform.Surfaces;
using Avalonia.Platform;
using SkiaSharp;

namespace Avalonia.Skia;

internal class RenderTargetBitmapImpl : WriteableBitmapImpl, IRenderTargetBitmapImpl, IBitmapImpl, IDisposable, IRenderTarget, IFramebufferPlatformSurface
{
	private readonly FramebufferRenderTarget _renderTarget;

	public bool IsCorrupted => false;

	public RenderTargetBitmapImpl(PixelSize size, Vector dpi)
		: base(size, dpi, (SKImageInfo.PlatformColorType == SKColorType.Rgba8888) ? PixelFormats.Rgba8888 : PixelFormat.Bgra8888, AlphaFormat.Premul)
	{
		_renderTarget = new FramebufferRenderTarget(this);
	}

	public IDrawingContextImpl CreateDrawingContext()
	{
		return _renderTarget.CreateDrawingContext();
	}

	public override void Dispose()
	{
		_renderTarget.Dispose();
		base.Dispose();
	}

	public IFramebufferRenderTarget CreateFramebufferRenderTarget()
	{
		return new FuncFramebufferRenderTarget(Lock);
	}
}
