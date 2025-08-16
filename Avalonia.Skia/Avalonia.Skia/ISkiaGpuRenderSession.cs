using System;
using SkiaSharp;

namespace Avalonia.Skia;

public interface ISkiaGpuRenderSession : IDisposable
{
	GRContext GrContext { get; }

	SKSurface SkSurface { get; }

	double ScaleFactor { get; }

	GRSurfaceOrigin SurfaceOrigin { get; }
}
