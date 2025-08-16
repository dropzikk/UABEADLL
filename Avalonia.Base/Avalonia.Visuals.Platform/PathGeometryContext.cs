using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Media;
using Avalonia.Platform;

namespace Avalonia.Visuals.Platform;

public class PathGeometryContext : IGeometryContext, IDisposable
{
	private PathFigure? _currentFigure;

	private PathGeometry? _pathGeometry;

	public PathGeometryContext(PathGeometry pathGeometry)
	{
		_pathGeometry = pathGeometry ?? throw new ArgumentNullException("pathGeometry");
	}

	public void Dispose()
	{
		_pathGeometry = null;
	}

	public void ArcTo(Point point, Size size, double rotationAngle, bool isLargeArc, SweepDirection sweepDirection)
	{
		ArcSegment item = new ArcSegment
		{
			Size = size,
			RotationAngle = rotationAngle,
			IsLargeArc = isLargeArc,
			SweepDirection = sweepDirection,
			Point = point
		};
		CurrentFigureSegments().Add(item);
	}

	public void BeginFigure(Point startPoint, bool isFilled)
	{
		ThrowIfDisposed();
		_currentFigure = new PathFigure
		{
			StartPoint = startPoint,
			IsClosed = false,
			IsFilled = isFilled
		};
		PathGeometry pathGeometry = _pathGeometry;
		if (pathGeometry.Figures == null)
		{
			PathFigures pathFigures2 = (pathGeometry.Figures = new PathFigures());
		}
		_pathGeometry.Figures.Add(_currentFigure);
	}

	public void CubicBezierTo(Point point1, Point point2, Point point3)
	{
		BezierSegment item = new BezierSegment
		{
			Point1 = point1,
			Point2 = point2,
			Point3 = point3
		};
		CurrentFigureSegments().Add(item);
	}

	public void QuadraticBezierTo(Point control, Point endPoint)
	{
		QuadraticBezierSegment item = new QuadraticBezierSegment
		{
			Point1 = control,
			Point2 = endPoint
		};
		CurrentFigureSegments().Add(item);
	}

	public void LineTo(Point point)
	{
		LineSegment item = new LineSegment
		{
			Point = point
		};
		CurrentFigureSegments().Add(item);
	}

	public void EndFigure(bool isClosed)
	{
		if (_currentFigure != null)
		{
			_currentFigure.IsClosed = isClosed;
		}
		_currentFigure = null;
	}

	public void SetFillRule(FillRule fillRule)
	{
		ThrowIfDisposed();
		_pathGeometry.FillRule = fillRule;
	}

	[MemberNotNull("_pathGeometry")]
	private void ThrowIfDisposed()
	{
		if (_pathGeometry == null)
		{
			throw new ObjectDisposedException("PathGeometryContext");
		}
	}

	private PathSegments CurrentFigureSegments()
	{
		ThrowIfDisposed();
		if (_currentFigure == null)
		{
			throw new InvalidOperationException("No figure in progress.");
		}
		if (_currentFigure.Segments == null)
		{
			throw new InvalidOperationException("Current figure's segments cannot be null.");
		}
		return _currentFigure.Segments;
	}
}
