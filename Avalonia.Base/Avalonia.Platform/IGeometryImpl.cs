using System.Diagnostics.CodeAnalysis;
using Avalonia.Media;
using Avalonia.Metadata;

namespace Avalonia.Platform;

[Unstable]
public interface IGeometryImpl
{
	Rect Bounds { get; }

	double ContourLength { get; }

	Rect GetRenderBounds(IPen? pen);

	bool FillContains(Point point);

	IGeometryImpl? Intersect(IGeometryImpl geometry);

	bool StrokeContains(IPen? pen, Point point);

	ITransformedGeometryImpl WithTransform(Matrix transform);

	bool TryGetPointAtDistance(double distance, out Point point);

	bool TryGetPointAndTangentAtDistance(double distance, out Point point, out Point tangent);

	bool TryGetSegment(double startDistance, double stopDistance, bool startOnBeginFigure, [NotNullWhen(true)] out IGeometryImpl? segmentGeometry);
}
