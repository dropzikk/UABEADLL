using SkiaSharp;

namespace Avalonia.Skia;

internal class RectangleGeometryImpl : GeometryImpl
{
	public override Rect Bounds { get; }

	public override SKPath StrokePath { get; }

	public override SKPath? FillPath => StrokePath;

	public RectangleGeometryImpl(Rect rect)
	{
		SKPath sKPath = new SKPath();
		sKPath.AddRect(rect.ToSKRect());
		StrokePath = sKPath;
		Bounds = rect;
	}
}
