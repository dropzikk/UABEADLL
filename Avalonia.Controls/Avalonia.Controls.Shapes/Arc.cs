using System;
using Avalonia.Media;
using Avalonia.Utilities;

namespace Avalonia.Controls.Shapes;

public class Arc : Shape
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

	static Arc()
	{
		StartAngleProperty = AvaloniaProperty.Register<Arc, double>("StartAngle", 0.0);
		SweepAngleProperty = AvaloniaProperty.Register<Arc, double>("SweepAngle", 0.0);
		Shape.StrokeThicknessProperty.OverrideDefaultValue<Arc>(1.0);
		Shape.AffectsGeometry<Arc>(new AvaloniaProperty[4]
		{
			Visual.BoundsProperty,
			Shape.StrokeThicknessProperty,
			StartAngleProperty,
			SweepAngleProperty
		});
	}

	protected override Geometry CreateDefiningGeometry()
	{
		double num = MathUtilities.Deg2Rad(StartAngle);
		double val = num + MathUtilities.Deg2Rad(SweepAngle);
		double num2 = Math.Min(num, val);
		double num3 = Math.Max(num, val);
		double num4 = RadToNormRad(num2);
		double num5 = RadToNormRad(num3);
		Rect rect = new Rect(base.Bounds.Size);
		if (num4 == num5 && num2 != num3)
		{
			return new EllipseGeometry(rect.Deflate(base.StrokeThickness / 2.0));
		}
		if (SweepAngle == 0.0)
		{
			return new StreamGeometry();
		}
		Rect rect2 = rect.Deflate(base.StrokeThickness / 2.0);
		double x = rect.Center.X;
		double y = rect.Center.Y;
		double num6 = rect2.Width / 2.0;
		double num7 = rect2.Height / 2.0;
		double num8 = RadToNormRad(num3 - num2);
		Point ringPoint = GetRingPoint(num6, num7, x, y, num2);
		Point ringPoint2 = GetRingPoint(num6, num7, x, y, num3);
		StreamGeometry streamGeometry = new StreamGeometry();
		using StreamGeometryContext streamGeometryContext = streamGeometry.Open();
		streamGeometryContext.BeginFigure(ringPoint, isFilled: false);
		streamGeometryContext.ArcTo(ringPoint2, new Size(num6, num7), num8, num8 >= Math.PI, SweepDirection.Clockwise);
		streamGeometryContext.EndFigure(isClosed: false);
		return streamGeometry;
	}

	private static double RadToNormRad(double inAngle)
	{
		return (inAngle % (Math.PI * 2.0) + Math.PI * 2.0) % (Math.PI * 2.0);
	}

	private static Point GetRingPoint(double radiusX, double radiusY, double centerX, double centerY, double angle)
	{
		return new Point(radiusX * Math.Cos(angle) + centerX, radiusY * Math.Sin(angle) + centerY);
	}
}
