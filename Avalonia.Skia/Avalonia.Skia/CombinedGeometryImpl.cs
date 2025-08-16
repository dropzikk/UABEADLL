using Avalonia.Media;
using Avalonia.Platform;
using SkiaSharp;

namespace Avalonia.Skia;

internal class CombinedGeometryImpl : GeometryImpl
{
	public override Rect Bounds { get; }

	public override SKPath? StrokePath { get; }

	public override SKPath? FillPath { get; }

	public CombinedGeometryImpl(SKPath? stroke, SKPath? fill)
	{
		StrokePath = stroke;
		FillPath = fill;
		Bounds = (stroke ?? fill)?.TightBounds.ToAvaloniaRect() ?? default(Rect);
	}

	public static CombinedGeometryImpl ForceCreate(GeometryCombineMode combineMode, IGeometryImpl g1, IGeometryImpl g2)
	{
		if (g1 is GeometryImpl g3 && g2 is GeometryImpl g4)
		{
			CombinedGeometryImpl combinedGeometryImpl = TryCreate(combineMode, g3, g4);
			if (combinedGeometryImpl != null)
			{
				return combinedGeometryImpl;
			}
		}
		return new CombinedGeometryImpl(null, null);
	}

	public static CombinedGeometryImpl? TryCreate(GeometryCombineMode combineMode, GeometryImpl g1, GeometryImpl g2)
	{
		SKPathOp op = combineMode switch
		{
			GeometryCombineMode.Intersect => SKPathOp.Intersect, 
			GeometryCombineMode.Xor => SKPathOp.Xor, 
			GeometryCombineMode.Exclude => SKPathOp.Difference, 
			_ => SKPathOp.Union, 
		};
		SKPath sKPath = ((g1.StrokePath != null && g2.StrokePath != null) ? g1.StrokePath.Op(g2.StrokePath, op) : null);
		SKPath sKPath2 = null;
		if (g1.FillPath != null && g2.FillPath != null)
		{
			sKPath2 = ((g1.FillPath != g1.StrokePath || g2.FillPath != g2.StrokePath) ? g1.FillPath.Op(g2.FillPath, op) : sKPath);
		}
		if (sKPath == null && sKPath2 == null)
		{
			return null;
		}
		return new CombinedGeometryImpl(sKPath, sKPath2);
	}
}
