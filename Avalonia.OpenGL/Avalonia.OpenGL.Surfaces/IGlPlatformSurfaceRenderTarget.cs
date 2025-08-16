using System;

namespace Avalonia.OpenGL.Surfaces;

public interface IGlPlatformSurfaceRenderTarget : IDisposable
{
	IGlPlatformSurfaceRenderingSession BeginDraw();
}
