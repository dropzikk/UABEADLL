using System.Collections.Generic;
using Avalonia.Media;
using Avalonia.Platform;
using SkiaSharp;

namespace Avalonia.Skia;

internal class GeometryGroupImpl : GeometryImpl
{
	public override Rect Bounds { get; }

	public override SKPath StrokePath { get; }

	public override SKPath FillPath { get; }

	public GeometryGroupImpl(FillRule fillRule, IReadOnlyList<IGeometryImpl> children)
	{
		SKPathFillType fillType = ((fillRule != FillRule.NonZero) ? SKPathFillType.EvenOdd : SKPathFillType.Winding);
		int count = children.Count;
		SKPath sKPath = new SKPath
		{
			FillType = fillType
		};
		bool flag = false;
		for (int i = 0; i < count; i++)
		{
			if (children[i] is GeometryImpl geometryImpl)
			{
				if (geometryImpl.StrokePath != null)
				{
					sKPath.AddPath(geometryImpl.StrokePath);
				}
				if (geometryImpl.StrokePath != geometryImpl.FillPath)
				{
					flag = true;
				}
			}
		}
		StrokePath = sKPath;
		if (flag)
		{
			SKPath sKPath2 = new SKPath
			{
				FillType = fillType
			};
			for (int j = 0; j < count; j++)
			{
				if (children[j] is GeometryImpl geometryImpl2)
				{
					SKPath fillPath = geometryImpl2.FillPath;
					if (fillPath != null)
					{
						sKPath2.AddPath(fillPath);
					}
				}
			}
			FillPath = sKPath2;
		}
		else
		{
			FillPath = sKPath;
		}
		Bounds = sKPath.TightBounds.ToAvaloniaRect();
	}
}
