using System.Collections.Generic;
using Avalonia.Media;

namespace Avalonia.Controls.Shapes;

public class Polyline : Shape
{
	public static readonly StyledProperty<IList<Point>> PointsProperty;

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

	static Polyline()
	{
		PointsProperty = AvaloniaProperty.Register<Polyline, IList<Point>>("Points");
		Shape.StrokeThicknessProperty.OverrideDefaultValue<Polyline>(1.0);
		Shape.AffectsGeometry<Polyline>(new AvaloniaProperty[1] { PointsProperty });
	}

	public Polyline()
	{
		Points = new Points();
	}

	protected override Geometry CreateDefiningGeometry()
	{
		return new PolylineGeometry(Points, isFilled: false);
	}
}
