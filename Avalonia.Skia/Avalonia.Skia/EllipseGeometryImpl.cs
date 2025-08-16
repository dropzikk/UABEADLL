using SkiaSharp;

namespace Avalonia.Skia;

internal class EllipseGeometryImpl : GeometryImpl
{
	public override Rect Bounds { get; }

	public override SKPath StrokePath { get; }

	public override SKPath FillPath => StrokePath;

	public EllipseGeometryImpl(Rect rect)
	{
		SKPath sKPath = new SKPath();
		sKPath.AddOval(rect.ToSKRect());
		StrokePath = sKPath;
		Bounds = rect;
	}
}
