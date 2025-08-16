using Avalonia.Platform;

namespace Avalonia.Media;

public class EllipseGeometry : Geometry
{
	public static readonly StyledProperty<Rect> RectProperty;

	public static readonly StyledProperty<double> RadiusXProperty;

	public static readonly StyledProperty<double> RadiusYProperty;

	public static readonly StyledProperty<Point> CenterProperty;

	public Rect Rect
	{
		get
		{
			return GetValue(RectProperty);
		}
		set
		{
			SetValue(RectProperty, value);
		}
	}

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

	public Point Center
	{
		get
		{
			return GetValue(CenterProperty);
		}
		set
		{
			SetValue(CenterProperty, value);
		}
	}

	static EllipseGeometry()
	{
		RectProperty = AvaloniaProperty.Register<EllipseGeometry, Rect>("Rect");
		RadiusXProperty = AvaloniaProperty.Register<EllipseGeometry, double>("RadiusX", 0.0);
		RadiusYProperty = AvaloniaProperty.Register<EllipseGeometry, double>("RadiusY", 0.0);
		CenterProperty = AvaloniaProperty.Register<EllipseGeometry, Point>("Center");
		Geometry.AffectsGeometry(RectProperty, RadiusXProperty, RadiusYProperty, CenterProperty);
	}

	public EllipseGeometry()
	{
	}

	public EllipseGeometry(Rect rect)
		: this()
	{
		Rect = rect;
	}

	public override Geometry Clone()
	{
		return new EllipseGeometry
		{
			Rect = Rect,
			RadiusX = RadiusX,
			RadiusY = RadiusY,
			Center = Center
		};
	}

	private protected sealed override IGeometryImpl? CreateDefiningGeometry()
	{
		IPlatformRenderInterface requiredService = AvaloniaLocator.Current.GetRequiredService<IPlatformRenderInterface>();
		if (Rect != default(Rect))
		{
			return requiredService.CreateEllipseGeometry(Rect);
		}
		double x = Center.X - RadiusX;
		double y = Center.Y - RadiusY;
		double width = RadiusX * 2.0;
		double height = RadiusY * 2.0;
		return requiredService.CreateEllipseGeometry(new Rect(x, y, width, height));
	}
}
