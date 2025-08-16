using Avalonia.Media;

namespace Avalonia.Controls.Shapes;

public class Line : Shape
{
	public static readonly StyledProperty<Point> StartPointProperty;

	public static readonly StyledProperty<Point> EndPointProperty;

	public Point StartPoint
	{
		get
		{
			return GetValue(StartPointProperty);
		}
		set
		{
			SetValue(StartPointProperty, value);
		}
	}

	public Point EndPoint
	{
		get
		{
			return GetValue(EndPointProperty);
		}
		set
		{
			SetValue(EndPointProperty, value);
		}
	}

	static Line()
	{
		StartPointProperty = AvaloniaProperty.Register<Line, Point>("StartPoint");
		EndPointProperty = AvaloniaProperty.Register<Line, Point>("EndPoint");
		Shape.StrokeThicknessProperty.OverrideDefaultValue<Line>(1.0);
		Shape.AffectsGeometry<Line>(new AvaloniaProperty[2] { StartPointProperty, EndPointProperty });
	}

	protected override Geometry CreateDefiningGeometry()
	{
		return new LineGeometry(StartPoint, EndPoint);
	}
}
