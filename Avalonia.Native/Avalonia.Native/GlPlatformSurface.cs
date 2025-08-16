using Avalonia.Native.Interop;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Surfaces;
using Avalonia.Threading;

namespace Avalonia.Native;

internal class GlPlatformSurface : IGlPlatformSurface
{
	private readonly IAvnWindowBase _window;

	public GlPlatformSurface(IAvnWindowBase window)
	{
		_window = window;
	}

	public IGlPlatformSurfaceRenderTarget CreateGlRenderTarget(IGlContext context)
	{
		if (!Dispatcher.UIThread.CheckAccess())
		{
			throw new RenderTargetNotReadyException();
		}
		GlContext glContext = (GlContext)context;
		return new GlPlatformSurfaceRenderTarget(_window.CreateGlRenderTarget(glContext.Context), glContext);
	}
}
