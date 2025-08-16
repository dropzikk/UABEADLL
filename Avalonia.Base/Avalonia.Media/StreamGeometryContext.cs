using System;
using Avalonia.Platform;

namespace Avalonia.Media;

public class StreamGeometryContext : IGeometryContext, IDisposable
{
	private readonly IStreamGeometryContextImpl _impl;

	private Point _currentPoint;

	public StreamGeometryContext(IStreamGeometryContextImpl impl)
	{
		_impl = impl;
	}

	public void SetFillRule(FillRule fillRule)
	{
		_impl.SetFillRule(fillRule);
	}

	public void ArcTo(Point point, Size size, double rotationAngle, bool isLargeArc, SweepDirection sweepDirection)
	{
		_impl.ArcTo(point, size, rotationAngle, isLargeArc, sweepDirection);
		_currentPoint = point;
	}

	public void PreciseArcTo(Point point, Size size, double rotationAngle, bool isLargeArc, SweepDirection sweepDirection)
	{
		PreciseEllipticArcHelper.ArcTo(this, _currentPoint, point, size, rotationAngle, isLargeArc, sweepDirection);
	}

	public void BeginFigure(Point startPoint, bool isFilled)
	{
		_impl.BeginFigure(startPoint, isFilled);
		_currentPoint = startPoint;
	}

	public void CubicBezierTo(Point point1, Point point2, Point point3)
	{
		_impl.CubicBezierTo(point1, point2, point3);
		_currentPoint = point3;
	}

	public void QuadraticBezierTo(Point control, Point endPoint)
	{
		_impl.QuadraticBezierTo(control, endPoint);
		_currentPoint = endPoint;
	}

	public void LineTo(Point point)
	{
		_impl.LineTo(point);
		_currentPoint = point;
	}

	public void EndFigure(bool isClosed)
	{
		_impl.EndFigure(isClosed);
	}

	public void Dispose()
	{
		_impl.Dispose();
	}
}
