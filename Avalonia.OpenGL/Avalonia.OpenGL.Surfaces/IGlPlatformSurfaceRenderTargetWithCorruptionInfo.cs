using System;

namespace Avalonia.OpenGL.Surfaces;

public interface IGlPlatformSurfaceRenderTargetWithCorruptionInfo : IGlPlatformSurfaceRenderTarget, IDisposable
{
	bool IsCorrupted { get; }
}
