using System;
using Avalonia.OpenGL.Surfaces;

namespace Avalonia.OpenGL.Egl;

public abstract class EglPlatformSurfaceRenderTargetBase : IGlPlatformSurfaceRenderTargetWithCorruptionInfo, IGlPlatformSurfaceRenderTarget, IDisposable
{
	private class Session : IGlPlatformSurfaceRenderingSession, IDisposable
	{
		private readonly EglContext _context;

		private readonly EglSurface _glSurface;

		private readonly EglDisplay _display;

		private readonly IDisposable _restoreContext;

		private readonly Action? _onFinish;

		public IGlContext Context => _context;

		public PixelSize Size { get; }

		public double Scaling { get; }

		public bool IsYFlipped { get; }

		public Session(EglDisplay display, EglContext context, EglSurface glSurface, PixelSize size, double scaling, IDisposable restoreContext, Action? onFinish, bool isYFlipped)
		{
			Size = size;
			Scaling = scaling;
			IsYFlipped = isYFlipped;
			_context = context;
			_display = display;
			_glSurface = glSurface;
			_restoreContext = restoreContext;
			_onFinish = onFinish;
		}

		public void Dispose()
		{
			_context.GlInterface.Flush();
			_display.EglInterface.WaitGL();
			_glSurface.SwapBuffers();
			_display.EglInterface.WaitClient();
			_display.EglInterface.WaitGL();
			_display.EglInterface.WaitNative(12379);
			_restoreContext.Dispose();
			_onFinish?.Invoke();
		}
	}

	protected EglContext Context { get; }

	public virtual bool IsCorrupted => Context.IsLost;

	protected EglPlatformSurfaceRenderTargetBase(EglContext context)
	{
		Context = context;
	}

	public virtual void Dispose()
	{
	}

	public IGlPlatformSurfaceRenderingSession BeginDraw()
	{
		if (Context.IsLost)
		{
			throw new RenderTargetCorruptedException();
		}
		return BeginDrawCore();
	}

	public abstract IGlPlatformSurfaceRenderingSession BeginDrawCore();

	protected IGlPlatformSurfaceRenderingSession BeginDraw(EglSurface surface, PixelSize size, double scaling, Action? onFinish = null, bool isYFlipped = false)
	{
		IDisposable disposable = Context.MakeCurrent(surface);
		bool flag = false;
		try
		{
			EglInterface eglInterface = Context.Display.EglInterface;
			eglInterface.WaitClient();
			eglInterface.WaitGL();
			eglInterface.WaitNative(12379);
			Context.GlInterface.BindFramebuffer(36160, 0);
			flag = true;
			return new Session(Context.Display, Context, surface, size, scaling, disposable, onFinish, isYFlipped);
		}
		finally
		{
			if (!flag)
			{
				disposable.Dispose();
			}
		}
	}
}
