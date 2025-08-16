using Avalonia.Platform;
using SkiaSharp;

namespace Avalonia.Skia.Helpers;

public static class DrawingContextHelper
{
	public static IDrawingContextImpl WrapSkiaCanvas(SKCanvas canvas, Vector dpi)
	{
		DrawingContextImpl.CreateInfo createInfo = default(DrawingContextImpl.CreateInfo);
		createInfo.Canvas = canvas;
		createInfo.Dpi = dpi;
		createInfo.DisableSubpixelTextRendering = true;
		return new DrawingContextImpl(createInfo);
	}
}
