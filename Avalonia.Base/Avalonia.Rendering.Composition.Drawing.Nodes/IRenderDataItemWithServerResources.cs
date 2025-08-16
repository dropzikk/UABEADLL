namespace Avalonia.Rendering.Composition.Drawing.Nodes;

internal interface IRenderDataItemWithServerResources : IRenderDataItem
{
	void Collect(IRenderDataServerResourcesCollector collector);
}
