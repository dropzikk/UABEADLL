using Avalonia.Metadata;

namespace Avalonia.Metal;

[PrivateApi]
public interface IMetalPlatformSurface
{
	IMetalPlatformSurfaceRenderTarget CreateMetalRenderTarget(IMetalDevice device);
}
