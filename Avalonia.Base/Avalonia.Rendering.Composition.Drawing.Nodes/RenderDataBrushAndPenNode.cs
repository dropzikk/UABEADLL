using Avalonia.Media;

namespace Avalonia.Rendering.Composition.Drawing.Nodes;

internal abstract class RenderDataBrushAndPenNode : IRenderDataItemWithServerResources, IRenderDataItem
{
	public IBrush? ServerBrush { get; set; }

	public IPen? ServerPen { get; set; }

	public IPen? ClientPen { get; set; }

	public abstract Rect? Bounds { get; }

	public void Collect(IRenderDataServerResourcesCollector collector)
	{
		collector.AddRenderDataServerResource(ServerBrush);
		collector.AddRenderDataServerResource(ServerPen);
	}

	public abstract void Invoke(ref RenderDataNodeRenderContext context);

	public abstract bool HitTest(Point p);
}
