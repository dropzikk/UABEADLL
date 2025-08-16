using System;
using Avalonia.Media;
using Avalonia.Metadata;

namespace Avalonia.Platform;

[Unstable]
public interface IDrawingContextImpl : IDisposable
{
	RenderOptions RenderOptions { get; set; }

	Matrix Transform { get; set; }

	void Clear(Color color);

	void DrawBitmap(IBitmapImpl source, double opacity, Rect sourceRect, Rect destRect);

	void DrawBitmap(IBitmapImpl source, IBrush opacityMask, Rect opacityMaskRect, Rect destRect);

	void DrawLine(IPen? pen, Point p1, Point p2);

	void DrawGeometry(IBrush? brush, IPen? pen, IGeometryImpl geometry);

	void DrawRectangle(IBrush? brush, IPen? pen, RoundedRect rect, BoxShadows boxShadows = default(BoxShadows));

	void DrawEllipse(IBrush? brush, IPen? pen, Rect rect);

	void DrawGlyphRun(IBrush? foreground, IGlyphRunImpl glyphRun);

	IDrawingContextLayerImpl CreateLayer(Size size);

	void PushClip(Rect clip);

	void PushClip(RoundedRect clip);

	void PopClip();

	void PushOpacity(double opacity, Rect? bounds);

	void PopOpacity();

	void PushOpacityMask(IBrush mask, Rect bounds);

	void PopOpacityMask();

	void PushGeometryClip(IGeometryImpl clip);

	void PopGeometryClip();

	object? GetFeature(Type t);
}
