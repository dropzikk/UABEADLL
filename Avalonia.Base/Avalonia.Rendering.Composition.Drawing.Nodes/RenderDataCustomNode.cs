using Avalonia.Media;
using Avalonia.Rendering.SceneGraph;

namespace Avalonia.Rendering.Composition.Drawing.Nodes;

internal class RenderDataCustomNode : IRenderDataItem
{
	public ICustomDrawOperation? Operation { get; set; }

	public Rect? Bounds => Operation?.Bounds;

	public bool HitTest(Point p)
	{
		return Operation?.HitTest(p) ?? false;
	}

	public void Invoke(ref RenderDataNodeRenderContext context)
	{
		Operation?.Render(new ImmediateDrawingContext(context.Context, ownsImpl: false));
	}
}
