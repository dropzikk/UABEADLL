using System;
using Avalonia.Metadata;

namespace Avalonia.Metal;

[PrivateApi]
public interface IMetalPlatformSurfaceRenderingSession : IDisposable
{
	IntPtr Texture { get; }

	PixelSize Size { get; }

	double Scaling { get; }

	bool IsYFlipped { get; }
}
