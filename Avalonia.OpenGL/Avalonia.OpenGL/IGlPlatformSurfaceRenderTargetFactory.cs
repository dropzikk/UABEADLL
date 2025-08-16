using Avalonia.OpenGL.Surfaces;

namespace Avalonia.OpenGL;

public interface IGlPlatformSurfaceRenderTargetFactory
{
	bool CanRenderToSurface(IGlContext context, object surface);

	IGlPlatformSurfaceRenderTarget CreateRenderTarget(IGlContext context, object surface);
}
