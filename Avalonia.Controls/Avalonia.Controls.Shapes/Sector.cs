using System;
using Avalonia.Media;
using Avalonia.Utilities;

namespace Avalonia.Controls.Shapes;

public class Sector : Shape
{
	public static readonly StyledProperty<double> StartAngleProperty;

	public static readonly StyledProperty<double> SweepAngleProperty;

	public double StartAngle
	{
		get
		{
			return GetValue(StartAngleProperty);
		}
		set
		{
			SetValue(StartAngleProperty, value);
		}
	}

	public double SweepAngle
	{
		get
		{
			return GetValue(SweepAngleProperty);
		}
		set
		{
			SetValue(SweepAngleProperty, value);
		}
	}

	static Sector()
	{
		StartAngleProperty = AvaloniaProperty.Register<Sector, double>("StartAngle", 0.0);
		SweepAngleProperty = AvaloniaProperty.Register<Sector, double>("SweepAngle", 0.0);
		Shape.StrokeThicknessProperty.OverrideDefaultValue<Sector>(1.0);
		Shape.AffectsGeometry<Sector>(new AvaloniaProperty[4]
		{
			Visual.BoundsProperty,
			Shape.StrokeThicknessProperty,
			StartAngleProperty,
			SweepAngleProperty
		});
	}

	protected override Geometry? CreateDefiningGeometry()
	{
		Rect rect = new Rect(base.Bounds.Size);
		Rect rect2 = rect.Deflate(base.StrokeThickness * 0.5);
		if (SweepAngle >= 360.0 || SweepAngle <= -360.0)
		{
			return new EllipseGeometry(rect2);
		}
		if (SweepAngle == 0.0)
		{
			return new StreamGeometry();
		}
		(double min, double max) minMaxFromDelta = MathUtilities.GetMinMaxFromDelta(MathUtilities.Deg2Rad(StartAngle), MathUtilities.Deg2Rad(SweepAngle));
		double item = minMaxFromDelta.min;
		double item2 = minMaxFromDelta.max;
		Point point = new Point(rect.Width * 0.5, rect.Height * 0.5);
		double num = rect2.Width * 0.5;
		double num2 = rect2.Height * 0.5;
		Point ellipsePoint = MathUtilities.GetEllipsePoint(point, num, num2, item);
		Point ellipsePoint2 = MathUtilities.GetEllipsePoint(point, num, num2, item2);
		Size size = new Size(num, num2);
		StreamGeometry streamGeometry = new StreamGeometry();
		using StreamGeometryContext streamGeometryContext = streamGeometry.Open();
		streamGeometryContext.BeginFigure(ellipsePoint, isFilled: false);
		streamGeometryContext.ArcTo(ellipsePoint2, size, 0.0, Math.Abs(SweepAngle) > 180.0, SweepDirection.Clockwise);
		streamGeometryContext.LineTo(point);
		streamGeometryContext.EndFigure(isClosed: true);
		return streamGeometry;
	}
}
