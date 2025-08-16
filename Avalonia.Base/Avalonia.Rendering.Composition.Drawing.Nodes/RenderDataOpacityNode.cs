namespace Avalonia.Rendering.Composition.Drawing.Nodes;

internal class RenderDataOpacityNode : RenderDataPushNode
{
	public double Opacity { get; set; }

	public override void Push(ref RenderDataNodeRenderContext context)
	{
		if (Opacity != 1.0)
		{
			context.Context.PushOpacity(Opacity, null);
		}
	}

	public override void Pop(ref RenderDataNodeRenderContext context)
	{
		if (Opacity != 1.0)
		{
			context.Context.PopOpacity();
		}
	}
}
