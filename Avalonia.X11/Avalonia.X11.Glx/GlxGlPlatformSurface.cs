using System;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Egl;
using Avalonia.OpenGL.Surfaces;

namespace Avalonia.X11.Glx;

internal class GlxGlPlatformSurface : IGlPlatformSurface
{
	private class RenderTarget : IGlPlatformSurfaceRenderTarget, IDisposable
	{
		private class Session : IGlPlatformSurfaceRenderingSession, IDisposable
		{
			private readonly GlxContext _context;

			private readonly EglGlPlatformSurface.IEglWindowGlPlatformSurfaceInfo _info;

			private readonly IDisposable _clearContext;

			public IGlContext Context => _context;

			public PixelSize Size => _info.Size;

			public double Scaling => _info.Scaling;

			public bool IsYFlipped { get; }

			public Session(GlxContext context, EglGlPlatformSurface.IEglWindowGlPlatformSurfaceInfo info, IDisposable clearContext)
			{
				_context = context;
				_info = info;
				_clearContext = clearContext;
			}

			public void Dispose()
			{
				_context.GlInterface.Flush();
				_context.Glx.WaitGL();
				_context.Display.SwapBuffers(_info.Handle);
				_context.Glx.WaitX();
				_clearContext.Dispose();
			}
		}

		private readonly GlxContext _context;

		private readonly EglGlPlatformSurface.IEglWindowGlPlatformSurfaceInfo _info;

		public RenderTarget(GlxContext context, EglGlPlatformSurface.IEglWindowGlPlatformSurfaceInfo info)
		{
			_context = context;
			_info = info;
		}

		public void Dispose()
		{
		}

		public IGlPlatformSurfaceRenderingSession BeginDraw()
		{
			IDisposable clearContext = _context.MakeCurrent(_info.Handle);
			_context.GlInterface.BindFramebuffer(36160, 0);
			return new Session(_context, _info, clearContext);
		}
	}

	private readonly EglGlPlatformSurface.IEglWindowGlPlatformSurfaceInfo _info;

	public GlxGlPlatformSurface(EglGlPlatformSurface.IEglWindowGlPlatformSurfaceInfo info)
	{
		_info = info;
	}

	public IGlPlatformSurfaceRenderTarget CreateGlRenderTarget(IGlContext context)
	{
		return new RenderTarget((GlxContext)context, _info);
	}
}
