using System;

namespace Avalonia.Media;

public sealed class QuadraticBezierSegment : PathSegment
{
	public static readonly StyledProperty<Point> Point1Property = AvaloniaProperty.Register<QuadraticBezierSegment, Point>("Point1");

	public static readonly StyledProperty<Point> Point2Property = AvaloniaProperty.Register<QuadraticBezierSegment, Point>("Point2");

	public Point Point1
	{
		get
		{
			return GetValue(Point1Property);
		}
		set
		{
			SetValue(Point1Property, value);
		}
	}

	public Point Point2
	{
		get
		{
			return GetValue(Point2Property);
		}
		set
		{
			SetValue(Point2Property, value);
		}
	}

	internal override void ApplyTo(StreamGeometryContext ctx)
	{
		ctx.QuadraticBezierTo(Point1, Point2);
	}

	public override string ToString()
	{
		return FormattableString.Invariant($"Q {Point1} {Point2}");
	}
}
