using System;
using Avalonia.Metadata;

namespace Avalonia.Platform;

[PrivateApi]
public interface IRenderTarget : IDisposable
{
	bool IsCorrupted { get; }

	IDrawingContextImpl CreateDrawingContext();
}
