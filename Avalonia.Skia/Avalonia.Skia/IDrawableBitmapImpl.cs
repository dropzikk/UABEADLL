using System;
using Avalonia.Platform;
using SkiaSharp;

namespace Avalonia.Skia;

internal interface IDrawableBitmapImpl : IBitmapImpl, IDisposable
{
	void Draw(DrawingContextImpl context, SKRect sourceRect, SKRect destRect, SKPaint paint);
}
