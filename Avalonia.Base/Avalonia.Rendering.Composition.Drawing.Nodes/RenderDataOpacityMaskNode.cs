using Avalonia.Media;

namespace Avalonia.Rendering.Composition.Drawing.Nodes;

internal class RenderDataOpacityMaskNode : RenderDataPushNode, IRenderDataItemWithServerResources, IRenderDataItem
{
	public IBrush? ServerBrush { get; set; }

	public Rect BoundsRect { get; set; }

	public void Collect(IRenderDataServerResourcesCollector collector)
	{
		collector.AddRenderDataServerResource(ServerBrush);
	}

	public override void Push(ref RenderDataNodeRenderContext context)
	{
		if (ServerBrush != null)
		{
			context.Context.PushOpacityMask(ServerBrush, BoundsRect);
		}
	}

	public override void Pop(ref RenderDataNodeRenderContext context)
	{
		context.Context.PopOpacityMask();
	}
}
