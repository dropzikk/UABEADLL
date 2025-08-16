using System;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Utilities;

namespace Avalonia.Rendering.Composition.Drawing.Nodes;

internal class RenderDataGlyphRunNode : IRenderDataItemWithServerResources, IRenderDataItem, IDisposable
{
	public IBrush? ServerBrush { get; set; }

	public IRef<IGlyphRunImpl>? GlyphRun { get; set; }

	public Rect? Bounds => (GlyphRun?.Item?.Bounds).GetValueOrDefault();

	public bool HitTest(Point p)
	{
		return GlyphRun?.Item.Bounds.ContainsExclusive(p) ?? false;
	}

	public void Invoke(ref RenderDataNodeRenderContext context)
	{
		context.Context.DrawGlyphRun(ServerBrush, GlyphRun.Item);
	}

	public void Collect(IRenderDataServerResourcesCollector collector)
	{
		collector.AddRenderDataServerResource(ServerBrush);
	}

	public void Dispose()
	{
		GlyphRun?.Dispose();
		GlyphRun = null;
	}
}
