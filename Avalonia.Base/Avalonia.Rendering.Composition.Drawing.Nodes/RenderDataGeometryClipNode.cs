using Avalonia.Platform;

namespace Avalonia.Rendering.Composition.Drawing.Nodes;

internal class RenderDataGeometryClipNode : RenderDataPushNode
{
	public IGeometryImpl? Geometry { get; set; }

	public bool Contains(Point p)
	{
		return Geometry?.FillContains(p) ?? false;
	}

	public override void Push(ref RenderDataNodeRenderContext context)
	{
		if (Geometry != null)
		{
			context.Context.PushGeometryClip(Geometry);
		}
	}

	public override void Pop(ref RenderDataNodeRenderContext context)
	{
		if (Geometry != null)
		{
			context.Context.PopGeometryClip();
		}
	}

	public override bool HitTest(Point p)
	{
		if (Geometry != null && !Geometry.FillContains(p))
		{
			return false;
		}
		return base.HitTest(p);
	}
}
