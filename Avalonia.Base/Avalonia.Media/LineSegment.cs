using System;

namespace Avalonia.Media;

public sealed class LineSegment : PathSegment
{
	public static readonly StyledProperty<Point> PointProperty = AvaloniaProperty.Register<LineSegment, Point>("Point");

	public Point Point
	{
		get
		{
			return GetValue(PointProperty);
		}
		set
		{
			SetValue(PointProperty, value);
		}
	}

	internal override void ApplyTo(StreamGeometryContext ctx)
	{
		ctx.LineTo(Point);
	}

	public override string ToString()
	{
		return FormattableString.Invariant($"L {Point}");
	}
}
