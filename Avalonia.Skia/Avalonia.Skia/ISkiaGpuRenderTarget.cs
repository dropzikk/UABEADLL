using System;

namespace Avalonia.Skia;

public interface ISkiaGpuRenderTarget : IDisposable
{
	bool IsCorrupted { get; }

	ISkiaGpuRenderSession BeginRenderingSession();
}
