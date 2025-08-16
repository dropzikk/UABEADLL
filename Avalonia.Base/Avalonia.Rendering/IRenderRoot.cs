using Avalonia.Metadata;

namespace Avalonia.Rendering;

[NotClientImplementable]
public interface IRenderRoot
{
	Size ClientSize { get; }

	IRenderer Renderer { get; }

	IHitTester HitTester { get; }

	double RenderScaling { get; }

	Point PointToClient(PixelPoint point);

	PixelPoint PointToScreen(Point point);
}
