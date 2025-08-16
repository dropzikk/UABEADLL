using System;

namespace Avalonia.Media;

public sealed class ArcSegment : PathSegment
{
	public static readonly StyledProperty<bool> IsLargeArcProperty = AvaloniaProperty.Register<ArcSegment, bool>("IsLargeArc", defaultValue: false);

	public static readonly StyledProperty<Point> PointProperty = AvaloniaProperty.Register<ArcSegment, Point>("Point");

	public static readonly StyledProperty<double> RotationAngleProperty = AvaloniaProperty.Register<ArcSegment, double>("RotationAngle", 0.0);

	public static readonly StyledProperty<Size> SizeProperty = AvaloniaProperty.Register<ArcSegment, Size>("Size");

	public static readonly StyledProperty<SweepDirection> SweepDirectionProperty = AvaloniaProperty.Register<ArcSegment, SweepDirection>("SweepDirection", SweepDirection.Clockwise);

	public bool IsLargeArc
	{
		get
		{
			return GetValue(IsLargeArcProperty);
		}
		set
		{
			SetValue(IsLargeArcProperty, value);
		}
	}

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

	public double RotationAngle
	{
		get
		{
			return GetValue(RotationAngleProperty);
		}
		set
		{
			SetValue(RotationAngleProperty, value);
		}
	}

	public Size Size
	{
		get
		{
			return GetValue(SizeProperty);
		}
		set
		{
			SetValue(SizeProperty, value);
		}
	}

	public SweepDirection SweepDirection
	{
		get
		{
			return GetValue(SweepDirectionProperty);
		}
		set
		{
			SetValue(SweepDirectionProperty, value);
		}
	}

	internal override void ApplyTo(StreamGeometryContext ctx)
	{
		ctx.ArcTo(Point, Size, RotationAngle, IsLargeArc, SweepDirection);
	}

	public override string ToString()
	{
		return FormattableString.Invariant($"A {Size} {RotationAngle} {(IsLargeArc ? 1 : 0)} {(int)SweepDirection} {Point}");
	}
}
