using System;
using Avalonia.Native.Interop;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Surfaces;

namespace Avalonia.Native;

internal class GlPlatformSurfaceRenderTarget : IGlPlatformSurfaceRenderTarget, IDisposable
{
	private IAvnGlSurfaceRenderTarget _target;

	private readonly IGlContext _context;

	public GlPlatformSurfaceRenderTarget(IAvnGlSurfaceRenderTarget target, IGlContext context)
	{
		_target = target;
		_context = context;
	}

	public IGlPlatformSurfaceRenderingSession BeginDraw()
	{
		return new GlPlatformSurfaceRenderingSession(_context, _target.BeginDrawing());
	}

	public void Dispose()
	{
		_target?.Dispose();
		_target = null;
	}
}
