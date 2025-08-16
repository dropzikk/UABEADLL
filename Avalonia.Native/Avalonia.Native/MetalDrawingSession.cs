using System;
using Avalonia.Metal;
using Avalonia.Native.Interop;

namespace Avalonia.Native;

internal class MetalDrawingSession : IMetalPlatformSurfaceRenderingSession, IDisposable
{
	private IAvnMetalRenderingSession _session;

	public IntPtr Texture => _session.Texture;

	public PixelSize Size
	{
		get
		{
			AvnPixelSize pixelSize = _session.PixelSize;
			return new PixelSize(pixelSize.Width, pixelSize.Height);
		}
	}

	public double Scaling => _session.Scaling;

	public bool IsYFlipped => false;

	public MetalDrawingSession(IAvnMetalRenderingSession session)
	{
		_session = session;
	}

	public void Dispose()
	{
		_session?.Dispose();
		_session = null;
	}
}
