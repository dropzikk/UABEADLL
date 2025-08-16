using System;
using SkiaSharp;

namespace Avalonia.Skia;

public interface ISkiaSurface : IDisposable
{
	SKSurface Surface { get; }

	bool CanBlit { get; }

	void Blit(SKCanvas canvas);
}
