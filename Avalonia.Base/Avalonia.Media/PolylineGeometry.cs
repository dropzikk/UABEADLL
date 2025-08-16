using System;
using System.Collections.Generic;
using Avalonia.Collections;
using Avalonia.Metadata;
using Avalonia.Platform;

namespace Avalonia.Media;

public class PolylineGeometry : Geometry
{
	public static readonly DirectProperty<PolylineGeometry, IList<Point>> PointsProperty;

	public static readonly StyledProperty<bool> IsFilledProperty;

	private IList<Point> _points;

	private IDisposable? _pointsObserver;

	[Content]
	public IList<Point> Points
	{
		get
		{
			return _points;
		}
		set
		{
			SetAndRaise(PointsProperty, ref _points, value);
		}
	}

	public bool IsFilled
	{
		get
		{
			return GetValue(IsFilledProperty);
		}
		set
		{
			SetValue(IsFilledProperty, value);
		}
	}

	static PolylineGeometry()
	{
		PointsProperty = AvaloniaProperty.RegisterDirect("Points", (PolylineGeometry g) => g.Points, delegate(PolylineGeometry g, IList<Point> f)
		{
			g.Points = f;
		});
		IsFilledProperty = AvaloniaProperty.Register<PolylineGeometry, bool>("IsFilled", defaultValue: false);
		Geometry.AffectsGeometry(IsFilledProperty);
		PointsProperty.Changed.AddClassHandler(delegate(PolylineGeometry s, AvaloniaPropertyChangedEventArgs e)
		{
			s.OnPointsChanged(e.NewValue as IList<Point>);
		});
	}

	public PolylineGeometry()
	{
		_points = new Points();
	}

	public PolylineGeometry(IEnumerable<Point> points, bool isFilled)
	{
		_points = new Points(points);
		IsFilled = isFilled;
	}

	public override Geometry Clone()
	{
		return new PolylineGeometry(Points, IsFilled);
	}

	private protected sealed override IGeometryImpl? CreateDefiningGeometry()
	{
		IStreamGeometryImpl streamGeometryImpl = AvaloniaLocator.Current.GetRequiredService<IPlatformRenderInterface>().CreateStreamGeometry();
		using (IStreamGeometryContextImpl streamGeometryContextImpl = streamGeometryImpl.Open())
		{
			IList<Point> points = Points;
			bool isFilled = IsFilled;
			if (points.Count > 0)
			{
				streamGeometryContextImpl.BeginFigure(points[0], isFilled);
				for (int i = 1; i < points.Count; i++)
				{
					streamGeometryContextImpl.LineTo(points[i]);
				}
				streamGeometryContextImpl.EndFigure(isFilled);
			}
		}
		return streamGeometryImpl;
	}

	private void OnPointsChanged(IList<Point>? newValue)
	{
		_pointsObserver?.Dispose();
		_pointsObserver = (newValue as IAvaloniaList<Point>)?.ForEachItem((Action<Point>)delegate
		{
			InvalidateGeometry();
		}, (Action<Point>)delegate
		{
			InvalidateGeometry();
		}, (Action)base.InvalidateGeometry, weakSubscription: false);
	}
}
