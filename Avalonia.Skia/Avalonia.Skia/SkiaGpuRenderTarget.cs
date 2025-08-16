using System;
using Avalonia.Platform;

namespace Avalonia.Skia;

internal class SkiaGpuRenderTarget : IRenderTarget, IDisposable
{
	private readonly ISkiaGpu _skiaGpu;

	private readonly ISkiaGpuRenderTarget _renderTarget;

	public bool IsCorrupted => _renderTarget.IsCorrupted;

	public SkiaGpuRenderTarget(ISkiaGpu skiaGpu, ISkiaGpuRenderTarget renderTarget)
	{
		_skiaGpu = skiaGpu;
		_renderTarget = renderTarget;
	}

	public void Dispose()
	{
		_renderTarget.Dispose();
	}

	public IDrawingContextImpl CreateDrawingContext()
	{
		ISkiaGpuRenderSession skiaGpuRenderSession = _renderTarget.BeginRenderingSession();
		DrawingContextImpl.CreateInfo createInfo = default(DrawingContextImpl.CreateInfo);
		createInfo.GrContext = skiaGpuRenderSession.GrContext;
		createInfo.Surface = skiaGpuRenderSession.SkSurface;
		createInfo.Dpi = SkiaPlatform.DefaultDpi * skiaGpuRenderSession.ScaleFactor;
		createInfo.Gpu = _skiaGpu;
		createInfo.CurrentSession = skiaGpuRenderSession;
		return new DrawingContextImpl(createInfo, skiaGpuRenderSession);
	}
}
