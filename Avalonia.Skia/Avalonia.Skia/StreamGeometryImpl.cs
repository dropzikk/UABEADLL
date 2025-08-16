using System;
using Avalonia.Media;
using Avalonia.Platform;
using SkiaSharp;

namespace Avalonia.Skia;

internal class StreamGeometryImpl : GeometryImpl, IStreamGeometryImpl, IGeometryImpl
{
	private class StreamContext : IStreamGeometryContextImpl, IGeometryContext, IDisposable
	{
		private readonly StreamGeometryImpl _geometryImpl;

		private bool _isFilled;

		private SKPath Stroke => _geometryImpl._strokePath;

		private SKPath Fill
		{
			get
			{
				StreamGeometryImpl geometryImpl = _geometryImpl;
				return geometryImpl._fillPath ?? (geometryImpl._fillPath = new SKPath());
			}
		}

		private bool Duplicate
		{
			get
			{
				if (_isFilled)
				{
					return _geometryImpl._fillPath != Stroke;
				}
				return false;
			}
		}

		public StreamContext(StreamGeometryImpl geometryImpl)
		{
			_geometryImpl = geometryImpl;
		}

		public void Dispose()
		{
			_geometryImpl._bounds = Stroke.TightBounds.ToAvaloniaRect();
			_geometryImpl.InvalidateCaches();
		}

		public void ArcTo(Point point, Size size, double rotationAngle, bool isLargeArc, SweepDirection sweepDirection)
		{
			SKPathArcSize largeArc = (isLargeArc ? SKPathArcSize.Large : SKPathArcSize.Small);
			SKPathDirection sweep = ((sweepDirection != SweepDirection.Clockwise) ? SKPathDirection.CounterClockwise : SKPathDirection.Clockwise);
			Stroke.ArcTo((float)size.Width, (float)size.Height, (float)rotationAngle, largeArc, sweep, (float)point.X, (float)point.Y);
			if (Duplicate)
			{
				Fill.ArcTo((float)size.Width, (float)size.Height, (float)rotationAngle, largeArc, sweep, (float)point.X, (float)point.Y);
			}
		}

		public void BeginFigure(Point startPoint, bool isFilled)
		{
			if (!isFilled && Stroke == Fill)
			{
				_geometryImpl._fillPath = Stroke.Clone();
			}
			_isFilled = isFilled;
			Stroke.MoveTo((float)startPoint.X, (float)startPoint.Y);
			if (Duplicate)
			{
				Fill.MoveTo((float)startPoint.X, (float)startPoint.Y);
			}
		}

		public void CubicBezierTo(Point point1, Point point2, Point point3)
		{
			Stroke.CubicTo((float)point1.X, (float)point1.Y, (float)point2.X, (float)point2.Y, (float)point3.X, (float)point3.Y);
			if (Duplicate)
			{
				Fill.CubicTo((float)point1.X, (float)point1.Y, (float)point2.X, (float)point2.Y, (float)point3.X, (float)point3.Y);
			}
		}

		public void QuadraticBezierTo(Point point1, Point point2)
		{
			Stroke.QuadTo((float)point1.X, (float)point1.Y, (float)point2.X, (float)point2.Y);
			if (Duplicate)
			{
				Fill.QuadTo((float)point1.X, (float)point1.Y, (float)point2.X, (float)point2.Y);
			}
		}

		public void LineTo(Point point)
		{
			Stroke.LineTo((float)point.X, (float)point.Y);
			if (Duplicate)
			{
				Fill.LineTo((float)point.X, (float)point.Y);
			}
		}

		public void EndFigure(bool isClosed)
		{
			if (isClosed)
			{
				Stroke.Close();
				if (Duplicate)
				{
					Fill.Close();
				}
			}
		}

		public void SetFillRule(FillRule fillRule)
		{
			Fill.FillType = ((fillRule == FillRule.EvenOdd) ? SKPathFillType.EvenOdd : SKPathFillType.Winding);
		}
	}

	private Rect _bounds;

	private readonly SKPath _strokePath;

	private SKPath? _fillPath;

	public override SKPath? StrokePath => _strokePath;

	public override SKPath? FillPath => _fillPath;

	public override Rect Bounds => _bounds;

	public StreamGeometryImpl(SKPath stroke, SKPath? fill, Rect? bounds = null)
	{
		_strokePath = stroke;
		_fillPath = fill;
		_bounds = bounds ?? stroke.TightBounds.ToAvaloniaRect();
	}

	private StreamGeometryImpl(SKPath path)
		: this(path, path, default(Rect))
	{
	}

	public StreamGeometryImpl()
		: this(CreateEmptyPath())
	{
	}

	public IStreamGeometryImpl Clone()
	{
		SKPath sKPath = _strokePath.Clone();
		SKPath fill = ((_fillPath == _strokePath) ? sKPath : _fillPath.Clone());
		return new StreamGeometryImpl(sKPath, fill, Bounds);
	}

	public IStreamGeometryContextImpl Open()
	{
		return new StreamContext(this);
	}

	private static SKPath CreateEmptyPath()
	{
		return new SKPath
		{
			FillType = SKPathFillType.EvenOdd
		};
	}
}
