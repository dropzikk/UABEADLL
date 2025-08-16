using Avalonia.Platform;
using SkiaSharp;

namespace Avalonia.Skia;

internal class TransformedGeometryImpl : GeometryImpl, ITransformedGeometryImpl, IGeometryImpl
{
	public override SKPath? StrokePath { get; }

	public override SKPath? FillPath { get; }

	public IGeometryImpl SourceGeometry { get; }

	public Matrix Transform { get; }

	public override Rect Bounds { get; }

	public TransformedGeometryImpl(GeometryImpl source, Matrix transform)
	{
		SourceGeometry = source;
		Transform = transform;
		SKMatrix matrix = transform.ToSKMatrix();
		SKPath sKPath = (StrokePath = source.StrokePath.Clone());
		sKPath?.Transform(matrix);
		Bounds = sKPath?.TightBounds.ToAvaloniaRect() ?? default(Rect);
		if (source.StrokePath == source.FillPath)
		{
			FillPath = sKPath;
		}
		else if (source.FillPath != null)
		{
			(FillPath = source.FillPath.Clone()).Transform(matrix);
		}
	}
}
