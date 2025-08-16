using Avalonia.OpenGL.Surfaces;

namespace Avalonia.OpenGL.Egl;

public abstract class EglGlPlatformSurfaceBase : IGlPlatformSurface
{
	public abstract IGlPlatformSurfaceRenderTarget CreateGlRenderTarget(IGlContext context);
}
