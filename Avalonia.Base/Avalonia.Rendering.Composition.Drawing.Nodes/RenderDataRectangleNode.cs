using Avalonia.Media;

namespace Avalonia.Rendering.Composition.Drawing.Nodes;

internal class RenderDataRectangleNode : RenderDataBrushAndPenNode
{
	public RoundedRect Rect { get; set; }

	public BoxShadows BoxShadows { get; set; }

	public override Rect? Bounds => BoxShadows.TransformBounds(Rect.Rect).Inflate((base.ServerPen?.Thickness ?? 0.0) / 2.0);

	public override bool HitTest(Point p)
	{
		Rect rect;
		if (base.ServerBrush != null)
		{
			rect = Rect.Rect;
			IPen? clientPen = base.ClientPen;
			return rect.Inflate((clientPen != null) ? (clientPen.Thickness / 2.0) : 0.0).ContainsExclusive(p);
		}
		rect = Rect.Rect;
		IPen? clientPen2 = base.ClientPen;
		Rect rect2 = rect.Inflate((clientPen2 != null) ? (clientPen2.Thickness / 2.0) : 0.0);
		rect = Rect.Rect;
		IPen? clientPen3 = base.ClientPen;
		Rect rect3 = rect.Deflate((clientPen3 != null) ? (clientPen3.Thickness / 2.0) : 0.0);
		if (rect2.ContainsExclusive(p))
		{
			return !rect3.ContainsExclusive(p);
		}
		return false;
	}

	public override void Invoke(ref RenderDataNodeRenderContext context)
	{
		context.Context.DrawRectangle(base.ServerBrush, base.ServerPen, Rect, BoxShadows);
	}
}
