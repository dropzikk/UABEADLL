using System.Collections.Generic;

namespace Avalonia.Media;

public sealed class PolyLineSegment : PathSegment
{
	public static readonly StyledProperty<IList<Point>> PointsProperty = AvaloniaProperty.Register<PolyLineSegment, IList<Point>>("Points");

	public IList<Point> Points
	{
		get
		{
			return GetValue(PointsProperty);
		}
		set
		{
			SetValue(PointsProperty, value);
		}
	}

	public PolyLineSegment()
	{
		Points = new Points();
	}

	public PolyLineSegment(IEnumerable<Point> points)
	{
		Points = new Points(points);
	}

	internal override void ApplyTo(StreamGeometryContext ctx)
	{
		IList<Point> points = Points;
		if (points.Count > 0)
		{
			for (int i = 0; i < points.Count; i++)
			{
				ctx.LineTo(points[i]);
			}
		}
	}

	public override string ToString()
	{
		if (Points.Count < 1)
		{
			return "";
		}
		return "L " + string.Join(" ", Points);
	}
}
