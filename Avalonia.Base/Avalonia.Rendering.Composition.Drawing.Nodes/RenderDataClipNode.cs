namespace Avalonia.Rendering.Composition.Drawing.Nodes;

internal class RenderDataClipNode : RenderDataPushNode
{
	public RoundedRect Rect { get; set; }

	public override void Push(ref RenderDataNodeRenderContext context)
	{
		context.Context.PushClip(Rect);
	}

	public override void Pop(ref RenderDataNodeRenderContext context)
	{
		context.Context.PopClip();
	}

	public override bool HitTest(Point p)
	{
		if (!Rect.Rect.Contains(p))
		{
			return false;
		}
		return base.HitTest(p);
	}
}
