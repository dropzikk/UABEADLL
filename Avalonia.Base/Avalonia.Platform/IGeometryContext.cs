using System;
using Avalonia.Media;

namespace Avalonia.Platform;

public interface IGeometryContext : IDisposable
{
	void ArcTo(Point point, Size size, double rotationAngle, bool isLargeArc, SweepDirection sweepDirection);

	void BeginFigure(Point startPoint, bool isFilled = true);

	void CubicBezierTo(Point point1, Point point2, Point point3);

	void QuadraticBezierTo(Point control, Point endPoint);

	void LineTo(Point point);

	void EndFigure(bool isClosed);

	void SetFillRule(FillRule fillRule);
}
