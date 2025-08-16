using System.Collections.Generic;
using Avalonia.Media;

namespace Avalonia.Controls.Shapes;

public class Polygon : Shape
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

	static Polygon()
	{
		PointsProperty = AvaloniaProperty.Register<Polygon, IList<Point>>("Points");
		Shape.AffectsGeometry<Polygon>(new AvaloniaProperty[1] { PointsProperty });
	}

	public Polygon()
	{
		Points = new Points();
	}

	protected override Geometry CreateDefiningGeometry()
	{
		return new PolylineGeometry(Points, isFilled: true);
	}
}
