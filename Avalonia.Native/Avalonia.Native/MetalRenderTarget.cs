using System;
using Avalonia.Metal;
using Avalonia.Native.Interop;

namespace Avalonia.Native;

internal class MetalRenderTarget : IMetalPlatformSurfaceRenderTarget, IDisposable
{
	private IAvnMetalRenderTarget _native;

	public MetalRenderTarget(IAvnMetalRenderTarget native)
	{
		_native = native;
	}

	public void Dispose()
	{
		_native?.Dispose();
		_native = null;
	}

	public IMetalPlatformSurfaceRenderingSession BeginRendering()
	{
		return new MetalDrawingSession(_native.BeginDrawing());
	}
}
