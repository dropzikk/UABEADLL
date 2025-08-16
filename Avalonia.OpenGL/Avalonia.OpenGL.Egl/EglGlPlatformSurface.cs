using System;
using Avalonia.OpenGL.Surfaces;

namespace Avalonia.OpenGL.Egl;

public class EglGlPlatformSurface : EglGlPlatformSurfaceBase
{
	public interface IEglWindowGlPlatformSurfaceInfo
	{
		IntPtr Handle { get; }

		PixelSize Size { get; }

		double Scaling { get; }
	}

	private class RenderTarget : EglPlatformSurfaceRenderTargetBase
	{
		private EglSurface? _glSurface;

		private readonly IEglWindowGlPlatformSurfaceInfo _info;

		private PixelSize _currentSize;

		private readonly IntPtr _handle;

		public RenderTarget(EglSurface glSurface, EglContext context, IEglWindowGlPlatformSurfaceInfo info)
			: base(context)
		{
			_glSurface = glSurface;
			_info = info;
			_currentSize = info.Size;
			_handle = _info.Handle;
		}

		public override void Dispose()
		{
			_glSurface?.Dispose();
		}

		public override IGlPlatformSurfaceRenderingSession BeginDrawCore()
		{
			if (_info.Size != _currentSize || _handle != _info.Handle || _glSurface == null)
			{
				_glSurface?.Dispose();
				_glSurface = null;
				_glSurface = base.Context.Display.CreateWindowSurface(_info.Handle);
				_currentSize = _info.Size;
			}
			return BeginDraw(_glSurface, _info.Size, _info.Scaling);
		}
	}

	private readonly IEglWindowGlPlatformSurfaceInfo _info;

	public EglGlPlatformSurface(IEglWindowGlPlatformSurfaceInfo info)
	{
		_info = info;
	}

	public override IGlPlatformSurfaceRenderTarget CreateGlRenderTarget(IGlContext context)
	{
		EglContext eglContext = (EglContext)context;
		return new RenderTarget(eglContext.Display.CreateWindowSurface(_info.Handle), eglContext, _info);
	}
}
