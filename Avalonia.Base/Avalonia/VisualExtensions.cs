using System;
using Avalonia.Media;
using Avalonia.Rendering;
using Avalonia.VisualTree;

namespace Avalonia;

public static class VisualExtensions
{
	public static Point PointToClient(this Visual visual, PixelPoint point)
	{
		IRenderRoot? obj = visual.VisualRoot ?? throw new ArgumentException("Control does not belong to a visual tree.", "visual");
		return TranslatePoint(point: obj.PointToClient(point), visual: (Visual)obj, relativeTo: visual).Value;
	}

	public static PixelPoint PointToScreen(this Visual visual, Point point)
	{
		IRenderRoot renderRoot = visual.VisualRoot ?? throw new ArgumentException("Control does not belong to a visual tree.", "visual");
		return renderRoot.PointToScreen(visual.TranslatePoint(point, (Visual)renderRoot).Value);
	}

	public static Matrix? TransformToVisual(this Visual from, Visual to)
	{
		Visual visual = from.FindCommonVisualAncestor(to);
		if (visual != null)
		{
			Matrix offsetFrom = GetOffsetFrom(visual, from);
			if (!GetOffsetFrom(visual, to).TryInvert(out var inverted))
			{
				return null;
			}
			return inverted * offsetFrom;
		}
		return null;
	}

	public static Point? TranslatePoint(this Visual visual, Point point, Visual relativeTo)
	{
		Matrix? matrix = visual.TransformToVisual(relativeTo);
		if (matrix.HasValue)
		{
			return point.Transform(matrix.Value);
		}
		return null;
	}

	private static Matrix GetOffsetFrom(Visual ancestor, Visual visual)
	{
		Matrix identity = Matrix.Identity;
		Visual visual2 = visual;
		while (visual2 != ancestor)
		{
			if (visual2.HasMirrorTransform)
			{
				Matrix matrix = new Matrix(-1.0, 0.0, 0.0, 1.0, visual2.Bounds.Width, 0.0);
				identity *= matrix;
			}
			ITransform? renderTransform = visual2.RenderTransform;
			if (renderTransform != null)
			{
				_ = renderTransform.Value;
				if (true)
				{
					Matrix matrix2 = Matrix.CreateTranslation(visual2.RenderTransformOrigin.ToPixels(visual2.Bounds.Size));
					Matrix matrix3 = -matrix2 * visual2.RenderTransform.Value * matrix2;
					identity *= matrix3;
				}
			}
			Point topLeft = visual2.Bounds.TopLeft;
			if (topLeft != default(Point))
			{
				identity *= Matrix.CreateTranslation(topLeft);
			}
			visual2 = visual2.VisualParent;
			if (visual2 == null)
			{
				throw new ArgumentException("'visual' is not a descendant of 'ancestor'.");
			}
		}
		return identity;
	}
}
