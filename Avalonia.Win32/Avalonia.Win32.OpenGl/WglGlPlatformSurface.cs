using System;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Egl;
using Avalonia.OpenGL.Surfaces;
using Avalonia.Win32.Interop;

namespace Avalonia.Win32.OpenGl;

internal class WglGlPlatformSurface : IGlPlatformSurface
{
	private class RenderTarget : IGlPlatformSurfaceRenderTarget, IDisposable
	{
		private class Session : IGlPlatformSurfaceRenderingSession, IDisposable
		{
			private readonly WglContext _context;

			private readonly IntPtr _hdc;

			private readonly EglGlPlatformSurface.IEglWindowGlPlatformSurfaceInfo _info;

			private readonly IDisposable _clearContext;

			public IGlContext Context => _context;

			public PixelSize Size => _info.Size;

			public double Scaling => _info.Scaling;

			public bool IsYFlipped { get; }

			public Session(WglContext context, IntPtr hdc, EglGlPlatformSurface.IEglWindowGlPlatformSurfaceInfo info, IDisposable clearContext)
			{
				_context = context;
				_hdc = hdc;
				_info = info;
				_clearContext = clearContext;
			}

			public void Dispose()
			{
				_context.GlInterface.Flush();
				UnmanagedMethods.SwapBuffers(_hdc);
				_clearContext.Dispose();
			}
		}

		private readonly WglContext _context;

		private readonly EglGlPlatformSurface.IEglWindowGlPlatformSurfaceInfo _info;

		private IntPtr _hdc;

		public RenderTarget(WglContext context, EglGlPlatformSurface.IEglWindowGlPlatformSurfaceInfo info)
		{
			_context = context;
			_info = info;
			_hdc = context.CreateConfiguredDeviceContext(info.Handle);
		}

		public void Dispose()
		{
			WglGdiResourceManager.ReleaseDC(_info.Handle, _hdc);
		}

		public IGlPlatformSurfaceRenderingSession BeginDraw()
		{
			IDisposable clearContext = _context.MakeCurrent(_hdc);
			_context.GlInterface.BindFramebuffer(36160, 0);
			return new Session(_context, _hdc, _info, clearContext);
		}
	}

	private readonly EglGlPlatformSurface.IEglWindowGlPlatformSurfaceInfo _info;

	public WglGlPlatformSurface(EglGlPlatformSurface.IEglWindowGlPlatformSurfaceInfo info)
	{
		_info = info;
	}

	public IGlPlatformSurfaceRenderTarget CreateGlRenderTarget(IGlContext context)
	{
		return new RenderTarget((WglContext)context, _info);
	}
}
