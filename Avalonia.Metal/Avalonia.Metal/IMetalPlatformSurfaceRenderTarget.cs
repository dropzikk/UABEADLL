using System;
using Avalonia.Metadata;

namespace Avalonia.Metal;

[PrivateApi]
public interface IMetalPlatformSurfaceRenderTarget : IDisposable
{
	IMetalPlatformSurfaceRenderingSession BeginRendering();
}
