using Avalonia.Platform;

namespace Avalonia.Media;

public class LineGeometry : Geometry
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

	static LineGeometry()
	{
		StartPointProperty = AvaloniaProperty.Register<LineGeometry, Point>("StartPoint");
		EndPointProperty = AvaloniaProperty.Register<LineGeometry, Point>("EndPoint");
		Geometry.AffectsGeometry(StartPointProperty, EndPointProperty);
	}

	public LineGeometry()
	{
	}

	public LineGeometry(Point startPoint, Point endPoint)
		: this()
	{
		StartPoint = startPoint;
		EndPoint = endPoint;
	}

	public override Geometry Clone()
	{
		return new LineGeometry(StartPoint, EndPoint);
	}

	private protected sealed override IGeometryImpl? CreateDefiningGeometry()
	{
		return AvaloniaLocator.Current.GetRequiredService<IPlatformRenderInterface>().CreateLineGeometry(StartPoint, EndPoint);
	}
}
