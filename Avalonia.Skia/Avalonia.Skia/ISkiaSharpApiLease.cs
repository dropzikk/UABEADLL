using System;
using Avalonia.Metadata;
using SkiaSharp;

namespace Avalonia.Skia;

[Unstable]
public interface ISkiaSharpApiLease : IDisposable
{
	SKCanvas SkCanvas { get; }

	GRContext? GrContext { get; }

	SKSurface? SkSurface { get; }

	double CurrentOpacity { get; }
}
