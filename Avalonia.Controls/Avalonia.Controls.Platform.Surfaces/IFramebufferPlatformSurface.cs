using Avalonia.Metadata;

namespace Avalonia.Controls.Platform.Surfaces;

[Unstable]
public interface IFramebufferPlatformSurface
{
	IFramebufferRenderTarget CreateFramebufferRenderTarget();
}
