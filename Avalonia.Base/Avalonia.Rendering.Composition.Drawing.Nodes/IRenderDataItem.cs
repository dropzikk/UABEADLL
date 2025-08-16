namespace Avalonia.Rendering.Composition.Drawing.Nodes;

internal interface IRenderDataItem
{
	Rect? Bounds { get; }

	void Invoke(ref RenderDataNodeRenderContext context);

	bool HitTest(Point p);
}
