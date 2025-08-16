using System;
using Avalonia.Native.Interop;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Surfaces;

namespace Avalonia.Native;

internal class GlPlatformSurfaceRenderingSession : IGlPlatformSurfaceRenderingSession, IDisposable
{
	private IAvnGlSurfaceRenderingSession _session;

	public IGlContext Context { get; }

	public PixelSize Size
	{
		get
		{
			AvnPixelSize pixelSize = _session.PixelSize;
			return new PixelSize(pixelSize.Width, pixelSize.Height);
		}
	}

	public double Scaling => _session.Scaling;

	public bool IsYFlipped => true;

	public GlPlatformSurfaceRenderingSession(IGlContext context, IAvnGlSurfaceRenderingSession session)
	{
		Context = context;
		_session = session;
	}

	public void Dispose()
	{
		_session?.Dispose();
		_session = null;
	}
}
