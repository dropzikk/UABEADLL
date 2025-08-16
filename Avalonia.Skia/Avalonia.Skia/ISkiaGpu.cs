using System;
using System.Collections.Generic;
using Avalonia.Platform;

namespace Avalonia.Skia;

public interface ISkiaGpu : IPlatformGraphicsContext, IDisposable, IOptionalFeatureProvider
{
	ISkiaGpuRenderTarget? TryCreateRenderTarget(IEnumerable<object> surfaces);

	ISkiaSurface? TryCreateSurface(PixelSize size, ISkiaGpuRenderSession? session);
}
