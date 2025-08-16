using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;

namespace Avalonia.Rendering.Composition.Drawing.Nodes;

internal class RenderDataGeometryNode : RenderDataBrushAndPenNode
{
	public IGeometryImpl? Geometry { get; set; }

	public override Rect? Bounds => Geometry?.GetRenderBounds(base.ServerPen).CalculateBoundsWithLineCaps(base.ServerPen) ?? default(Rect);

	public override bool HitTest(Point p)
	{
		if (Geometry == null)
		{
			return false;
		}
		if (base.ServerBrush == null || !Geometry.FillContains(p))
		{
			if (base.ClientPen != null)
			{
				return Geometry.StrokeContains(base.ClientPen, p);
			}
			return false;
		}
		return true;
	}

	public override void Invoke(ref RenderDataNodeRenderContext context)
	{
		context.Context.DrawGeometry(base.ServerBrush, base.ServerPen, Geometry);
	}
}
