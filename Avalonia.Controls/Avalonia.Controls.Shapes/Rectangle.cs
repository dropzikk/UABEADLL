using Avalonia.Media;

namespace Avalonia.Controls.Shapes;

public class Rectangle : Shape
{
	private const double PiOver2 = 1.57079633;

	public static readonly StyledProperty<double> RadiusXProperty;

	public static readonly StyledProperty<double> RadiusYProperty;

	public double RadiusX
	{
		get
		{
			return GetValue(RadiusXProperty);
		}
		set
		{
			SetValue(RadiusXProperty, value);
		}
	}

	public double RadiusY
	{
		get
		{
			return GetValue(RadiusYProperty);
		}
		set
		{
			SetValue(RadiusYProperty, value);
		}
	}

	static Rectangle()
	{
		RadiusXProperty = AvaloniaProperty.Register<Rectangle, double>("RadiusX", 0.0);
		RadiusYProperty = AvaloniaProperty.Register<Rectangle, double>("RadiusY", 0.0);
		Shape.AffectsGeometry<Rectangle>(new AvaloniaProperty[4]
		{
			Visual.BoundsProperty,
			RadiusXProperty,
			RadiusYProperty,
			Shape.StrokeThicknessProperty
		});
	}

	protected override Geometry CreateDefiningGeometry()
	{
		double radiusX = RadiusX;
		double radiusY = RadiusY;
		if (radiusX == 0.0 && radiusY == 0.0)
		{
			return new RectangleGeometry(new Rect(base.Bounds.Size).Deflate(base.StrokeThickness / 2.0));
		}
		Rect rect = new Rect(base.Bounds.Size).Deflate(base.StrokeThickness / 2.0);
		StreamGeometry streamGeometry = new StreamGeometry();
		Size size = new Size(radiusX, radiusY);
		using StreamGeometryContext streamGeometryContext = streamGeometry.Open();
		streamGeometryContext.BeginFigure(new Point(rect.Left + radiusX, rect.Top), isFilled: true);
		streamGeometryContext.LineTo(new Point(rect.Right - radiusX, rect.Top));
		streamGeometryContext.ArcTo(new Point(rect.Right, rect.Top + radiusY), size, 1.57079633, isLargeArc: false, SweepDirection.Clockwise);
		streamGeometryContext.LineTo(new Point(rect.Right, rect.Bottom - radiusY));
		streamGeometryContext.ArcTo(new Point(rect.Right - radiusX, rect.Bottom), size, 1.57079633, isLargeArc: false, SweepDirection.Clockwise);
		streamGeometryContext.LineTo(new Point(rect.Left + radiusX, rect.Bottom));
		streamGeometryContext.ArcTo(new Point(rect.Left, rect.Bottom - radiusY), size, 1.57079633, isLargeArc: false, SweepDirection.Clockwise);
		streamGeometryContext.LineTo(new Point(rect.Left, rect.Top + radiusY));
		streamGeometryContext.ArcTo(new Point(rect.Left + radiusX, rect.Top), size, 1.57079633, isLargeArc: false, SweepDirection.Clockwise);
		streamGeometryContext.EndFigure(isClosed: true);
		return streamGeometry;
	}

	protected override Size MeasureOverride(Size availableSize)
	{
		return new Size(base.StrokeThickness, base.StrokeThickness);
	}
}
