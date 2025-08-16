using System;
using Avalonia.Media;

namespace Avalonia.Rendering.SceneGraph;

public interface ICustomDrawOperation : IEquatable<ICustomDrawOperation>, IDisposable
{
	Rect Bounds { get; }

	bool HitTest(Point p);

	void Render(ImmediateDrawingContext context);
}
